using GameNetcodeStuff;
using HarmonyLib;

namespace LethalHands.Patches
{
    internal class EnemyAIPatch
    {
        [HarmonyPatch(typeof(EnemyAI), "HitEnemyOnLocalClient")]
        [HarmonyPrefix]
        static bool PreHitEnemyOnLocalClient(EnemyAI __instance, int force, PlayerControllerB playerWhoHit, bool playHitSFX, int hitID)
        {
            if (force != -22) return true; // not a punch, let og play out
            __instance.HitEnemy(EnemyFloatHealth.Instance.DamageEnemy(__instance), playerWhoHit, playHitSFX, hitID);
            __instance.HitEnemyServerRpc(force, (int)playerWhoHit.playerClientId, playHitSFX, hitID);
            return false; // skip base method
        }

        [HarmonyPatch(typeof(EnemyAI), "HitEnemyClientRpc")]
        [HarmonyPrefix]
        static void PreHitEnemyClientRpc(EnemyAI __instance, ref int force, int playerWhoHit)
        {
            if (force != -22) return;
            if (playerWhoHit == (int)GameNetworkManager.Instance.localPlayerController.playerClientId) return;
            force = EnemyFloatHealth.Instance.DamageEnemy(__instance);
        }
    }
}
