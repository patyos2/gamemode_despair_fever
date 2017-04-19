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

		%name = %client.getPlayerName();
		if (!isObject(%player))
		{
			for (%i = 0; %i < ClientGroup.getCount(); %i++)
			{
				%member = ClientGroup.getObject(%i);

				if (!isObject(%member.player) || %member.miniGame != $DefaultMiniGame)
				{
					messageClient(%member, '', '<color:808080>%1\c6<color:b0b0b0>: %2', %name, %text);
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
		if(isObject(%hat = %player.tool[%player.hatSlot]) && %hat.disguise && isObject(%player.getMountedImage(2)) && %player.getMountedImage(2) == nameToID(%hat.image))
			%name = "Unknown";

		serverPlay3D(DespairChatSound, %player.getHackPosition());

		%shape = new Item()
		{
			datablock = DespairEmptyFloatItem;
			position = VectorAdd(%player.position, "0 0 2");
		};

		%shape.setCollisionTimeout(%player);
		%shape.setShapeName(%text);
		%shape.setShapeNameDistance(30);
		%shape.setVelocity("0 0 0.5");
		%shape.deleteSchedule = %shape.schedule(3000, delete);
		echo("-+ " @ %name @ " (" @ %client.getPlayerName() @ "): " @ %text);

		for (%i = 0; %i < ClientGroup.getCount(); %i++)
		{
			%member = ClientGroup.getObject(%i);
			if (!isObject(%member.player) || %member.miniGame != $DefaultMiniGame)
			{
				messageClient(%member, '', '<color:ffff80>%1\c6<color:fffff0>: %2', %name, %text);
				continue;
			}
			%a = %player.getEyePoint();
			%b = %member.player.getEyePoint();
			if (vectorDist(%a, %b) > 32)
				continue;
			messageClient(%member, '', '<color:ffff80>%1\c6<color:fffff0>: %2', %name, %text);
		}
	}
	function serverCmdTeamMessageSent(%client, %text) //OOC
	{
	}
};

activatePackage("DespairChat");