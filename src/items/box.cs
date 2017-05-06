datablock itemData(KillerBoxItem)
{
	shapeFile = $Despair::Path @ "res/shapes/items/Box.dts";
	image = KillerBoxImage;
	mass = 1;
	drag = 0.3;
	density = 0.2;
	elasticity = 0;
	friction = 1;
	doColorShift = true;
	colorShiftColor = "0.6 0.6 0.6 1";
	uiName = "Box";
	canDrop = true;
};

datablock ShapeBaseImageData(KillerBoxImage)
{
	item = KillerBoxItem;
	shapeFile = $Despair::Path @ "res/shapes/items/Box.dts";
	doColorShift = true;
	colorShiftColor = "0.6 0.6 0.6 1";
	offset = "0 0 0";
	rotation = eulerToMatrix("0 0 0");

	armReady = true;

	stateName[0]					= "Activate";
	stateTimeoutValue[0]			= 0.5;
	stateTransitionOnTimeout[0]		= "Ready";
	stateSound[0]					= "";
	
	stateName[1] = "Ready";
	stateAllowImageChange[1] = true;
	stateTransitionOnTriggerDown[1] = "Use";

	stateName[2] = "Use";
	stateScript[2] = "onUse";
	stateAllowImageChange[2] = false;
	stateTransitionOnTriggerUp[2] = "Ready";
};

function KillerBoxImage::onMount(%this, %obj, %slot)
{
	fixArmReady(%obj);
	if (isObject(%obj.client))
		commandToClient(%obj.client, 'CenterPrint', "<color:FFFF00>This box contains something...\nClick to dump its contents!");
}

function KillerBoxImage::onUnMount(%this, %obj, %slot)
{
	%props = %obj.getItemProps();
	if (isObject(%obj.client))
		commandToClient(%obj.client, 'ClearCenterPrint');
}

function KillerBoxImage::onUse(%this, %obj, %slot)
{
	%count = getRandom(2, 4);
	%loot = "HatSkimaskItem CoatItem LockpickItem CleanSprayItem KnifeItem TaserItem DisguiseItem";
	while(%count-- >= 0)
	{
		%data = getWord(%loot, %i = getRandom(0, getWordCount(%loot)-1));
		%loot = removeWord(%loot, %i);
		%item = new Item() {
			dataBlock = %data;
			position = %obj.getPosition();
		};
		%item.setTransform(%obj.getEyePoint());
		%targVel = vectorSpread(VectorScale(%obj.getEyeVector(), 5), 0.25);
		%item.setVelocity(VectorAdd(%obj.getVelocity(), %targVel));
		%item.setCollisionTimeout(%obj);
	}

	%obj.removeTool(%obj.currTool);

	if (isObject(%obj.client))
		commandToClient(%obj.client, 'CenterPrint', "<color:FFFF00>You dump the contents and discard the box.", 2);
}