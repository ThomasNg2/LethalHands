using EmotesAPI;
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
            LethalHandsPlugin.Instance.lethalHands.SquareDown();
        }

        [HarmonyPatch("PerformEmote")]
        [HarmonyPrefix]
        static void PrePerformEmote()
        {
            LethalHandsPlugin.Instance.lethalHands.SquareDown();
        }

        [HarmonyPatch("KillPlayer")]
        [HarmonyPostfix]
        static void PostKillPlayer()
        {
            if (LethalHandsPlugin.Instance.lethalHands.playerControllerInstance.isPlayerDead)
            {
                LethalHandsPlugin.Instance.lethalHands.SquareDown();
            }
        }

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void PostUpdate()
        {
            if (LethalHandsPlugin.Instance.lethalHands.punchCooldown > 0f)
            {
                LethalHandsPlugin.Instance.lethalHands.punchCooldown -= Time.deltaTime;
            }
        }
    }
}
