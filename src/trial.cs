datablock StaticShapeData(DespairStand)
{
	shapeFile = $Despair::Path @ "res/shapes/stand.dts";
	canDismount = false;
};
datablock StaticShapeData(DespairMemorial)
{
	shapeFile = $Despair::Path @ "res/shapes/memorial.dts";
};

datablock ShapeBaseImageData(MemorialCrossImage)
{
	shapeFile = $Despair::Path @ "res/shapes/cross.dts";
	mountPoint = $headSlot;
	doColorShift = true;
	colorShiftColor = "0.65 0 0 1";
	offset = "0 0.5 0";
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
	$stand16 = createShape(DespairStand, "0 0 -300");
	$stand17 = createShape(DespairStand, "0 0 -300");
	$stand18 = createShape(DespairStand, "0 0 -300");
	$stand19 = createShape(DespairStand, "0 0 -300");
	$stand20 = createShape(DespairStand, "0 0 -300");
	$stand21 = createShape(DespairStand, "0 0 -300");
	$stand22 = createShape(DespairStand, "0 0 -300");
	$stand23 = createShape(DespairStand, "0 0 -300");
	
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
	$memorial16 = createShape(DespairMemorial, "0 0 -300");
	$memorial17 = createShape(DespairMemorial, "0 0 -300");
	$memorial18 = createShape(DespairMemorial, "0 0 -300");
	$memorial19 = createShape(DespairMemorial, "0 0 -300");
	$memorial20 = createShape(DespairMemorial, "0 0 -300");
	$memorial21 = createShape(DespairMemorial, "0 0 -300");
	$memorial22 = createShape(DespairMemorial, "0 0 -300");
	$memorial23 = createShape(DespairMemorial, "0 0 -300");

	$instantGroup = %oldInstantGroup;
}
if (!isObject(MissionCleanup))
	schedule("0", "0", "createCourtroom");

function despairOnKill(%victim, %attacker, %crit)
{
	if(!isObject(%victim) || !isObject(%attacker))
		return;

	if(%victim == %attacker)
	{
		%victim.player.suicide = true;
		return 1;
	}

	if(!%victim.killer && !%attacker.killer)
	{
		%attacker.player.setSpeedScale(0.1);
		%attacker.player.noWeapons = true;
		%msg = "<font:Impact:30>" @ %attacker.getplayername() SPC "RDMed" SPC %victim.getPlayerName() @ "!";
		echo("-+ " @ %msg);
		%count = ClientGroup.getCount();
		for (%i = 0; %i < %count; %i++)
		{
			%other = ClientGroup.getObject(%i);
			if (%other.isAdmin)
			{
				messageClient(%other, '', %msg);
				%other.play2d(DespairAdminBwoinkSound);
			}
		}
		return 0;
	}

	if(%victim.killer || %attacker.killer)
	{
		%player = %victim.player;
		if(!isObject(%player))
			%player = %victim.character.player;
		if(!%crit)
			$deathCount++;
		%player.isMurdered = true;
		if(%victim.killer && !%attacker.killer)
		{
			%attacker.killer = true;
			$currentKiller = %attacker;
			%attacker.play2d(KillerJingleSound);
			%msg = "<color:FF0000>You murdered the killer in cold blood! Now it's your turn to get away with it...";
			messageClient(%attacker, '', "<font:impact:30>" @ %msg);
			if(%attacker.player.unconscious)
				%attacker.player.WakeUp();
			if(%attacker.player.statusEffect[$SE_sleepSlot] !$= "")
					%attacker.player.removeStatusEffect($SE_sleepSlot);
		}
		if ($deathCount >= $maxDeaths)
			DespairSetWeapons(0);
		//if(!isEventPending($DefaultMiniGame.missingSchedule))
		//	$DefaultMiniGame.missingSchedule = schedule($Despair::MissingLength*1000, 0, "despairStartInvestigation");
		return 1;
	}
}

