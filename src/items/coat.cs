datablock ItemData(CoatItem)
{
	shapeFile = $Despair::Path @ "res/shapes/items/coat.dts";
	image = CoatImage;
	mass = 1;
	drag = 0.3;
	density = 0.2;
	elasticity = 0;
	friction = 1;
	doColorShift = false;
	uiName = "Coat";
	canDrop = true;

	hideAppearance = true; //hides arms, chest and pants
};
datablock ShapeBaseImageData(CoatImage)
{
	item = CoatItem;
	shapeFile = $Despair::Path @ "res/shapes/items/coat.dts";
	doColorShift = false;
	mountPoint = 2;
	eyeOffset = "0 0 -50";
};

function CoatItem::onUse(%this, %obj, %slot)
{
	%obj.unMountImage(0);
}


//Functions defined by hats.cs
function CoatItem::onWear(%this, %player)
{
	if(isObject(%img = %player.getMountedImage(3)) && %img != %this.image)
	{
		%player.unMountImage(3);
	}
	else
	{
		%player.mountImage(%this.image, 3);
	}
	if(isObject(%player.client))
		%player.client.applyBodyParts();
}

function CoatItem::onDrop(%this, %player, %index)
{
	%player.unMountImage(3);

	if(isObject(%client = %player.client))
	{
		messageClient(%client, 'MsgItemPickup', '', %index, "", true);
		%client.applyBodyParts();
	}
}