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

### 1.4.1.2 to 1.4.1.3 ###
#### Battlefield 4 ####
- Added re-fetching of all server-variables if "vars.preset" is triggered with the "override" setting
- Added missing settings for "vars.roundTimeLimit" and "vars.teamKillKickForBan"
- Updated missing weapon definitions and added *China Rising* maps to maplist
- Added DamageTypes for DMRs and Carbines
- Changed "Disable idle kick" to send "vars.idleTimeout 86400" instead of "vars.idleTimeout 0"

#### UI ####
- Adapted limit for "vars.idleTimeout" setting to allow new maximum value
- Fixed default URL for bf4stats.com
- Removed the "test connection" link when starting a Procon Layer since it hasn't been functional in ages
- Fixed "Unlock mode" setting not working properly

#### Core ####
- Added check to shut down existing client-connection to a Procon Layer if another connection from the same IP/port is being opened
- Improved linux compatibility a bit and removed a compiler warning
- Slightly modified account permissions to allow users with "Use map functions" permissions to select/run maps, but not modify the maplist
- Changed encoding of some definition files from UTF-16LE to UTF-8
- Added support for different encodings for localisation files
- Updated GeoIP database used for displaying a user's country

#### Plugins ####
- Updated default plugins to use the latest plugin API


## Credits & contributions ##
Procon and the Procon layer are developed by [Myrcon Ptd. Ltd.](https://myrcon.com "Official homepage of Myrcon Ptd. Ltd.").

The Battlefield franchise is a product of [DICE](http://dice.se "Digital Illusions Creative Entertainment AB").

Plugins for Procon are developed by third parties, credits and responsibilities lie with the respective plugin author.
