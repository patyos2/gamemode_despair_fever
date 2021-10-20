//Editable
$Despair::StatusEffectLoop = 1000;
//Initialization
$Despair::SE[C] = 0;

//0 - Role
//1 - Mood
//2 - Tiredness
//3 - Passive
//4 - DamageSlot
addStatusEffect("Mood",1,"",true,15,25);
addStatusEffect("Tiredness",2,"",true,1,3);
addStatusEffect("Dieing",4,"",true,1,2);
addStatusEffect("Shock",4);
addStatusEffect("Bleeding",4,"",true,1,3);
addStatusEffect("Suffocating",4);
addStatusEffect("Sore Back",3);
addStatusEffect("Fresh",3,"Sore Back" TAB "Shining");
addStatusEffect("Drowsy",3);
addStatusEffect("Shining",3);
addStatusEffect("Sleeping",-1);
addStatusEffect("Unconsious",-1);
addStatusEffect("Unsettled",-1);

//When a status effect is added to a player it will call the despairSEEffectAdd function where Effect is the status effect's name
//When a status effect is removed from a player it will call the despairSEEffectRemove function where Effect is the status effect's name
//When a status effect is created an entry for it will be added to a global variable depending on input variables
//When a status effect is added to a player if it has a despairSEEffectLoop function where Effect is the status effect's
//it will be added to that player's status effect loop list to be called every loop cycle
//When a status effect is removed from a player if it has a despairSEEffectLoop function where Effect is the status effect's name
//it will be removed from that player's status effect loop list

function addStatusEffect(%name,%displaySlot,%notPrecident,%upgradeable,%default,%maxUpgrades)
{
	$Despair::SE[%name] = $Despair::SE[C];
	$Despair::SE[$Despair::SE[C],N] = %name;
	$Despair::SE[$Despair::SE[C],S] = %displaySlot;
	$Despair::SE[$Despair::SE[C],P] = %notPrecident;

	if (%upgradeable)
	{
		$Despair::SE[$Despair::SE[C],U] = %upgradeable;
		$Despair::SE[$Despair::SE[C],D]	= %default;
		$Despair::SE[$Despair::SE[C],M]	= %maxUpgrades;
	}
	
	$Despair::SE[C]++;
}

function Player::clearStatusEffects(%player) 
{
	for (%i = 0; %i < $Despair::SE[C]; %i++) 
	{
		%player.removeStatusEffect($Despair::SE[%i,N]);
	}
}

function Player::addStatusEffect(%player, %effect, %timeOut) 
{
	if ($Despair::SE[%effect] $= "")
	{
		echo("Player::addStatusEffect" SPC "invalid effect added" SPC %effect);
		return "";
	}
	if (%player.se[%effect] == false) 
	{
		%index = $Despair::SE[%effect];
		%slot = $Despair::SE[%index,S];
		if (%slot > -1)
		{
			if (%player.se[S,%slot] !$= "")
			{
				%holder = %player.se[S,%slot];
				%notPrecident = $Despair::SE[%index,P];
				if (getField(%notPrecident,%holder) > -1)
				{
					return "";
				}
				else
				{
					//remove previous and put this effect in it's slot
					%player.removeStatusEffect(%holder);
				}
			}
			//add the effect to the slot
			%player.se[S,%slot] = %effect;
		}

		%player.se[%effect] = true;
		

		if ($Despair::SE[%index,U]) 
		{
			%default = $Despair::SE[%index,D];
			if (%default $= "") 
			{
				%default = 0;
			}

			%player.se[%effect] = %default;
		}

		if (isFunction("DespairSE" @ %effect)) 
		{
			call("DespairSE" @ %effect, %player);	
		}
		//add to the loop list if it exists
		if (isFunction("DespairSE" @ %effect @ "Loop"))
		{
			
			%count = %player.se[L];
			//find the first availible space
			for(%i = 0; %i < %count; %i++)
			{
				if (%player.se[L,%i] $= "")
				{
					break;
				}
			}
			//add index = name
			//and name = index
			//for easy purposes for removing later
			%player.se[L,%i] = %effect;
			%player.se[L,%effect] = %i;
			//if the current index equals the count we need to add 1 because the last index should equal count - 1
			if (%i == %count)
			{
				%player.se[L]++;
			}

			if (!isEventPending(%player.se[L,S]))
			{
				%player.se[L,S] = %player.schedule($Despair::StatusEffectLoop,"statusEffectLoop",getSimTime());
			}
		}

		if (%timeOut !$= "" && %timeOut > -1)
		{
			%player.se[%effect,T] = %player.schedule(%timeOut, "removeStatusEffect", %effect);
		}
	}
}

