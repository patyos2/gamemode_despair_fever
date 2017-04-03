$Despair::Path = filePath(expandFileName("./description.txt")) @ "/";
function r(%p)
{
	setModPaths(getModPaths());
	if (%p !$= "")
		exec("./src/" @ %p @ ".cs");
	else
		exec("./server.cs");
}

//lib
exec("./lib/daycycles.cs");
exec("./lib/decals.cs");
exec("./lib/hitregion.cs");
exec("./lib/itemfuncs.cs");
exec("./lib/itemprops.cs");
exec("./lib/misc.cs");
//src
exec("./src/blood.cs");
exec("./src/door.cs");
exec("./src/health.cs");
exec("./src/minigame.cs");
exec("./src/player.cs");
exec("./src/weapons.cs");
//items
exec("./src/items/key.cs");
//weapons
exec("./src/weapons/sword.cs");
exec("./src/weapons/bat.cs");
exec("./src/weapons/wrench.cs");