function despairCheckInvestigation(%player, %corpse)
{
	if(!%corpse.suicide && !%corpse.checkedBy[%player])
	{
		if(isObject(%player.client))
			%player.client.play2d(DespairBodyDiscoverySound @ getRandom(1, 3));
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
	%time = getDayCycleTime();
	%time += 0.25; //so Zero = 6 AM aka morning, Youse's daycycle begins from morning at 0 fraction
	%time = %time - mFloor(%time); //get rid of excess stuff

	%time = getDayCycleTimeString(%time, 1);
	$DefaultMiniGame.messageAll('', '\c7[\c6%3\c7]\c0%2 on premises! \c5You guys have %1 minutes to investigate them before the trial starts.',
		MCeil(($investigationStart - $Sim::Time)/60), %unfound ? "There are corpses to be found" : ($announcements > 1 ? "Another body has been discovered" : "A body has been discovered"), %time);

	%profile = DespairMusicInvestigationIntro1;
	if($announcements > 2)
		%profile = DespairMusicInvestigationIntro2;
	if(!isObject(ServerMusic) || (ServerMusic.profile !$= %profile && ServerMusic.profile !$= %profile.loopProfile))
	{
		if(ServerMusic.profile $= DespairMusicInvestigationLoop1)
		{
			ServerPlaySong(%profile);
		}
		else
		{
			cancel($musicSchedule);
			$musicSchedule = schedule(15000, 0, ServerPlaySong, %profile);
		}
	}
}

function despairStartInvestigation(%no_announce)
{
	//%maxDeaths = mCeil(GameCharacters.getCount() / 4); //16 chars = 4 deaths, 8 chars = 2 deaths
	//if ($deathCount >= %maxDeaths)
		DespairSetWeapons(0);
	if ($deathCount > 0 && $investigationStart $= "")
	{
		$investigationStart = $Sim::Time + $Despair::InvestigationLength;
		if (!%no_announce)
			despairMakeBodyAnnouncement(1);
		cancel($DefaultMiniGame.missingSchedule);
		cancel($DefaultMiniGame.eventSchedule);
		$DefaultMiniGame.eventSchedule = schedule($Despair::InvestigationLength*1000, 0, "courtPlayers");
		serverPlay2d("DespairMusicInvestigationStart");
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

	%evidencePapers = $deathCount > 0 ? 0 : ($days + (getRandom(0, 2) - 1));
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
			%props.contents = getPaperEvidence();
			%evidencePapers--;
			continue;
		}
		else if(%tipsPapers >= 0)
		{
			%props.name = "LifeHack";
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
		break;
	}

	if($days == 1 && $deathCount <= 0)
	{
		%brick = BrickGroup_888888.NTObject["_r" @ $currentKiller.character.room @ "_closet", 0];
		if(isObject(%brick))
		{
			%brick.setItem("KillerBoxItem");
			messageClient($currentKiller, '', '<font:Impact:24>\c5Your closet now contains a box of useful items.');
		}
		//else if($currentKiller.player.addTool("KillerBoxItem") == -1)
		//	messageClient($currentKiller, '', '<font:Impact:24>\c5You have a box of useful items in your inventory!');
	}

	if($days == 2)
	{
		$DefaultMiniGame.chatMessageAll('', "\c0<font:impact:30>WARNING\c5: This is the last day! Have you found any evidence?");
		if($deathCount <= 0)
			messageClient($currentKiller, '', "<font:impact:30>WARNING\c6: This is your last chance to kill. If you don't commit murder you will be \c0AUTOBANNED\c6.");
	}

	if($days >= 3 && $investigationStart $= "") //Court 'em on the third day if there's no investigation
	{
		if($deathCount <= 0)
			banBLID($pickedKiller.bl_id, 300, "Stalling for 3 days straight as the killer.");
		else
			courtPlayers();
	}
}

function despairOnNoon()
{
	
}

function despairOnEvening()
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
}

function despairOnLateEvening()
{
	for (%i = 0; %i < $DefaultMiniGame.numMembers; %i++)
	{
		%client = $DefaultMiniGame.member[%i];
		%player = %client.player;
		if(isObject(%player))
		{
			%player.updateStatusEffect($SE_sleepSlot);
			if(!%client.killer && %player.statusEffect[$SE_sleepSlot] $= "")
				%player.setStatusEffect($SE_sleepSlot, "sleepy");
		}
	}

	if($days == 2)
		$DefaultMiniGame.chatMessageAll('', "\c0<font:impact:30>WARNING\c5: This is going to be your last night! After this, trial period starts. Have you gathered enough evidence?");	
}

