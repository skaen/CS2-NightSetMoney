# CS2-NightSetMoney
Set or give money to players at night

# Installation
1. Install [CounterStrike Sharp](https://github.com/roflmuffin/CounterStrikeSharp) and [Metamod:Source](https://www.sourcemm.net/downloads.php/?branch=master)
3. Download [CS2-NightSetMoney](https://github.com/skaen/CS2-NightSetMoney/releases)
4. Unzip the archive and upload it to the game server

# Config
The config is created automatically in the place: `addons/counterstrikesharp/configs/plugins/NightSetMoney`
```
{
	"PluginStartTime": "20:00:00",		
	"PluginEndTime": "06:00:00",		
	"Money": "16000", 					// If you put ++ before the number, the amount will be added in each round. "Money": "16000" set a fixed amount
	"SkipPistolRound": true,			// Skip the pistol round (true) / Give out money on the pistol round (false)
}
```

# Commands
`css_stime / stime` - find out the current server time (only from the console)
