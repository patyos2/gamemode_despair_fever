datablock AudioProfile(HeartBeatSound) {
	fileName = $Despair::Path @ "res/sounds/heartbeat.wav";
	description = AudioDefault3d;
	preload = true;
};

package DespairHealth
{
	function Armor::onAdd(%data, %player)
	{
		parent::onAdd(%data, %player);
		if(%player.health $= "")
		{
			%player.maxhealth = 100;
			%player.health = 100;
		}
	}

	function Armor::damage(%data, %player, %src, %pos, %damage, %type)
	{
		%client = %player.client;
		if (%client.miniGame != $DefaultMiniGame)
			return Parent::damage(%data, %player, %src, %pos, %damage, %type);

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
		%player.attackSource[%player.attackCount] = %src;
		%player.attackDot[%player.attackCount] = %dot;
		%player.attackClient[%player.attackCount] = %attacker;
		%player.attackTime[%player.attackCount] = $Sim::Time;

		if (%pos !$= "")
		{
			%region = %player.getRegion(%pos, true);
			%player.attackRegion[%player.attackCount] = %region;

			%color = 0.75 + 0.1 * getRandom() @ " 0 0 1"; //cool bloods
			switch$ (%region)
			{
			case "head":
				%player.bloody["head"] = true;
				if (isObject(%player.client))
					%player.client.applyBodyParts();
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
				if (isObject(%player.client))
					%player.client.applyBodyParts();
			}
			%player.bloody = true;
		}

		%player.health -= %damage;
		//if(%player.health <= 0)
		//{
			//critical state
		//}
		if(%player.health <= 0) //-100
		{
			if (isObject(%player.client))
				despairOnKill(%player.client, %attacker);
			%p = new Projectile()
			{
				datablock = cubeHighExplosionProjectile;
				initialPosition = %pos;
				initialVelocity = vectorScale(%normal, 4);
			};
			%p.explode();
			%player.setDamageLevel(100);
			return;
		}
		%player.playPain();
		%player.setDamageFlash((%player.maxhealth - %player.health) / %player.maxhealth * 0.5);
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
		}

		%player.isDead = 1;
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