function GameConnection::updateBottomprint(%this)
{
	//CLOCK
	%time = getDayCycleTime();
	%time += 0.25; //so Zero = 6 AM aka morning, Youse's daycycle begins from morning at 0 fraction
	%time = %time - mFloor(%time); //get rid of excess stuff

	%time = getDayCycleTimeString(%time, 1);
	%mod12 = getWord(%time, 1);
	%time = getWord(%time, 0) SPC (%mod12 $= "PM" ? "\c1" : "\c3") @ %mod12;

	//NAME AND STUFF
	%name = %this.character.name;
	//STATUS
	if(isObject(%player = %this.player))
	{
		for(%i=0; %i<$SE_maxStatusEffects; %i++)
		{
			%se = %player.statusEffect[%i];
			%color = getStatusEffectColor(%se);
			%stats = %stats SPC %color @ %se;
		}
	}

	%str = "<tab:232,464><font:Verdana:26><color:FFFFFF>" @ %time TAB %name TAB %stats;

	commandToClient(%this, 'bottomPrint', %str, 0, 1);
}