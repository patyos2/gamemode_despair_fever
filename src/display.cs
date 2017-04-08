function GameConnection::updateBottomprint(%this)
{
	%time = getDayCycleTime();
	%time += 0.25; //so Zero = 6 AM aka morning, Youse's daycycle begins from morning at 0 fraction
	%time = %time - mFloor(%time); //get rid of excess stuff

	%str = getDayCycleTimeString(%time, 1);
	%mod12 = getWord(%str, 1);
	%str = "<font:Verdana:26>\c6" @ getWord(%str, 0) SPC (%mod12 $= "PM" ? "\c1" : "\c3") @ %mod12;
	commandToClient(%this, 'bottomPrint', %str, 0, 1);
}