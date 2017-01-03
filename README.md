# ObjectManager
A fully functional object manager for vanilla world of warcraft (1.12.1.5875 client)

## Usage
```C#
var process = Process.GetProcessesByName("WoW").FirstOrDefault();
ObjectManager.Start(process);

var me = ObjectManager.Me;
var players = ObjectManager.Players;
var units = ObjectManager.Units;

ObjectManager.Stop();
```
