# LethalHands

Allows the usage of your hands to fight back in dire situations.

### Usage

Press 'J' (rebindable) to toggle the ability to throw left (LMB) and right (RMB) punches.
For most entities, punches will do half the damage of a standard shovel.
Upon squaring up, all your items will be dropped. This along with a few balancing aspects can be changed in the mod's config.


### Installation

The future is now, use a mod manager you fool

Every player needs to have the mod installed.

### Configuration

How balanced the mod is can be adjusted in the `BepInEx/config/SlapitNow.LethalHands.cfg` file or using [LethalConfig](https://thunderstore.io/c/lethal-company/p/AinaVT/LethalConfig/).
Upon joining a lobby, the host's config will be used.

The adjustable values are:
- 'PunchRange' : How far the punches can reach [1.2]  (for reference, shovel is 1.5 and knife is 0.75)
- 'PunchCooldown' : How many seconds you must wait between punches [1] (some mods may affect this)
- 'EnemyPunchDamage' : How much damage punches deal to enemies (1 is equivalent to a shovel/knife hit) [0.5]
- 'PlayerPunchDamage' : How much damage punches deal to other players (shovels/knives do 20 per hit) [10]

- 'PunchOffClingersChance' : How likely (%) your own punches are to hit snarefleas/tulip snakes/fox tongues that are attached to yourself [100%]

- 'StaminaDrain' : How much stamina (% of max stamina) is drained per punch [0%]
- 'StaminaRequirement' : How much stamina (% of max stamina) is required to punch [0%]
- 'PunchingHaltsStaminaRegen' : Whether punching should halt stamina regen for a brief period [false]

- 'ItemDropMode' : Whether all items, only the 4 main slot items, the current item, or no items should be dropped upon squaring up [All]
- 'AllowItems' : Whether items can be held while squared up [false]

### Credits

- [Gemumoddo](https://thunderstore.io/c/lethal-company/p/Gemumoddo/) : For greatly reducing the brain damage induced by the animation part of this mod with their [API](https://thunderstore.io/c/lethal-company/p/Gemumoddo/LethalEmotesAPI)
- Sounds from TF2

### Feedback

Throw your complaints/bugs :
- in the dedicated thread in the [Lethal Company Modding Discord](https://discord.gg/lcmod)
- on [GitHub](https://github.com/ThomasNg2/LethalHands)

Suggestions are also appreciated but I'm no longer looking to expand the mod.
