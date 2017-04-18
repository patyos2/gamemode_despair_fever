datablock StaticShapeData(DespairStand)
{
	shapeFile = $Despair::Path @ "res/shapes/stand.dts";
	canDismount = false;
};
datablock StaticShapeData(DespairMemorial)
{
	shapeFile = $Despair::Path @ "res/shapes/memorial.dts";
};

function createShape(%data, %position, %rotation, %scale, %color)
{
	if (%color $= "") %color = "0.5 0.3 0.1 1";
	%shape = new StaticShape()
	{
		datablock = %data;
		position = %position;
		rotation = %rotation;
		scale = %scale;
	};
	%shape.setNodeColor("ALL", %color);
	return %shape;
}

function createCourtroom()
{
	if (isObject(CourtroomGroup))
		CourtroomGroup.deleteAll();
	else
		new SimGroup(CourtroomGroup);
	%oldInstantGroup = $instantGroup;
	$instantGroup = CourtroomGroup;

	$stand0 = createShape(DespairStand,  "0 0 -300");
	$stand1 = createShape(DespairStand,  "0 0 -300");
	$stand2 = createShape(DespairStand,  "0 0 -300");
	$stand3 = createShape(DespairStand,  "0 0 -300");
	$stand4 = createShape(DespairStand,  "0 0 -300");
	$stand5 = createShape(DespairStand,  "0 0 -300");
	$stand6 = createShape(DespairStand,  "0 0 -300");
	$stand7 = createShape(DespairStand,  "0 0 -300");
	$stand8 = createShape(DespairStand,  "0 0 -300");
	$stand9 = createShape(DespairStand,  "0 0 -300");
	$stand10 = createShape(DespairStand, "0 0 -300");
	$stand11 = createShape(DespairStand, "0 0 -300");
	$stand12 = createShape(DespairStand, "0 0 -300");
	$stand13 = createShape(DespairStand, "0 0 -300");
	$stand14 = createShape(DespairStand, "0 0 -300");
	$stand15 = createShape(DespairStand, "0 0 -300");

	$memorial0 = createShape(DespairMemorial,  "0 0 -300");
	$memorial1 = createShape(DespairMemorial,  "0 0 -300");
	$memorial2 = createShape(DespairMemorial,  "0 0 -300");
	$memorial3 = createShape(DespairMemorial,  "0 0 -300");
	$memorial4 = createShape(DespairMemorial,  "0 0 -300");
	$memorial5 = createShape(DespairMemorial,  "0 0 -300");
	$memorial6 = createShape(DespairMemorial,  "0 0 -300");
	$memorial7 = createShape(DespairMemorial,  "0 0 -300");
	$memorial8 = createShape(DespairMemorial,  "0 0 -300");
	$memorial9 = createShape(DespairMemorial,  "0 0 -300");
	$memorial10 = createShape(DespairMemorial, "0 0 -300");
	$memorial11 = createShape(DespairMemorial, "0 0 -300");
	$memorial12 = createShape(DespairMemorial, "0 0 -300");
	$memorial13 = createShape(DespairMemorial, "0 0 -300");
	$memorial14 = createShape(DespairMemorial, "0 0 -300");
	$memorial15 = createShape(DespairMemorial, "0 0 -300");

	$instantGroup = %oldInstantGroup;
}
if (!isObject(MissionCleanup))
	schedule("0", "0", "createCourtroom");

function despairOnKill(%victim, %attacker)
{
	if(!isObject(%victim) || !isObject(%attacker))
		return;

	if(%victim == %attacker)
	{
		//check for killer-induced suicide by comparing logs
		return;
	}

	if(!%victim.killer && !%attacker.killer)
	{
		%msg = "<font:Impact:30>" @ %attacker.getplayername() SPC "RDMed";
		echo("-+ " @ %msg);
		%count = ClientGroup.getCount();
		for (%i = 0; %i < %count; %i++)
		{
			%other = ClientGroup.getObject(%i);
			if (%other.isAdmin)
			{
				messageClient(%other, '', %msg);
			}
		}
		return;
	}

	if(%victim.killer || %attacker.killer)
	{
		%player = %victim.player;
		if(!isObject(%player))
			%player = %victim.character.player;
		%player.isMurdered = true;
		echo("-+ Killer murdered! Yay!");
		if(%victim.killer)
		{
			%attacker.killer = true;
			%attacker.play2d(KillerJingleSound);
			%msg = "<color:FF0000>You murdered the killer in cold blood! It's no self defence. You become the murderer yourself!";
			messageClient(%attacker, '', "<font:impact:30>" @ %msg);
		}
		$deathCount++;
		%maxDeaths = mCeil(GameCharacters.getCount() / 4); //16 chars = 4 deaths, 8 chars = 2 deaths
		if ($deathCount >= %maxDeaths)
			DespairSetWeapons(0);
		if(!isEventPending($DefaultMiniGame.eventSchedule))
			$DefaultMiniGame.eventSchedule = schedule($Despair::MissingLength*1000, 0, "despairStartInvestigation");
		return;
	}
}

