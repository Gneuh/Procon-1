# Procon 1 #
Procon 1 is developed by [Myrcon](https://myrcon.com "Official homepage of Myrcon").

## About Procon ##
Procon is a free remote control (RCON) tool for gameservers, currently supporting Battlefield: Bad Company 2, Battlefield 3, Medal of Honor: Warfighter and Battlefield 4. It is developed by [Myrcon](https://myrcon.com "Official homepage of Myrcon") and also available as open source software on [GitHub](https://github.com/Myrcon/Procon-1 "Procon 1 on GitHub").

In addition to providing basic features to control your gameserver, users can extend Procon's functionality using plugins, which can control Procon's behavior and add additional possibilities for gameserver admins. Furthermore, Procon provides a layer system, which allows running plugins and managing admin accounts in a central location instead of distributing it to every admin connected to the gameserver.


## Support ##
Are you experiencing troubles while using Procon, would like to suggest a new feature or discuss settings and plugins with fellow admins? Feel free to pay our [Myrcon Community](https://forum.myrcon.com "Myrcon Community") a visit!

If you are looking for a list of available plugins, head over to the [plugins section](https://forum.myrcon.com/forumdisplay.php?13-Plugins "Procon 1 plugins") of our forums.


## Known bugs ##
Please refer to the [issues section](https://github.com/Myrcon/Procon-1/issues?labels=bug&page=1&state=open "List of known bugs for Procon 1") for issues tagged as "bug" for a list of known bugs.


## Changelog ##
To check out older changelogs, please refer to our [full changelogs list](https://forum.myrcon.com/showthread.php?240-Full-Change-Log "Full changelog of Procon 1") on the Myrcon Community forums.

### 1.4.2.2 to 1.4.2.3 ###
#### Battlefield 4 ####
- Updates and changes to BF4.def
- on chat tab if show disconnects is selected no player left is shown to reduce status spam
- removed placeholder whitespace in case a player has no clantag transmitted by the server via RCON

#### Core ####
- added plugin wide usable function SecondsToText(UInt32 iSeconds, string[] a_strTimeDescriptions, bool blShowSecondsOver60) to CPRoConMarshalByRefObject.cs
  which converts seconds to d h m s. In case blShowSecondsOver60 is false seconds are only shown if smaller than 60.

#### UI ####
- added PlayTime column for all games. Procon monitors the playtime (session time) since join / procon connect of a player
- added sync of PlayTime for layer clients (procon.player.syncPlayTimes) to have layer clients show the same times the layer host has

#### Default plugins ####
- added @mytimes to have a player request his playtime & @times <playername> to have players request another players playtime to BasicInGameInfo as an usage example


## Credits & contributions ##
Procon and the Procon layer are developed by [Myrcon](https://myrcon.com "Official homepage of Myrcon").

The Battlefield franchise is a product of [DICE](http://dice.se "Digital Illusions Creative Entertainment AB").

Plugins for Procon are developed by third parties, credits and responsibilities lie with the respective plugin author.
