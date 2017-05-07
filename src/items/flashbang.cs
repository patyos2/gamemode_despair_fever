datablock AudioProfile(flashBangSound)
{
	filename    = $Despair::Path @ "res/sounds/weapons/flashbang.wav";
	description = AudioClose3d;
	preload = false;
};

datablock ExplosionData(FlashbangExplosion)
{
	explosionShape = "Add-Ons/Weapon_Rocket_Launcher/explosionsphere1.dts";
	lifeTimeMS = 150;

	soundProfile = flashBangSound;

	faceViewer     = true;
	explosionScale = "1 1 1";

	shakeCamera = true;
	camShakeFreq = "7.0 8.0 7.0";
	camShakeAmp = "1.0 1.0 1.0";
	camShakeDuration = 0.5;
	camShakeRadius = 15.0;

	// Dynamic light
	lightStartRadius = 0;
	lightEndRadius = 0;
	lightStartColor = "0.45 0.3 0.1";
	lightEndColor = "0 0 0";

	//impulse
	impulseRadius = 17;
	impulseForce = 2000;

	//radius damage
	damageRadius = 17;
	radiusDamage = 1;
};

datablock ProjectileData(FlashbangExplosionProjectile)
{
	directDamageType	= $DamageType::RocketDirect;
	radiusDamageType	= $DamageType::RocketRadius;
	explosion 			= FlashbangExplosion;
	particleEmitter     = "";
	sound				= "";

	velInheritFactor    = 1;
	explodeOnDeath		= true;

	armingDelay         = 0; 
	lifetime            = 0;
	fadeDelay           = 0;
};

function FlashbangExplosionProjectile::radiusDamage(%this, %obj, %col, %factor, %pos, %force)
{
	if(isObject(%col.client) && !%col.client.killer)
		%col.knockOut(20);
}

datablock ItemData(FlashbangItem)
{
	image = FlashbangImage;
	shapeFile = $Despair::Path @ "res/shapes/items/flashlight.dts";

	uiName = "Flashlight";
	canDrop = true;

	doColorShift = true;
	colorShiftColor = "0.3 0.3 0.35 1";

	image = FlashbangImage;

	mass = 1;
	drag = 0.3;
	density = 0.2;
	elasticity = 0;
	friction = 1;
	emap = true;

	itemPropsClass = "ExplosiveProps";
	itemPropsAlways = true;

	customPickupAlways = true;
	customPickupMultiple = false;
};

function ExplosiveProps::onAdd(%this)
{
	%this.primed = false;
	%this.timer = 5;
}

function ExplosiveProps::timerSchedule(%this)
{
	cancel(%this.timerSchedule);
	if(!%this.primed)
		return;
	if(%this.timer <= 0)
	{
		%projectile = new projectile()
		{
			dataBlock = FlashbangExplosionProjectile;
			initialPosition = %this.owner.getPosition();
			initialVelocity = %this.owner.getVelocity();
			sourceObject = %this.sourcePlayer;
			client = %obj.sourceClient;
		};
		MissionCleanup.add(%projectile);
		%projectile.explode();
		if(%this.owner.getType() & $TypeMasks::itemObjectType)
			%this.owner.delete();
		else if(%this.owner.getType() & $TypeMasks::playerObjectType)
		{
			%this.owner.removeTool(%this.itemSlot, 1, 1);
			if(%this.owner.client.killer)
				%this.owner.knockOut(20);
		}
		if(isObject(%this))
			%this.delete();
		return;
	}
	%this.timer--;
	%this.timerSchedule = %this.schedule(1000, timerSchedule);
}

datablock ShapeBaseImageData(FlashbangImage)
{
	shapeFile = $Despair::Path @ "res/shapes/items/flashlight.dts";
	hasLight = true;

	item = FlashbangItem;

	emap = true;
	offset = "0 0 0";
	mountPoint = 0;
	armReady = false;

	doColorShift = true;
	colorShiftColor = "0.3 0.3 0.35 1";

	stateName[0]					= "Activate";
	stateTimeoutValue[0]			= 0.01;
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

function FlashbangImage::onMount(%this, %obj, %slot)
{
	fixArmReady(%obj);
	%props = %obj.getItemProps();
	if (isObject(%obj.client) && %obj.client.killer)
		commandToClient(%client, 'CenterPrint', "<color:FFFF00>This is a \c6flashbang\c3 disguised as a flashlight!\nPrime time is \c6" @ %props.timer @ " seconds\c3. Be sure to drop it, or you'll be knocked out as well!");
}

function FlashbangImage::onUnMount(%this, %obj, %slot)
{
	if (isObject(%obj.client) && %obj.client.killer)
		commandToClient(%obj.client, 'ClearCenterPrint');
}

function FlashbangImage::onUse(%this, %obj, %slot)
{
	%props = %obj.getItemProps();
	if(!isObject(%client = %obj.client))
		return;
	if(!%client.killer)
	{
		commandToClient(%client, 'CenterPrint', "\c3It seems broken...");
		return;
	}
	if(!%props.primed)
	{
		commandToClient(%client, 'CenterPrint', "\c3It is primed to explode in \c6" @ %props.timer @ " seconds\c3!\n\c5Throw it away!");
		%props.primed = true;
		%props.timerSchedule();
	}
}