function despairOnNight()
{
	if(!$pickedKiller)
	{
		%client = chooseNextClient("Killer");
		%client.play2d(KillerJingleSound);
		%msg = "<color:FF0000>You are plotting murder against someone! Kill them and do it in such a way that nobody finds out it\'s you!";
		messageClient(%client, '', "<font:impact:30>" @ %msg);
		commandToClient(%client, 'messageBoxOK', "MURDER TIME!", %msg);
		%client.killer = true;
		echo(%client.getplayername() SPC "is killa");
		$pickedKiller = %client;
		$currentKiller = %client;
		ServerPlaySong("DespairMusicOpeningIntro");

		if(%client.player.unconscious)
			%client.player.WakeUp();
		if(%client.player.statusEffect[$SE_sleepSlot] !$= "")
			%client.player.removeStatusEffect($SE_sleepSlot);
	}
	if($days > 0)
		return;

	ClearFlaggedCharacters(); //call this so joiners-leavers before night 1 are not counted for anything


	// prepare
	for (%i = 0; %i < $DefaultMiniGame.numMembers; %i++)
		%a[%i] = %i;
	// shuffle
	while (%i--)
	{
		%j = getRandom(%i);
		%x = %a[%i - 1];
		%a[%i - 1] = %a[%j];
		%a[%j] = %x;
	}

	%detectiveCount = getRandom(0, 2);
	for (%i = 0; %i < $DefaultMiniGame.numMembers; %i++)
	{
		%client = $DefaultMiniGame.member[%a[%i]];
		%player = %client.player;
		if(isObject(%player))
		{
			%player.updateStatusEffect($SE_sleepSlot); //Update all tiredness-related status effects
			%client.updateBottomprint();
			if(%detectiveCount > 0)
			{
				%player.character.detective = true;
				messageClient(%client, '', "\c5You have a \c3Detective\c5 trait! You will get more information from bodies.");
				%detectiveCount--;
			}
		}
	}

	%brick = BrickGroup_888888.NTObject["_guestlist", 0];
	%brick.setItem("PaperItem");
	%props = %brick.item.getItemProps();
	%props.name = "Guest List";
	%props.contents = getGuestList(0);
}

function DespairSpecialChat(%client, %text)
{
	if(!$DespairTrial)
		return 0;
	if(!isObject(%player = %client.player))
		return 0;

	if($DespairTrialOpening && %client != $DespairTrialCurrSpeaker)
	{
		messageClient(%client, '', "\c0You cannot talk yet! \c5Wait for your turn.");
		return 1;
	}

	return 0;
}

function courtPlayers()
{
	cancel($DefaultMiniGame.missingSchedule);
	cancel($DefaultMiniGame.eventSchedule);
	cancel(DayCycle.timeSchedule);
	DayCycle.setDayLength(1800); //30 mins
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

	for (%i = 0; %i < 24; %i++)
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
			%doll = new Player()
			{
				datablock = playerFrozenArmor;
				character = %character;
			};
			%doll.setTransform(%transform);
			%doll.setScale("1 0.05 1");
			%doll.desaturate = true;
			%doll.applyAppearance(%character);
			%doll.hideNode("larm");
			%doll.hideNode("larmslim");
			%doll.hideNode("rarm");
			%doll.hideNode("rarmslim");
			%doll.hideNode("lhand");
			%doll.hideNode("rhand");
			%doll.hideNode("chest");
			%doll.hideNode("femchest");
			%doll.hideNode("pants");
			%doll.hideNode("lshoe");
			%doll.hideNode("rshoe");
			%doll.mountImage(MemorialCrossImage, 0);
			%doll.noExamine = true;
			%doll.setShapeName(%character.name SPC "(" @ %state @ ")", "8564862");

			GameRoundCleanup.add(%doll);
		}
		else
		{
			if(%player.unconscious)
			{
				%player.WakeUp();
			}

			if(%player.inCameraEvent)
			{
				%player.inCameraEvent = false;
			}

			$stand[%i].player = %player;
			%player.setTransform(vectorAdd(getWords(%transform, 0, 2), "0 0 0.1") SPC getWords(%transform, 3, 6));
			%player.changeDataBlock(playerFrozenArmor);
			%player.playThread(0, "standing");
			%player.setVelocity("0 0 0");
		}
		if(isObject(%client))
			%client.playPath(TrialIntroPath);
	}

	ServerPlaySong("DespairMusicOpeningIntro");
	$DefaultMiniGame.chatMessageAll('', "\c5<font:impact:30>Everyone now has 30 seconds to prepare their opening statements! Before that, nobody can talk.");
	$DefaultMiniGame.eventSchedule = schedule(30000, 0, DespairStartOpeningStatements);

	$chatDelay = 0.5;

	$forceVoteCount = 0;
	$DespairTrialCurrSpeaker = "";
	$DespairTrial = $Sim::Time;
	$DespairTrialVote = false;
	$DespairTrialOpening = true;
	$DespairTrialDiscussion = "";
	DespairSetWeapons(0);

	despairBottomPrintLoop();
}

