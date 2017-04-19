if (!isObject(GameRoundCleanup))
	new SimSet(GameRoundCleanup);

if (!isObject(GameCharacters))
	new SimSet(GameCharacters);

function roomPlayers()
{
	%freeCount = $Despair::RoomCount;

	for (%i = 0; %i < %freeCount; %i++)
	{
		%room = %i + 1;
		%freeRoom[%i] = %room;
		%roomDoor = BrickGroup_888888.NTObject["_r" @ %room @ "_door", 0];
		%roomDoor.lockId = "R"@%room;
		%roomDoor.lockState = true;
		%roomSpawn = BrickGroup_888888.NTObject["_r" @ %room @ "_spawn", 0];
	}

	for (%i = 0; %i < $DefaultMiniGame.numMembers && %freeCount; %i++)
	{
		%client = $DefaultMiniGame.member[%i];

		if (%client.player)
			%client.player.delete();

		%freeCount--;
		%freeIndex = getRandom(%freeCount);
		%room = %freeRoom[%freeIndex];
		for (%j = %freeIndex; %j < %freeCount; %j++)
			%freeRoom[%j] = %freeRoom[%j + 1];

		%freeRoom[%freeCount] = "";

		%roomDoor = BrickGroup_888888.NTObject["_r" @ %room @ "_door", 0];
		%roomSpawn = BrickGroup_888888.NTObject["_r" @ %room @ "_spawn", 0];

		//Create character
		%gender = getRandomGender();
		%character = new ScriptObject()
		{
			gender = %gender;
			name = getRandomName(%gender);
			appearance = getRandomAppearance(%gender);
			room = %room;

			client = %client;
			clientName = %client.getPlayerName();
		};

		GameCharacters.add(%character);
		%client.character = %character;

		//Assign character to client
		%client.killer = false;
		%client.spawnPlayer(); //PROTIP: Create players as AIPlayers so you can control them like bots in cutscenes
		%player = %client.player;
		%player.character = %character; //post-death reference to character
		%character.player = %player;
		%player.setDatablock(PlayerDespairArmor);
		%player.room = %room;
		%player.setShapeNameDistance(0);
		%player.setShapeNameColor("1 1 1");
		%player.setTransform(%roomSpawn.getSpawnPoint());

		centerPrint(%client, "");
		commandToClient(%client,'PlayGui_CreateToolHud',PlayerDespairArmor.maxTools);

		//Hat icon for hats
		%data = noHatIcon.getID();
		%player.hatSlot = %player.getDataBlock().maxTools - 1;
		%player.tool[%player.hatSlot] = %data;
		messageClient(%client, 'MsgItemPickup', '', %player.hatSlot, %data, true);

		//Weapon icon for weapons
		%data = noWeaponIcon.getID();
		%player.weaponSlot = 0;
		%player.tool[%player.weaponSlot] = %data;
		messageClient(%client, 'MsgItemPickup', '', %player.weaponSlot, %data, true);

		%props = KeyItem.newItemProps(%player, 0);
		%props.name = "Room #" @ $roomNum[%room] @ " Key";
		%props.id = %roomDoor.lockId;

		%player.addTool(KeyItem, %props);

		%client.playPath(IntroPath);
		%client.schedule(6000, setControlObject, %player);
	}
}

function despairEndGame()
{
	cancel($DefaultMiniGame.missingSchedule);
	cancel($DefaultMiniGame.restartSchedule);
	cancel($DefaultMiniGame.eventSchedule);
	$DefaultMiniGame.restartSchedule = $DefaultMiniGame.schedule(5000, reset, 0);
}

