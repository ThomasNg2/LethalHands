### v22.0.7 :
	- Implemented the changes from the MeleeFixes mod by ButteryStancakes (suggested by NeatWolf)
	- Terrain now plays its sound when punched
	- Slightly delayed the punch hit registration to sync the hit with the animation
	- Added more stamina-related config options (suggested by Kondakov02) :
		- Stamina required to perform a punch (0% by default)
		- Punching stops stamina regen for some time (false by default)
	

### v22.0.6 :
	- Animations now use new parameters to force first person instead of overwriting LethalEmotesAPI's settings

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