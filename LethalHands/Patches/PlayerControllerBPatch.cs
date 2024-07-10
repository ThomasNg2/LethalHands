using GameNetcodeStuff;
using HarmonyLib;
using System;
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

        [HarmonyPatch("IHittable.Hit")]
        [HarmonyPrefix]
        static bool PreHit(PlayerControllerB __instance, int force, Vector3 hitDirection, PlayerControllerB playerWhoHit)
        {
            if (force != -22) return true;
            // yoinked checks from original method
            if (!__instance.AllowPlayerDeath()) return false;
            CentipedeAI[] snarefleas = UnityEngine.Object.FindObjectsByType<CentipedeAI>(FindObjectsSortMode.None);
            for (int i = 0; i < snarefleas.Length; i++)
            {
                if (snarefleas[i].clingingToPlayer == __instance) return false;
            }
            if ((bool)__instance.inAnimationWithEnemy) return false;

            __instance.DamagePlayerFromOtherClientServerRpc(LethalHands.Instance.playerPunchDamage, hitDirection, (int)playerWhoHit.playerClientId);
            return false;
        }
    }
}
