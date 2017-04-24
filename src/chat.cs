datablock AudioProfile(DespairChatSound)
{
    fileName = $Despair::Path @ "res/sounds/chat.wav";
    description = AudioClosest3d;
    preload = true;
};


datablock ItemData(DespairEmptyFloatItem)
{
	shapeFile = "base/data/shapes/empty.dts";
	gravityMod = 0;
	canPickup = 0;
};

function DespairEmptyFloatItem::onPickup() {}

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
		if (isEventPending($DefaultMiniGame.restartSchedule))
			return Parent::serverCmdMessageSent(%client, %text);

		%text = getSubStr(trim(stripMLControlChars(%text)), 0, $Pref::Server::MaxChatLen);

		if (%text $= "")
			return;

		if(DespairSpecialChat(%client, %text))
			return;

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
			for (%i = 0; %i < ClientGroup.getCount(); %i++)
			{
				%member = ClientGroup.getObject(%i);

				if (!isObject(%member.player) || %member.miniGame != $DefaultMiniGame)
				{
					messageClient(%member, '', '\c7[%1]<color:808080>%2<color:b0b0b0>: %3', %time, %name, %text);
				}
			}
			echo("-+ (DEAD) " @ %name @ ": " @ %text);
			return;
		}
		if(%player.unconscious)
			return;

		%player.playThread(0, "talk");
		%player.schedule(strLen(%text) * 35, "playThread", 0, "root");

		%name = %client.character.name;
		if(!$despairTrial)
		{
			if(isObject(%hat = %player.tool[%player.hatSlot]) && %hat.disguise && isObject(%img = %player.getMountedImage(2)) && %img == nameToID(%hat.image))
				%name = "Unknown";
		}

		%sound = DespairChatSound;
		%type = "says";
		%range = 32;
		if(getSubStr(%text, 0, 1) $= "@") //Whispering
		{
			%text = getSubStr(%text, 1, strLen(%text));
			%type = "whispers";
			%range = 4;
		}
		if(isObject(%img = %player.getMountedImage(0)) && %img == nameToID(radioImage) && (%slot = %member.player.findTool("RadioItem")) != -1)
		{
			%sound = radioTalkSound;
			%type = "radios";
			%range = 16;
			%props = %member.player.getItemProps(%slot);
			radioTransmitMessage(%player, %props.channel, %text);
		}
		if(%type !$= "whispers")
		{
			serverPlay3D(%sound, %player.getHackPosition());

			%shape = new Item()
			{
				datablock = DespairEmptyFloatItem;
				position = VectorAdd(%player.position, "0 0 2");
			};

			%shape.setCollisionTimeout(%player);
			%shape.setShapeName(%text);
			%shape.setShapeNameDistance(%range);
			%shape.setVelocity("0 0 0.5");
			%shape.deleteSchedule = %shape.schedule(3000, delete);
		}

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
			if(%member.player.unconscious)
			{
				%_name = "Someone";
				%_text = muffleText(%text, 0.5);
				%_range *= 0.5;
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
		if (%text $= "")
			return;
		for (%i = 0; %i < ClientGroup.getCount(); %i++)
		{
			%member = ClientGroup.getObject(%i);
			if(%member.isAdmin)
			{
				if(%killer)
					messageClient(%member, '', '\c2--[<color:FF8080>%1<color:FFF0F0>: %2', %name, %text);
				else
					messageClient(%member, '', '\c2--[<color:80FF80>%1<color:F0FFF0>: %2', %name, %text);
			}
			if(%killer && %member.killer)
				messageClient(%member, '', '\c2--[ADMIN]<color:FF8080>%1<color:FFF0F0>: %2', "Admin", %text);
		}
	}
};

activatePackage("DespairChat");