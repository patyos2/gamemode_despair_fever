datablock TSShapeConstructor(mDespairDts) {
	baseShape = "base/data/shapes/player/m_df.dts";
	sequence0 = "base/data/shapes/player/default.dsq";
	sequence1 = "base/data/shapes/player/player_anim_1h.dsq";
	sequence2 = "base/data/shapes/player/player_anim_misc.dsq";
};

datablock PlayerData(PlayerDespairArmor : PlayerStandardArmor)
{
	shapeFile = "base/data/shapes/player/m_df.dts";
	uiName = "Despair Fever Player";

	cameraMaxDist = 2;
	cameraVerticalOffset = 1.25;
	maxFreelookAngle = 2;

	canJet = 0;
	mass = 120;
	maxTools = 5;

	runForce = 6000;

	maxForwardSpeed = 8;
	maxBackwardSpeed = 5;
	maxSideSpeed = 7;

	maxEnergy = 100;

	jumpForce = 1200;
	jumpDelay = 30;

	jumpEnergyDrain = 50;
	minJumpEnergy = 75;

	showEnergyBar = 1;

	airControl = 0.05;

	jumpSound = "";

	maxTools = 6;
};

function PlayerDespairArmor::onAdd(%data, %player)
{
	parent::onAdd(%data, %player);
	if(%player.health $= "")
	{
		%player.maxhealth = 100;
		%player.health = 100;
	}
	if(%player.swingSpeedMod $= "")
	{
		%player.swingSpeedMod = 1;
	}
}

datablock PlayerData(PlayerCorpseArmor : PlayerStandardArmor)
{
	shapeFile = "base/data/shapes/player/m_df.dts";
	uiName = "Corpse Player";

	boundingBox = "5 5 4";
	crouchBoundingBox = "5 5 4";

	canJet = 0;
	mass = 120;
	maxTools = 5;

	maxForwardSpeed = 0;
	maxBackwardSpeed = 0;
	maxSideSpeed = 0;

	maxForwardCrouchSpeed = 0;
	maxBackwardCrouchSpeed = 0;
	maxSideCrouchSpeed = 0;

	jumpForce = 0;

	maxTools = 6;
};

datablock PlayerData(PlayerFrozenArmor : PlayerStandardArmor)
{
	shapeFile = "base/data/shapes/player/m_df.dts";
	uiName = "Frozen Player";

	crouchBoundingBox = "5 5 10.6"; //no dodging votes fucko

	canJet = 0;
	mass = 120;
	maxTools = 5;

	maxForwardSpeed = 0;
	maxBackwardSpeed = 0;
	maxSideSpeed = 0;

	maxForwardCrouchSpeed = 0;
	maxBackwardCrouchSpeed = 0;
	maxSideCrouchSpeed = 0;

	jumpForce = 0;

	maxTools = 6;
};

function PlayerDespairArmor::doDismount(%this, %obj)
{
	if(%obj.getObjectMount().getDatablock() == nameToID(DespairStand))
		return;
	parent::doDismount(%this, %obj);
}

function PlayerDespairArmor::onUnMount(%this, %obj, %mount, %slot)
{
	Parent::onUnMount(%this, %obj, %mount, %slot);
	if (!isObject(%mount.heldCorpse) || %mount.heldCorpse != %obj)
		return;
	%mount.heldCorpse = "";
	%mount.playThread(2, "root");
}

function PlayerDespairArmor::killerDash(%this, %obj, %end)
{
	if(%end)
	{
		%obj.setSpeedScale(1);
		return;
	}
	%obj.setWhiteOut(0.1);
	%obj.setSpeedScale(1.3);
	%this.schedule(1000, killerDash, %obj, 1);
}

