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
	if(isObject(%this.client))
		%this.client.play2d(HeartBeatSound);

	%this.health -= 5;
	if(%this.health <= -100)
	{
		%this.damage(%this.attackSource[%this.attackCount], %this.getPosition(), 5, "bleed");
		return;
	}
	%player.setDamageFlash(( - %player.health) / %player.maxhealth * 0.5);
	%this.critLoop = %this.schedule(1000, "critLoop");
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
				%player.bloody["chest_front"] = true;
				%client.applyBodyParts();
			}
			%player.bloody = true;
			%player.bloodyWriting = 2;
		}

		%player.health -= %damage;
		if(%player.health <= -150)
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
		if(%player.health <= 0)
		{
			if(despairOnKill(%client, %attacker, true))
			{
				if(!isEventPending(%player.critLoop))
				{
					%player.changeDataBlock(PlayerCorpseArmor);
					%player.playThread(0, "sit");
					%player.noWeapons = true;
					%player.critLoop();
					messageClient(%client, '', "\c5You can use the last of your strength to /write your final message!");
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
			%client.camera.setMode("Corpse", %player);
			%client.setControlObject(%client.camera);
			%client.camera.setControlObject(%client.camera);
			%client.player = "";
			%player.client = "";
			commandToClient(%client, 'ClearCenterPrint');
		}

		%player.isDead = 1;
		%player.isBody = 1;
		%player.playDeathCry();
		%player.setDamageFlash(1);
		%player.setImageTrigger(0, 0);
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