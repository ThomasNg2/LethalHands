﻿using BepInEx.Configuration;

namespace LethalHands
{
    public class LocalConfig
    {
        public static ConfigEntry<float> punchRange;
        public static ConfigEntry<float> punchCooldown;
        public static ConfigEntry<int> punchDamage;
        public static ConfigEntry<int> chanceToDealDamage;

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
            punchDamage = cfg.Bind("Balancing", "PunchDamage", 1, "Damage dealt by a successful roll");
            chanceToDealDamage = cfg.Bind("Balancing", "ChanceToDealDmg", 50, "Chance (%) to deal damage (doesn't apply to some entities)");

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
