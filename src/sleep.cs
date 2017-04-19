datablock StaticShapeData(PlaneShapeGlowData)
{
	shapeFile = $Despair::Path @ "res/shapes/plane_glow.dts";
};

function Player::KnockOut(%this, %duration)
{
	%this.changeDataBlock(PlayerCorpseArmor);
	%client = %this.client;
	if (isObject(%client) && isObject(%client.camera))
	{
		//messageClient(%client, '', 'You will be unconscious for %1 seconds.', %duration / 1000);
		//if (%client.getControlObject() != %client.camera)
		//{
			// %client.camera.setMode("Corpse", %this);
			// %client.setControlObject(%client.camera);
			if (!isObject($KOScreenShape))
			{
				$KOScreenShape = new StaticShape()
				{
					datablock = PlaneShapeGlowData;
					scale = "1 1 1";
					position = "0 0 -400"; //units below ground level, woo
				};
				$KOScreenShape.setNodeColor("ALL", "0 0 0 1");
			}
			%camera = %client.camera;
			//aim the camera at the target
			%pos = vectorAdd($KOScreenShape.position, "0.2 0 0");
			%delta = vectorSub($KOScreenShape.position, %pos);
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
			%client.setControlObject(%camera);
			%camera.setControlObject(%client.dummyCamera);
		//}
	}

	%this.setArmThread(land);
	%this.setImageTrigger(0, 0);
	%this.playThread(0, "death1");
	%this.playThread(1, "root");
	%this.playThread(2, "root");
	%this.playThread(3, "root");
	%this.setActionThread("root");

	%this.unconscious = true;
	//%this.setShapeNameDistance(0);
	%this.isBody = true;
	if (%duration > 0)
	{
		%this.knockoutStart = getSimTime();
		%this.knockoutLength = %duration;
	}
	%this.setStatusEffect($SE_sleepSlot, "sleeping");
	%this.KnockOutTick(%duration);
}

function Player::KnockOutTick(%this, %ticks, %done)
{
	cancel(%this.wakeUpSchedule);
	if (%this.getState() $= "Dead" || !%this.unconscious)
		return;
	if (%done >= %ticks)
	{
		%this.WakeUp();
		return;
	}
	if (isObject(%this.client))
	{
		%this.client.centerPrint("\c6" @ %ticks - %done SPC "seconds left until you wake up.", 2);
		if (getRandom() < 0.1)
		{
			%dream = getDreamText();
			if (getRandom() < 0.15) //less chance for a random character name to appear
			{
				%character = GameCharacters.getObject(getRandom(0, GameCharacters.getCount()));
				if (isObject(%character))
					%dream = %character.name;
			}
			messageClient(%this.client, '', '   \c1... %1 ...', %dream);
		}
	}
	%this.wakeUpSchedule = %this.schedule(1000, KnockOutTick, %ticks, %done++);
}

function Player::WakeUp(%this)
{
	cancel(%this.wakeUpSchedule);
	if (%this.getState() $= "Dead" || !%this.unconscious)
		return;
	%this.knockoutStart = "";
	%client = %this.client;
	if (isObject(%client) && isObject(%client.camera))
	{
		%client.camera.setMode("Player", %this);
		%client.camera.setControlObject(%client);
		%client.setControlObject(%this);
	}
	%this.setArmThread(look);
	%this.unconscious = false;
	%this.isBody = false;
	%this.setStatusEffect($SE_sleepSlot, "");
	//%this.setShapeNameDistance($defaultMinigame.shapeNameDistance);
	%this.changeDataBlock(PlayerDespairArmor);
	%this.playThread(0, "root");

	%pos = %this.getPosition();

	%ray = containerRayCast(%pos, vectorSub(%pos, "0 0 1"), $TypeMasks::FxBrickObjectType, %this);
	if(!%ray || %ray.getName() !$= "_bed")
		%this.setStatusEffect($SE_passiveSlot, "sore back");
	%client.updateBottomPrint();
}

function serverCmdSleep(%this, %bypass)
{
	if(!isObject(%pl = %this.player) || %this.miniGame != $defaultMinigame || $DespairTrial)
		return;
	%se = %pl.statusEffect[$SE_sleepSlot];
	if (%se !$= "sleepy" && %se !$= "tired" && %se !$= "exhausted")
	{
		%this.chatMessage("\c6You can't sleep yet - you don't feel tired!");
		return;
	}
	%sec = %se $= "exhausted" ? 80 : 60;
	if (%bypass)
	{
		if (%pl.unconscious)
			return;
		%pl.KnockOut(%sec);
		%this.updateBottomPrint();
		return;
	}
	%message = "\c2Are you sure you want to sleep?\nYou will be unconscious for" SPC %sec SPC "seconds!";
	commandToClient(%this, 'messageBoxYesNo', "Sleep Prompt", %message, 'SleepAccept');
}

function serverCmdSleepAccept(%this)
{
	serverCmdSleep(%this, 1);
}

function loadDreamList()
{
	%file = new FileObject();
	%fileName = $Despair::Path @ "data/dreams.txt";

	if (!%file.openForRead(%fileName))
	{
		error("ERROR: Failed to open '" @ %fileName @ "' for reading");
		%file.delete();
		return;
	}

	deleteVariables("$Despair::DreamListItem_*");
	%max = -1;

	while (!%file.isEOF())
	{
		%line = %file.readLine();
		$Despair::DreamListItem[%max++] = %line;
	}

	%file.close();
	%file.delete();

	$Despair::DreamListMax = %max;
}

if (!$Despair::LoadedDreamList)
{
	$Despair::LoadedDreamList = true;
	loadDreamList();
}

function getDreamText()
{
	%text = $Despair::DreamListItem[getRandom($Despair::DreamListMax)];
	return %text;
}