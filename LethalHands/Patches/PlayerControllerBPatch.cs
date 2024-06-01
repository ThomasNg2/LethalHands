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
            LethalHands.Instance.SquareDown(false);
        }

        [HarmonyPatch("BeginGrabObject")]
        [HarmonyPrefix]
        static void PreBeginGrabObject()
        {
            LethalHands.Instance.SquareDown(false);
        }

        [HarmonyPatch("PerformEmote")]
        [HarmonyPrefix]
        static void PrePerformEmote()
        {
            LethalHands.Instance.SquareDown(false);
        }

        [HarmonyPatch("KillPlayer")]
        [HarmonyPostfix]
        static void PostKillPlayer()
        {
            if (LethalHands.Instance.playerControllerInstance.isPlayerDead)
            {
                LethalHands.Instance.SquareDown(false);
            }
        }

        [HarmonyPatch("CancelSpecialTriggerAnimations")]
        [HarmonyPrefix]
        static void PreCancelSpecialTriggerAnimations()
        {
            LethalHands.Instance?.SquareDown(false);
        }

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void PostUpdate()
        {
            if (LethalHands.Instance != null && LethalHands.Instance.punchCooldown > 0f)
            {
                LethalHands.Instance.punchCooldown -= Time.deltaTime;
            }
        }

        // Stamina regen prevention or something
        [HarmonyPatch("LateUpdate")]
        [HarmonyPostfix]
        static void PostLateUpdate()
        {
            if (LethalHands.Instance == null) return;
            if(LethalHands.Instance.punchingHaltsStaminaRegen && LethalHands.Instance.freezeStaminaRegen)
            {
                LethalHands.Instance.playerControllerInstance.sprintMeter = Mathf.Clamp(
                    LethalHands.Instance.playerControllerInstance.sprintMeter,
                    0,
                    LethalHands.Instance.recordedStamina);
                LethalHands.Instance.playerControllerInstance.sprintMeterUI.fillAmount = LethalHands.Instance.playerControllerInstance.sprintMeter;
            }
        }
    }
}
