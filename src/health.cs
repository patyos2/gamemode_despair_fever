datablock AudioProfile(HeartBeatSound) {
	fileName = $Despair::Path @ "res/sounds/heartbeat.wav";
	description = AudioDefault3d;
	preload = true;
};
datablock AudioProfile(fallFatalSound)
{
	fileName = $Despair::Path @ "res/sounds/gore/fallFatal.wav";
	description = AudioClose3d;
	preload = true;
};
datablock AudioProfile(fallInjurySound)
{
	fileName = $Despair::Path @ "res/sounds/gore/fallInjury.wav";
	description = AudioClose3d;
	preload = true;
};

function Player::critLoop(%this)
{
	cancel(%this.critLoop);
	if(%this.getState() $= "Dead")
		return;

	//if($deathCount >= $maxDeaths)
	//{
	//	cancel(%this.critLoop);
	//	%this.health = %this.maxhealth;
	//	%this.KnockOut(30);
	//	return;
	//}

	%percent = mAbs(%this.health / $Despair::CritThreshold);
	%delay = 1250 * (1 - %percent);
	if(isObject(%this.client))
	{
		%this.client.play2d(HeartBeatSound);
		commandToClient(%this.client, 'SetVignette', true, %percent SPC "0 0" SPC %percent);
	}

	if(%this.attackSource[%this.attackCount] == %this || $Sim::Time - %this.attackTime[%this.attackCount] > 1) //So you don't bleed to death while being beaten to death!
	{
		%this.health -= 5;
	}
	if(%this.health <= $Despair::CritThreshold)
	{
		%this.damage(%this.attackSource[%this.attackCount], %this.getPosition(), 5, "bleed");
		return;
	}
	%this.critLoop = %this.schedule(getMax(400, %delay), "critLoop");
}

