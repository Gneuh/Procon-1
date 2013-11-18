# Procon 1 #
----------

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

### 1.4.1.1 to 1.4.1.2 ###
#### Battlefield 4 ####
- Changed representation of "commo rose" messages in chat tab and added option to hide them
- Fixed bug displaying current round one value too low (now shows 1/1 rounds)
- Hid banner URL option and map tab since they are no longer functional
- Removed unknown/old commands being fired on startup
- Added setting for vars.alwaysAllowSpectators and vars.preset
- Updated and fixed weapon definitions and localisation

#### UI ####
- Fixed bug displaying a player's rank as their ping
- Added "add player to spectator slot list" via right click in playerlist
- Removed smooth fading animation of connect/disconnect buttons on start page
- Changed ping to be shown as -1 if above 5000 to filter out invalid responses from the gameserver

#### Core ####
- Fixed bug triggering a reconnect even after pressing "disconnect"
- Fixed bug preventing the MySQL connector from working with sandbox enabled

#### Plugin ####
- Fixed bug preventing all plugins from loading if one of them crashed during compilation
- Fixed bug parsing string-values of plugin settings not properly


## Credits & contributions ##
Procon and the Procon layer are developed by [Myrcon Ptd. Ltd.](https://myrcon.com "Official homepage of Myrcon Ptd. Ltd.").

The Battlefield franchise is a product of [DICE](http://dice.se "Digital Illusions Creative Entertainment AB").

Plugins for Procon are developed by third parties, credits and responsibilities lie with the respective plugin author.