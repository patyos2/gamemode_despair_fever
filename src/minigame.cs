if (!isObject(GameRoundCleanup))
	new SimSet(GameRoundCleanup);

if (!isObject(GameCharacters))
	new SimSet(GameCharacters);

function ClearFlaggedCharacters()
{
	for(%i=0; %i < GameCharacters.getCount(); %i++)
	{
		%char = GameCharacters.getObject(%i);
		if(%char.deleteMe || !isObject(%char.client))
			%char.delete();
	}
}

function createPlayer(%client)
{
	if(%client.spectating)
	{
		messageClient(%client, '', '\c5You didn\'t spawn because you are spectating.');
		return;
	}

	if(!$freeCount)
	{
		messageClient(%client, '', '\c5All rooms are occupied - you will have to live with someone else (or be a bum)');
		%roomSpawn = BrickGroup_888888.NTObject["_bumSpawn", getRandom(0, BrickGroup_888888.NTObjectCount["_bumSpawn"] - 1)];
	}
	else
	{
		$freeCount--;
		%freeIndex = getRandom($freeCount);
		%room = $freeRoom[%freeIndex];
		for (%j = %freeIndex; %j < $freeCount; %j++)
			$freeRoom[%j] = $freeRoom[%j + 1];

		$freeRoom[$freeCount] = "";

		%roomDoor = BrickGroup_888888.NTObject["_r" @ %room @ "_door", 0];
		%roomSpawn = BrickGroup_888888.NTObject["_r" @ %room @ "_spawn", 0];
	}
	//Create character if none exists
	%gender = getRandomGender();
	if(!isObject(%character = %client.character) || %character.deleteMe || %character.client.noPersistance)
	{
		if(isObject(%character))
			%character.delete();
		%character = new ScriptObject()
		{
			class = "Character";

			gender = %gender;
			name = getRandomName(%gender);
			appearance = getRandomAppearance(%gender);

			client = %client;
			clientName = %client.getPlayerName();
		};

		GameCharacters.add(%character);
		%client.character = %character;
	}
	else
	{
		%character.detective = 0;
		messageClient(%client, '', '\c5Since you survived last round, you will be \c6%1\c5 once more!', %character.name);
	}
	%character.room = %room;
	//Assign character to client
	%client.killer = false;
	%client.spawnPlayer();
	%player = %client.player;
	%player.character = %character; //post-death reference to character
	%character.player = %player;
	%player.setDatablock(PlayerDespairArmor);
	%player.room = %room;
	%player.setShapeNameDistance(0);
	%player.setShapeNameColor("1 1 1");
	%player.setTransform(%roomSpawn.getSpawnPoint());
	%client.updateAFKCheck();

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

	if(%room !$= "")
	{
		%props = KeyItem.newItemProps(%player, 0);
		%props.name = "Room #" @ $roomNum[%room] @ " Key";
		%props.id = %roomDoor.lockId;

		%player.addTool(KeyItem, %props);
	}
	return %player;
}

function roomPlayers()
{
	$freeCount = $Despair::RoomCount;

	for (%i = 0; %i < $freeCount; %i++)
	{
		%room = %i + 1;
		$freeRoom[%i] = %room;
		%roomDoor = BrickGroup_888888.NTObject["_r" @ %room @ "_door", 0];
		%roomDoor.lockId = "R"@%room;
		%roomDoor.lockState = true;
		%roomSpawn = BrickGroup_888888.NTObject["_r" @ %room @ "_spawn", 0];
	}

	ClearFlaggedCharacters();

	%count = $DefaultMiniGame.numMembers;
	// prepare
	for (%i = 0; %i < %count; %i++)
		%a[%i] = %i;
	// shuffle
	while (%i--)
	{
		%j = getRandom(%i);
		%x = %a[%i - 1];
		%a[%i - 1] = %a[%j];
		%a[%j] = %x;
	}

	for (%i = 0; %i < %count; %i++)
	{
        %client = $DefaultMiniGame.member[%a[%i]];

		if (%client.player)
			%client.player.delete();

		%player = createPlayer(%client);

		if(isObject(%player))
		{
			%client.playPath(IntroPath);
			%client.schedule(6000, setControlObject, %player);
			%client.camera.schedule(6000, setControlObject, %client.camera);
		}
		else
		{
			%client.camera.setMode("Observer");
			%client.setControlObject(%client.camera);
			%client.camera.setControlObject(%client.camera);
		}
	}
}

