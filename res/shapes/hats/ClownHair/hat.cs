datablock ItemData(HatClownItem)
{
	category = "Hat";
	classname = "Hat";
	shapeFile = "./Clown.dts";
	image = HatClownImage;
	mass = 1;
	drag = 0.3;
	density = 0.2;
	elasticity = 0;
	friction = 1;
	doColorShift = false;
	uiName = "Hat Clown";
	canDrop = true;
	iconName = $Despair::Path @ "res/shapes/hats/icon_hat";

	disguise = true;
	disguiseName = "Bozo the Clown";
	hidehair = false;

	waitForKiller = true; //Wait for killer to be picked before this can be picked up
};
datablock ShapeBaseImageData(HatClownImage)
{
	item = HatClownItem;
	shapeFile = "./Clown.dts";
	doColorShift = false;
	mountPoint = $headSlot;
	eyeOffset = "0 0 -50";
};
