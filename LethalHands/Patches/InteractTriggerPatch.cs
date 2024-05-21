using HarmonyLib;

namespace LethalHands.Patches
{
    [HarmonyPatch(typeof(InteractTrigger))]
    internal class InteractTriggerPatch
    {
        [HarmonyPatch("SetUsingLadderOnLocalClient")]
        [HarmonyPrefix]
        static void PreSetUsingLadderOnLocalClient()
        {
            LethalHands.Instance.SquareDown(false); 
        }
    }
}
