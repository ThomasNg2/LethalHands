using HarmonyLib;

namespace LethalHands.Patches
{
    [HarmonyPatch(typeof(HUDManager))]
    internal class HudManagerPatch
    {
        [HarmonyPatch("CanPlayerScan")]
        [HarmonyPostfix]
        private static void CanPlayerScanPostfix(ref bool __result)
        {
            __result = __result && (LethalHands.Instance != null && !LethalHands.Instance.isSquaredUp);
        }
    }
}