function DespairStartOpeningStatements()
{
	cancel($DefaultMiniGame.eventSchedule);
	ServerPlaySong("DespairMusicOpeningLoop");
	$DefaultMiniGame.chatMessageAll('', "\c5Let's hear everybody out.");
	$DefaultMiniGame.eventSchedule = schedule(1000, 0, DespairCycleOpeningStatements, 0);
}

function DespairCycleOpeningStatements(%j)
{
	if(%j > GameCharacters.getCount())
	{
		$DefaultMiniGame.eventSchedule = schedule(1000, 0, DespairStartDiscussion);
		$DefaultMiniGame.chatMessageAll('', "\c5All statements have been heard.");
		return;
	}
	%player = $stand[%j].player;
	%client = %player.client;
	if(isObject(%player))
	{
		$DespairTrialCurrSpeaker = %client;
		for (%i = 0; %i < $DefaultMiniGame.numMembers; %i++)
		{
			%targ = $DefaultMiniGame.member[%i];
			%camera = %targ.camera;
			//aim the camera at the target
			%pos = vectorAdd(%player.getHackPosition(), vectorScale(%player.getForwardVector(), 3));
			%delta = vectorSub(%player.getHackPosition(), %pos);
			%deltaX = getWord(%delta, 0);
			%deltaY = getWord(%delta, 1);
			%deltaZ = getWord(%delta, 2);
			%deltaXYHyp = vectorLen(%deltaX SPC %deltaY SPC 0);

			%rotZ = mAtan(%deltaX, %deltaY) * -1; 
			%rotX = mAtan(%deltaZ, %deltaXYHyp);

			%aa = eulerRadToMatrix(%rotX SPC 0 SPC %rotZ); //this function should be called eulerToAngleAxis...

			%camera.setTransform(%pos SPC %aa);
			%camera.setFlyMode();
			%camera.mode = "Observer";
			%targ.setControlObject(%camera);
			%camera.setControlObject(%targ.dummyCamera);
		}
		$DefaultMiniGame.chatMessageAll('', "\c5What will" SPC %player.character.name SPC "say?");
		%client.chatMessage("\c5(It's your turn!)");
		$DefaultMiniGame.eventSchedule = schedule(10000, 0, DespairCycleOpeningStatements, %j+1);
	}
	else
	{
		DespairCycleOpeningStatements(%j+1);
	}
}

function DespairStartDiscussion()
{
	cancel($DefaultMiniGame.eventSchedule);
	for (%i = 0; %i < $DefaultMiniGame.numMembers; %i++)
	{
		%client = $DefaultMiniGame.member[%i];
		%client.playPath(TrialDiscussionPath);
		%client.camera.schedule(5000, setControlObject, %client.camera);
		if(isObject(%client.player))
			%client.schedule(5000, setControlObject, %client.player);
		else
			%client.schedule(5000, setControlObject, %client.camera);
	}
	$DespairTrialOpening = false;
	$DespairTrialDiscussion = $Sim::Time;
	$chatDelay = 0.75; //less spam, please
	ServerPlaySong("DespairMusicTrialDiscussionIntro" @ getRandom(1, 3));
	$DefaultMiniGame.chatMessageAll('', "\c5You have \c3" @ $Despair::DiscussPeriod / 60 @ " minutes\c5 to discuss and reveal the killer.");
	$DefaultMiniGame.chatMessageAll('', "\c5After time has passed you will have to \c0eliminate the killer\c5 by \c3voting.");
	$DefaultMiniGame.chatMessageAll('', "\c0You cannot afford a mistake. \c5Choose wrong, and everyone but the killer will die.");
	$DefaultMiniGame.eventSchedule = schedule($Despair::DiscussPeriod * 1000, 0, DespairStartVote);
}

