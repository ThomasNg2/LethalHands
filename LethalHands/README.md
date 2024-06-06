# LethalHands

Allows the usage of your hands to fight back in dire situations.

### Usage

Press 'J' (configurable) to toggle the ability to throw left (LMB) and right (RMB) punches.
For most entities, punches will do half the damage of a standard shovel (configurable).

### Installation

The future is now, use a mod manager you fool

Every player needs to have the mod installed.

### Configuration

How balanced the mod is can be adjusted in the `BepInEx/config/SlapitNow.LethalHands.cfg` file or using [LethalConfig](https://thunderstore.io/c/lethal-company/p/AinaVT/LethalConfig/).
The adjustable values are:
- 'PunchRange' : How far the punches can reach [1.2]  (for reference, shovel is 1.5 and knife is 0.75)
- 'PunchCooldown' : How many seconds you must wait between punches [1] (some mods may affect this)
- 'PunchDamage' : How much damage punches deal (1 is equivalent to a shovel/knife hit) [0.5]

- 'StaminaDrain' : How much stamina (% of max stamina) is drained per punch [0]
- 'StaminaRequirement' : How much stamina (% of max stamina) is required to punch [0]
- 'PunchingHaltsStaminaRegen' : Whether punching should halt stamina regen for a brief period [false]

- 'ItemDropMode' : Whether all items, the current item, or no items should be dropped upon squaring up [All]
- 'AllowItems' : Whether items can be held while squared up [false]

### Credits

- [Gemumoddo](https://thunderstore.io/c/lethal-company/p/Gemumoddo/) : For greatly reducing the brain damage induced by the animation part of this mod with their [API](https://thunderstore.io/c/lethal-company/p/Gemumoddo/LethalEmotesAPI)
- Sounds from TF2

### Feedback

Throw your suggestions/complaints/bugs on the [GitHub](https://github.com/ThomasNg2/LethalHands) or directly at me on Discord (dooglett)
