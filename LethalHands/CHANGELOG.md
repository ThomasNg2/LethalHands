### v22.1.5 :
	- Changed the clinger config to be a probability instead of a boolean (suggested by Xeraccoon)

### v22.1.4 :
	- Updated animation parameters to no longer trigger LethalEmotesAPI's animated healthbar player outline (thank u Nunchuk part 2)

### v22.1.3 :
	- Added a config option for player damage

### v22.1.2 (v55/56) :
	- Added fox tongues to the list of clingers
	- Sitting in the Company Cruiser now exits the fighting mode

### v22.1.1 :
	- Added a config option to toggle being able to punch snarefleas/tulip snakes (suggested by teacyn)
	- Switched to a different system for cooldowns : 
		- The cooldown should be more accurate when using mods like MoreCompany
		- Punching at incredible hihg speed is possible again

### v22.1.0 :
	- Punches now do fixed, possibly non integer damage
	- Dead enemies no longer absorb punches

### v22.0.10 :
	- Fixed 'Current' item drop option not allowing squaring up unless an item is held
	- Fixed trying to interact while squared up exiting the fighting mode

### v22.0.9 :
	- Added an extra check to exit the fighting mode after picking up an item (credits to NecroWing for sparing me some research)
	- Added extra config options regarding items :
		- Which items are dropped upon squaring up : all, current or none (all by default)
		- Whether or not holding an item in the fighting mode is allowed (false by default)

### v22.0.8 :
	- Fixed an issue with TooManyEmotes that would prevent the player from scanning/punching

### v22.0.7 :
	- Implemented the changes from the MeleeFixes mod by ButteryStancakes (suggested by NeatWolf)
	- Terrain now plays its sound when punched
	- Slightly delayed the punch hit registration to sync the hit with the animation
	- Added more stamina-related config options (suggested by Kondakov02) :
		- Stamina required to perform a punch (0% by default)
		- Punching stops stamina regen for some time (false by default)
	

### v22.0.6 :
	- Animations now use new parameters to force first person instead of overwriting LethalEmotesAPI's settings (thank u Nunchuk)

### v22.0.5 :
	- Added configuration for punches (range, cooldown, damage, stamina drain,chance to deal damage)
	- Adjusted some default values : 
		- Punch range : 1f -> 1.2f (knife is 0.75f and shovel is 1.5f for reference)
		- Stamina drain : 10% -> 0%
	- Fixed being able to throw punches while climbing ladders

### v22.0.4 :
	- Changed control tips to reflect the correct controls (any instance of [RMB] in the tips is replaced by the game with [LMB] for some reason)
	- The mod now changes LethalEmotesAPI's third person mode to Normal upon loading, this should fix people being in third person

### v22.0.3 :
	- Fixed being able to punch while being killed by a masked

### v22.0.2 :
	- Disabled scanning while squared up

### v22.0.1 :
	- File structure should now be correct for mod manager installation

### v22.0.0 :
	- I'm freeeeeeeeeeeeeeeeeeee