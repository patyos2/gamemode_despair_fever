
$Despair::DayLength = 420; //7 minutes for a full cycle

//MOTEL MAP PREFS:
$Despair::RoomCount = 16;
//male block
$roomNum[1] = "16";
$roomNum[2] = "17";
$roomNum[3] = "18";
$roomNum[4] = "26";
$roomNum[5] = "27";
$roomNum[6] = "28";

//female block
$roomNum[7] = "11";
$roomNum[8] = "12";
$roomNum[9] = "13";
$roomNum[10] = "14";
$roomNum[11] = "15";
$roomNum[12] = "21";
$roomNum[13] = "22";
$roomNum[14] = "23";
$roomNum[15] = "24";
$roomNum[16] = "25";
////

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
			player = %player;
		};

		GameCharacters.add(%character);
		%client.character = %character;

		//Assign character to client
		%client.killer = 0;
		%client.spawnPlayer(); //PROTIP: Create players as AIPlayers so you can control them like bots in cutscenes
		%player = %client.player;
		%player.gender = %gender; //so screams are correct post-death
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
	}
}

function despairEndGame()
{
	cancel($DefaultMiniGame.restartSchedule);
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

	roomPlayers();

	%client = $DefaultMiniGame.member[getRandom(1, $DefaultMiniGame.numMembers) - 1];

	%itemList = "axeItem batItem katanaItem knifeItem leadpipeItem macheteItem shovelItem sledgehammerItem umbrellaItem pipewrenchItem";
	%client.player.setTool(%client.player.weaponSlot, getWord(%itemList, getRandom(0, getWordCount(%itemList) - 1)));
	%client.centerPrint("wow you are killer go kill shit", 3);
	%client.killer = true;
	echo(%client.getplayername() SPC "is killa");

	$days = 0;
	if($EnvGuiServer::DayCycleEnabled <= 0)
	{
		$EnvGuiServer::DayCycleFile = "Add-Ons/DayCycle_DespairFever/fever.daycycle";
		loadDayCycle($EnvGuiServer::DayCycleFile);
		$EnvGuiServer::DayLength = $Despair::DayLength;
		DayCycle.setDayLength($EnvGuiServer::DayLength);
		$EnvGuiServer::DayCycleEnabled = 1;
		DayCycle.setEnabled($EnvGuiServer::DayCycleEnabled);
	}
	setDayCycleTime(0);
}

function despairOnKill(%victim, %attacker)
{
	if(isObject(%victim) && !%victim.killer && isObject(%attacker) && !%attacker.killer && %victim != %attacker)
		talk("WOW" SPC %attacker.getplayername() SPC "RDMed WHAT A FAGGOT");
}

function despairCycleStage(%stage)
{
	talk("It is now \c3" @ %stage);
	if(%stage $= "NIGHT")
		talk("GO SLEEP FUCKASSES");

	if(%stage $= "MORNING")
	{
		$days++;
		talk("DAY" SPC $days);
	}

	if(%stage $= "NOON")
		talk("GO EAT ASSWITS");

	if($days >= 2)
	{
		talk("wow survivors won you suck killer");
		despairEndGame();
	}
}


function fxDayCycle::timeSchedule(%this, %lastStage)
{
	cancel(%this.timeSchedule);

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

package DespairFever
{
	function fxDayCycle::setEnabled(%this, %bool)
	{
		parent::setEnabled(%this, %bool);
		if(%bool)
			%this.timeSchedule();
		else
			cancel(%this.timeSchedule);
	}

	function Player::removeBody(%player)
	{
		%player.delete();
	}

	function MiniGameSO::addMember(%miniGame, %client)
	{
		Parent::addMember(%miniGame, %client);

		if (!%miniGame.owner && %miniGame.numMembers == 1)
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

	function MiniGameSO::checkLastManStanding(%miniGame)
	{
		if (%miniGame.owner)
			return Parent::checkLastManStanding(%miniGame);
		
		%alive = 0;
		%killerAlive = 0;
		%otherAlive = 0;
		
		for (%i = 0; %i < %miniGame.numMembers; %i++)
		{
			%client = %miniGame.member[%i];
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
		if(!%killerAlive)
		{
			talk("Killer is dead, rip");
			despairEndGame();
		}
		return 0;
	}

	function MiniGameSO::pickSpawnPoint(%miniGame, %client)
	{
		if (%miniGame.owner)
			return Parent::pickSpawnPoint(%miniGame, %client);
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
		//if (%client.miniGame != $DefaultMiniGame)
			return Parent::serverCmdSuicide(%client);
	}

	function Item::schedulePop(%this)
	{
		GameRoundCleanup.add(%this);
	}
};
activatePackage("DespairFever");