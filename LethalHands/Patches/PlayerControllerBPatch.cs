using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace LethalHands.Patches
{
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
            if (LethalHands.playerControllerInstance.isPlayerDead)
            {
                LethalHands.Instance.SquareUp(false);
            }
        }

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void PostUpdate()
        {
            if (LethalHands.punchCooldown > 0f)
            {
                LethalHands.punchCooldown -= Time.deltaTime;
            }
        }
    }
}
