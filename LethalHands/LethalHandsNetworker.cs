using GameNetcodeStuff;
using Unity.Netcode;
using UnityEngine;

namespace LethalHands
{
    internal class LethalHandsNetworker : NetworkBehaviour
    {
        public static LethalHandsNetworker Instance { get; private set; }

        public override void OnNetworkSpawn()
        {
            if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer)
            {
                Instance?.gameObject.GetComponent<NetworkObject>().Despawn();
            }
            Instance = this;

            base.OnNetworkSpawn();
        }

        [ServerRpc(RequireOwnership = false)]
        public void PunchHitSoundServerRpc(int playerID, int soundIndex, int terrainIndex)
        {
            PunchHitSoundClientRpc(playerID, soundIndex, terrainIndex);
        }

        [ClientRpc]
        public void PunchHitSoundClientRpc(int playerID, int soundIndex, int terrainIndex)
        {
            LethalHands.Instance.PunchHitSound(playerID, soundIndex, terrainIndex);
        }
    }
}
