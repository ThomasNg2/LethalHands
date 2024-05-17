using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace LethalHands
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class LethalHandsPlugin : BaseUnityPlugin
    {
        private const string modGUID = "SlapitNow.LethalHands";
        private const string modName = "Lethal Hands";
        private const string modVersion = "22.0.0";

        public static LethalHandsPlugin Instance;
        private readonly Harmony harmony = new Harmony(modGUID);
        public ManualLogSource manualLogSource;
        public LethalHands lethalHands;

        public static PluginInfo PInfo { get; private set; }

        void Awake()
        {
            Instance = this;
            PInfo = ((BaseUnityPlugin)this).Info;
            manualLogSource = BepInEx.Logging.Logger.CreateLogSource(modGUID);
            manualLogSource.LogInfo("Successfully caught these hands");
            harmony.PatchAll(typeof(LethalHandsPlugin));
            harmony.PatchAll(typeof(Patches.PlayerControllerBPatch));
            harmony.PatchAll(typeof(Patches.TerminalPatch));
            Assets.LoadAssetBundlesFromFolder("assetbundles");
            lethalHands = new LethalHands();
            lethalHands.Awake();
            Animation.instantiateAnimations();
        }
    }
}