function despairPrepareGame()
{
	cancel($DefaultMiniGame.restartSchedule);
	GameRoundCleanup.deleteAll();
	GameCharacters.deleteAll();
	clearDecals();

	// Close *all* doors
	%count = BrickGroup_888888.getCount();

	for (%i = 0; %i < %count; %i++)
	{
		%brick = BrickGroup_888888.getObject(%i);

		%data = %brick.getDataBlock();
		%name = %brick.getName();
		if (%data.isDoor)
		{
			%brick.lockState = false;
			%brick.doorHits = 0;
			%brick.broken = false;
			%brick.setDataBlock(%brick.isCCW ? %data.closedCCW : %data.closedCW);
			%brick.doorMaxHits = 4;
		}
		//Consistent item spawns
		if(isObject(%brick.itemData))
			%brick.setItem(%brick.itemData);
		if(isObject(%brick.item))
			%brick.itemData = %brick.item.getDataBlock().getName();
	}

	//Hats!
	%choices = "HatBlindItem HatCatItem HatChefItem HatClownItem HatCowboyItem HatDisguiseItem HatDogeItem HatDuckItem HatFancyItem HatFedoraItem HatFoxItem HatGangsterItem HatGasmaskItem HatMountyItem HatMummyItem HatNinjaItem HatPartyhatItem HatRHoodItem HatRichardItem HatSkimaskItem HatStrawItem HatSunglassesItem HatTophatItem HatWizardItem";
	for (%i = 0; %i < BrickGroup_888888.NTObjectCount["_hatSpawn"]; %i++)
	{
		%brick = BrickGroup_888888.NTObject["_hatSpawn", %i];
		%pick = getWord(%choices, %index = getRandom(0, getWordCount(%choices)-1));
		if(isObject(%pick))
			%brick.setItem(%pick);
		%choices = removeWord(%choices, %index); //Only one of a kind
	}

	//Reset papers
	for (%i = 0; %i < BrickGroup_888888.NTObjectCount["_evidence"]; %i++)
	{
		%brick = BrickGroup_888888.NTObject["_evidence", %i];
		%brick.setItem("");
	}

	for (%i = 0; %i < 16; %i++)
	{
		$stand[%i].setTransform("0 0 -300");
		$memorial[%i].setTransform("0 0 -300");
	}

	roomPlayers();

	$DespairTrial = false;
	$announcements = 0;
	$investigationStart = "";
	$pickedKiller = false;
	$days = 0;
	$deathCount = 0;
	if($EnvGuiServer::DayCycleEnabled <= 0)
	{
		$EnvGuiServer::DayCycleFile = "Add-Ons/DayCycle_DespairFever/fever.daycycle";
		loadDayCycle($EnvGuiServer::DayCycleFile);
		$EnvGuiServer::DayCycleEnabled = 1;
		DayCycle.setEnabled($EnvGuiServer::DayCycleEnabled);
	}
	$EnvGuiServer::DayLength = $Despair::DayLength;
	DayCycle.setDayLength($EnvGuiServer::DayLength);
	setDayCycleTime(0.4); //Starts at evening
	if(!isEventPending(DayCycle.timeSchedule))
		DayCycle.timeSchedule();
	DespairSetWeapons(1);
	ServerPlaySong("MusicGameStart");
}

function DespairSetWeapons(%tog)
{
	$DefaultMiniGame.noWeapons = !%tog;
	for (%i = 0; %i < $DefaultMiniGame.numMembers; %i++)
	{
		%member = $DefaultMiniGame.member[%i];
		%player = %member.player;
		if (!isObject(%player))
			continue;
		if (%member.killer && !%tog)
			messageClient(%member, '', "<font:impact:24>\c5You will be unable to swing your weapons anymore.");
		//if (%player.getMountedImage(0) && %player.getMountedImage(0).item.className $= "DespairWeapon")
		//	%player.unMountImage(0);
	}
}

function fxDayCycle::timeSchedule(%this, %lastStage)
{
	cancel(%this.timeSchedule);

	if($DespairTrial)
		return;

	%time = getDayCycleTime();

	if(%time > 0.75)
		%stage = "NIGHT";
	else if(%time > 0.4)
		%stage = "EVENING";
	else if(%time > 0.25)
		%stage = "NOON";
	else
		%stage = "MORNING";

	if(%stage !$= %lastStage)
		despairCycleStage(%stage);

	if(isObject($DefaultMiniGame))
		for (%i = 0; %i < $DefaultMiniGame.numMembers; %i++)
			$DefaultMiniGame.member[%i].updateBottomprint();

	%sched = getMax(50, (%this.DayLength * 60) / 86400); //insanely weird and complicated thingy to make schedule happen every time a "second" actually passes
	%this.timeSchedule = %this.schedule(%sched, timeSchedule, %stage);
}

