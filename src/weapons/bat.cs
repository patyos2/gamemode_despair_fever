datablock ItemData(BaseballItem)
{
	shapeFile = $Despair::Path @ "res/shapes/weapons/baseballbat.dts";
	image = BaseballImage;
	mass = 1;
	drag = 0.3;
	density = 0.2;
	elasticity = 0;
	friction = 1;
	doColorShift = true;
	colorShiftColor = "0.47 0.35 0.2 1";
};

datablock ShapeBaseImageData(BaseballImage)
{
	shapeFile = $Despair::Path @ "res/shapes/weapons/baseballbat.dts";
	item = BaseballItem;
	doColorShift = true;
	colorShiftColor = "0.47 0.35 0.2 1";

	name = "Baseball Bat";
	type = "melee";
	fireDelay = 0.6;
	moveSpeed = 0.95;
	points = 800;
	blunt = true;
	canExecute = true;
	pickup = true;
	meleeRange = 4;
};

function BaseballImage::onMount(%image, %player, %slot)
{
	%player.lastMeleeSide = 0;
	%player.playThread(1, swing1);
	%player.schedule(32, stopThread, 1);
}

function BaseballImage::onFire(%image, %player)
{
	%player.playThread(1, %player.lastMeleeSide ? swing2 : swing1);

	%player.lastMeleeSide = !%player.lastMeleeSide;
	%player.lastFireTime = $Sim::Time;

	play3D(MeleeMediumSwing @ getRandom(1, 4), %player.getSlotTransform(0), %player.client);
	return fireMelee(%image, %player);
}

function BaseballImage::onMeleeHit(%image, %player, %object, %position, %normal)
{
	if (!isObject(%object))
		return;
	
	if (%object.getType() & $TypeMasks::PlayerObjectType)
	{
		play3D(BaseballBatHitPlSound @ getRandom(1, 2), %position);
		sprayBloodWide(%position, VectorScale(%normal, 6));
		%damage = 1;
		if (%player.inManualExecution)
		{
			%damage = 0.35;
			cancel(%player.executionSchedule);
			%player.executionSchedule = %player.schedule(1000, executeManualTimeout);
		}
		return %object.damage(%damage, %player.client, %image, %position);
	}

	play3D(BaseballBatHitEnvSound @ getRandom(1, 2), %position);
}