function DespairStartVote()
{
	cancel($DefaultMiniGame.eventSchedule);
	$DespairTrialVote = true;
	for (%i = 0; %i < $DefaultMiniGame.numMembers; %i++)
	{
		%client = $DefaultMiniGame.member[%i];
		%player = %client.player;
		if(isObject(%player))
		{
			%player.voteTarget = "0";
			%player.canReceiveVote = "1";
			%player.canCastVote = "1";
			%player.playThread(2, armReadyRight);
			DespairUpdateCastVote(%player);
		}
	}

	$chatDelay = 0.5; //BURNING DISCUSSION

	ServerPlaySong("DespairMusicVoteStart");
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
			%player.voteTarget.votes++;
			%player.playThread(2, root);
			cancel(%player.DespairUpdateCastVote);
			%player.canCastVote = "";
			%player.voteTarget = "";
		}
	}
	$DefaultMiniGame.chatMessageAll('', "\c5Your votes have been cast.");
	$DefaultMiniGame.eventSchedule = schedule(5000, 0, DespairEndTrial);
}

function DespairEndTrial()
{
	for (%i = 0; %i < $DefaultMiniGame.numMembers; %i++)
	{
		%client = $DefaultMiniGame.member[%i];
		%player = %client.player;
		
		if (%player.canReceiveVote)
		{
			if (%highestVotes !$= "" && %player.votes $= %highestVotes)
				%tie = 1;
			else if (%highestVotes $= "" || %player.votes > %highestVotes)
			{
				%tie = 0;
				%highestVotes = %player.votes;
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
		if(%unfortunate.client.killer)
		{
			$DefaultMiniGame.chatMessageAll('', "\c5The killer has been eliminated! Innocents won.");
			%unfortunate.kill();
			%win = 1;
		}
		else
		{
			$DefaultMiniGame.chatMessageAll('', '\c5Majority vote - \c6%1\c5 - is innocent. Killer wins.', %unfortunate.client.getPlayerName());
		}
	}
	else
	{
		$DefaultMiniGame.chatMessageAll('', "\c5Nobody voted. Killer wins.");
	}

	if(!%win)
	{
		for (%i = 0; %i < $DefaultMiniGame.numMembers; %i++)
		{
			%client = $DefaultMiniGame.member[%i];
			%player = %client.player;
			if(!%client.killer)
				%player.kill();
		}
		serverPlay2d("DespairMusicKillerWin");
	}
	else
		serverPlay2d("DespairMusicInnocentsWin");

	despairEndGame();
}

function DespairUpdateCastVote(%player)
{
	cancel(%player.DespairUpdateCastVote);

	if (!%player.canCastVote)
		return;

	%a = %player.getEyePoint();
	%v = %player.getAimVector();
	%b = VectorAdd(%a, VectorScale(%v, 100));
	%mask = $TypeMasks::PlayerObjectType;
	%ray = containerRayCast(%a, %b, %mask, %player);
	%col = firstWord(%ray);

	if (%col && !%col.canReceiveVote)
		%col = "0";
	
	%player.voteTarget = %col;
	if(isObject(%col) && isObject(%col.client) && isObject(%col.character))
		commandToClient(%player.client, 'CenterPrint', "\c3You are voting against\c6" SPC %col.character.name, 1);
	else
		commandToClient(%player.client, 'ClearCenterPrint');
	%player.DespairUpdateCastVote = schedule(32, %player, "DespairUpdateCastVote", %player);
}

function DespairTrialDropTool(%cl, %slot)
{
	%pl = %cl.player;
	if (!isObject(%pl.tool[%slot]))
		return;
	%tool = %pl.tool[%slot];
	//if(%tool.isIcon)
	//	return;
	%pl.tool[%slot] = "";
	messageClient(%cl, 'MsgItemPickup', '', %slot, -1, 1);
	%value = 3.825 + %pl.itemOffset;
	if (isObject(%props = %pl.itemProps[%slot]))
		%pl.itemProps[%slot] = "";
	if (%tool.className $= "Hat")
	{
		%pl.unMountImage(2);
		%cl.applyBodyParts();
	}
	if (%tool.getName() $= "CoatItem")
	{
		%pl.unMountImage(1);
		%cl.applyBodyParts();
	}
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
		%item.setShapeName(%item.itemProps.name);
	else
		%item.setShapeName(%tool.uiName);
	%item.canPickUp = false;
	MissionCleanup.add(%item);
	GameRoundCleanup.add(%item);
}