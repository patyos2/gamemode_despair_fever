$SE_sleepSlot = 0; //reserved sleepo
$SE_maxStatusEffects = 3;

function Player::clearStatusEffects(%player)
{
	for(%i=0; %i<$SE_maxStatusEffects; %i++)
	{
		%player.statusEffect[%i] = "";
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
			cancel(%player.sleepSchedule);
			%player.sleepSchedule = %player.schedule(15000, "sleep");
		case "sleeping":
			//%player.sleep() handles this
		case "drowsy":
			//
		case "sore back":
			//
		case "fresh":
			//
		case "shining":
			//

		//damage-related
		case "wounded arm":
			//
		case "wounded leg":
			//
		case "concussion":
			//
		case "abdominal trauma":
			//

		//misc
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

function Player::sleep(%player)
{

}