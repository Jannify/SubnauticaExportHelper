# SubnauticaExportHelper, a plugin for AssetRipper

This is a plugin for the unity ripping software AssetRipper. It fixes a lot of errors in the ripped output and makes the play mode usable.

## Instructions for making Subnautica mostly playable in the Unity Editor

#### Time needed: 15-40 min ripping 1/2-2 hours unity importing

###### 1. Download the GUI version of [AssetRipper](https://github.com/AssetRipper/AssetRipper/releases)

###### 2. Download [SubnauticaExportHelper](https://github.com/Jannify/SubnauticaExportHelper/releases)

    Create a folder named "Plugins" next to "AssetRipper.exe".
    Extract the content of the downloaded SubnauticaExportHelper zip in this folder.

###### 3. Rip Subnautica with AssetRipper

    Start "AssetRipper.exe"
    Change the "Shader Export Format" to "YAML Asset"
    Click on "File/Open File" and open your Subnautica.exe
    Wait for the program to load the game
    Click on "Export/Export all Files" and choose your desired output folder

###### 4. If the rip is completed, open the project with the Unity Editor (it has to be 2019.2.17f1)

    If the "API Update Required" window pops up click "I Made a Backup. Go Ahead!"
    If the "Missing AssetBundles" window pops up click "Yes".

###### 5. Congratulations :D Your Subnautica in the Editor is now usable

## A few tips on using the project

###### 1. To start the game properly in play mode you need to open "Scenes/StartScreen.unity"

###### 2. Errors (and warnings) are completely normal and can be ignored in 98% of the cases. They should not effect the playability
