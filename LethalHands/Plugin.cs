using BepInEx;
using BepInEx.Logging;
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

        private static LethalHands instance;
        private readonly Harmony harmony = new Harmony(modGUID);
        internal ManualLogSource manualLogSource;

        internal static SquareUpInput squareUpInput = SquareUpInput.Instance;

        void Awake()
        {
            if(instance == null) instance = this;
            manualLogSource = BepInEx.Logging.Logger.CreateLogSource(modGUID);
            manualLogSource.LogInfo("Successfully caught these hands");
            harmony.PatchAll(typeof(LethalHands));
        }

        public class SquareUpInput : LcInputActions
        {
            public static SquareUpInput Instance = new SquareUpInput();

            [InputAction(kbmPath: "<Keyboard>/t", Name = "Square up")]

            public InputAction SquareUpKey { get; set; }
        }
    }
}
