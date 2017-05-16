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
		//pick traits
		%typePositive = $Despair::Traits::Positive;
		%typeNeutral = $Despair::Traits::Neutral;
		%typeNegative = $Despair::Traits::Negative;
		%traitCount = getRandom(2, 3);
		while(%traitCount-- >= 0)
		{
			if(%typeStr $= "positive")
				%typeStr = "negative";
			else if(getRandom(0, 1) == 0)
			{
				%typeStr = "positive";
				if(%traitCount == 0) //guaranteed negative
					%traitCount++;
			}
			else
				%typeStr = "neutral";
			%lastTrait = %trait;
			%trait = getField(%type[%typeStr], %index = getRandom(0, getFieldCount(%type[%typeStr]) - 1));
			%type[%typeStr] = removeField(%type[%typeStr], %index);

			//Check if we picked conflicting traits
			if((%lastTrait $= "Extra Tough" && %trait $= "Frail") || (%lastTrait $= "Athletic" && %trait $= "Sluggish") || (%lastTrait $= "Loudmouth" && %trait $= "Softspoken"))
			{
				%trait = %lastTrait; //rollback a bit, we still need a negative
				%traitCount++;
				continue;
			}
			%character.trait[%trait] = true;
			%character.traitList = setField(%character.traitList, getFieldCount(%character.traitList), %trait);

			%color = %typeStr $= "positive" ? "\c2" : (%typeStr $= "negative" ? "\c0" : "\c6");
			%desc = $Despair::Traits::Description[%trait];
			messageClient(%client, '', '\c5You now have %1%2\c5 trait! %3', %color, %trait, %desc);
		}
		messageClient(%client, '', '\c5Say \c3/traits\c5 to bring up your traits again.');
		GameCharacters.add(%character);
		%client.character = %character;
	}
	else
	{
		messageClient(%client, '', '\c5Since you survived last round, you will be \c6%1\c5 once more!', %character.name);
	}

	if(getField(%character.appearance, 3) $= "")
		%character.appearance = setField(%character.appearance, 3, getRandomHairName(%character.gender));

	%room = %character.room;
	if(%room $= "")
	{
		if(!$freeCount)
		{
			messageClient(%client, '', '\c5All rooms are occupied - you will have to live with someone else (or be a bum)');
			%roomSpawn = BrickGroup_888888.NTObject["_bumSpawn", getRandom(0, BrickGroup_888888.NTObjectCount["_bumSpawn"] - 1)];
		}
		else
		{
			%freeIndex = getRandom(1, $freeCount);
			%room = $freeRoom[%freeIndex];
			$roomOwner[%room] = %character;
			$freeRoom[%freeIndex] = $freeRoom[$freeCount];
			$freeCount--;

			%roomDoor = BrickGroup_888888.NTObject["_r" @ %room @ "_door", 0];
			%roomSpawn = BrickGroup_888888.NTObject["_r" @ %room @ "_spawn", 0];
			%roomCloset = BrickGroup_888888.NTObject["_r" @ %room @ "_closet", 0];
		}
	}
	else
	{
		%roomDoor = BrickGroup_888888.NTObject["_r" @ %room @ "_door", 0];
		%roomSpawn = BrickGroup_888888.NTObject["_r" @ %room @ "_spawn", 0];
		%roomCloset = BrickGroup_888888.NTObject["_r" @ %room @ "_closet", 0];
	}

	%character.room = %room;
	//Assign character to client
	%client.killer = false;
	%client.spawnPlayer();
	%player = %client.player;
	%player.character = %character; //post-death reference to character

	if(%character.trait["Frail"])
	{
		%player.maxhealth = 90;
		%player.health = 90;
	}

	%character.player = %player;
	%player.setDatablock(PlayerDespairArmor);
	%player.room = %room;
	%player.setShapeNameDistance(0);
	%player.setShapeNameColor("1 1 1");
	%player.setTransform(%roomSpawn.getSpawnPoint());
	%client.updateAFKCheck();

	centerPrint(%client, "<bitmap:" @ $Despair::Path @ "res/logo>", 6);
	commandToClient(%client,'PlayGui_CreateToolHud',PlayerDespairArmor.maxTools);
	commandToClient(%client, 'SetVignette', true, $EnvGuiServer::VignetteMultiply SPC $EnvGuiServer::VignetteColor);

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

	if(%character.trait["Hatter"])
	{
		%choices = "HatBlindItem HatCatItem HatChefItem HatCowboyItem HatDuckItem HatFancyItem HatFedoraItem HatFoxItem HatGangsterItem HatMountyItem HatPartyhatItem HatRHoodItem HatStrawItem HatSunglassesItem HatTophatItem HatWizardItem";
		%pick = getWord(%choices, getRandom(0, getWordCount(%choices)-1));
		if(isObject(%roomCloset))
			%roomCloset.spawnItem("0 0 1", %pick);
		else
			%pick.onPickup("", %player);
	}

	if(%character.trait["Gang Member"])
	{
		if(isObject(%roomCloset))
			%roomCloset.spawnItem("0 0 1", "LockpickItem");
		else
			%player.addTool(LockpickItem);
		HatGangsterRedItem.onPickup("", %player);
	}
	return %player;
}

