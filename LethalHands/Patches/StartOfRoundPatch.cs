using HarmonyLib;

namespace LethalHands.Patches
{
    [HarmonyPatch(typeof(StartOfRound))]
    internal class StartOfRoundPatch
    {
        [HarmonyPatch("StartGame")]
        [HarmonyPostfix]
        static void PostStartGame()
        {
            EnemyFloatHealth.Instance.Reset();
        }
    }
}
