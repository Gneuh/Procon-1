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

### 1.4.1.0 to 1.4.1.1 ###
#### Battlefield 4 ####
- Added the hit indicator var to plugin events & the bf4 gameplay settings panel
- Removes the spectator slot list if client is not bf4
- Added the settings panel controls and plugin events for command & server type, replacing the ranked checkbox.
- Added new variables to CServerInfo for blaze.
- Added support through out for vars.forceReloadWholeMags
- Added in vars pass through for vars we have not implemented in the server settings panel yet.

#### UI ####
- Moved minimum size of frm main to 800x600, down from 1024x768

#### Core ####
- Cleanup the layer connection handling a little bit
- Fix for new reconnection handler not locking modifcation from other threads before checking a collection of sent packets.

#### Plugin ####
- Removed deprecated OnPlayerKilled(name, name) method from plugins. It's been marked as deprecated for two years (Since BFBC2 R9)
- Removed deprecated maplist plugin command that would break the maplist back into a simple list of map names, passing them (Since BFBC2 R9)
- Sort of reverted a change where we cache the reflection lookup of a method. Instead we've met it half way, it's still improved on what we had originally but only marginally.


## Credits & contributions ##
Procon and the Procon layer are developed by [Myrcon Ptd. Ltd.](https://myrcon.com "Official homepage of Myrcon Ptd. Ltd.").

The Battlefield franchise is a product of [DICE](http://dice.se "Digital Illusions Creative Entertainment AB").

Plugins for Procon are developed by third parties, credits and responsibilities lie with the respective plugin author.