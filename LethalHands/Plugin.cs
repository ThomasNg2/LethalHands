using BepInEx;
using BepInEx.Logging;
using GameNetcodeStuff;
using HarmonyLib;
using LethalCompanyInputUtils.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using static System.Net.Mime.MediaTypeNames;

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
        static float punchCooldown = 0f;
        static int shovelMask = 11012424;
        static PlayerControllerB playerControllerInstance;
        static readonly string[] controlTips = { "Punch : [LMB]", "Punch but right : [RMB]" };

        void Awake()
        {
            if (Instance == null) Instance = this;
            manualLogSource = BepInEx.Logging.Logger.CreateLogSource(modGUID);
            manualLogSource.LogInfo("Successfully caught these hands");
            harmony.PatchAll(typeof(LethalHands));
            harmony.PatchAll(typeof(PlayerControllerBPatch));
            harmony.PatchAll(typeof(TerminalPatch));
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
            if (context.performed && isSquaredUp && punchCooldown <= 0f)
            {
                Punch();
            }
        }

        public void PunchButRightPerformed(InputAction.CallbackContext context)
        {
            if (context.performed && isSquaredUp && punchCooldown <= 0f)
            {
                PunchButRight();
            }
        }

        public void Punch()
        {
            manualLogSource.LogInfo("Punching");
            // Play left punch animation
            PunchHit();
        }

        public void PunchButRight()
        {
            manualLogSource.LogInfo("Punching but right");
            // Play right punch 
            PunchHit();
        }

        public void PunchHit()
        {
            punchCooldown = 4f;
            playerControllerInstance.sprintMeter = Mathf.Max(0f, playerControllerInstance.sprintMeter - 0.1f);

            RaycastHit[] objectsHitByPunch = Physics.SphereCastAll(playerControllerInstance.gameplayCamera.transform.position, 0.8f, playerControllerInstance.gameplayCamera.transform.forward, 1.2f, shovelMask, QueryTriggerInteraction.Collide);
            List<RaycastHit> objectsHitByPunchList = objectsHitByPunch.OrderBy((RaycastHit raycast) => raycast.distance).ToList();

            bool hitSomething = false;
            bool hitHittable = false;
            int hitTerrainIndex = -1;
            IHittable hittable;
            RaycastHit hitInfo;
            foreach(RaycastHit hit in objectsHitByPunchList)
            {
                if (hit.transform.gameObject.layer == 8 || hit.transform.gameObject.layer == 11) // terrain
                {
                    hitSomething = true;
                    string hitObjectTag = hit.collider.gameObject.tag;
                    for (int j = 0; j < StartOfRound.Instance.footstepSurfaces.Length; j++)
                    {
                        if (StartOfRound.Instance.footstepSurfaces[j].surfaceTag == hitObjectTag)
                        {
                            manualLogSource.LogInfo($"Hit {hitObjectTag}");
                            hitTerrainIndex = j;
                            break;
                        }
                    }
                }
                else if (hit.transform.TryGetComponent<IHittable>(out hittable) &&
                    (hit.point == Vector3.zero || !Physics.Linecast(playerControllerInstance.gameplayCamera.transform.position, hit.point, out hitInfo, StartOfRound.Instance.collidersAndRoomMaskAndDefault)))
                {
                    if (hit.transform == playerControllerInstance.transform) continue; // Stop hitting yourself, (unless TZP ???)
                    hitSomething = true;
                    Vector3 forward = playerControllerInstance.gameplayCamera.transform.forward;
                    if (hit.collider)
                    {
                        EnemyAICollisionDetect enemyCollision = hit.collider.GetComponent<EnemyAICollisionDetect>();
                        if (enemyCollision != null)
                        {
                            enemyCollision.onlyCollideWhenGrounded = false; // magic flag that makes enemies not get hit otherwise
                        }
                    }
                    try
                    {
                        hittable.Hit(1, forward, playerControllerInstance, playHitSFX: true);
                        hitHittable = true;
                    }
                    catch (Exception e)
                    {
                        manualLogSource.LogError($"Exception when punching {e}");
                    }
                    break;
                }
            }
            if (hitSomething)
            {
                if(!hitHittable && hitTerrainIndex != -1) {
                
                    // Play audio
                    // RPC to play sounds
                }

            }
        }

        [HarmonyPatch(typeof(PlayerControllerB))]
        internal class PlayerControllerBPatch
        {
            [HarmonyPatch("GrabObject")]
            [HarmonyPrefix]
            static void PreGrabObject()
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

            [HarmonyPatch("Update")]
            [HarmonyPostfix]
            static void PostUpdate()
            {
                if (punchCooldown > 0f)
                {
                    punchCooldown -= Time.deltaTime;
                }
            }
        }

        [HarmonyPatch(typeof(Terminal))]
        internal class TerminalPatch
        {
            [HarmonyPatch("BeginUsingTerminal")]
            [HarmonyPrefix]
            static void PreBeginUsingTerminal()
            {
                LethalHands.Instance.SquareUp(false);
            }
        }
    }
}