function despairCycleStage(%stage)
{
	talk("It is now \c3" @ %stage);
	if(%stage $= "NIGHT")
	{
		despairOnNight();
	}

	if(%stage $= "EVENING")
	{
		despairOnEvening();
	}

	if(%stage $= "NOON")
	{
		despairOnNoon();
	}

	if(%stage $= "MORNING")
	{
		$days++;
		talk("DAY" SPC $days);
		despairOnMorning();
	}

	if($days >= 3) //Court 'em on the third day no matter what
	{
		courtPlayers();
	}
}

package DespairFever
{
	function Player::mountImage(%this, %image, %slot, %loaded, %skinTag)
	{
		//if (isObject(%this.client) && %this.client.miniGame == $DefaultMiniGame && (%image.item.className $= "DespairWeapon" && $DefaultMiniGame.noWeapons))
		//{
		//	fixArmReady(%this);
		//	return;
		//}
		parent::mountImage(%this, %image, %slot, %loaded, %skinTag);
	}

	function fxDayCycle::setEnabled(%this, %bool)
	{
		parent::setEnabled(%this, %bool);
	}

	function Player::removeBody(%player)
	{
		%player.delete();
	}

	function MiniGameSO::addMember($DefaultMiniGame, %client)
	{
		Parent::addMember($DefaultMiniGame, %client);

		if (!$DefaultMiniGame.owner && $DefaultMiniGame.numMembers == 2)
			despairPrepareGame();
	}

	function MiniGameSO::Reset(%this, %client)
	{
		if (%this.owner != 0)
			return Parent::reset(%this, %client);

		// Play nice with the default rate limiting.
		if (getSimTime() - %this.lastResetTime < 5000)
			return;

		Parent::reset(%this, %client);
		despairPrepareGame();
	}

	function MiniGameSO::checkLastManStanding($DefaultMiniGame)
	{
		if ($DefaultMiniGame.owner)
			return Parent::checkLastManStanding($DefaultMiniGame);
		
		%alive = 0;
		%killerAlive = 0;
		%otherAlive = 0;
		
		for (%i = 0; %i < $DefaultMiniGame.numMembers; %i++)
		{
			%client = $DefaultMiniGame.member[%i];
			%player = %client.player;

			if (!%player)
				continue;

			if (%client.killer)
				%killerAlive = 1;
			else
				%otherAlive = 1;

			%alive++;
			%last = %client;
		}
		if(!%otherAlive)
		{
			talk("Everybody is dead dave");
			despairEndGame();
		}
		if(!%killerAlive && $pickedKiller)
		{
			talk("Killer is dead, rip");
			despairEndGame();
		}
		if(%alive == 1)
		{
			talk("I AM THE ONE AND ONLY");
			despairEndGame();
		}
		return 0;
	}

	function MiniGameSO::pickSpawnPoint($DefaultMiniGame, %client)
	{
		if ($DefaultMiniGame.owner || $DefaultMiniGame.numMembers < 2)
			return Parent::pickSpawnPoint($DefaultMiniGame, %client);
	}

	function serverCmdLight(%client)
	{
		if (%client.miniGame != $DefaultMiniGame)
			Parent::serverCmdLight(%client);

		if(%client.player && %client.killer)
		{
			if ($Sim::Time - %client.lastKillerScan < 3)
				return;
			%client.lastKillerScan = $Sim::Time;

			for (%i = 0; %i < $DefaultMiniGame.numMembers; %i++)
			{
				%member = $DefaultMiniGame.member[%i];

				if (%member.player && %member.player != %client.player)
					%client.play3d(HeartBeatSound, %member.player.getEyePoint());
			}
		}
	}

	function serverCmdSuicide(%client)
	{
		if (%client.miniGame != $DefaultMiniGame)
			return Parent::serverCmdSuicide(%client);
	}

	function Item::schedulePop(%this)
	{
		GameRoundCleanup.add(%this);
	}

	function ItemData::onAdd(%this, %item)
	{
		Parent::onAdd(%this, %item);
		if (%this.canPickUp !$= "")
			%item.canPickUp = %this.canPickUp;
	}
};
activatePackage("DespairFever");