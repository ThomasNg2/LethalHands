using HarmonyLib;

namespace LethalHands.Patches
{
    [HarmonyPatch(typeof(Terminal))]
    internal class TerminalPatch
    {
        [HarmonyPatch("BeginUsingTerminal")]
        [HarmonyPrefix]
        static void PreBeginUsingTerminal()
        {
            LethalHands.Instance.SquareDown(false);
        }
    }
}
