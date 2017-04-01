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
exec("./lib/itemfuncs.cs");
exec("./lib/itemprops.cs");

//src
exec("./src/player.cs");
exec("./src/minigame.cs");

//items
exec("./src/items/key.cs");
//weapons
