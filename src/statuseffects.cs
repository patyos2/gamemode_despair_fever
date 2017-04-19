$SE_sleepSlot = 0; //reserved sleepo
$SE_passiveSlot = 1;
$SE_damageSlot = 2;
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
	if(%player.statusEffect[%slot] !$= %effect) //it changed
		return;
	%player.setStatusEffect(%slot, "");
	if(isObject(%player.client))
		%player.client.updateBottomPrint();
}

function Player::setStatusEffect(%player, %slot, %effect)
{
	%player.statusEffect[%slot] = %effect;
	//onAdd, essentially
	switch$ (%effect)
	{
		//Sleep-related
		case "sleepy":
			//nothing happens
		case "tired":
			%player.setSpeedScale(0.9);
			%player.swingSpeedMod = 1.1;
		case "exhausted":
			%player.setSpeedScale(0.6);
			%player.swingSpeedMod = 1.6;
		case "sleeping":

		//passive buffs/debuffs
		case "drowsy":
			//moderate slowdown
		case "sore back":
			%player.setSpeedScale(0.8);
			%player.swingSpeedMod = 1.1;
			cancel(%player.statusSchedule[%slot]);
			%player.statusSchedule[%slot] = %player.schedule(60000, removeStatusEffect, %slot, %effect);
		case "fresh":
			//nothing particulary exciting, scheduled update
		case "shining":
			//ooo good shit

		//damage-related
		case "bleeding":
			//start bleeding schedule
		case "shock":
			//slowdown

		//damage modelling
		case "wounded arm":
			//
		case "wounded leg":
			//
		case "concussion":
			//
		case "abdominal trauma":
			//

		default:
			%player.setSpeedScale(1);
			%player.swingSpeedMod = 1;
	}
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
			//boop
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
