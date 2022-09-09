# VortexGDO

## Overview

A BepInEx plugin for PotionCraft. Without the mod, the teleport exit point for a given vortex is only visible after you have teleported through it once. With the mod, you only need to pass close to the vortex for the exit point to appear (the exact distance being configurable). The plugin also includes a couple of debug commands:

- `VortexGDO-SetThreshold:`Takes a single float argument that becomes the new threshold value used to determine whether to unlock the nearest vortex. Will accept a value between 0.1 and 2.5 (any value outside that range will be clamped to the which ever is closest, 0.1 or 2.5).
- `VortexGDO-UnlockAll:`Unlocks all vortexes on the current map

## Installation

 1. Download the latest version of [BepInEx v5 x64](https://github.com/BepInEx/BepInEx/releases/tag/v5.4.21)
 2. Extract the contents of the zip to the same folder as the game executable
 3. Run the game once to generate necessary folders and files
 4. Download the latest version of [this plugin](https://github.com/rswallen/VortexGDO/releases)
 5. Extract the contents of the download to \BepInEx\plugins\
 6. Run the game