function Player::removeStatusEffect(%player, %effect) 
{
	if ($Despair::SE[%effect] $= "")
	{
		echo("Player::addStatusEffect" SPC "invalid effect remove" SPC %effect);
		return "";
	}
	if (%player.se[%effect] > 0) 
	{
		if (%player.se[%effect,T] !$= "")
		{
			cancel(%player.se[%effect,T]);
			%player.se[%effect,T] = "";
		}

		%player.se[S,$Despair::SE[$Despair::SE[%effect],S]] = "";
		%player.se[%effect] = false;

		//remove from the loop list if it exists
		if (isFunction("DespairSE" @ %effect @ "Loop"))
		{
			%index = %player.se[L,%effect];
			%player.se[L,%index] = "";
			%player.se[L,%effect] = "";

			%count = %player.se[L];

			if (%index == (%count - 1))
			{
				//we need to update the list length as the last value was removed

				//find last spot with a value in it
				for (%i = %index - 1; %i >= 0; %i--)
				{
					if (%player.se[L,%i] !$= "")
					{
						break;
					}
				}

				%player.se[L] = %i + 1;
			}
		}

		if (isFunction("DespairSE" @ %effect)) 
		{
			call("DespairSE" @ %effect, %player);
		}
	}
}

function Player::setStatusEffect(%player, %effect, %state, %timeOut,%a1,%a2,%a3)
{
	if (%player.se[%effect] > 0) 
	{
		if (%state $= "")
		{
			%state = $Despair::SE[$Despair::SE[%effect],D];
		}

		%max = $Despair::SE[$Despair::SE[%effect],M];
		if(%max $= "")
		{
			%max = 1;
		}

	 	if (%state < 1)
		{
			%state = 1;
		}
		else if (%state > %max)
		{
			%state = %max;
		}

		%player.se[%effect] = %state;

		if (isFunction("DespairSE" @ %effect)) 
		{
			call("DespairSE" @  %effect, %player,%a1,%a2,%a3);
		}

		if (%timeOut !$= "" && %timeOut > -1)
		{
			cancel(%player.se[%effect,T]);
			%player.se[%effect,T] = %player.schedule(%timeOut, "removeStatusEffect", %effect);
		}
	}
}

function Player::upgradeStatusEffect(%player, %effect, %ammount, %timeOut,%a1,%a2,%a3)
{
	//call status effect upgrade modifier
	if (isFunction("DespairSE" @ %effect @ "UpgradeModifier"))
	{
		%ammount = call("DespairSE" @ %effect @ "UpgradeModifier",%player, %ammount,%a1,%a2,%a3);
	}

	%player.setStatusEffect(%effect, %player.se[%effect] + %ammount, %timeOut);
}

function Player::statusEffectLoop(%player,%lastTime)
{
	cancel(%player.se[L,S]);

	%time = getSimTime();
	%delta = %time - %lastTime;
	%count =  %player.se[L];

	for (%i = 0; %i <  %count; %i++)
	{
		%effect = %player.se[L,%i];
		if(%effect $= "")
		{
			continue;
		}

		if (isFunction("DespairSE" @ %effect @ "Loop"))
		{
			call("DespairSE" @ %effect @ "Loop",%player,%delta);
		}
	}
	talk(%delta SPC %count);
	if (%count > 0)
	{
		%player.se[L,S] = %player.schedule($Despair::StatusEffectLoop,"statusEffectLoop",%time);
	}
}

