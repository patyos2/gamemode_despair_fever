datablock ItemData(HatMummyItem)
{
	category = "Hat";
	classname = "Hat";
	shapeFile = "./Mummy.dts";
	image = HatMummyImage;
	mass = 1;
	drag = 0.3;
	density = 0.2;
	elasticity = 0;
	friction = 1;
	doColorShift = false;
	uiName = "Hat Mummy";
	canDrop = true;
	iconName = $Despair::Path @ "res/shapes/hats/icon_hat";

	disguise = true;
	hidehair = false;
	replaceHairMale = "hair_messy";
	replaceHairFemale = "hair_ponytail";
};
datablock ShapeBaseImageData(HatMummyImage)
{
	item = HatMummyItem;
	shapeFile = "./Mummy.dts";
	doColorShift = false;
	mountPoint = $headSlot;
	eyeOffset = "0 0 -50";
};
