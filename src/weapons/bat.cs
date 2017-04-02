datablock ItemData(BatItem)
{
	shapeFile = $Despair::Path @ "res/shapes/weapons/bat.dts";
	image = BatImage;
	mass = 1;
	drag = 0.3;
	density = 0.2;
	elasticity = 0;
	friction = 1;
	doColorShift = true;
	colorShiftColor = "0.47 0.35 0.2 1";
	uiName = "Baseball Bat";
	canDrop = true;
};

function BatItem::onAdd(%this, %obj)
{
	parent::onAdd(%this, %obj);
	%obj.playThread(0, "root");
}

datablock ShapeBaseImageData(BatImage)
{
	className = "WeaponImage";
	item = BatItem;

	shapeFile = $Despair::Path @ "res/shapes/weapons/bat.dts";
	doColorShift = true;
	colorShiftColor = "0.47 0.35 0.2 1";

	type = "blunt";

	raycastEnabled = 1;
	raycastRange = 4;
	raycastFromEye = true;

	stateName[0]					= "Activate";
	stateAllowImageChange[0]		= 1;
	stateSequence[0]				= "root";
	stateTransitionOnAmmo[0]		= "Blood";

	stateName[2]					= "Blood";
	stateSequence[2]				= "blood";
};

function BatImage::onMount(%image, %player, %slot)
{
	%player.setImageAmmo(%slot, %player.bloody);
}