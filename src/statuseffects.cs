$SE_sleepSlot = 0; //reserved sleepo
$SE_passiveSlot = 1;
$SE_maxStatusEffects = 4;

function Player::clearStatusEffects(%player)
{
	for(%i=0; %i<$SE_maxStatusEffects; %i++)
	{
		%player.statusEffect[%i] = "";
	}
}

function Player::onAddStatusEffect(%player, %slot)
{
	%effect = %player.statusEffect[%slot];
	switch$ (%effect)
	{
		//Sleep-related
		case "tired":
			//minor slowdown
		case "exhausted":
			//major slowdown
		case "sleeping":
			//%player.sleep() handles this

		//passive buffs/debuffs
		case "drowsy":
			//moderate slowdown
		case "sore back":
			//fuck 'em up somehow
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
	}
}

function Player::updateStatusEffect(%player, %slot)
{
	%effect = %player.statusEffect[%slot];
	switch$ (%effect)
	{
		//Sleep-related
		case "tired":
			%player.statusEffect[%slot] = "exhausted";
		case "exhausted":
			%player.sleep(true);
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
		case "tired":
			%color = "<color:605292>";
		case "exhausted":
			%color = "<color:605292>";
		case "sleeping":
			%color = "<color:172457>";
		case "drowsy":
			%color = "<color:172457>";
		case "sore back":
			%color = "<color:41817F\>";
		case "fresh":
			%color = "<color:4C996B>";
		case "shining":
			%color = "<color:FFFFAA>";

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
