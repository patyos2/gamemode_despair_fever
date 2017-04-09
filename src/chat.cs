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
	canPickup = 1;
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

		%name = %client.getPlayerName();
		if (!isObject(%player))
		{
			for (%i = 0; %i < $DefaultMiniGame.numMembers; %i++)
			{
				%member = $DefaultMiniGame.member[%i];

				if (!isObject(%member.player))
				{
					messageClient(%member, '', '<color:808080>%1\c6<color:b0b0b0>: %2', %name, %text);
				}
			}
			return;
		}
		%name = %client.character.name;
		if(isObject(%player.tool[%player.hatSlot]) && %player.tool[%player.hatSlot].disguise)
			%name = "Unknown";

		serverPlay3D(DespairChatSound, %player.getHackPosition());

		%shape = new Item()
		{
			datablock = DespairEmptyFloatItem;
			position = VectorAdd(%player.position, "0 0 2");
		};

		%shape.setCollisionTimeout(%player);
		%shape.setShapeName(%text);
		%shape.setVelocity("0 0 0.5");
		%shape.deleteSchedule = %shape.schedule(3000, delete);

		messageAll('', '<color:ffff80>%1\c6<color:fffff0>: %2', %name, %text);
	}
};

activatePackage("DespairChat");