function PlayerDespairArmor::onTrigger(%this, %obj, %slot, %state)
{
	if(%slot == 0 && %state) //pick items up if we don't have any active inventory items selected
	{
		%item = %obj.tool[%obj.currTool];
		if(%item && isFunction(%item.getName(), "onWear"))
		{
			%item.onWear(%obj);
			return;
		}
		else if(!isObject(%obj.getMountedImage(0)))
		{
			%range = 6;
			if($DespairTrial)
				%range = 16;

			%a = %obj.getEyePoint();
			%b = vectorAdd(%a, vectorScale(%obj.getEyeVector(), %range));

			%mask =
				$TypeMasks::FxBrickObjectType |
				$TypeMasks::PlayerObjectType |
				$TypeMasks::CorpseObjectType |
				$TypeMasks::StaticShapeObjectType |
				$TypeMasks::ItemObjectType;

			%ray = containerRayCast(%a, %b, %mask, %obj);
			if(isObject(%ray))
			{
				if(%ray.getClassName() $= "Item")
				{
					if(!%ray.canPickup) //examine
					{
						%obj.client.examineObject(%ray);
						return;
					}
					%data = %ray.getDataBlock();
					if(%data.className $= "DespairWeapon")
					{
						if(%obj.tool[%obj.weaponSlot] == nameToID(noWeaponIcon))
						{
							%obj.setTool(%obj.weaponSlot, %data, %ray.itemProps, 1, 0);
							%ray.itemProps = "";
							%ray.delete();
						}
						return;
					}
					if(%data.className $= "Hat")
					{
						%data.onPickup(%ray, %obj);
						return;
					}
					if(%data.getName() $= "PaperstackItem")
					{
						%data.onPickUp(%ray, %obj);
						return;
					}
					if (%ray.canPickup && (%slot = %obj.addTool(%data, %ray.itemProps, 1, 0)) != -1)
					{
						if(%data.getName() $= "MopItem" || %data.getName() $= "RadioItem")
							%data.onPickup(%ray, %obj, %slot);
						%ray.itemProps = "";
						%ray.delete();
						return;
					}
				}
				else if(%ray.getType() & $TypeMasks::PlayerObjectType && !%ray.unconscious) //corpse.cs handles both carrying and examination of sleeping folks
				{
					if(isObject(%obj.client))
						%obj.client.examineObject(%ray);
					return;
				}
			}

			initContainerRadiusSearch(getWords(%ray, 1, 3), 0.1,
				$TypeMasks::StaticShapeObjectType);

			if (isObject(%col = containerSearchNext()))
			{
				if(isObject(%obj.client))
					%obj.client.examineObject(%col);
			}
		}
	}
	Parent::onTrigger(%this, %obj, %slot, %state);
	if(%obj.client.killer && %slot == 4 && %state)
	{
		if(isObject(%img = %obj.getMountedImage(0)) && %img.item.className !$= "DespairWeapon")
			return;
		if($Sim::Time - %obj.lastKillerDash < 5)
			return;
		%obj.lastKillerDash = $Sim::Time;
		%this.killerDash(%obj);
	}
}

function PlayerFrozenArmor::onTrigger(%this, %obj, %slot, %state)
{
	PlayerDespairArmor::onTrigger(%this, %obj, %slot, %state);
}

function player::applyAppearance(%pl,%cl)
{
	%pl.hideNode("ALL");
	%char = %cl.character;
	%app = %char.appearance;
	%female = %char.gender $= "Female";

	%handColor = getField(%app, 0);
	%headColor = %handColor;
	%faceName = getField(%app, 1);
	%decalName = getField(%app, 2);
	%hairName = getField(%app, 3);
	%shirtColor = getField(%app, 4);
	%pantsColor = getField(%app, 5);
	%shoesColor = getField(%app, 6);
	%hairColor = getField(%app, 7);

	if(isObject(%pl.getMountedImage(1)) && %pl.getMountedImage(1).item.hideAppearance)
		%hideApp = true;
	if(isObject(%hat = %pl.tool[%pl.hatSlot]) && isObject(%pl.getMountedImage(2)) && %pl.getMountedImage(2) == nameToID(%hat.image))
	{
		if(%hat.replaceHair !$= "")
			%hairName = %hat.replaceHair;
		if(%hat.replaceHair[%char.gender] !$= "")
			%hairName = %hat.replaceHair[%char.gender];
		if(%hat.hideHair)
			%hairName = "";
		if(%hat.disguise)
		{
			%faceName = "smiley";
			if(%hideApp)
			{
				%shoesColor = "0.25 0.25 0.25 1";
				%handColor = "0.25 0.25 0.25 1";
			}
		}
		if(%hat.nodeColor !$= "")
			%headColor = %hat.nodeColor SPC "1";
	}

	if(!%hideApp)
	{
		%pl.unHideNode((%female ? "femChest" : "chest"));

		%pl.unHideNode((%female ? "rarmSlim" : "rarm"));
		%pl.unHideNode((%female ? "larmSlim" : "larm"));
	}

	%pl.unHideNode((%rhook ? "rhook" : "rhand"));
	%pl.unHideNode((%lhook ? "lhook" : "lhand"));

	%pl.unHideNode("headskin");

	if (%hairName !$= "")
		%pl.unHideNode(%hairName);

	%pl.setHeadUp(false);
	//if($pack[%cl.pack] !$= "none")
	//{
	//	%pl.unHideNode($pack[%cl.pack]);
	//	%pl.setNodeColor($pack[%cl.pack],%cl.packColor);
	//}
	//if($secondPack[%cl.secondPack] !$= "none")
	//{
	//	%pl.unHideNode($secondPack[%cl.secondPack]);
	//	%pl.setNodeColor($secondPack[%cl.secondPack],%cl.secondPackColor);
	//}
	//if($hat[%cl.hat] !$= "none")
	//{
	//	%pl.unHideNode($hat[%cl.hat]);
	//	%pl.setNodeColor($hat[%cl.hat],%cl.hatColor);
	//}
	
	if(%hip && !%hideApp)
	{
		%pl.unHideNode("skirthip");
		%pl.unHideNode("skirttrimleft");
		%pl.unHideNode("skirttrimright");
	}
	else
	{
		if(!%hideApp)
			%pl.unHideNode("pants");
		%pl.unHideNode((%rpeg ? "rpeg" : "rshoe"));
		%pl.unHideNode((%lpeg ? "lpeg" : "lshoe"));
	}

	if (%pl.bloody["lshoe"])
		%pl.unHideNode("lshoe_blood");
	if (%pl.bloody["rshoe"])
		%pl.unHideNode("rshoe_blood");
	if (%pl.bloody["lhand"])
		%pl.unHideNode("lhand_blood");
	if (%pl.bloody["rhand"])
		%pl.unHideNode("rhand_blood");
	if (%pl.bloody["head"])
		%pl.unHideNode("blood_head");
	if(!%hideApp)
	{
		if (%pl.bloody["chest_front"])
			%pl.unHideNode((%female ? "fem" : "") @ "chest_blood_front");
		if (%pl.bloody["chest_back"])
			%pl.unHideNode((%female ? "fem" : "") @ "chest_blood_back");
	}

	%pl.setFaceName(%faceName);
	%pl.setDecalName(%decalName);
	
	%pl.setNodeColor("headskin", %headColor);
	//hair color
	if(%hairName !$= "")
		%pl.setNodeColor(%hairName, %hairColor);

	%pl.setNodeColor("chest",%shirtColor);
	%pl.setNodeColor("femChest",%shirtColor);
	%pl.setNodeColor("pants",%pantsColor);
	%pl.setNodeColor("skirthip",%pantsColor);
	
	%pl.setNodeColor("rarm",%shirtColor);
	%pl.setNodeColor("larm",%shirtColor);
	%pl.setNodeColor("rarmSlim",%shirtColor);
	%pl.setNodeColor("larmSlim",%shirtColor);
	
	%pl.setNodeColor("rhand",%handColor);
	%pl.setNodeColor("lhand",%handColor);
	%pl.setNodeColor("rhook","0.35 0.35 0.35 1");
	%pl.setNodeColor("lhook","0.35 0.35 0.35 1");
	
	%pl.setNodeColor("rshoe",%shoesColor);
	%pl.setNodeColor("lshoe",%shoesColor);
	%pl.setNodeColor("rpeg","0.47 0.35 0.2 1");
	%pl.setNodeColor("lpeg","0.47 0.35 0.2 1");
	%pl.setNodeColor("skirttrimright",%pantsColor);
	%pl.setNodeColor("skirttrimleft",%pantsColor);

	//Set blood colors.
	%pl.setNodeColor("lshoe_blood", "0.7 0 0 1");
	%pl.setNodeColor("rshoe_blood", "0.7 0 0 1");
	%pl.setNodeColor("lhand_blood", "0.7 0 0 1");
	%pl.setNodeColor("rhand_blood", "0.7 0 0 1");
	%pl.setNodeColor("blood_head", "0.7 0 0 1");
	%pl.setNodeColor("chest_blood_front", "0.7 0 0 1");
	%pl.setNodeColor("chest_blood_back", "0.7 0 0 1");
	%pl.setNodeColor("femchest_blood_front", "0.7 0 0 1");
	%pl.setNodeColor("femchest_blood_back", "0.7 0 0 1");
}

