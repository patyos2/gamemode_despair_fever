datablock itemData(DisguiseItem)
{
	shapeFile = $Despair::Path @ "res/shapes/items/satchel.dts";
	image = DisguiseImage;
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

datablock ShapeBaseImageData(DisguiseImage)
{
	item = DisguiseItem;
	shapeFile = "base/data/shapes/empty.dts";
	armReady = false;

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

function DisguiseImage::onMount(%this, %obj, %slot)
{
	fixArmReady(%obj);
	if (isObject(%obj.client))
		commandToClient(%obj.client, 'CenterPrint', "<color:FFFF00>Click on a corpse to completely steal their identity.\nThis is one use and will mangle the corpse beyond recognition.");
}

function DisguiseImage::onUnMount(%this, %obj, %slot)
{
	if (isObject(%obj.client))
		commandToClient(%obj.client, 'ClearCenterPrint');
}

function DisguiseImage::onUse(%this, %obj, %slot)
{
	%a = %obj.getEyePoint();
	%b = vectorAdd(%a, vectorScale(%obj.getEyeVector(), 5));

	%mask =
		$TypeMasks::FxBrickObjectType |
		$TypeMasks::PlayerObjectType;

	%ray = containerRayCast(%a, %b, %mask, %obj);
	if(%ray && %ray.getType() & $TypeMasks::PlayerObjectType)
	{
		if(%ray.isDead)
		{
			%obj.character.appearance = %ray.character.appearance;
			%obj.fakeName = %ray.character.name;
			%obj.applyAppearance();
			%ray.applyAppearance();
		}
		else
		{
			commandToClient(%obj.client, 'CenterPrint', "<color:FFFF00>Target must be dead for this to work!", 2);
			return;
		}
	}

	%obj.removeTool(%obj.currTool);

	if (isObject(%obj.client))
		commandToClient(%obj.client, 'CenterPrint', "<color:FFFF00>You take on the appearance of\c6" SPC %name, 2);
}