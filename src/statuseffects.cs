$SE_sleepSlot = 0; //reserved sleepo
$SE_passiveSlot = 1;
$SE_damageSlot = 2;
$SE_damageSlot1 = 3;
$SE_maxStatusEffects = 6;

function Player::clearStatusEffects(%player)
{
	for(%i=0; %i<$SE_maxStatusEffects; %i++)
	{
		%player.setStatusEffect(%slot, "");
	}
	if(isObject(%player.client))
		%player.client.updateBottomPrint();
}

function Player::removeStatusEffect(%player, %slot, %effect)
{
	if(%effect !$= "" && %player.statusEffect[%slot] !$= %effect) //it changed
		return;
	%player.setStatusEffect(%slot, "");
	if(isObject(%player.client))
		%player.client.updateBottomPrint();
}

function Player::setStatusEffect(%player, %slot, %effect, %nomsg)
{
	//onAdd, essentially
	switch$ (%effect)
	{
		//Sleep-related
		case "sleepy":
			%player.updateSpeedScale(1);
			%player.swingSpeedMod = 1;
			if(!%nomsg && isObject(%player.client))
				%player.client.chatMessage("\c5You are getting sleepy... Find a \c3bed\c5 and type \c3/sleep\c5!");
		case "tired":
			%player.setSpeedScale(0.9);
			%player.swingSpeedMod = 1.1;
		case "exhausted":
			%player.setSpeedScale(0.6);
			%player.swingSpeedMod = 1.6;
		case "sleeping":
			%player.updateSpeedScale(1);
			%player.swingSpeedMod = 1;
		//passive buffs/debuffs
		case "drowsy":
			%player.setSpeedScale(0.8);
			%player.swingSpeedMod = 1.5;
			cancel(%player.statusSchedule[%slot]);
			%player.statusSchedule[%slot] = %player.schedule(60000, removeStatusEffect, %slot, %effect);
		case "sore back":
			%player.setSpeedScale(0.8);
			%player.swingSpeedMod = 1.2;
			cancel(%player.statusSchedule[%slot]);
			%player.statusSchedule[%slot] = %player.schedule(75000, removeStatusEffect, %slot, %effect);
		case "fresh":
			if(%player.statusEffect[%slot] $= "sore back" || %player.statusEffect[%slot] $= "shining")
				return 0;
			%player.updateSpeedScale(1);
			%player.swingSpeedMod = 1;
			cancel(%player.statusSchedule[%slot]);
			%player.statusSchedule[%slot] = %player.schedule(20000, removeStatusEffect, %slot, %effect);
		case "shining":
			%player.setSpeedScale(1.1);
			%player.swingSpeedMod = 0.9;
			cancel(%player.statusSchedule[%slot]);
			%player.statusSchedule[%slot] = %player.schedule(60000, removeStatusEffect, %slot, %effect);
			if(!%nomsg && isObject(%player.client))
				%player.client.chatMessage("\c5You had a \c3good night's sleep\c5!");

		//damage-related
		case "bleeding":
			%player.bleedTicks = 6;
			cancel(%player.statusSchedule[%slot]);
			%player.statusSchedule[%slot] = %player.schedule(2000, updateStatusEffect, %slot);
			if(!%nomsg && isObject(%player.client))
				%player.client.chatMessage("\c5You are " @ getStatusEffectColor(%effect) @ "bleeding\c5!");
		case "shock":
			if(%player.statusEffect[%slot] $= "shock")
				return 0; //Don't stack it

			%player.setSpeedScale(0.6);
			%player.swingSpeedMod = 1.6;
			cancel(%player.statusSchedule[%slot]);
			%player.statusSchedule[%slot] = %player.schedule(3000, removeStatusEffect, %slot, %effect);
			if(!%nomsg && isObject(%player.client))
				%player.client.chatMessage("\c5You are suffering " @ getStatusEffectColor(%effect) @ "shock\c5!");

		//damage modelling
		case "wounded arm":
			%player.swingSpeedMod = 1.4;
			if(getRandom() < 0.25)
				%player.dropTool(%player.currTool, 5);
			cancel(%player.statusSchedule[%slot]);
			%player.statusSchedule[%slot] = %player.schedule(3000, removeStatusEffect, %slot, %effect);
		case "wounded leg":
			%player.setSpeedScale(0.7);
			cancel(%player.statusSchedule[%slot]);
			%player.statusSchedule[%slot] = %player.schedule(3000, removeStatusEffect, %slot, %effect);
		case "concussion":
			%player.setWhiteOut(0.4); //BOOM!! FREAK THE FUCK OUT!!
			%player.swingSpeedMod = 1.2;
			%player.setSpeedScale(0.8);
			cancel(%player.statusSchedule[%slot]);
			%player.statusSchedule[%slot] = %player.schedule(3000, removeStatusEffect, %slot, %effect);
		case "abdominal trauma": //Todo: puking blood
			%player.swingSpeedMod = 1.2;
			%player.setSpeedScale(0.8);
			cancel(%player.statusSchedule[%slot]);
			%player.statusSchedule[%slot] = %player.schedule(3000, removeStatusEffect, %slot, %effect);

		default:
			if(%player.statusEffect[%slot] !$= "")
				%player.setStatusEffect($SE_sleepSlot, %player.statusEffect[%slot], 1); //Sleep slot is priority when it comes to slowdowns
			else
			{
				%player.updateSpeedScale(1);
				%player.swingSpeedMod = 1;
			}
	}
	%player.statusEffect[%slot] = %effect;
	return %slot;
}

