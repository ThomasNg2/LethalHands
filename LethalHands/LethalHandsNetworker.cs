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
        public void PunchHitServerRpc(IHittable target, Vector3 hitDirection, PlayerControllerB source)
        {
            target.Hit(1, hitDirection, playerWhoHit: source, playHitSFX: true);
        }
    }
}
