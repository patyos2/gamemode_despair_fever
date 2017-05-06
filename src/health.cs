datablock AudioProfile(HeartBeatSound) {
	fileName = $Despair::Path @ "res/sounds/heartbeat.wav";
	description = AudioDefault3d;
	preload = true;
};

function Player::critLoop(%this)
{
	cancel(%this.critLoop);
	if(%this.getState() $= "Dead")
		return;

	if($deathCount >= $maxDeaths)
	{
		cancel(%this.critLoop);
		%this.health = %this.maxhealth;
		%this.KnockOut(30);
		return;
	}

	if(isObject(%this.client))
		%this.client.play2d(HeartBeatSound);

	%percent = (-%this.health) / $Despair::CritThreshold;
	%delay = 1250 * (1 - (%percent * 0.7));

	if($Sim::Time - %this.attackTime[%this.attackCount] > 1) //So you don't bleed to death while being beaten to death!
	{
		%this.health -= 5;
		if(%this.health <= $Despair::CritThreshold)
		{
			%this.damage(%this.attackSource[%this.attackCount], %this.getPosition(), 5, "bleed");
			return;
		}
	}
	%this.setDamageFlash(%percent);
	%this.critLoop = %this.schedule(getMax(500, %delay), "critLoop");
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

		if(%player.isCrouched())
			%damage *= 3;

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

		if(%type $= $DamageType::Impact || %type $= $DamageType::Fall)
			%type = "fall";
		if(%type $= $DamageType::Direct || %type $= $DamageType::Suicide)
			%type = "self";

		%normal = %sourceObject.getEyeVector();
		%dot = vectorDot(%player.getForwardVector(), %normal);

		%player.attackCount++;
		%player.attackType[%player.attackCount] = %type;
		%player.attackDot[%player.attackCount] = %dot;
		%player.attackSource[%player.attackCount] = %src;
		%player.attackClient[%player.attackCount] = %attacker;
		%player.attackCharacter[%player.attackCount] = %attacker.character;
		%player.attackTime[%player.attackCount] = $Sim::Time;
		%player.attackDayTime[%player.attackCount] = getDayCycleTime();
		%player.attackDay[%player.attackCount] = $days;

		if (%pos !$= "")
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

		%player.health -= %damage;
		if(%player.health <= $Despair::CritThreshold)
		{
			if(despairOnKill(%client, %attacker))
			{
				%p = new Projectile()
				{
					datablock = cubeHighExplosionProjectile;
					initialPosition = %pos;
					initialVelocity = vectorScale(%normal, 4);
				};
				%p.explode();
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
		else if(%player.health <= 0)
		{
			if(despairOnKill(%client, %attacker, true))
			{
				if(!isEventPending(%player.critLoop))
				{
					%player.wakeUp();
					%player.changeDataBlock(PlayerCorpseArmor);
					%player.playThread(0, "sit");
					%player.noWeapons = true;
					%player.critLoop();
					messageClient(%client, '', "\c5You can use the last of your strength to \c6/write\c5 your final message! Be sure to look at a surface.");
					commandToClient(%client, 'CenterPrint', "\c5You can \c6/write\c5 your final message!\nBe sure to look at a surface.", 4);
				}
			}
			else
			{
				cancel(%player.critLoop);
				%player.health = %player.maxhealth;
				%player.KnockOut(30);
				return 1;
			}
		}
		%player.playPain();
		%player.setDamageFlash((%player.maxhealth - %player.health) / %player.maxhealth * 0.5);
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
		}

		%player.isDead = 1;
		%player.isBody = 1;
		if(%player.attackSource[%player.attackCount] == %player)
			%player.suicide = true;
		%player.playDeathCry();
		%player.setDamageFlash(1);
		%player.setImageTrigger(0, 0);

		if (%obj.attackDot[%obj.attackCount] > 0)
		{
			%obj.playThread(0, "crouch");
			%obj.playThread(2, "jump");
			%obj.schedule(100, playThread, 2, "plant");
		}
		else
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