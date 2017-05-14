$Despair::Traits::Tick = 3000; //miliseconds

$Despair::Traits::Positive = "Investigative	Heavy Sleeper	Gang Member	Extra Tough	Bodybuilder	Athletic	Survivalist	Medium	Loudmouth";
$Despair::Traits::Neutral = "Snorer	Radioactive	Feel No Pain	Hatter	Chain Smoker	Anemic";
$Despair::Traits::Negative = "Clumsy	Paranoid	Nervous	Schizo	Frail	Cold	Sluggish	Hemophiliac	Squeamish	Klutz	Narcoleptic	Insomniac	Softspoken";

$Despair::Traits::Description["Investigative"] = "You will get more information from corpses.";
$Despair::Traits::Description["Heavy Sleeper"] = "Can sleep on any surface without suffering sore back!";
$Despair::Traits::Description["Gang Member"] = "Placeholder Trait.";
$Despair::Traits::Description["Extra Tough"] = "More resistant to weapon damage!";
$Despair::Traits::Description["Bodybuilder"] = "Faster weapon swings!";
$Despair::Traits::Description["Athletic"] = "Slightly faster run speed!";
$Despair::Traits::Description["Survivalist"] = "Placeholder Trait.";
$Despair::Traits::Description["Medium"] = "Placeholder Trait.";
$Despair::Traits::Description["Loudmouth"] = "Louder speech, as well as a Scream ability during trial to shut everyone up.";

$Despair::Traits::Description["Snorer"] = "Snore loudly when sleeping.";
$Despair::Traits::Description["Radioactive"] = "Placeholder Trait.";
$Despair::Traits::Description["Feel No Pain"] = "No pain effects!";
$Despair::Traits::Description["Hatter"] = "Spawn with a random hat in your room!";
$Despair::Traits::Description["Chain Smoker"] = "Placeholder Trait.";
$Despair::Traits::Description["Anemic"] = "Placeholder Trait.";

$Despair::Traits::Description["Clumsy"] = "Placeholder Trait.";
$Despair::Traits::Description["Paranoid"] = "Placeholder Trait.";
$Despair::Traits::Description["Nervous"] = "Stuttered speech, easily stressed out.";
$Despair::Traits::Description["Schizo"] = "Daydreaming and spooky voices!!";
$Despair::Traits::Description["Frail"] = "Less health.";
$Despair::Traits::Description["Cold"] = "Constantly ill...";
$Despair::Traits::Description["Sluggish"] = "Slightly slower run speed.";
$Despair::Traits::Description["Hemophiliac"] = "Placeholder Trait.";
$Despair::Traits::Description["Squeamish"] = "Placeholder Trait.";
$Despair::Traits::Description["Klutz"] = "Placeholder Trait.";
$Despair::Traits::Description["Narcoleptic"] = "Randomly pass out.";
$Despair::Traits::Description["Insomniac"] = "Placeholder Trait.";
$Despair::Traits::Description["Softspoken"] = "quieter speech, unable to use caps...";

function Player::traitSchedule(%obj)
{
	cancel(%obj.traitSchedule);
	if(%obj.getState() $= "Dead")
		return;

	if($despairTrial $= "")
	{
		if(%obj.character.trait["Narcoleptic"] && !%obj.unconscious && $Sim::Time - %obj.lastKO > 120)
		{
			if(((%se = %obj.statusEffect[$SE_sleepSlot]) !$= "" && getRandom() < 0.05) || getRandom() < 0.02)
			{
				messageClient(%obj.client, '', "\c5You pass out due to your \c3narcolepsy\c5...");
				%sec = %se $= "exhausted" ? 60 : 30;
				%obj.knockOut(%sec);
			}
		}
	}
	if(%obj.character.trait["Schizo"])
	{
		%random = GameCharacters.getObject(getRandom(0, GameCharacters.getCount()));
		if(getRandom() < 0.15)
		{
			%dream = getDreamText();
			if (getRandom() < 0.15) //less chance for a random character name to appear
				%dream = %random.name;
			messageClient(%obj.client, '', '   \c1... %1 ...', %dream);
		}
		else if(getRandom() < 0.2)
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
		if(getRandom() < 0.1)
		{
			%text[%high++] = "sneezes!";
			%text[%high++] = "sniffs!";
			%text[%high++] = "coughs!";
			%text = %text[getRandom(%high)];
			serverCmdMe(%obj.client, %text);
		}
	}
	%obj.traitSchedule = %obj.schedule(getMax(500, $Despair::Traits::Tick), traitSchedule, %obj);
}