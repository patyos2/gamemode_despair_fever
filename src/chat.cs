datablock AudioProfile(DespairChatSound)
{
	fileName = $Despair::Path @ "res/sounds/chat.wav";
	description = AudioClosest3d;
	preload = true;
};

datablock AudioProfile(DespairAdminChatSound)
{
	fileName = $Despair::Path @ "res/sounds/adminchat.wav";
	description = Audio2d;
	preload = true;
};

datablock AudioProfile(DespairAdminBwoinkSound)
{
	fileName = $Despair::Path @ "res/sounds/bwoink.wav";
	description = Audio2d;
	preload = true;
};

datablock ItemData(DespairEmptyFloatItem)
{
	shapeFile = "base/data/shapes/empty.dts";
	gravityMod = 0;
	canPickup = 0;
};

function DespairEmptyFloatItem::onPickup() {}

function serverCmdMe(%client, %m1, %m2, %m3, %m4, %m5, %m6, %m7, %m8, %m9, %m10, %m11, %m12, %m13, %m14, %m15, %m16, %m17, %m18, %m19, %m20, %m20, %m22, %m23, %m24)
{
	if (!isObject(%pl = %client.player) || (%pl.unconscious && !%pl.currResting))
		return;

	if($Sim::Time - %client.lastSpeakTime < $chatDelay)
	{
		messageClient(%client, '', '\c5Slow down\c6!');
		return;
	}

	if($Sim::Time < %client.timeOut) //Special trial ability
	{
		messageClient(%client, '', '\c5You\'re unable to act\c6!');
		return;
	}

	if(%pl.unconscious)
		return;

	%text = %m1;
	for (%i=2; %i<=24; %i++)
		%text = %text SPC %m[%i];
	%text = trim(stripMLControlChars(%text));
	if (%text $= "")
		return;
	
	%client.lastSpeakTime = $Sim::Time;
	%name = getCharacterName(%client.character, $despairTrial);

	if(!$despairTrial)
	{
		%time = getDayCycleTime();
		%time += 0.25; //so Zero = 6 AM aka morning, Youse's daycycle begins from morning at 0 fraction
		%time = %time - mFloor(%time); //get rid of excess stuff

		%time = getDayCycleTimeString(%time, 1);
	}
	else
	{
		%time = getTimeString(mFloor($Sim::Time - $DespairTrial));
	}

	echo("-+ (ACTION) " @ %name SPC %text);

	%count = ClientGroup.getCount();
	for (%i = 0; %i < %count; %i++)
	{
		%other = ClientGroup.getObject(%i);
		if (%other.miniGame == $DefaultMiniGame && isObject(%other.player))
		{
			if(%other.player.unconscious)
				continue;
			if (vectorDist(%other.player.getEyePoint(), %pl.getEyePoint()) > 24) //Out of range
				continue;
		}

		messageClient(%other, '', '\c7[%1]<color:ffff80>%2 %3', %time, %name, %text);
	}
}

