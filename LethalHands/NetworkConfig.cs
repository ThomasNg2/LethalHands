using BepInEx.Configuration;
using System;
using Unity.Collections;
using Unity.Netcode;

namespace LethalHands
{
    [Serializable]
    public class NetworkConfig : SyncedInstance<NetworkConfig> 
    {
        public float punchRange;
        public float punchCooldown;
        public int punchDamage;
        public int staminaDrain;
        public int chanceToDealDamage;

        public NetworkConfig()
        {
            InitInstance(this);
            punchRange = LocalConfig.punchRange.Value;
            punchCooldown = LocalConfig.punchCooldown.Value;
            punchDamage = LocalConfig.punchDamage.Value;
            staminaDrain = LocalConfig.staminaDrain.Value;
            chanceToDealDamage = LocalConfig.chanceToDealDamage.Value;
        }

        public static void RequestSync()
        {
            if (!IsClient) return;

            using FastBufferWriter stream = new(IntSize, Allocator.Temp);
            MessageManager.SendNamedMessage("SlapitNow.LethalHands_OnRequestConfigSync", 0uL, stream);
        }

        public static void OnRequestSync(ulong clientId, FastBufferReader _)
        {
            if (!IsHost) return;

            LethalHandsPlugin.Instance.manualLogSource.LogInfo($"Config sync request received from client: {clientId}");

            byte[] array = SerializeToBytes(Instance);
            int value = array.Length;

            using FastBufferWriter stream = new(value + IntSize, Allocator.Temp);

            try
            {
                stream.WriteValueSafe(in value, default);
                stream.WriteBytesSafe(array);

                MessageManager.SendNamedMessage("SlapitNow.LethalHands_OnReceiveConfigSync", clientId, stream);
            }
            catch (Exception e)
            {
                LethalHandsPlugin.Instance.manualLogSource.LogInfo($"Error occurred syncing config with client: {clientId}\n{e}");
            }
        }

        public static void OnReceiveSync(ulong _, FastBufferReader reader)
        {
            if (!reader.TryBeginRead(IntSize))
            {
                LethalHandsPlugin.Instance.manualLogSource.LogError("Config sync error: Could not begin reading buffer.");
                return;
            }

            reader.ReadValueSafe(out int val, default);
            if (!reader.TryBeginRead(val))
            {
                LethalHandsPlugin.Instance.manualLogSource.LogError("Config sync error: Host could not sync.");
                return;
            }

            byte[] data = new byte[val];
            reader.ReadBytesSafe(ref data, val);

            SyncInstance(data);
            LethalHands.Instance.LoadConfigValues();
            LethalHandsPlugin.Instance.manualLogSource.LogInfo("Successfully synced config with host.");
        }
    }
}
