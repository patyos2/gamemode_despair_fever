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
exec("./lib/los.cs");
exec("./lib/misc.cs");
exec("./lib/noObservers.cs");
//src
exec("./src/blood.cs");
exec("./src/character.cs");
exec("./src/chat.cs");
exec("./src/corpse.cs");
exec("./src/display.cs");
exec("./src/door.cs");
exec("./src/events.cs");
exec("./src/footsteps.cs");
exec("./src/hats.cs");
exec("./src/health.cs");
exec("./src/minigame.cs");
exec("./src/namelist.cs");
exec("./src/player.cs");
exec("./src/statuseffects.cs");
exec("./src/sounds.cs");
exec("./src/trial.cs");
exec("./src/vocal.cs");
exec("./src/weapons.cs");
exec("./src/write.cs");
//items
exec("./src/items/coat.cs");
exec("./src/items/key.cs");
exec("./src/items/mop.cs");
exec("./src/items/paper.cs");
exec("./src/items/pen.cs");
//weapons
exec("./src/weapons/axe.cs");
exec("./src/weapons/bat.cs");
exec("./src/weapons/katana.cs");
exec("./src/weapons/knife.cs");
exec("./src/weapons/leadpipe.cs");
exec("./src/weapons/machete.cs");
exec("./src/weapons/wrench.cs");
exec("./src/weapons/sledgehammer.cs");
exec("./src/weapons/shovel.cs");
exec("./src/weapons/umbrella.cs");