package DespairChat
{
	function serverCmdStartTalking(%client)
	{
		if (%client.miniGame != $DefaultMiniGame)
			Parent::serverCmdStopTalking(%client);
	}
	function serverCmdMessageSent(%client, %text)
	{
		%player = %client.player;

		if (%client.miniGame != $DefaultMiniGame && %client.hasSpawnedOnce)
			return Parent::serverCmdMessageSent(%client, %text);

		%text = getSubStr(trim(stripMLControlChars(%text)), 0, $Pref::Server::MaxChatLen);

		if (%text $= "")
			return;

		if(DespairSpecialChat(%client, %text))
			return;

		if($Sim::Time - %client.lastSpeakTime < $chatDelay)
		{
			messageClient(%client, '', '\c5Slow down\c6!');
			return;
		}

		if($Sim::Time < %client.timeOut) //Special trial ability
		{
			messageClient(%client, '', '\c5You\'re unable to speak\c6!');
			return;
		}

		if(!$despairTrial)
		{
			%time = getDayCycleTime();
			%time += 0.25; //so Zero = 6 AM aka morning, Youse's daycycle begins from morning at 0 fraction
			%time = %time - mFloor(%time); //get rid of excess stuff

			%time = getDayCycleTimeString(%time, 1);
		}
		else
		{
			%time = getTimeString(mFloor($Sim::Time - $DespairTrial));
		}
		%name = %client.getPlayerName();
		if (!isObject(%player))
		{
			%client.lastSpeakTime = $Sim::Time;
			for (%i = 0; %i < ClientGroup.getCount(); %i++)
			{
				%member = ClientGroup.getObject(%i);

				if (!isObject(%member.player) || isEventPending($DefaultMiniGame.restartSchedule) || %member.miniGame != $DefaultMiniGame)
				{
					messageClient(%member, '', '\c7[%1]<color:808080>%2<color:b0b0b0>: %3', %time, %name, %text);
				}
				$lastDeadText = %text; //for medium
			}
			echo("-+ (DEAD) " @ %name @ ": " @ %text);
			return;
		}
		if(%player.unconscious)
			return;

		%player.playThread(3, "talk");
		%player.schedule(strLen(%text) * 35, "playThread", 3, "root");

		%name = getCharacterName(%client.character, $despairTrial);

		%sound = DespairChatSound;
		%type = "says";
		%range = 32;

		if(%player.health <= 0) //critical health
		{
			%type = "stammers";
			%range = 8;
			%text = stutterText(%text, 0.1);
		}
		if(getSubStr(%text, 0, 1) $= "@") //Whispering
		{
			%text = getSubStr(%text, 1, strLen(%text));
			%type = "whispers";
			%range = 4;
			if (%text $= "")
				return;
		}
		if(isObject(%img = %player.getMountedImage(0)) && %img == nameToID(radioImage) && (%slot = %player.findTool("RadioItem")) != -1)
		{
			%sound = radioTalkSound;
			%type = "radios";
			%range = 16;
			%props = %player.getItemProps(%slot);
			radioTransmitMessage(%player, %props.channel, %text);
		}

		if(%player.character.trait["Nervous"])
		{
			%text = stutterText(%text);
		}

		if(%player.character.trait["Loudmouth"])
		{
			%range += 3;
		}
		else if(%player.character.trait["Softspoken"])
		{
			%range *= 0.8;
			%text = softSpeakText(%text);
		}

		if(%type $= "says")
		{
			serverPlay3D(%sound, %player.getHackPosition());

			%shape = new Item()
			{
				datablock = DespairEmptyFloatItem;
				position = VectorAdd(%player.position, "0 0 2");
			};
			%shape.noExamine = true;
			%shape.setCollisionTimeout(%player);
			%shape.setShapeName(%text);
			%shape.setShapeNameDistance(%range);
			%shape.setVelocity("0 0 0.5");
			%shape.deleteSchedule = %shape.schedule(3000, delete);
		}
		%client.lastSpeakTime = $Sim::Time;
		echo("-+ " @ %name @ " (" @ %client.getPlayerName() @ "): " @ %text);
		for (%i = 0; %i < ClientGroup.getCount(); %i++)
		{
			%member = ClientGroup.getObject(%i);
			if (!isObject(%member.player) || %member.miniGame != $DefaultMiniGame)
			{
				if(%member.miniGame != $DefaultMiniGame)
					messageClient(%member, '', '\c7[%1]<color:ffff80>%2 %3<color:fffff0>, %4', %time, %name SPC "(" @ %client.getPlayerName() @ ")", %type, %text);
				else
					messageClient(%member, '', '\c7[%1]<color:ffff80>%2 %3<color:fffff0>, %4', %time, %name, %type, %text);
				continue;
			}
			%_name = %name;
			%_text = %text;
			%_range = %range;
			if(%member.player.unconscious && !%member.player.currResting)
			{
				%_name = "Someone";
				if(%type !$= "whispers") //whispering to someone sleeping is LOUD AND CLEAR
				{
					%_text = muffleText(%text, 0.5);
					%_range *= 0.5;
				}
			}
			%a = %player.getEyePoint();
			%b = %member.player.getEyePoint();
			if (vectorDist(%a, %b) > %_range)
				continue;
			messageClient(%member, '', '\c7[%1]<color:ffff80>%2 %3<color:fffff0>, %4', %time, %_name, %type, %_text);
		}
	}
	function serverCmdTeamMessageSent(%client, %text) //Adminchat
	{
		if(!%client.isAdmin && !%client.killer)
			return;
		if (%text $= "")
			return;
		%name = %client.getPlayerName();

		if(%client.killer && !%client.isAdmin)
		{
			%killer = true;
			%name = "Killer";
		}
		if(getSubStr(%text, 0, 1) $= "@") //Killer chat
		{
			%text = getSubStr(%text, 1, strLen(%text));
			%killer = true;
		}

		echo("-+ (" @ (%killer ? "KILLER" : "ADMIN") @ ") " @ %client.getPlayerName() @ ": " @ %text);

		for (%i = 0; %i < ClientGroup.getCount(); %i++)
		{
			%member = ClientGroup.getObject(%i);
			if(%member.isAdmin)
			{
				if(%killer)
				{
					if(isObject(%member.player) && %member.miniGame == $DefaultMiniGame)
						continue;
					messageClient(%member, '', '\c2--[<color:FF8080>%1<color:FFF0F0>: %2', %name, %text);
				}
				else
					messageClient(%member, '', '\c2--[<color:80FF80>%1<color:F0FFF0>: %2', %name, %text);
				%member.play2d(DespairAdminChatSound);
			}
		}
		if(%killer && isObject($currentKiller))
		{
			messageClient($currentKiller, '', '\c2--[ADMIN]<color:FF8080>%1<color:FFF0F0>: %2', "Admin", %text);
			$currentKiller.play2d(DespairAdminChatSound);
		}
	}
};

