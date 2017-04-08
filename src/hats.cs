datablock ItemData(noHatIcon)
{
	iconName = $Despair::Path @ "res/shapes/hats/icon_nohat";
	uiName = "No Hat";
};

function loadHats()
{
	%pattern = $Despair::Path @ "res/shapes/hats/*/hat.txt";
	%list = "generic 0";

	for(%file = findFirstFile(%pattern); %file !$= ""; %file = findNextFile(%pattern))
	{
		%fo = new FileObject();

		if (!%fo.openForRead(%file))
		{
			error("ERROR: Failed to open '" @ %file @ "' for reading");
			%fo.delete();
			continue;
		}

		while (!%fo.isEOF())
		{
			%line = %fo.readLine();
			if(getWord(%line, 0) $= "name")
				%name = getWord(%line, 1);
			if(getWord(%line, 0) $= "disguise")
				%disguise = getWord(%line, 1);
			if(getWord(%line, 0) $= "hidehair")
				%hidehair = getWord(%line, 1);
		}
		talk(%name SPC %disguise SPC %hidehair);
		%shape = findFirstFile(filePath(%file) @ "/*.dts");
		talk(%shape);
		if(%name $= "")
			%name = fileBase(%shape);

		%fo.close();
		%fo.delete();

		%a = "Hat" @ %name @ "Image";
		datablock ItemData(HatItem)
		{
			category = "Hat";
			classname = "Hat";
			shapeFile = %shape;
			image = %a;
			mass = 1;
			drag = 0.3;
			density = 0.2;
			elasticity = 0;
			friction = 1;
			doColorShift = false;
			uiName = %name;
			canDrop = true;
			iconName = $Despair::Path @ "res/shapes/hats/icon_hat";

			disguise = %disguise;
			hidehair = %hidehair;
		};
		%a = "Hat" @ %name @ "Item";
		datablock ShapeBaseImageData(HatImage)
		{
			item = %a;
			shapeFile = %shape;
			doColorShift = false;
			mountPoint = $headSlot;
		};

		if(!isObject(%item = nameToID("HatItem")))
			continue;
		if(!isObject(%image = nameToID("HatImage")))
			continue;

		%item.setName("Hat" @ %name @ "Item");
		%image.setName("Hat" @ %name @ "Image");
		%count++;
	}
	echo("\c1Loaded\c4" SPC %count SPC "\c1hats.");
}

//loadHats();

function Hat::onPickup(%this, %obj, %player)
{
	if(%player.tool[%player.hatSlot] != nameToID(noHatIcon))
		return;

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
		{
			if(isObject(%img = %player.getMountedImage(2)) && %img == %hat.image)
				%player.unMountImage(2);
			%a = true;
		}
		Parent::serverCmdDropTool(%client, %index);
		if(%a)
		{
			%player.tool[%player.hatSlot] = NoHatIcon.getID();
			messageClient(%client, 'MsgItemPickup', '', %player.hatSlot, NoHatIcon.getID(), true);
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