package DespairPlayerPackage
{
	function Observer::onTrigger(%this, %obj, %slot, %state)
	{
		%client = %obj.getControllingClient();
		if (isObject(%pl = %client.player))
		{
			if (%pl.currResting && %state)
			{
				%pl.currResting = 0;
				%pl.WakeUp();
			}
			if (%pl.unconscious)
				return;
		}
		Parent::onTrigger(%this, %obj, %slot, %state);
	}
	function gameConnection::applyBodyColors(%cl,%o) 
	{
		parent::applyBodyColors(%cl,%o);
		if(isObject(%pl = %cl.player))
		{
			if((%pl.getDatablock()).shapeFile $= "base/data/shapes/player/m_df.dts")
				%pl.applyAppearance(%cl);
		}
	}
	function gameConnection::applyBodyParts(%cl,%o) 
	{
		parent::applyBodyParts(%cl,%o);
		if(isObject(%pl = %cl.player))
		{
			if((%pl.getDatablock()).shapeFile $= "base/data/shapes/player/m_df.dts")
				%pl.applyAppearance(%cl);
		}
	}
	function Armor::onCollision(%this, %obj, %col, %velocity, %speed)
	{
		if (isObject(%col) && %col.getClassName() $= "Item" && %obj.client.miniGame == $defaultMiniGame)
			return;
		Parent::onCollision(%this, %obj, %col, %velocity, %speed);
	}
	function serverCmdDropTool(%client, %index)
	{
		if($DespairTrial)
		{
			DespairTrialDropTool(%client, %index);
			return;
		}

		if(isObject(%client.player))
			%item = %client.player.tool[%index];
		Parent::serverCmdDropTool(%client, %index);
		if(isObject(%item) && isFunction(%item.getName(), "onDrop"))
			%item.onDrop(%client.player, %index);
	}
	function serverCmdSit(%client)
	{
		if($DespairTrial)
			return;
		parent::serverCmdSit(%client);
	}
	function serverCmdUseTool(%client, %num)
	{
		parent::serverCmdUseTool(%client, %num);		
	}
};
activatePackage(DespairPlayerPackage);