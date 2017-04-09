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

function Hat::onPickup(%this, %obj, %player)
{
	if(%player.tool[%player.hatSlot] != nameToID(noHatIcon))
		return 0;

	%id = %this.getId();
	%player.tool[%player.hatSlot] = %id;
	if (isObject(%player.client))
		messageClient(%player.client, 'MsgItemPickup', '', %player.hatSlot, %id, true);
	%obj.delete();
}

function Hat::onUse(%this, %obj, %slot)
{
	%obj.unMountImage(0);
	%obj.currtool = %slot;
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

package DespairHats
{
	function serverCmdDropTool(%client, %index)
	{
		if(isObject(%player = %client.player) && isObject(%hat = %player.tool[%index]) && %hat.classname $= "Hat")
			%a = true;
		Parent::serverCmdDropTool(%client, %index);
		if(%a)
		{
			%player.tool[%player.hatSlot] = NoHatIcon.getID();
			messageClient(%client, 'MsgItemPickup', '', %player.hatSlot, NoHatIcon.getID(), true);
			%player.unMountImage(2);
			%client.applyBodyParts();
		}
	}

	function Player::activateStuff(%this)
	{
		%item = %this.tool[%this.currTool];
		if (!isObject(%item) || %item.classname !$= "Hat")
		{
			Parent::activateStuff(%this);
			return;
		}
		%item.onWear(%this);
	}

};
activatePackage(DespairHats);