function DespairSEMood (%player)
{
	%state = %player.se["Mood"];
    
	//TODO:
	//Add Mood effects
	//Add vignette
	//Support disabling mood
	if (%player.character.trait["Optimistic"] && %state < 8)
		%moodMin = -7; //Can only be sad
	if (%player.character.trait["Melancholic"] && %state > 16)
		%moodMax = 1; //can't be happy, rip
	
	%player.client.vignette = $EnvGuiServer::VignetteMultiply SPC $EnvGuiServer::VignetteColor;

	%text = "\c0";
	if (%state == 0)
	{
		talk("mood removed");
	}
	else if (%state >= 22)
	{
		%text = "overjoyed";
	}
    else if (%state >= 17)
	{
		%text = "happy";
	}
    else if (%state >= 13)
	{
		%text = "neutral";
	}
    else if (%state >= 8)
	{
		%text = "sad";
		%player.client.vignette = "0 0 0 0.7";
	}
    else
	{
		%text = "depressed";
		%palyer.client.vignette = "0 0 0 1";
	}

	commandToClient(%obj.client, 'SetVignette', true, %obj.client.vignette);
}

function DespairSEMoodUpgradeModifier (%player,%ammount,%text,%textcheck)
{
	if (%player.character.trait["Mood Swings"])
		 %ammmount *= 2; //Eat burger to feel overjoyed.

	if(!(%textcheck && %player.lastMoodText $= %text))
	{
		%player.lastMoodChange = $Sim::Time;
		if(isObject(%obj.client) && %message !$= "")
		{
			%player.client.chatMessage((%ammount >= 0 ? "  \c2++" : "  \c0--") @ "MOOD\c5: " @ %text);
			%player.lastMoodText = %text;
		}
	}

	return %ammount;
}

function DespairSEMoodLoop (%player,%delta)
{
	%modifier = %delta / $Despair::Mood::Tick;
	%state = %player.se["Mood"];

	if(%state < 14.9)
	{
		%player.upgradeStatusEffect("mood", 1 * %modifier);
	}
	else if (%state > 15.1)
	{
		%player.upgradeStatusEffect("mood", -1 * %modifier);
	}
	//TODO: REWORK THIS INTO IT'S OWN STATUS EFFECT
	if (!%player.character.trait["Investigative"])
	{
		%ges = %player.getEnvironmentStress();
		%stress = getWord(%ges, 0);

		if (%stress == 1 && %player.character.trait["Squeamish"] ||  %stress > 1)
			%player.addStatusEffect("Unsettled", $Despair::Mood::Tick);
		
	}
}

function DespairSETiredness (%player)
{
	%state = %player.se["Tiredness"];

	switch(%state)
	{
		case 0:
			%player.removeSpeedScale("Tiredness");
			%player.removeSwingMod("Tiredness");
		case 1:
			%player.removeSpeedScale("Tiredness");
			%player.removeSwingMod("Tiredness");
			if(isObject(%player.client))
				%player.client.chatMessage("\c5You are getting sleepy... Find a \c3bed\c5 and type \c3/sleep\c5!");
		case 2:
			%player.addSpeedScale("Tiredness",-0.1);
			%player.addSwingMod("Tiredness",0.1);
			%player.upgradeStatusEffect("Mood",-2, "You feel tired...", 1);
		case 3:
			%player.addSpeedScale("Tiredness",-0.4);
			%player.addSwingMod("Tiredness",0.6);
			
			%player.upgradeStatusEffect("Mood",-4, "You feel exhausted...", 1);
	}
}

