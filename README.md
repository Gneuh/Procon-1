# Procon 1 #

Procon 1 is developed by [Myrcon Ptd. Ltd.](https://myrcon.com "Official homepage of Myrcon Ptd. Ltd.").

## About Procon ##
Procon is a free remote control (RCON) tool for gameservers, currently supporting Battlefield: Bad Company 2, Battlefield 3, Medal of Honor: Warfighter and Battlefield 4. It is developed by [Myrcon Ptd. Ltd.](https://myrcon.com "Official homepage of Myrcon Ptd. Ltd.") and also available as open source software on [GitHub](https://github.com/Myrcon/Procon-1 "Procon 1 on GitHub").

In addition to providing basic features to control your gameserver, users can extend Procon's functionality using plugins, which can control Procon's behavior and add additional possibilities for gameserver admins. Furthermore, Procon provides a layer system, which allows running plugins and managing admin accounts in a central location instead of distributing it to every admin connected to the gameserver.


## Support ##
Are you experiencing troubles while using Procon, would like to suggest a new feature or discuss settings and plugins with fellow admins? Feel free to pay our [Myrcon Community](https://forum.myrcon.com "Myrcon Community") a visit!

If you are looking for a list of available plugins, head over to the [plugins section](https://forum.myrcon.com/forumdisplay.php?13-Plugins "Procon 1 plugins") of our forums.


## Known bugs ##
Please refer to the [issues section](https://github.com/Myrcon/Procon-1/issues?labels=bug&page=1&state=open "List of known bugs for Procon 1") for issues tagged as "bug" for a list of known bugs.


## Changelog ##
To check out older changelogs, please refer to our [full changelogs list](https://forum.myrcon.com/showthread.php?240-Full-Change-Log "Full changelog of Procon 1") on the Myrcon Community forums.

### 1.4.2.0 to 1.4.2.1 ###
#### Battlefield 4 ####
- Added parsing of R27's *player.onDisconnect* event
- Added weapon definitions and localisation for vehicle RCON codes
- Added new weapon definitions from *Naval Strike* DLC
- Fixed errors in categorisation of weapons

#### Core ####
- Fixed *vars.teamFactionOverride* not being recognised by Procon layers
- Plugin configs are now split into different files, preventing all information from being lost if once configuration fails to load/save
- Fixed crash caused by invalid parsing of decimal numbers
- Fixed crash caused by failing to load localisation properly
- Fixed error causing a connection to get reconnected after pressing "disconnect"

#### UI ####
- Added *OnPlayerDisconnected* messages to chat tab with checkbox to disable them
- Added check for current round count not being able to go above the maximum round count

#### Plugins ####
- Added option to enable compilation of plugins with debug information
- Added plugin API for *OnPlayerDisconnected* event
- Fixed InGameAdmin (f)move without destination on other games than BFBC2


## Credits & contributions ##
Procon and the Procon layer are developed by [Myrcon Ptd. Ltd.](https://myrcon.com "Official homepage of Myrcon Ptd. Ltd.").

The Battlefield franchise is a product of [DICE](http://dice.se "Digital Illusions Creative Entertainment AB").

Plugins for Procon are developed by third parties, credits and responsibilities lie with the respective plugin author.
