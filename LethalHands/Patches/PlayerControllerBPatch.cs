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
            if(!LethalHands.Instance.allowItems) LethalHands.Instance.SquareDown(false);
        }

        [HarmonyPatch("BeginGrabObject")]
        [HarmonyPostfix]
        static void PostBeginGrabObject()
        {
            if (!LethalHands.Instance.allowItems &&
                LethalHands.Instance.playerControllerInstance.isGrabbingObjectAnimation) LethalHands.Instance.SquareDown(false);
        }

        [HarmonyPatch("SwitchToItemSlot")]
        [HarmonyPostfix]
        static void PostSwitchToItemSlot()
        {
            if (LethalHands.Instance.allowItems) return;
            if(LethalHands.Instance.playerControllerInstance.isHoldingObject) LethalHands.Instance.SquareDown(false);
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
