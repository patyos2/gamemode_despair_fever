function MeleeProps::onAdd(%this)
{
	%this.bloody = false;
}

function Player::tryStartFireWeapon(%player, %manual)
{
	%image = %player.getMountedImage(0);

	if (%manual || (!%image.fireManual && %player.getImageTrigger(0)))
		%player.fireWeapon(false, %manual);
}

function Player::fireWeapon(%player, %ignoreTime, %manual)
{
	cancel(%player.fireSchedule);

	if (%player.health <= 0 || (!%player.getImageTrigger(0) && !%manual))
		return;

	%image = %player.getMountedImage(0);

	if (!%image)
		return;
	
	%time = %image.fireDelay;
	if ($Sim::Time - %player.lastFireTime < %time && !%ignoreTime)
	{
		if (!%image.fireManual)
		{
			%time -= $Sim::Time - %player.lastFireTime;
			%player.fireSchedule = %player.schedule(%time * 1000, fireWeapon, true);
		}
		return;
	}

	%func = %image.fireScript;
	if(%image.fireScript $= "")
		%func = "onFire";

	if (!eval("return %image."@%func@"(%player);"))
		return;

	%player.lastFireTime = $Sim::Time;

	if (!%image.fireManual)
		%player.fireSchedule = %player.schedule(%time * 1000, fireWeapon, true);
}

function fireMelee(%image, %player)
{
	%angle = %image.meleeAngle;
	%range = %image.meleeRange;
	%count = %image.meleeCount;
	// if (%angle $= "") %angle = 1.57079 * 0.5;
	if (%angle $= "") %angle = 1.57079 * 0.75;
	if (%range $= "") %range = 3.5;
	if (%count $= "") %count = 4; // 3;

	%start = %angle * -0.5;
	%i = %count;
	%count--;
	while (%i-- >= 0)
	{
		%time = 100 * ((1 - %i) / %count);
		%yaw = %start + %angle * (%i / %count);
		schedule(%time, %player, fireMeleeCheck, %image, %player, %range, %yaw);
	}
}

function fireMeleeCheck(%image, %player, %range, %yaw)
{
	if (%player.health <= 0 || %player.getMountedImage(0) != %image)
		return;

	%origin = %player.getEyePoint();
	// %b = VectorAdd(%a, MatrixMulVector("0 0 0 " @ %player.getUpVectorHack() @ " " @ %yaw,
	//         VectorScale(%player.getAimVector(), %range)));
	%a = VectorAdd(%origin, MatrixMulVector("0 0 0 0 0 1 " @ (%yaw * 2.5),
			VectorScale(%player.getAimVector(), 0.5)));
	%b = VectorAdd(%origin, MatrixMulVector("0 0 0 0 0 1 " @ %yaw,
			VectorScale(%player.getAimVector(), %range)));

	%ray = containerRayCast(%a, %b, $TypeMasks::PlayerObjectType | $TypeMasks::FxBrickObjectType, %player);
	%col = firstWord(%ray);
	// schedule(1500, 0, freeLine, drawLine("", %a, %b, "1 0 0 1"));

	if (!%col)
		return;
	
	if (!(%col.getType() & $TypeMasks::PlayerObjectType))
	{
		if ($Sim::Time - %player.lastMeleeTerrainHit < 0.1)
			return;
		%player.lastMeleeTerrainHit = $Sim::Time;
	}

	if ($Sim::Time - %player.lastMeleeHitTo[%col] < 0.1)
		return;
	%player.lastMeleeHitTo[%col] = $Sim::Time;

	%image.onMeleeHit(%player, %col, getWords(%ray, 1, 3), getWords(%ray, 4, 6));
}

package DespairWeapons
{
	function Armor::onTrigger(%data, %player, %slot, %state)
	{
		Parent::onTrigger(%data, %player, %slot, %state);
		if (%slot != 0)
			return;
		
		%image = %player.getMountedImage(0);

		if (!isObject(%image) || !%image.useCustomStates)
			return;

		if (%state)
			%player.tryStartFireWeapon(true);
		else
			cancel(%player.fireSchedule);
	}
};

activatePackage("DespairWeapons");