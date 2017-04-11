$pattern = $Despair::Path @ "res/shapes/hats/*.cs";
echo("\c4Executing hats...");
for ( $file = findFirstFile( $pattern ) ; $file !$= "" ; $file = findNextFile( $pattern ) )
{
	exec( $file );
}
echo("\c4Done!");

datablock ItemData(noHatIcon)
{
	iconName = $Despair::Path @ "res/shapes/hats/icon_nohat";
	uiName = "No Hat";
};

function noHatIcon::onUse(%this, %obj, %slot)
{
	%obj.currtool = -1;
	fixArmReady(%obj);
}

function Hat::onPickup(%this, %obj, %player)
{
	if(%player.tool[%player.hatSlot] != nameToID(noHatIcon))
		return false;

	%id = %this.getId();
	%player.tool[%player.hatSlot] = %id;
	if (isObject(%player.client))
		messageClient(%player.client, 'MsgItemPickup', '', %player.hatSlot, %id, false);
	%obj.delete();
	return true;
}

function Hat::onUse(%this, %obj, %slot)
{
	%obj.unMountImage(0);
	%obj.currtool = %slot;
	fixArmReady(%obj);
}

function Hat::onWear(%this, %player)
{
	if(isObject(%img = %player.getMountedImage(2)) && %img != %this.image)
	{
		%player.unMountImage(2);
	}
	else
	{
		%player.mountImage(%this.image, 2);
	}
	if(isObject(%player.client))
		%player.client.applyBodyParts();
}

function Hat::onDrop(%this, %player, %index)
{
	%player.tool[%player.hatSlot] = NoHatIcon.getID();
	%player.unMountImage(2);
	%player.currtool = -1;
	if(isObject(%client = %player.client))
	{
		messageClient(%client, 'MsgItemPickup', '', %player.hatSlot, NoHatIcon.getID(), true);
		%client.applyBodyParts();
	}
}

package DespairHats
{
	function serverCmdDropTool(%client, %index)
	{
		if(isObject(%client.player))
			%item = %client.player.tool[%index];
		Parent::serverCmdDropTool(%client, %index);
		if(isObject(%item) && isFunction(%item.getName(), "onDrop"))
			%item.onDrop(%client.player, %index);
	}

	function Player::activateStuff(%this)
	{
		%item = %this.tool[%this.currTool];
		if (!isObject(%item) || !isFunction(%item.getName(), "onWear"))
		{
			Parent::activateStuff(%this);
			return;
		}
		%item.onWear(%this);
	}

};
activatePackage(DespairHats);