activatePackage("DespairChat");

//Text parse funcs
function muffleText(%text, %prob, %char)
{
	if (%text $= "")
		return;
	if (%prob $= "")
		%prob = 0.2;
	if (%prob <= 0)
		return %text;
	if (%char $= "")
		%char = "#";
	%result = %text;
	for (%i=0;%i<strlen(%text);%i++)
	{
		if (getSubStr(%text, %i, 1) $= " ") //space character
			continue;
		if (getRandom() < %prob)
			%result = getSubStr(%result, 0, %i) @ %char @ getSubStr(%result, %i+1, strlen(%result));
	}
	return %result;
}

function softSpeakText(%text)
{
	if (%text $= "")
		return;
	%result = strlwr(%text);
	for (%i=0;%i<strlen(%text);%i++)
	{
		if ((%char = getSubStr(%result, %i, 1)) $= " ") //space character
			continue;
		if (%char $= "." || %char $= "!" || %char $= "?")
		{
			%result = getSubStr(%result, 0, %i+1) @ ".." @ getSubStr(%result, getMin(%i+2, strlen(%result)), strlen(%result));
			%i += 2;
		}
	}
	if(getSubStr(%result, strlen(%result)-1, 1) !$= ".")
		%result = %result @ "...";
	return %result;
}

function stutterText(%text, %prob)
{
	if (%text $= "")
		return;
	if (%prob $= "")
		%prob = 0.2;
	if (%prob <= 0)
		return %text;
	%result = %text;
	for (%i=0;%i<strlen(%result);%i++)
	{
		if (strpos("bcdfghjklmnpqrstvxzwy", %char = getSubStr(%result, %i, 1)) == -1) //incompatible
			continue;
		if (getRandom() < %prob)
			%result = getSubStr(%result, 0, %i) @ %char @ %char @ getSubStr(%result, %i+1, strlen(%result));
	}
	return %result;
}