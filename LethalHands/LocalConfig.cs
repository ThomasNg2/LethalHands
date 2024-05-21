using BepInEx.Configuration;

namespace LethalHands
{
    public class LocalConfig
    {
        public static ConfigEntry<float> punchRange;
        public static ConfigEntry<float> punchCooldown;
        public static ConfigEntry<int> punchDamage;
        public static ConfigEntry<int> staminaDrain;
        public static ConfigEntry<int> chanceToDealDamage;
        public static NetworkConfig networkConfig;

        public LocalConfig(ConfigFile cfg)
        {
            punchRange = cfg.Bind("Balancing", "PunchRange", 1.2f, "Range of the punch (for reference, shovels are 1.5 and knives are 0.75)");
            punchCooldown = cfg.Bind("Balancing", "PunchCooldown", 1f, "Cooldown (in seconds) between punches");
            punchDamage = cfg.Bind("Balancing", "PunchDamage", 1, "Damage dealt by a successful roll");
            staminaDrain = cfg.Bind("Balancing", "StaminaDrain", 0, "Max stamina drained (%) per punch");
            chanceToDealDamage = cfg.Bind("Balancing", "ChanceToDealDmg", 50, "Chance (%) to deal damage (doesn't apply to some entities)");
            networkConfig = new NetworkConfig();
        }
    }
}
