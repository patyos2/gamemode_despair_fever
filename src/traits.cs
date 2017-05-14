$Despair::Traits::Tick = 3000; //miliseconds

$Despair::Traits::Positive = "Investigative	Heavy Sleeper	Gang Member	Extra Tough	Bodybuilder	Athletic	Survivalist	Medium	Loudmouth	Anemic";
$Despair::Traits::Neutral = "Snorer	Radioactive	Feel No Pain	Hatter	Chain Smoker	Anemic";
$Despair::Traits::Negative = "Clumsy	Paranoid	Nervous	Schizo	Frail	Cold	Sluggish	Hemophiliac	Squeamish	Klutz	Narcoleptic	Insomniac	Softspoken";

$Despair::Traits::Description["Investigative"] = "You will get more information from corpses.";
$Despair::Traits::Description["Heavy Sleeper"] = "Placeholder Trait.";
$Despair::Traits::Description["Gang Member"] = "Placeholder Trait.";
$Despair::Traits::Description["Extra Tough"] = "More resistant to weapon damage!";
$Despair::Traits::Description["Bodybuilder"] = "Faster weapon swings!";
$Despair::Traits::Description["Athletic"] = "Slightly faster run speed!";
$Despair::Traits::Description["Survivalist"] = "Placeholder Trait.";
$Despair::Traits::Description["Medium"] = "Placeholder Trait.";
$Despair::Traits::Description["Loudmouth"] = "Louder speech, as well as a Scream ability during trial to shut everyone up.";
$Despair::Traits::Description["Anemic"] = "Placeholder Trait.";

$Despair::Traits::Description["Snorer"] = "Snore loudly when sleeping.";
$Despair::Traits::Description["Radioactive"] = "Placeholder Trait.";
$Despair::Traits::Description["Feel No Pain"] = "No pain effects!";
$Despair::Traits::Description["Hatter"] = "Spawn with a random hat in your room!";
$Despair::Traits::Description["Chain Smoker"] = "Placeholder Trait.";

$Despair::Traits::Description["Clumsy"] = "Placeholder Trait.";
$Despair::Traits::Description["Paranoid"] = "Placeholder Trait.";
$Despair::Traits::Description["Nervous"] = "Stuttered speech, easily stressed out.";
$Despair::Traits::Description["Schizo"] = "Placeholder Trait.";
$Despair::Traits::Description["Frail"] = "Less health.";
$Despair::Traits::Description["Cold"] = "Placeholder Trait.";
$Despair::Traits::Description["Sluggish"] = "Slightly slower run speed.";
$Despair::Traits::Description["Hemophiliac"] = "Placeholder Trait.";
$Despair::Traits::Description["Squeamish"] = "Placeholder Trait.";
$Despair::Traits::Description["Klutz"] = "Placeholder Trait.";
$Despair::Traits::Description["Narcoleptic"] = "Placeholder Trait.";
$Despair::Traits::Description["Insomniac"] = "Placeholder Trait.";
$Despair::Traits::Description["Softspoken"] = "quieter speech, unable to use caps...";

function Player::traitSchedule(%obj)
{
	cancel(%obj.traitSchedule);
	if(%obj.character.trait["Narcoleptic"] && !%obj.unconscious)
	{
		if(((%se = %obj.statusEffect[$SE_sleepSlot]) !$= "" && getRandom() < 0.2) || getRandom() < 0.1)
		{
			messageClient(%obj.client, '', "\c5You pass out due to your \c3narcolepsy\c5...");
			%sec = %se $= "exhausted" ? 80 : 60;
			%obj.knockOut(%sec);
		}
	}
	%obj.traitSchedule = %obj.schedule(getMax(500, $Despair::Traits::Tick), traitSchedule, %obj);
}