function despairEndGame()
{
	if (isEventPending($DefaultMiniGame.restartSchedule))
		return;
	cancel($musicSchedule);
	cancel($DefaultMiniGame.missingSchedule);
	cancel($DefaultMiniGame.restartSchedule);
	cancel($DefaultMiniGame.eventSchedule);
	if($DefaultMiniGame.numMembers <= 1)
	{
		cancel(DayCycle.timeSchedule);
		return;
	}
	$DefaultMiniGame.chatMessageAll('', '\c6%1 (as %2)\c5 was the killer!', $currentKiller.getPlayerName(), $currentKiller.character.name);
	$currentKiller.character.deleteMe = true;
	$DefaultMiniGame.restartSchedule = $DefaultMiniGame.schedule(10000, reset, 0);
}

function despairPrepareGame()
{
	cancel($DefaultMiniGame.restartSchedule);
	GameRoundCleanup.deleteAll();
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
	%choices[1] = "HatBlindItem HatCatItem HatChefItem HatCowboyItem HatDuckItem HatFancyItem HatFedoraItem HatFoxItem HatGangsterItem HatMountyItem HatPartyhatItem HatRHoodItem HatStrawItem HatSunglassesItem HatTophatItem HatWizardItem";
	%choices[2] = "HatClownItem HatDisguiseItem HatDogeItem HatRichardItem HatSkimaskItem HatGasmaskItem HatMummyItem HatNinjaItem"; //Items that disguise you
	for (%i = 0; %i < BrickGroup_888888.NTObjectCount["_hatSpawn"]; %i++)
	{
		%brick = BrickGroup_888888.NTObject["_hatSpawn", %i];
		%type = 1;
		if(getWordCount(%choices[2]) && getRandom() < 0.4) //less chance for it to be disguise
			%type = 2;
		%pick = getWord(%choices[%type], %index = getRandom(0, getWordCount(%choices[%type])-1));
		if(isObject(%pick))
			%brick.setItem(%pick);
		%choices[%type] = removeWord(%choices[%type], %index); //Only one of a kind
	}

	//Random items!
	%choices = "RepairkitItem LockpickItem PenItem FlashlightItem RepairkitItem LockpickItem PenItem FlashlightItem RadioItem RadioItem RadioItem";
	for (%i = 0; %i < BrickGroup_888888.NTObjectCount["_randomItem"]; %i++)
	{
		%brick = BrickGroup_888888.NTObject["_randomItem", %i];
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

	//Reset Guest list
	%brick = BrickGroup_888888.NTObject["_guestlist", 0];
	%brick.setItem("");

	for (%i = 0; %i < 16; %i++)
	{
		$stand[%i].setTransform("0 0 -300");
		$memorial[%i].setTransform("0 0 -300");
	}

	roomPlayers();

	$chatDelay = 0.5;

	$DespairTrial = "";
	$DespairTrialVote = false;
	$announcements = 0;
	$investigationStart = "";
	$pickedKiller = "";
	$currentKiller = "";
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
	ServerPlaySong("DespairMusicGameStart");
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

	if(isEventPending($bottomPrintSchedule))
		cancel($bottomPrintSchedule);

	%time = getDayCycleTime();

	if(%time > 0.75)
		%stage = "NIGHT";
	else if(%time > 0.583333) //8 PM
		%stage = "LATE EVENING";
	else if(%time > 0.4)
		%stage = "EVENING";
	else if(%time > 0.25)
		%stage = "NOON";
	else
		%stage = "MORNING";

	if(%stage !$= %lastStage)
		despairCycleStage(%stage);

	if(isObject($DefaultMiniGame))
	{
		for (%i = 0; %i < $DefaultMiniGame.numMembers; %i++)
		{
			%member = $DefaultMiniGame.member[%i];
			%member.updateBottomprint();
		}
	}

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

	if(%stage $= "LATE EVENING")
	{
		despairOnLateEvening();
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

	if($days >= 3 && $investigationStart $= "") //Court 'em on the third day if there's no investigation
	{
		courtPlayers();
	}
}

function GameConnection::updateAFKCheck(%this, %previous)
{
	cancel(%this.updateAFKCheck);

	if (!isObject(%player = %this.player))
		return;

	if($despairTrial)
		return;

	%transform = %player.getTransform();

	if (!%player.unconscious && %transform $= %previous && $Sim::Time - %this.lastSpeakTime >= 60)
	{
		%delay = 2000;
		if(!%this.afk)
		{
			%this.afk = true;
			for (%i = 0; %i < ClientGroup.getCount(); %i++)
			{
				%member = ClientGroup.getObject(%i);
				if(%member.isAdmin)
				{
					messageClient(%member, '', '\c2--[\c5%1 is afk.', %this.getPlayerName());
				}
			}
		}
	}
	else
	{
		%delay = 40000;
		if(%this.afk)
		{
			%this.afk = false;
			for (%i = 0; %i < ClientGroup.getCount(); %i++)
			{
				%member = ClientGroup.getObject(%i);
				if(%member.isAdmin)
				{
					messageClient(%member, '', '\c2--[\c5%1 is no longer afk.', %this.getPlayerName());
				}
			}
		}
	}

	%this.updateAFKCheck = %this.schedule(%delay, "updateAFKCheck", %transform);
}

package DespairFever
{
	function Player::removeBody(%player)
	{
		%player.delete();
	}

	function MiniGameSO::addMember($DefaultMiniGame, %client)
	{
		Parent::addMember($DefaultMiniGame, %client);

		if (!$DefaultMiniGame.owner && $DefaultMiniGame.numMembers == 2)
			despairPrepareGame();

		if(!$currentKiller)
			createPlayer(%client);
	}

	function MiniGameSO::removeMember($DefaultMiniGame, %client)
	{
		if(isObject(%pl = %client.player))
		{
			if(%pl.health <= 0)
			{
				%pl.health = $Despair::CritThreshold;
				%pl.critLoop();
			}
			else
			{
				for(%i=0;%i<%pl.getDataBlock().maxTools;%i++)
				{
					serverCmdDropTool(%client, %i);
				}
			}
		}
		if(isObject(%client.character))
		{
			%client.character.deleteMe = true;
		}
		Parent::removeMember($DefaultMiniGame, %client);
	}

	function MiniGameSO::Reset(%this, %client)
	{
		if (%this.owner != 0)
			return Parent::reset(%this, %client);

		// Play nice with the default rate limiting.
		if (getSimTime() - %this.lastResetTime < 5000)
			return;

		Parent::reset(%this, %client);
		if($DefaultMiniGame.numMembers >= 1)
			despairPrepareGame();
	}

	function MiniGameSO::checkLastManStanding($DefaultMiniGame)
	{
		if ($DefaultMiniGame.owner)
			return Parent::checkLastManStanding($DefaultMiniGame);
		if (isEventPending($DefaultMiniGame.restartSchedule))
			return;
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

		if(isObject(%client.player))
			%client.player.onLight();
	}

	function serverCmdSuicide(%client)
	{
		if (%client.miniGame != $DefaultMiniGame)
			return Parent::serverCmdSuicide(%client);

		if(isObject(%player = %client.player) && %player.health <= 0)
		{
			%player.health = -100;
			%player.critLoop();
			messageClient(%client, '', "\c5You succumb to your wounds...");
		}
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