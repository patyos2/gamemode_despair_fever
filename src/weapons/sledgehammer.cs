datablock ItemData(SledgehammerItem)
{
	shapeFile = $Despair::Path @ "res/shapes/weapons/Sledgehammer.dts";
	image = SledgehammerImage;
	mass = 1;
	drag = 0.3;
	density = 0.2;
	elasticity = 0;
	friction = 1;
	doColorShift = true;
	colorShiftColor = "0.5 0.5 0.5 1";
	uiName = "Sledgehammer";
	canDrop = true;

	itemPropsClass = "MeleeProps";
	itemPropsAlways = true;
};

function SledgehammerItem::onAdd(%this, %obj)
{
	parent::onAdd(%this, %obj);
	if(%obj.itemProps.bloody)
		%obj.playThread(0, "blood");
	else
		%obj.playThread(0, "root");
}

datablock ShapeBaseImageData(SledgehammerImage)
{
	className = "WeaponImage";
	item = SledgehammerItem;

	shapeFile = $Despair::Path @ "res/shapes/weapons/Sledgehammer.dts";
	doColorShift = true;
	colorShiftColor = "0.5 0.5 0.5 1";

	isWeapon = true;

	useCustomStates = true;
	type = "blunt";

	armReady = false;

	fireManual = true;

	windUp = 0.3;
	fireDelay = 1;
	fireScript = "onFire";
	meleeRange = 4;

	damage = 65;

	stateName[0]					= "Activate";
	stateAllowImageChange[0]		= 1;
	stateSequence[0]				= "root";
	stateTimeoutValue[0]			= 0.01;
	stateTransitionOnTimeOut[0]		= "CheckBlood";

	stateName[1]					= "CheckBlood";
	stateTransitionOnAmmo[1]		= "Blood";
	stateSequence[1]				= "root";

	stateName[2]					= "Blood";
	stateSequence[2]				= "blood";
};

function SledgehammerImage::onMount(%image, %player, %slot)
{
	%props = %player.getItemProps();
	%player.setImageAmmo(%slot, %props.bloody);
	if(!%player.updateBloody)
	{
		%player.playThread(1, "1hpre1");
		%player.schedule(32, stopThread, 1);
	}
	%player.updateBloody = 0;
}

function SledgehammerImage::onUnMount(%image, %player, %slot)
{
	cancel(%player.windUpSchedule);
}

function SledgehammerImage::onWindUp(%image, %player)
{
	%player.swingType = getRandom(1, 2);
	%player.playThread(1, "1hpre" @ %player.swingType);
	%windUp = %image.windUp*1000;
	%player.lastFireTime = $Sim::Time + %windUp;
	%player.windUpSchedule = %image.schedule(%windUp, onFire, %player);
}

function SledgehammerImage::onFire(%image, %player)
{
	cancel(%player.windUpSchedule);
	%player.swingType = getRandom(1, 2);
	%player.playThread(1, "1hswing" @ %player.swingType);
	%player.lastFireTime = $Sim::Time;
	fireMelee(%image, %player);
}

function SledgehammerImage::onMeleeHit(%image, %player, %object, %position, %normal)
{
	if (!isObject(%object))
		return;

	%damage = %image.damage;
	%props = %player.getItemProps();
	if (%object.getType() & $TypeMasks::PlayerObjectType)
	{
		sprayBloodWide(%position, VectorScale(%normal, -10));
		if(!%props.bloody)
		{
			%props.bloody = %object.health - %damage <= 0 || getRandom() < 0.4; //higher chance to get bloody due to heaviness
			if(%props.bloody)
			{
				%player.updateBloody = 1;
				%player.unMountImage(0); %player.mountImage(%image, 0); //update blood
			}
		}

		if(%props.bloody && getRandom() < 0.6) //Another random chance to get bloody hand
		{
			%player.bloody["rhand"] = true;
			%player.bloody = true;
			if (isObject(%player.client))
				%player.client.applyBodyParts();
		}

		if(%props.bloody && getRandom() < 0.6) //And random chance to get some blood on chest
		{
			%player.bloody["chest_front"] = true;
			%player.bloody = true;
			if (isObject(%player.client))
				%player.client.applyBodyParts();
		}

		return %object.damage(%player, %position, %damage, %image.type);
	}
	if(%object.getType() & $TypeMasks::FxBrickObjectType && %object.getDataBlock().isDoor)
	{
		ServerPlay3D(WoodHitSound, %position);
		return %object.doorDamage(1);
	}
}