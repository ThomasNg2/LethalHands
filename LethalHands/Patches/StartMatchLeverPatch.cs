using HarmonyLib;

namespace LethalHands.Patches
{
    [HarmonyPatch(typeof(StartMatchLever))]
    internal class StartMatchLeverPatch
    {
        [HarmonyPatch("LeverAnimation")]
        [HarmonyPrefix]
        static void PreLeverAnimation()
        {
            LethalHandsPlugin.Instance.lethalHands.SquareDown(false);
        }
    }
}
