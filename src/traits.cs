$Despair::Traits::Tick = 3000; //miliseconds

$Despair::Traits::Positive = "Investigative	Heavy Sleeper	Gang Member	Extra Tough	Bodybuilder	Athletic	Loudmouth"; //Medium
$Despair::Traits::Neutral = "Snorer	Feel No Pain	Hatter	Chain Smoker";
$Despair::Traits::Negative = "Clumsy	Paranoid	Nervous	Frail	Cold	Sluggish	Hemophiliac	Squeamish	Softspoken"; //Schizo Narcoleptic

//positive
$Despair::Traits::Description["Investigative"] = "You will get more information from corpses.";
$Despair::Traits::Description["Heavy Sleeper"] = "Can sleep on any surface without suffering sore back!";
$Despair::Traits::Description["Gang Member"] = "Totally gangsta. Has a cool hat and some lockpicks.";
$Despair::Traits::Description["Extra Tough"] = "More resistant to weapon damage!";
$Despair::Traits::Description["Bodybuilder"] = "Faster weapon swings!";
$Despair::Traits::Description["Athletic"] = "Slightly faster run speed!";
$Despair::Traits::Description["Loudmouth"] = "Louder speech, as well as a Scream ability during trial to shut everyone up.";
$Despair::Traits::Description["Pickpocket"] = "Can loot people even if they're conscious! Can't steal weapons and worn items.";
//disabled
$Despair::Traits::Description["Medium"] = "You're not supposed to have this";//"Hear the dead when sleeping...";

//neutral
$Despair::Traits::Description["Snorer"] = "Snore loudly when sleeping.";
$Despair::Traits::Description["Feel No Pain"] = "No pain effects!";
$Despair::Traits::Description["Hatter"] = "Spawn with a random hat in your room!";
$Despair::Traits::Description["Chain Smoker"] = "Cough constantly.";

//negative
$Despair::Traits::Description["Clumsy"] = "Trip on blood and dropped items, chance to drop held item when tripping!";
$Despair::Traits::Description["Paranoid"] = "Constantly alert. Never able to get a good night's rest.";
$Despair::Traits::Description["Nervous"] = "Stuttered speech, easily stressed out.";
$Despair::Traits::Description["Frail"] = "Less health.";
$Despair::Traits::Description["Cold"] = "Constantly ill...";
$Despair::Traits::Description["Sluggish"] = "Slightly slower run speed.";
$Despair::Traits::Description["Hemophiliac"] = "Bleed more.";
$Despair::Traits::Description["Squeamish"] = "Blood makes you scream! Seeing corpses will make you faint.";
$Despair::Traits::Description["Softspoken"] = "quieter speech, unable to use caps...";
//disabled
$Despair::Traits::Description["Narcoleptic"] = "You're not supposed to have this";//"Randomly pass out.";
$Despair::Traits::Description["Schizo"] = "You're not supposed to have this";//"Daydreaming and spooky voices!!";

function Player::traitSchedule(%obj)
{
	cancel(%obj.traitSchedule);
	if(%obj.getState() $= "Dead")
		return;

	if($despairTrial $= "")
	{
		if(!%obj.client.killer && !%obj.unconscious && !isEventPending(%obj.passOutSchedule))
		{
			if((%se = %obj.statusEffect[$SE_sleepSlot]) $= "exhausted" && getRandom() < 0.05)
			{
				messageClient(%obj.client, '', "\c5You're about to pass out due to your \c3exhaustion\c5...");
				%obj.passOutSchedule = %obj.schedule(1000, knockOut, 90);
			}
		}
		if(%obj.character.trait["Squeamish"])
		{
			%ges = %obj.getEnvironmentStress();
			%stress = getWord(%ges, 0);
			%foundCorpse = getWord(%ges, 1);

			if(%obj.client.killer)
				%foundCorpse = ""; //what corpse?

			if(isObject(%foundCorpse) && !%foundcorpse.checkedBy[%obj])
				serverCmdAlarm(%obj.client, 1); //very easy (and lazy) way of doing this. despairCheckInvestigation has Squeamish check for fainting, too.
			else if(%stress && getRandom() < 0.03 * %stress)
			{
				%text[%high++] = "hyperventilates!";
				%text[%high++] = "freaks out!";
				%text[%high++] = "gasps!";
				%text = %text[getRandom(%high)];
				serverCmdMe(%obj.client, %text);
			}
		}
	}
	if(%obj.character.trait["Schizo"])
	{
		%random = GameCharacters.getObject(getRandom(0, GameCharacters.getCount()-1));
		if(getRandom() < 0.03)
		{
			%dream = getDreamText();
			if (getRandom() < 0.15 && isObject(%random)) //less chance for a random character name to appear
				%dream = %random.name;
			messageClient(%obj.client, '', '   \c1... %1 ...', %dream);
		}
		else if(getRandom() < 0.05)
		{
			%high = -1;
			%type[%high++] = "whispers";
			%type[%high++] = "says";
			%type[%high++] = "yells";
			%type[%high++] = "stammers";
			%type[%high++] = "radios";
			%type = %type[getRandom(%high)];
			%time = getDayCycleTimeString(getRandom() * 1.5, 1);
			messageClient(%obj.client, '', '\c7[%1]<color:ffff80>%2 %3<color:fffff0>, %4', %time, "Unknown", %type, getDreamText() SPC getDreamText() SPC getDreamText() SPC getDreamText());
		}
	}
	if(%obj.character.trait["Cold"])
	{
		if(getRandom() < 0.015)
		{
			%text[%high++] = "sneezes!";
			%text[%high++] = "sniffs!";
			%text[%high++] = "coughs!";
			%text = %text[getRandom(%high)];
			serverCmdMe(%obj.client, %text);
		}
	}
	if(%obj.character.trait["Chain Smoker"])
	{
		if(getRandom() < 0.015)
			serverCmdMe(%obj.client, "coughs!");
	}
	%obj.traitSchedule = %obj.schedule(getMax(500, $Despair::Traits::Tick), traitSchedule, %obj);
}