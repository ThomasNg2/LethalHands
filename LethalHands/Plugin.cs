using BepInEx;
using BepInEx.Logging;
using GameNetcodeStuff;
using HarmonyLib;
using LethalCompanyInputUtils.Api;
using UnityEngine.InputSystem;

namespace LethalHands
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class LethalHands : BaseUnityPlugin
    {
        private const string modGUID = "SlapitNow.LethalHands";
        private const string modName = "Lethal Hands";
        private const string modVersion = "22.0.0";

        private static LethalHands Instance;
        private readonly Harmony harmony = new Harmony(modGUID);
        internal ManualLogSource manualLogSource;

        internal static SquareUpInput squareUpInput = SquareUpInput.Instance;
        static bool isSquaredUp = false;
        static PlayerControllerB playerControllerInstance;
        static readonly string[] controlTips = { "Punch : [LMB]", "Punch but right : [RMB]" };


        void Awake()
        {
            if(Instance == null) Instance = this;
            manualLogSource = BepInEx.Logging.Logger.CreateLogSource(modGUID);
            manualLogSource.LogInfo("Successfully caught these hands");
            harmony.PatchAll(typeof(LethalHands));
            harmony.PatchAll(typeof(PlayerControllerBPatch));
            SetupSquareUpCallback();
        }

        public class SquareUpInput : LcInputActions
        {
            public static SquareUpInput Instance = new SquareUpInput();

            [InputAction(kbmPath: "<Keyboard>/j", Name = "Square up")]
            public InputAction SquareUpKey { get; set; }
        }


        public void SquareUpPerformed(InputAction.CallbackContext context) {
            if (playerControllerInstance == null)
            {
                playerControllerInstance = GameNetworkManager.Instance.localPlayerController;
            }
            if (context.performed && !playerControllerInstance.quickMenuManager.isMenuOpen &&
                ((playerControllerInstance.IsOwner && playerControllerInstance.isPlayerControlled && 
                (!playerControllerInstance.IsServer || playerControllerInstance.isHostPlayerObject)) || playerControllerInstance.isTestingPlayer) &&
                !playerControllerInstance.inSpecialInteractAnimation && !playerControllerInstance.isTypingChat)
            {
                SquareUp(!isSquaredUp);
            }
        }
        public void SetupSquareUpCallback()
        {
            squareUpInput.SquareUpKey.performed += SquareUpPerformed;
        }

        public void SquareUp(bool squareUp) { 
            if (squareUp && !isSquaredUp && !playerControllerInstance.inSpecialInteractAnimation)
            {
                playerControllerInstance.DropAllHeldItemsAndSync();
                playerControllerInstance.performingEmote = false;
                playerControllerInstance.StopPerformingEmoteServerRpc();
                playerControllerInstance.timeSinceStartingEmote = 0f;
                isSquaredUp = true;

                IngamePlayerSettings.Instance.playerInput.actions.FindAction("ActivateItem").performed += PunchPerformed;
                IngamePlayerSettings.Instance.playerInput.actions.FindAction("PingScan").performed += PunchButRightPerformed;
                HUDManager.Instance.ClearControlTips();
                HUDManager.Instance.ChangeControlTipMultiple(controlTips);
                manualLogSource.LogInfo("Squaring up " + isSquaredUp);
            }
            else if (!squareUp && isSquaredUp)
            {
                IngamePlayerSettings.Instance.playerInput.actions.FindAction("ActivateItem").performed -= PunchPerformed;
                IngamePlayerSettings.Instance.playerInput.actions.FindAction("PingScan").performed -= PunchButRightPerformed;
                HUDManager.Instance.ClearControlTips();
                isSquaredUp = false;
                manualLogSource.LogInfo("Squaring down " + isSquaredUp);
            }
        }

        public void PunchPerformed(InputAction.CallbackContext context)
        {
            if (context.performed && isSquaredUp)
            {
                Punch();
            }
        }

        public void PunchButRightPerformed(InputAction.CallbackContext context)
        {
            if (context.performed && isSquaredUp)
            {
                PunchButRight();
            }
        }

        public void Punch()
        {
            manualLogSource.LogInfo("Punching");
        }

        public void PunchButRight()
        {
            manualLogSource.LogInfo("Punching but right");
        }

        [HarmonyPatch(typeof(PlayerControllerB))]
        internal class PlayerControllerBPatch
        {
            [HarmonyPatch("BeginGrabObject")]
            [HarmonyPrefix]
            static void PreBeginGrabObject()
            {
                LethalHands.Instance.SquareUp(false);
            }

            [HarmonyPatch("PerformEmote")]
            [HarmonyPrefix]
            static void PrePerformEmote()
            {
                LethalHands.Instance.SquareUp(false);
            }

            [HarmonyPatch("KillPlayer")]
            [HarmonyPostfix]
            static void PostKillPlayer()
            {
                if (playerControllerInstance.isPlayerDead)
                {
                    LethalHands.Instance.SquareUp(false);
                }
            }
        }
    }
}