package DespairHealth
{
	function Armor::onAdd(%data, %player)
	{
		parent::onAdd(%data, %player);
	}

	function Armor::damage(%data, %player, %src, %pos, %damage, %type)
	{
		%client = %player.client;
		if (%client.miniGame != $DefaultMiniGame)
			return;

		%playPain = 1;
		%blood = true;
		if(%player.character.trait["Extra Tough"] && (%type $= "blunt" || %type $= "sharp"))
			%damage *= 0.9;

		if(%type $= $DamageType::Impact || %type $= $DamageType::Fall)
		{
			if(%client.killer)
				%damage = 1;
			%fatal = %player.health - %damage <= 0;
			%sound = %fatal ? fallFatalSound : fallInjurySound;
			%playPain = 0;
			serverPlay3D(%sound, %pos);
			%type = "fall";
			if(%fatal)
				%player.health = $Despair::CritThreshold;
			%blood = false;
		}
		else if(%player.isCrouched())
			%damage *= 3;
		if(%type $= $DamageType::Direct || %type $= $DamageType::Suicide)
		{
			%type = "accident";
			%blood = false;
		}

		if(isObject(%src))
		{
			if (%src.getType() & $TypeMasks::PlayerObjectType)
			{
				%sourceObject = %src;
				%attacker = %sourceObject.client;
			}
			else
			{
				%sourceObject = %src.sourceObject;
				%attacker = %sourceObject.sourceClient;
			}
			%normal = %sourceObject.getEyeVector();
			%dot = vectorDot(%player.getForwardVector(), %normal);
		}
		else
		{
			if(isObject(%player.attackSource[%player.attackCount]) && $Sim::Time - %player.attackTime[%player.attackCount] <= 5)
			{
				%src = %player.attackSource[%player.attackCount];
				%sourceObject = %src;
				%attacker = %sourceObject.client;
			}
			else if(isObject(%player.lastShover) && $Sim::Time - %player.lastShoved <= 5) //Pushed off a building or something
			{
				%src = %player.lastShover;
				%sourceObject = %src;
				%attacker = %sourceObject.client;
			}
			else if(isObject(%player.lastTosser) && $Sim::Time - %player.carryEnd <= 5) //Assisted suicide
			{
				%src = %player.lastTosser;
				%sourceObject = %src;
				%attacker = %sourceObject.client;
			}
		}

		%player.attackCount++;
		%player.attackType[%player.attackCount] = %type;
		%player.attackDot[%player.attackCount] = %dot;
		%player.attackSource[%player.attackCount] = %src;
		%player.attackClient[%player.attackCount] = %attacker;
		%player.attackImage[%player.attackCount] = %sourceObject.getMountedImage(0);
		%player.attackCharacter[%player.attackCount] = %attacker.character;
		%player.attackTime[%player.attackCount] = $Sim::Time;
		%player.attackDayTime[%player.attackCount] = getDayCycleTime();
		%player.attackDay[%player.attackCount] = $days;

		if(%attacker)
			RS_Log("[DMGLOG]" SPC %attacker.getPlayerName() SPC "[" @ %attacker.getBLID() @ "] harmed " @ 
					%client.getPlayerName() SPC "[" @ %client.getBLID() @ "], type " @ %type @ " for " @ %damage @ " damage.", "\c4");
		else
			RS_Log("[DMGLOG]" SPC %client.getPlayerName() SPC "[" @ %client.getBLID() @ "] harmed himself, type " @ %type @ " for " @ %damage @ " damage.", "\c4");

		if (%pos !$= "" && %blood)
		{
			%region = %player.getRegion(%pos, true);
			%player.attackRegion[%player.attackCount] = %region;

			%color = 0.75 + 0.1 * getRandom() @ " 0 0 1"; //cool bloods
			switch$ (%region)
			{
			case "head":
				%player.bloody["head"] = true;
				%client.applyBodyParts();
			case "rleg":
				%player.setNodeColor("RShoe", %color);
			case "lleg":
				%player.setNodeColor("LShoe", %color);
			case "rarm":
				%player.setNodeColor("RArm", %color);
				%player.setNodeColor("RHand", %color);
			case "larm":
				%player.setNodeColor("LArm", %color);
				%player.setNodeColor("LHand", %color);
			case "hip":
				%player.setNodeColor("pants", %color);
			case "chest":
				%player.bloody[%dot > 0 ? "chest_back" : "chest_front"] = true;
				%client.applyBodyParts();
			}
			%player.bloody = true;
			%player.bloodyWriting = 2;
		}

		if(getRandom() < 0.3)
			%player.spawnFiber();

		%player.health -= %damage;
		if(%player.health <= $Despair::CritThreshold)
		{
			if(despairOnKill(%client, %attacker))
			{
				%player.wakeUp();
				%p = new Projectile()
				{
					datablock = cubeHighExplosionProjectile;
					initialPosition = %pos;
					initialVelocity = vectorScale(%normal, 4);
				};
				%p.explode();
				if(%playPain)
					%player.playDeathCry();
				%player.setDamageLevel(100);
			}
			else
			{
				cancel(%player.critLoop);
				%player.health = %player.maxhealth;
				%player.KnockOut(30);
			}
			return 1;
		}
		else if(!isEventPending(%player.critLoop) && %player.health <= 0)
		{
			if(despairOnKill(%client, %attacker, true))
			{
				%player.wakeUp();
				%player.changeDataBlock(PlayerCorpseArmor);
				%player.playThread(0, "sit");
				%player.noWeapons = true;
				%player.critLoop();
				messageClient(%client, '', "\c5You can use the last of your strength to \c6/write\c5 your final message! Be sure to look at a surface.");
				commandToClient(%client, 'CenterPrint', "\c5You can \c6/write\c5 your final message!\nBe sure to look at a surface.", 4);
			}
			else
			{
				cancel(%player.critLoop);
				%player.health = %player.maxhealth;
				%player.KnockOut(30);
				return 1;
			}
		}
		if(!%player.character.trait["Feel No Pain"])
		{
			if(%playPain && %player.health > 0 && !%player.unconscious)
				%player.playPain();
			%player.setDamageFlash((%player.maxhealth - %player.health) / %player.maxhealth * 0.5);
		}
		if((%type $= "blunt" || %type $= "sharp") && (%player.character.trait["Hemophiliac"] || getRandom() > (%type $= "sharp" ? 0.3 : 0.15)))
		{
			%player.setStatusEffect($SE_damageSlot, "bleeding");
			if(%player.character.trait["Hemophiliac"])
				%player.bleedTicks = 13;
		}

		if(%player.unconscious)
		{
			%player.wakeUp();
			%player.setStatusEffect($SE_passiveSlot, "drowsy");
		}

		return 1;
	}

	function Armor::onDisabled(%data, %player, %state)
	{
		%client = %player.client;
		if (%client.miniGame != $DefaultMiniGame)
			return Parent::onDisabled(%data, %player, %state);

		if (isObject(%client))
		{
			// centerPrint(%client, "");
			%client.character.deleteMe = true;
			%client.camera.setMode("Corpse", %player);
			%client.setControlObject(%client.camera);
			%client.camera.setControlObject(%client.camera);
			%client.player = "";
			%player.client = "";
			commandToClient(%client, 'ClearCenterPrint');
			commandToClient(%client, 'SetVignette', true, "0 0 0 1");

			RS_Log("[DMGLOG]" SPC %client.getPlayerName() @ " [" @ %client.getBLID() @ "] died.", "\c4");
		}

		%player.isDead = 1;
		%player.isBody = 1;
		if(%player.attackSource[%player.attackCount] == %player || %player.attackSource[%player.attackCount] <= 0)
		{
			%player.suicide = true;
			%player.pools = 1000;
		}
		%player.setDamageFlash(1);
		%player.setImageTrigger(0, 0);

		//if (%player.attackDot[%player.attackCount] > 0)
		//{
		//	%player.playThread(0, "crouch");
		//	%player.playThread(2, "jump");
		//	%player.schedule(100, playThread, 2, "plant");
		//}
		//else
			%player.playThread(0, "death1");

		%player.playThread(1, "root");
		%player.playThread(2, "root");
		%player.playThread(3, "root");

		GameRoundCleanup.add(%player);

		$DefaultMiniGame.checkLastManStanding();
	}

	function projectileData::onCollision(%this, %obj, %col, %pos, %fade, %normal)
	{
		%obj.normal = %normal;
		parent::onCollision(%this, %obj, %col, %pos, %fade, %normal);
	}
};
activatePackage("DespairHealth");