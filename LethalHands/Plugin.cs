using BepInEx;
using BepInEx.Logging;
using EmotesAPI;
using HarmonyLib;
using LethalCompanyInputUtils;
using System.Reflection;
using UnityEngine;

namespace LethalHands
{
    [BepInPlugin(modGUID, modName, modVersion)]
    [BepInDependency(CustomEmotesAPI.PluginGUID, BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.rune580.LethalCompanyInputUtils", BepInDependency.DependencyFlags.HardDependency)]
    public class LethalHandsPlugin : BaseUnityPlugin
    {
        private const string modGUID = "SlapitNow.LethalHands";
        private const string modName = "Lethal Hands";
        private const string modVersion = "23.0.0";

        public static LethalHandsPlugin Instance;
        private readonly Harmony harmony = new Harmony(modGUID);
        public ManualLogSource manualLogSource;
        public static LocalConfig MyConfig { get; internal set; }

        public static BepInEx.PluginInfo PInfo { get; private set; }

        private static void NetcodePatcher()
        {
            var types = Assembly.GetExecutingAssembly().GetTypes();
            foreach (var type in types)
            {
                var methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                foreach (var method in methods)
                {
                    var attributes = method.GetCustomAttributes(typeof(RuntimeInitializeOnLoadMethodAttribute), false);
                    if (attributes.Length > 0)
                    {
                        method.Invoke(null, null);
                    }
                }
            }
        }

        void Awake()
        {
            Instance = this;
            PInfo = Info;
            manualLogSource = BepInEx.Logging.Logger.CreateLogSource(modGUID);
            Assets.LoadAssetBundlesFromFolder("assetbundles");
            MyConfig = new(base.Config);
            harmony.PatchAll(typeof(LethalHandsPlugin));
            harmony.PatchAll(typeof(Patches.NetworkingPatches));
            harmony.PatchAll(typeof(Patches.PlayerControllerBPatch));
            harmony.PatchAll(typeof(Patches.TerminalPatch));
            harmony.PatchAll(typeof(Patches.StartMatchLeverPatch));
            harmony.PatchAll(typeof(Patches.HudManagerPatch));
            harmony.PatchAll(typeof(Patches.InteractTriggerPatch));
            harmony.PatchAll(typeof(Patches.EnemyAIPatch));
            harmony.PatchAll(typeof(Patches.StartOfRoundPatch));
            harmony.PatchAll(typeof(Patches.VehicleControllerPatch));
            Animation.instantiateAnimations();
            NetcodePatcher();
            manualLogSource.LogInfo("Successfully caught these hands");
        }
    }
}
