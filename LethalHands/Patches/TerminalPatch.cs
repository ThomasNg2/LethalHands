using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LethalHands.Patches
{
    [HarmonyPatch(typeof(Terminal))]
    internal class TerminalPatch
    {
        [HarmonyPatch("BeginUsingTerminal")]
        [HarmonyPrefix]
        static void PreBeginUsingTerminal()
        {
            LethalHandsPlugin.Instance.lethalHands.SquareDown(false);
        }
    }
}