function DespairSEUnsettled (%player)
{
	%state = %player.se["Unsettled"];

	if(!%state) //check unsettled level on remove and readd if unsettled
	{
		%ges = %player.getEnvironmentStress();
		%stress = getWord(%ges, 0);

		if (%stress == 1 && %player.character.trait["Squeamish"])
			%text = "You are nervous of your surroundings.";
		else if (%stress == 2)
			%text = "You are unnerved by your surroundings.";
		else if (%stress == 3)
			%text = "You are horrified of your surroundings!";

		if (%text !$= "" && $Sim::Time - %player.lastMoodChange > 30 && %player.lastMoodText !$= %text)
			%player.upgradeStatusEffect("Mood",-%stress * (%player.character.trait["Squeamish"] ? 3 : 2), %text, true);

		if (%test !$= "")
		{
			%player.addStatusEffect("Unsettled", $Despair::Mood::Tick);
		}
	}
}

// $SE_sleepSlot = 0; //reserved sleepo
// $SE_passiveSlot = 1;
// $SE_damageSlot = 2;
// $SE_damageSlot1 = 3;
// $SE_maxStatusEffects = 6;


// function Player::removeStatusEffect(%player, %slot, %effect)
// {
// 	if(%effect !$= "" && %player.statusEffect[%slot] !$= %effect) //it changed
// 		return;
// 	%player.setStatusEffect(%slot, "");
// 	if(isObject(%player.client))
// 		%player.client.updateBottomPrint();
// }

// function Player::setStatusEffect(%player, %slot, %effect, %nomsg)
// {
// 	//onAdd, essentially
// 	switch$ (%effect)
// 	{
// 		//Sleep-related
// 		case "sleepy":
// 			if(%slot != $SE_sleepSlot)
// 				return %slot;
// 			%player.updateSpeedScale(1);
// 			%player.swingSpeedMod = 1;
// 			if(!%nomsg && isObject(%player.client))
// 				%player.client.chatMessage("\c5You are getting sleepy... Find a \c3bed\c5 and type \c3/sleep\c5!");
// 		case "tired":
// 			if(%slot != $SE_sleepSlot)
// 				return %slot;
// 			%player.updateSpeedScale(0.9);
// 			%player.upgradeStatusEffect("Mood",-2, "You feel tired...", 1);
// 			%player.swingSpeedMod = 1.1;
// 		case "exhausted":
// 			if(%slot != $SE_sleepSlot)
// 				return %slot;
// 			%player.updateSpeedScale(0.6);
// 			%player.upgradeStatusEffect("Mood",-4, "You feel exhausted...", 1);
// 			%player.swingSpeedMod = 1.6;
// 		case "sleeping":
// 			if(%slot != $SE_sleepSlot)
// 				return %slot;
// 			%player.updateSpeedScale(1);
// 			%player.swingSpeedMod = 1;
// 		//passive buffs/debuffs
// 		case "drowsy":
// 			if(%slot != $SE_passiveSlot)
// 				return %slot;
// 			%player.updateSpeedScale(0.8);
// 			%player.swingSpeedMod = 1.5;
// 			%player.upgradeStatusEffect("Mood",-3, "You feel drowsy...", 1);
// 			cancel(%player.statusSchedule[%slot]);
// 			%player.statusSchedule[%slot] = %player.schedule(30000, removeStatusEffect, %slot, %effect);
// 		case "sore back":
// 			if(%slot != $SE_passiveSlot)
// 				return %slot;
// 			%player.updateSpeedScale(0.8);
// 			%player.swingSpeedMod = 1.2;
// 			%player.upgradeStatusEffect("Mood",-3, "Your back is sore...", 1);
// 			cancel(%player.statusSchedule[%slot]);
// 			%player.statusSchedule[%slot] = %player.schedule(75000, removeStatusEffect, %slot, %effect);
// 		case "fresh":
// 			if(%slot != $SE_passiveSlot)
// 				return %slot;
// 			if(%player.statusEffect[%slot] $= "sore back" || %player.statusEffect[%slot] $= "shining")
// 				return %slot;
// 			%player.updateSpeedScale(1);
// 			%player.swingSpeedMod = 1;
// 			if(%player.statusEffect[%slot] !$= "fresh" && %player.lastMoodText !$= "You had a good shower!")
// 				%player.upgradeStatusEffect("Mood",2, "You had a good shower!", 1);
// 			cancel(%player.statusSchedule[%slot]);
// 			%player.statusSchedule[%slot] = %player.schedule(20000, removeStatusEffect, %slot, %effect);
// 		case "shining":
// 			if(%slot != $SE_passiveSlot)
// 				return %slot;
// 			%player.updateSpeedScale(1);
// 			%player.swingSpeedMod = 0.9;
// 			%player.upgradeStatusEffect("Mood",7, (%nomsg? "": "You had a \c3good night's sleep\c5!"), 1);
// 			cancel(%player.statusSchedule[%slot]);
// 			%player.statusSchedule[%slot] = %player.schedule(60000, removeStatusEffect, %slot, %effect);