function roomPlayers()
{
	ClearFlaggedCharacters();
	$freeCount = 0;
	for (%i = 0; %i < $Despair::RoomCount; %i++)
	{
		%room = %i + 1;
		%roomDoor = BrickGroup_888888.NTObject["_r" @ %room @ "_door", 0];
		%roomDoor.lockId = "R"@%room;
		%roomDoor.lockState = true;
		%roomBathDoor = BrickGroup_888888.NTObject["_r" @ %room @ "_bathdoor", 0];
		%roomSpawn = BrickGroup_888888.NTObject["_r" @ %room @ "_spawn", 0];
		%roomCloset = BrickGroup_888888.NTObject["_r" @ %room @ "_closet", 0];
		%roomCloset.setItem("");
		if(isObject($roomOwner[%room])) //$roomOwner[%room] is a character. If that character is deleted, rip.
			continue;

		$freeRoom[$freeCount++] = %room;
	}

	%count = $DefaultMiniGame.numMembers;
	for (%i = 0; %i < %count; %i++)
	{
		%client = $DefaultMiniGame.member[%i];

		if (%client.player)
			%client.player.delete();

		%player = createPlayer(%client);
		%ln = getWord(%player.character.name, 1);
		if(%sibling[%ln] !$= "")
		{
			messageClient(%client, '', '\c5You have a \c6sibling\c5 this round! Their name is %1 and they live in room %2.', %sibling[%ln].character.name, %sibling[%ln].character.room);
			messageClient(%sibling[%ln].client, '', '\c5You have a \c6sibling\c5 this round! Their name is %1 and they live in room %2.', %player.character.name, %player.character.room);
		}
		%sibling[%ln] = %player;

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
			%brick.itemData = %brick.item.getDataBlock();
	}

	//Hats!
	%choices[1] = "HatBlindItem HatCatItem HatChefItem HatCowboyItem HatDuckItem HatFancyItem HatFedoraItem HatFoxItem HatGangsterItem HatMountyItem HatPartyhatItem HatRHoodItem HatStrawItem HatSunglassesItem HatTophatItem HatWizardItem";
	%choices[2] = "HatClownItem HatDisguiseItem HatDogeItem HatRichardItem HatSkimaskItem HatGasmaskItem HatMummyItem HatNinjaItem"; //Items that disguise you
	for (%i = 0; %i < BrickGroup_888888.NTObjectCount["_hatSpawn"]; %i++)
	{
		%brick = BrickGroup_888888.NTObject["_hatSpawn", %i];
		%type = 1;
		if(getWordCount(%choices[2]) && getRandom() < 0.15) //less chance for it to be disguise
			%type = 2;
		%pick = getWord(%choices[%type], %index = getRandom(0, getWordCount(%choices[%type])-1));
		if(isObject(%pick))
			%brick.setItem(%pick);
		%choices[%type] = removeWord(%choices[%type], %index); //Only one of a kind
	}

	//Random items!
	%choices = "RazorItem RepairkitItem RepairkitItem LockpickItem LockpickItem PenItem PenItem FlashlightItem FlashlightItem RadioItem RadioItem RadioItem CleanSprayItem CleanSprayItem";
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

	for (%i = 0; %i < 24; %i++)
	{
		$stand[%i].setTransform("0 0 -300");
		$memorial[%i].setTransform("0 0 -300");
	}

	$chatDelay = 0.5;

	$DespairTrial = "";
	$DespairTrialVote = false;
	$announcements = 0;
	$investigationStart = "";
	$pickedKiller = "";
	$currentKiller = "";
	$days = 0;
	$deathCount = 0;
	$maxDeaths = getMax(1, $aliveCount);
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

	//update fog
	$EnvGuiServer::VisibleDistance = 200;
	Sky.visibleDistance = $EnvGuiServer::VisibleDistance;
	$EnvGuiServer::FogDistance = 120;
	Sky.fogDistance = $EnvGuiServer::FogDistance;
	Sky.sendUpdate();

	roomPlayers();

	DespairSetWeapons(1);
	serverStopSong();
	serverPlay2d("DespairMusicGameStart");
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
	else if(%time > 0.6666) //10 PM
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
}

function GameConnection::updateAFKCheck(%this, %previous)
{
	cancel(%this.updateAFKCheck);

	if (!isObject(%player = %this.player))
		return;

	if($despairTrial)
		return;

	%transform = %player.getTransform();

	if (%transform $= %previous && $Sim::Time - %this.lastSpeakTime >= 60)
	{
		%delay = 2000;
		if(!%player.unconscious && !%this.afk)
		{
			%this.afk = true;
			messageClient(%this.client, '', '\c2<font:Impact:20>Warning\c6: You are considered AFK. If you don\'t come back until trial you will be considered dead.');
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
			messageClient(%this.client, '', '\c2<font:Impact:20>Notice\c6: You are no longer considered AFK.');
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

		messageClient(%client, '', '\c5--> \c4Please read \c3/rules\c4 and \c3/help\c4!', %this.getPlayerName());
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
		if(!%client.isAdmin && %client.killer && isObject(%client.player))
			banBLID(%client.bl_id, 5, "Leaving the game as the killer.");
		Parent::removeMember($DefaultMiniGame, %client);
	}

	function MiniGameSO::Reset(%this, %client)
	{
		if (%this.owner != 0)
			return Parent::reset(%this, %client);

		// Play nice with the default rate limiting.
		if (getSimTime() - %this.lastResetTime < 5000)
			return;

		cancel($musicSchedule);
		cancel($DefaultMiniGame.missingSchedule);
		cancel($DefaultMiniGame.restartSchedule);
		cancel($DefaultMiniGame.eventSchedule);
		if($DefaultMiniGame.numMembers < 1)
			cancel(DayCycle.timeSchedule);

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

		$aliveCount = 0;
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

			$aliveCount++;
			%last = %client;
		}

		$maxDeaths = getMax(1, $aliveCount - 4);

		if($deathCount >= $maxDeaths)
			DespairSetWeapons(0);

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
		if($aliveCount == 1)
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
			%player.health = $Despair::CritThreshold;
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