function despairCheckInvestigation(%player, %corpse)
{
	if(!%corpse.checkedBy[%player])
	{
		if(isObject(%player.client))
			%player.client.play2d(BodyDiscoverySound @ getRandom(1, 2));
		%corpse.checkedBy[%player] = true;
		%corpse.checked++;
		if(%corpse.checked >= 2 && !%corpse.discovered) //2 people screamed at this corpse!
		{
			despairStartInvestigation(1);
			despairMakeBodyAnnouncement();
			%corpse.discovered = true;
		}
	}
}

function despairMakeBodyAnnouncement(%unfound)
{
	serverPlay2d(AnnouncementSound);
	if (!%unfound)
		$announcements++;
	$DefaultMiniGame.messageAll('', '\c0%2 on school premises! \c5You guys have %1 minutes to investigate them before the trial starts.',
		MCeil(($investigationStart - $Sim::Time)/60), %unfound ? "There are corpses to be found" : ($announcements > 1 ? "Another body has been discovered" : "A body has been discovered"));
}

function despairStartInvestigation(%no_announce)
{
	%maxDeaths = mCeil(GameCharacters.getCount() / 4); //16 chars = 4 deaths, 8 chars = 2 deaths
	if ($deathCount >= %maxDeaths)
		DespairSetWeapons(0);
	if ($deathCount > 0 && $investigationStart $= "")
	{
		$investigationStart = $Sim::Time + $Despair::InvestigationLength;
		if (!%no_announce)
			%this.makeBodyAnnouncement(1);
		cancel($DefaultMiniGame.eventSchedule);
		$DefaultMiniGame.eventSchedule = schedule($Despair::InvestigationLength*1000, 0, "courtPlayers");
	}
}

function despairOnMorning()
{
	for (%i = 0; %i < $DefaultMiniGame.numMembers; %i++)
	{
		%client = $DefaultMiniGame.member[%i];
		%player = %client.player;
		if(isObject(%player))
		{
			%player.updateStatusEffect($SE_sleepSlot); //Update all tiredness-related status effects
			%client.updateBottomprint();
		}
	}

	if($deathCount > 0) //No evidence will spawn if there were deaths
		return;

	%count = BrickGroup_888888.NTObjectCount["_evidence"];
	if(%count <= 0)
		return;
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

	%evidencePapers = $days + (getRandom(0, 2) - 1);
	%tipsPapers = getRandom(3, 6);
	%trashPapers = getRandom(3, 6);
	//Spawn evidence
	for (%i = 0; %i < %count; %i++)
	{
		%brick = BrickGroup_888888.NTObject["_evidence", %a[%i]];
		%brick.setItem(PaperItem);
		%props = %brick.item.getItemProps();

		if(%evidencePapers >= 0)
		{
			%props.name = "Daily News";
			if(getRandom() < 0.6) //Only 60% accurate
				%char = $pickedKiller.character;
			else
				%char = GameCharacters.getObject(getRandom(0, GameCharacters.getCount()-1)); //Unless it accidentaly picks killer again, oops.

			%props.contents = getPaperEvidence(%char);
			%evidencePapers--;
			continue;
		}
		else if(%tipsPapers >= 0)
		{
			%props.name = "lifeHaxx0r";
			%props.contents = getPaperTips();
			%tipsPapers--;
			continue;
		}
		else if(%trashPapers >= 0)
		{
			%props.name = "Paper";
			%props.contents = getPaperTrash();
			%trashPapers--;
			continue;
		}
		return;
	}
}

function despairOnNoon()
{
	
}

function despairOnEvening()
{
}

