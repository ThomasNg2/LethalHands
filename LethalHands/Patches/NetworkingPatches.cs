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
    }
}