function Player::updateStatusEffect(%player, %slot)
{
	%effect = %player.statusEffect[%slot];
	switch$ (%effect)
	{
		//Sleep-related
		case "sleepy":
			%player.setStatusEffect(%slot, "tired");
		case "tired":
			%player.setStatusEffect(%slot, "exhausted");
		case "exhausted":
			if(isObject(%player.client))
				%player.client.chatMessage("\c5You pass out...");
			%player.KnockOut(90);
		case "sleeping":
			//%player.sleep() handles this

		//passive buffs/debuffs
		case "drowsy":
			//onAdd, set player speeds
		case "sore back":
			//
		case "fresh":
			//
		case "shining":
			//

		//damage-related
		case "bleeding":
			cancel(%player.statusSchedule[%slot]);
			%player.bleedTicks--;
			if(%player.bleedTicks <= 0)
			{
				%player.removeStatusEffect(%slot, %effect);
				return 0;
			}
			%pos = %player.getPosition();
			%decal = spawnDecalFromRayCast(NewBloodDecal, containerRayCast(%pos, VectorSub(%pos, "0 0 1"), $SprayBloodMask));
			%decal.color = 0.75 + 0.1 * getRandom() @ " 0 0 1";
			%decal.setNodeColor("ALL", %decal.color);
			%decal.hideNode("ALL");
			%decal.unHideNode(blood @ getRandom(3, 4));
			%decal.spillTime = $Sim::Time;
			%decal.freshness = 0;
			%decal.isBlood = true;
			%size = 1 + 0.5 * getRandom();
			%decal.setScale(%size SPC %size SPC %size);
			if(getRandom() < 0.45)
				serverPlay3d(BloodSplat @ getRandom(1,3), getWords(%ray, 1, 3));
			%player.health = getMax(1, %player.health - 2);
			%player.setDamageFlash(0.2);
			%player.statusSchedule[%slot] = %player.schedule(1000, updateStatusEffect, %slot);
		case "shock":
			//beep

		//damage modelling
		case "wounded arm":
			//
		case "wounded leg":
			//
		case "concussion":
			//
		case "abdominal trauma":
			//
	}
}

function getStatusEffectColor(%effect)
{
	%color = "\c0";
	switch$ (%effect)
	{
		//Sleep-related
		case "sleepy":
			%color = "<color:4169E8>";
		case "tired":
			%color = "<color:4C47FF>";
		case "exhausted":
			%color = "<color:7241E8>";
		case "sleeping":
			%color = "<color:B14CFF>";

		//passive
		case "drowsy":
			%color = "<color:4CA6FF>";
		case "sore back":
			%color = "<color:41817F>";
		case "fresh":
			%color = "<color:8EFFFE>";
		case "shining":
			%color = "<color:FFF39D>";

		//damage-related
		case "bleeding":
			%color = "<color:e83d3a>";
		case "shock":
			%color = "<color:ff75fa>";
		case "wounded arm":
			%color = "<color:AA3939>";
		case "wounded leg":
			%color = "<color:AA3939>";
		case "concussion":
			%color = "<color:801515>";
		case "abdominal trauma":
			%color = "<color:801515>";
	}
	return %color;
}
