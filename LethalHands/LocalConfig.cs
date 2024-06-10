using BepInEx.Configuration;

namespace LethalHands
{
    public class LocalConfig
    {
        public static ConfigEntry<float> punchRange;
        public static ConfigEntry<float> punchCooldown;
        public static ConfigEntry<float> punchDamage;

        public static ConfigEntry<bool> punchOffClingers;

        public static ConfigEntry<int> staminaDrain;
        public static ConfigEntry<int> staminaRequirement;
        public static ConfigEntry<bool> punchingHaltsStaminaRegen;

        public static ConfigEntry<ItemMode> itemDropMode;
        public static ConfigEntry<bool> allowItems;


        public static NetworkConfig networkConfig;

        public LocalConfig(ConfigFile cfg)
        {
            punchRange = cfg.Bind("Balancing", "PunchRange", 1.2f, "Range of the punch (for reference, shovels are 1.5 and knives are 0.75)");
            punchCooldown = cfg.Bind("Balancing", "PunchCooldown", 1f, "Cooldown (in seconds) between punches");
            punchDamage = cfg.Bind("Balancing", "PunchDamage", 0.5f, "Damage dealt by a punch");

            punchOffClingers = cfg.Bind("Balancing", "PunchOffClingers", true, "Whether punches should hit snarefleas/tulip snakes that are attached");

            staminaDrain = cfg.Bind("Stamina Balancing", "StaminaDrain", 0, "Max stamina drained (%) per punch");
            staminaRequirement = cfg.Bind("Stamina Balancing", "StaminaRequirement", 0, "Max stamina required (%) to punch");
            punchingHaltsStaminaRegen = cfg.Bind("Stamina Balancing", "PunchingHaltsStaminaRegen", false, "Whether punching should halt stamina regeneration for a brief period");

            itemDropMode = cfg.Bind("Item Interaction", "ItemDropMode", ItemMode.All, "Whether to drop all items, only the current item, or no items upon squaring up");
            allowItems = cfg.Bind("Item Interaction", "AllowItems", false, "Whether to allow items to be held while squared up");
            networkConfig = new NetworkConfig();
        }
    }

    public enum ItemMode
    {
        All,
        Current,
        None
    }
}