function despairOnNight()
{
	if(!$pickedKiller)
	{
		%max = $DefaultMiniGame.numMembers;
		// prepare
		for (%i = 0; %i < %max; %i++)
			%a[%i] = %i;
		// shuffle
		while (%i--)
		{
			%j = getRandom(%i);
			%x = %a[%i - 1];
			%a[%i - 1] = %a[%j];
			%a[%j] = %x;
		}
		for (%i = 0; %i < %max; %i++) //Why a for loop? So we can skip dead/unfit players and only pick live ones, duh.
		{
			%client = $DefaultMiniGame.member[%a[%i]];
			if(!isObject(%client.player))
				continue;
			if($KillerBlacklistBLID[%client.getBLID()]) //Blacklisted. Must be a stealth blacklist, otherwise confirmed innocent.
				continue;
			break; //We got our man
		}
		%client.play2d(KillerJingleSound);
		%msg = "<color:FF0000>You are plotting murder against someone! Kill them and do it in such a way that nobody finds out it\'s you!";
		messageClient(%client, '', "<font:impact:30>" @ %msg);
		commandToClient(%client, 'messageBoxOK', "MURDER TIME!", %msg);
		%client.killer = true;
		echo(%client.getplayername() SPC "is killa");
		$pickedKiller = %client;
	}

	for (%i = 0; %i < $DefaultMiniGame.numMembers; %i++)
	{
		%client = $DefaultMiniGame.member[%i];
		%player = %client.player;
		if(isObject(%player))
		{
			%player.updateStatusEffect($SE_sleepSlot); //Update all tiredness-related status effects
			if(!%client.killer && %player.statusEffect[$SE_sleepSlot] $= "")
				%player.statusEffect[$SE_sleepSlot] = "tired";
			%client.updateBottomprint();
		}
	}
}

function courtPlayers()
{
	cancel($DefaultMiniGame.eventSchedule);
	cancel(DayCycle.timeSchedule);
	DayCycle.setDayLength(900); //15 mins
	setDayCycleTime(0.25);
	%charCount = GameCharacters.getCount();
	// prepare
	for (%i = 0; %i < %charCount; %i++)
		%a[%i] = %i;
	// shuffle
	while (%i--)
	{
		%j = getRandom(%i);
		%x = %a[%i - 1];
		%a[%i - 1] = %a[%j];
		%a[%j] = %x;
	}

	for (%i = 0; %i < 16; %i++)
	{
		$stand[%i].setTransform("0 0 -300");
		$memorial[%i].setTransform("0 0 -300");
	}
	
	%radius = getMax(2, %charCount * 0.5);
	for (%i = 0; %i < %charCount; %i++)
	{
		%angle = $m2pi * (%i / %charCount);
		%x = mCos(%angle) * %radius;
		%y = 8 + mSin(%angle) * %radius;
		$stand[%i].setTransform(%x SPC %y SPC "100 0 0 -1" SPC (%angle + $piOver2));

		%character = GameCharacters.getObject(%a[%i]);
		%player = %character.player;
		%client = %character.client;
		%transform = $stand[%i].getSlotTransform(1);
		if(!isObject(%client) || !isObject(%player) || %player.isDead)
		{
			$memorial[%i].setTransform(%transform);
			%state = "LEFT";
			if(isObject(%player))
			{
				if(%player.isMurdered)
					%state = "MURDER";
				else if(%player.suicide)
					%state = "SUICIDE";
				else
					%state = "RDM";
			}
			$memorial[%i].setShapeName(%character.name SPC "(" @ %state @ ")");
		}
		else
		{
			if(%player.inCameraEvent)
			{
				%player.inCameraEvent = false;
				%client.setControlObject(%player);
			}

			%player.setTransform(vectorAdd(getWords(%transform, 0, 2), "0 0 0.1") SPC getWords(%transform, 3, 6));
			%player.changeDataBlock(playerFrozenArmor);
			%player.playThread(0, "standing");
			%player.setVelocity("0 0 0");
		}
	}
	$DefaultMiniGame.chatMessageAll('', "\c5You have \c3" @ $Despair::DiscussPeriod / 60 @ " minutes\c5 to discuss and reveal the killer.");
	$DefaultMiniGame.chatMessageAll('', "\c5After time has passed you will have to \c0eliminate the killer\c5 by \c3voting.");
	$DefaultMiniGame.chatMessageAll('', "\c0You cannot afford a mistake. \c5Choose wrong, and everyone but the killer will die.");
	$DefaultMiniGame.eventSchedule = schedule($Despair::DiscussPeriod * 1000, 0, DespairStartVote);

	$DespairTrial = true;
	DespairSetWeapons(0);
}