// 		//damage-related
// 		case "bleeding":
// 			if(%slot != $SE_damageSlot)
// 				return %slot;
// 			%player.bleedTicks = 6;
// 			cancel(%player.statusSchedule[%slot]);
// 			%player.statusSchedule[%slot] = %player.schedule(2000, updateStatusEffect, %slot);
// 			if(!%nomsg && isObject(%player.client) && %player.statusEffect[%slot] !$= "bleeding")
// 				%player.upgradeStatusEffect("Mood",-3, "You are " @ getStatusEffectColor(%effect) @ "bleeding\c5!", 1);
// 		case "shock":
// 			if(%slot != $SE_damageSlot)
// 				return %slot;
// 			if(%player.statusEffect[%slot] $= "shock")
// 				return %slot; //Don't stack it

// 			%player.updateSpeedScale(0.6);
// 			%player.swingSpeedMod = 1.6;
// 			%player.lastFireTime = $Sim::Time; //Makes you unable to immediately swing
// 			%player.upgradeStatusEffect("Mood",-3);
// 			cancel(%player.statusSchedule[%slot]);
// 			%player.statusSchedule[%slot] = %player.schedule(3000, removeStatusEffect, %slot, %effect);
// 			if(!%nomsg && isObject(%player.client))
// 				%player.client.chatMessage("\c5You are suffering " @ getStatusEffectColor(%effect) @ "shock\c5!", 1);

// 		//damage modelling
// 		case "wounded arm":
// 			%player.swingSpeedMod = 1.4;
// 			if(getRandom() < 0.25)
// 				%player.dropTool(%player.currTool, 5);
// 			cancel(%player.statusSchedule[%slot]);
// 			%player.statusSchedule[%slot] = %player.schedule(3000, removeStatusEffect, %slot, %effect);
// 		case "wounded leg":
// 			%player.updateSpeedScale(0.7);
// 			cancel(%player.statusSchedule[%slot]);
// 			%player.statusSchedule[%slot] = %player.schedule(3000, removeStatusEffect, %slot, %effect);
// 		case "concussion":
// 			%player.setWhiteOut(0.4); //BOOM!! FREAK THE FUCK OUT!!
// 			%player.swingSpeedMod = 1.2;
// 			%player.updateSpeedScale(0.8);
// 			cancel(%player.statusSchedule[%slot]);
// 			%player.statusSchedule[%slot] = %player.schedule(3000, removeStatusEffect, %slot, %effect);
// 		case "abdominal trauma": //Todo: puking blood
// 			%player.swingSpeedMod = 1.2;
// 			%player.updateSpeedScale(0.8);
// 			cancel(%player.statusSchedule[%slot]);
// 			%player.statusSchedule[%slot] = %player.schedule(3000, removeStatusEffect, %slot, %effect);

