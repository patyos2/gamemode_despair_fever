function GameConnection::updateBottomprint(%this)
{
	%client = %this;
	if (isObject(%cam = %this.getControlObject()) && %cam.getClassName() $= "Camera" && isObject(%cam.getOrbitObject().client))
	{
		%client = %targ;
		%isSpectate = 1;
	}

	//CLOCK
	if(isEventPending($defaultMiniGame.eventSchedule))
	{
		%subtimer = getTimeString(mFloor(getTimeRemaining($defaultminigame.eventSchedule)/1000));
	}

	%timestr = %subtimer;
	if(!$despairTrial)
	{
		%time = getDayCycleTime();
		%time += 0.25; //so Zero = 6 AM aka morning, Youse's daycycle begins from morning at 0 fraction
		%time = %time - mFloor(%time); //get rid of excess stuff

		%time = getDayCycleTimeString(%time, 1);
		%mod12 = getWord(%time, 1);
		%time = getWord(%time, 0) SPC (%mod12 $= "PM" ? "\c1" : "\c3") @ %mod12;

		%timestr = %time TAB %subtimer;
	}

	//NAME AND STUFF
	%name = %name @ "\c6" @ %client.character.name;

	//STATUS
	if(isObject(%player = %client.player))
	{
		for(%i=0; %i<$SE_maxStatusEffects; %i++)
		{
			%se = %player.statusEffect[%i];
			%color = getStatusEffectColor(%se);
			%stats = %stats SPC %color @ %se;
		}
	}

	if(%isSpectate)
		%a = "\c3 (Spectating)";

	%str = "<font:Verdana:26><tab:120,240><color:FFFFFF>" @ %timestr @ "<just:right>" @ %stats @ "<just:left>\n" @ %name @ %a;

	commandToClient(%this, 'bottomPrint', %str, 0, 1);
}