function DespairStartVote()
{
	cancel($DefaultMiniGame.eventSchedule);

	for (%i = 0; %i < $DefaultMiniGame.numMembers; %i++)
	{
		%client = $DefaultMiniGame.member[%i];
		%player = %client.player;
		if(isObject(%player))
		{
			%player.voteTarget = "0";
			%player.canReceiveVote = "1";
			%player.canCastVote = "1";
			%player.playThread(1, armReadyRight);
			DespairUpdateCastVote(%player);
		}
	}

	$DefaultMiniGame.chatMessageAll('', "\c5Look at the person you think is the killer within 30 seconds. The person with the most votes \c0will die.");
	$DefaultMiniGame.eventSchedule = schedule(30000, 0, DespairEndVote);
}

function DespairEndVote()
{
	cancel($DefaultMiniGame.eventSchedule);
	for (%i = 0; %i < $DefaultMiniGame.numMembers; %i++)
	{
		%client = $DefaultMiniGame.member[%i];
		%player = %client.player;

		if (%player.canCastVote)
		{
			%votes[%player.voteTarget]++;
			%player.playThread(1, root);
			cancel(%player.DespairUpdateCastVote);
			%player.canCastVote = "";
			%player.voteTarget = "";
		}
	}

	for (%i = 0; %i < $DefaultMiniGame.numMembers; %i++)
	{
		%client = $DefaultMiniGame.member[%i];
		%player = %client.player;
		
		if (%player.canReceiveVote)
		{
			if (%highestVotes !$= "" && %votes[%player] $= %highestVotes)
				%tie = 1;
			else if (%highestVotes $= "" || %votes[%player] > %highestVotes)
			{
				%tie = 0;
				%highestVotes = %votes[%player];
				%unfortunate = %player;
			}

			%player.canReceiveVote = "";
		}
	}

	if (%tie)
	{
		$DefaultMiniGame.chatMessageAll('', "\c5It's a tie. Killer wins.");
	}
	else if (isObject(%unfortunate))
	{
		$DefaultMiniGame.chatMessageAll('', "\c5The person with the most votes has been eliminated.");
		%unfortunate.kill();
	}
	else
	{
		$DefaultMiniGame.chatMessageAll('', "\c5Nobody voted. Killer wins.");
	}
	despairEndGame();
}

function DespairUpdateCastVote(%player)
{
	cancel(%player.DespairUpdateCastVote);

	if (!%player.canCastVote)
		return;

	%a = %player.getEyePoint();
	%v = %player.getEyeVector();
	%b = VectorAdd(%a, VectorScale(%v, 100));
	%mask = $TypeMasks::PlayerObjectType;
	%ray = containerRayCast(%a, %b, %mask, %player);
	%col = firstWord(%ray);

	if (%col && !%col.canReceiveVote)
		%col = "0";
	
	%player.voteTarget = %col;
	%player.DespairUpdateCastVote = schedule(32, %player, "DespairUpdateCastVote", %player);
}

function DespairTrialDropTool(%cl, %slot)
{
	%pl = %cl.player;
	if (!isObject(%pl.tool[%slot]))
		return;
	%tool = %pl.tool[%slot];
	%pl.tool[%slot] = "";
	messageClient(%cl, 'MsgItemPickup', '', %slot, -1, 1);
	%value = 3.825 + %pl.itemOffset;
	if (isObject(%props = %pl.itemProps[%slot]))
		%pl.itemProps[%slot] = "";
	%item = new Item()
	{
		position = "0 0 0";
		datablock = %tool;
		itemProps = %props;
		minigame = %cl;
		static = true;
		rotate = true;
	};
	%value -= getWord(%box = %item.getWorldBox(), 2);
	%height = getWord(%box, 5) - getWord(%box, 2);
	%pl.itemOffset += %height + 0.2;
	%offset = setWord("0 0 0", 2, %value);
	%item.setTransform(vectorAdd(%pl.getPosition(), %offset));
	if (%tool.getName() $= "KeyItem")
		%item.setShapeName(%item.itemProps.name);
	else if (%tool.getName() $= "PaperItem")
		%item.setShapeName("\"" @ %item.itemProps.contents @ "\"");
	else
		%item.setShapeName(%tool.uiName);
	%item.canPickUp = false;
	MissionCleanup.add(%item);
	GameRoundCleanup.add(%item);
}