// 		default:
// 			if($SE_sleepSlot !$= %slot && %player.statusEffect[%slot] !$= "")
// 				%player.setStatusEffect($SE_sleepSlot, %player.statusEffect[%slot], 1); //Sleep slot is priority when it comes to slowdowns
// 			else
// 			{
// 				%player.updateSpeedScale(1);
// 				%player.swingSpeedMod = 1;
// 			}
// 	}
// 	%player.statusEffect[%slot] = %effect;
// 	return %slot;
// }

function Player::updateStatusEffect(%player, %slot)
{
	%effect = %player.statusEffect[%slot];
	switch$ (%effect)
	{
		//Sleep-related
		case "sleepy":
			%player.setStatusEffect(%slot, "tired");
		case "tired":
			%player.setStatusEffect(%slot, "exhausted");
		case "exhausted":
			if(isObject(%player.client))
				%player.client.chatMessage("\c5You pass out...");
			%player.KnockOut(90);
		case "sleeping":
			//%player.sleep() handles this

		//passive buffs/debuffs
		case "drowsy":
			//onAdd, set player speeds
		case "sore back":
			//
		case "fresh":
			//
		case "shining":
			//

		//damage-related
		case "bleeding":
			cancel(%player.statusSchedule[%slot]);
			%player.bleedTicks--;
			if(%player.bleedTicks <= 0 || %player.health <= 0) //ticks ran out or we're crit
			{
				%player.removeStatusEffect(%slot, %effect);
				return 0;
			}
			%pos = %player.getPosition();
			%decal = spawnDecalFromRayCast(NewBloodDecal, containerRayCast(%pos, VectorSub(%pos, "0 0 1"), $SprayBloodMask));
			%decal.color = 0.75 + 0.1 * getRandom() @ " 0 0 1";
			%decal.setNodeColor("ALL", %decal.color);
			%decal.hideNode("ALL");
			%decal.unHideNode(blood @ getRandom(3, 4));
			%decal.spillTime = $Sim::Time;
			%decal.freshness = 0;
			%decal.isBlood = true;
			%decal.source = %player;
			%size = 1 + 0.5 * getRandom();
			%decal.setScale(%size SPC %size SPC %size);
			if(getRandom() < 0.45)
				serverPlay3d(BloodSplat @ getRandom(1,3), getWords(%ray, 1, 3));
			if(isObject(%player.client) && %player.client.killer && %player.health - 2 <= 0) //Killers can't get downed from bleeding
			{
				%player.bleedTicks = 0;
				%player.removeStatusEffect(%slot, %effect);
				return 0;
			}
			else
				%player.damage(%player.attackSource[%player.attackCount], %player.getPosition(), 2, "bleed");
			%player.setDamageFlash(0.2);
			%player.statusSchedule[%slot] = %player.schedule(1000, updateStatusEffect, %slot);
		case "shock":
			//beep

		//damage modelling
		case "wounded arm":
			//
		case "wounded leg":
			//
		case "concussion":
			//
		case "abdominal trauma":
			//
	}
}

function getStatusEffectColor(%effect)
{
	%color = "\c0";
	switch$ (%effect)
	{
		//Sleep-related
		case "sleepy":
			%color = "<color:4169E8>";
		case "tired":
			%color = "<color:4C47FF>";
		case "exhausted":
			%color = "<color:7241E8>";
		case "sleeping":
			%color = "<color:B14CFF>";

		//passive
		case "drowsy":
			%color = "<color:4CA6FF>";
		case "sore back":
			%color = "<color:41817F>";
		case "fresh":
			%color = "<color:8EFFFE>";
		case "shining":
			%color = "<color:FFF39D>";

		//damage-related
		case "bleeding":
			%color = "<color:e83d3a>";
		case "shock":
			%color = "<color:ff75fa>";
		case "wounded arm":
			%color = "<color:AA3939>";
		case "wounded leg":
			%color = "<color:AA3939>";
		case "concussion":
			%color = "<color:801515>";
		case "abdominal trauma":
			%color = "<color:801515>";
	}
	return %color;
}
