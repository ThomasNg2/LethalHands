using GameNetcodeStuff;
using HarmonyLib;
using Unity.Netcode;
using UnityEngine;

namespace LethalHands.Patches
{
    internal class NetworkingPatches
    {
        static GameObject networkPrefab;

        [HarmonyPatch(typeof(GameNetworkManager), "Start")]
        [HarmonyPostfix]
        public static void Init()
        {
            if (networkPrefab != null) return;
            networkPrefab = Assets.Load<GameObject>("lethalhandsnetworker.prefab");
            networkPrefab.AddComponent<LethalHandsNetworker>();

            NetworkManager.Singleton.AddNetworkPrefab(networkPrefab);   
        }

        [HarmonyPatch(typeof(StartOfRound), "Awake")]
        [HarmonyPostfix]
        static void SpawnLethalHandsNetworker()
        {
            if(NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer)
            {
                var lethalHandsNetworker = Object.Instantiate(networkPrefab, Vector3.zero, Quaternion.identity);
                lethalHandsNetworker.GetComponent<NetworkObject>().Spawn();
            }
        }

        [HarmonyPatch(typeof(PlayerControllerB), "ConnectClientToPlayerObject")]
        [HarmonyPostfix]
        public static void InitializeLocalPlayer()
        {
            if(LethalHands.Instance == null)
            {
                LethalHands lethalHands = GameNetworkManager.Instance.localPlayerController.gameObject.AddComponent<LethalHands>();
            }
            LethalHandsPlugin.Instance.manualLogSource.LogInfo("Player initialized, setting up config...");
            if (NetworkConfig.IsHost)
            {
                NetworkConfig.MessageManager.RegisterNamedMessageHandler("SlapitNow.LethalHands_OnRequestConfigSync", NetworkConfig.OnRequestSync);
                NetworkConfig.Synced = true;

                return;
            }

            NetworkConfig.Synced = false;
            NetworkConfig.MessageManager.RegisterNamedMessageHandler("SlapitNow.LethalHands_OnReceiveConfigSync", NetworkConfig.OnReceiveSync);
            NetworkConfig.RequestSync();
        }

        [HarmonyPatch(typeof(GameNetworkManager), "StartDisconnect")]
        [HarmonyPostfix]
        public static void PlayerLeave()
        {
            LethalHandsPlugin.Instance.manualLogSource.LogInfo("Player disconnected, restoring config...");
            LethalHands.Instance.Sleep();
            NetworkConfig.RevertSync();
        }
    }
}
