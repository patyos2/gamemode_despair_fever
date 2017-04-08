datablock TSShapeConstructor(mDespairDts) {
	baseShape = "base/data/shapes/player/m_df.dts";
	sequence0 = "base/data/shapes/player/default.dsq";
	sequence1 = "base/data/shapes/player/player_anim_1h.dsq";
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
};

function PlayerDespairArmor::killerDash(%this, %obj, %end)
{
	if(%end)
	{
		%obj.setMaxForwardSpeed(%this.maxForwardSpeed);
		%obj.setMaxBackwardSpeed(%this.maxBackwardSpeed);
		%obj.setMaxSideSpeed(%this.maxSideSpeed);
		return;
	}
	%obj.setWhiteOut(0.1);
	%obj.setMaxForwardSpeed(%this.maxForwardSpeed * 1.3);
	%obj.setMaxBackwardSpeed(%this.maxBackwardSpeed * 1.3);
	%obj.setMaxSideSpeed(%this.maxSideSpeed * 1.3);
	%this.schedule(1000, killerDash, %obj, 1);
}

function PlayerDespairArmor::onTrigger(%this, %obj, %slot, %state)
{
	Parent::onTrigger(%this, %obj, %slot, %state);

	if(%obj.client.killer && %slot == 4 && %state)
	{
		if(isObject(%obj.getMountedImage(0)) && !%obj.getMountedImage(0).isWeapon)
			return;
		if($Sim::Time - %obj.lastKillerDash < 5)
			return;
		%obj.lastKillerDash = $Sim::Time;
		%this.killerDash(%obj);
	}
}

function player::applyAppearance(%pl,%cl)
{
	%pl.hideNode("ALL");
	%char = %cl.character;
	%app = %char.appearance;
	%female = %char.gender $= "Female";

	%skinColor = getField(%app, 0);
	%faceName = getField(%app, 1);
	%decalName = getField(%app, 2);
	%hairName = getField(%app, 3);
	%shirtColor = getField(%app, 4);
	%pantsColor = getField(%app, 5);
	%shoesColor = getField(%app, 6);
	%hairColor = getField(%app, 7);

	if(isObject(%pl.tool[%pl.hatSlot]) && %pl.tool[%pl.hatSlot].hideHair)
		%hairName = "";

	%pl.unHideNode((%female ? "femChest" : "chest"));
	
	%pl.unHideNode((%rhook ? "rhook" : "rhand"));
	%pl.unHideNode((%lhook ? "lhook" : "lhand"));
	%pl.unHideNode((%female ? "rarmSlim" : "rarm"));
	%pl.unHideNode((%female ? "larmSlim" : "larm"));
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
	if(%hip)
	{
		%pl.unHideNode("skirthip");
		%pl.unHideNode("skirttrimleft");
		%pl.unHideNode("skirttrimright");
	}
	else
	{
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
	if (%pl.bloody["chest_front"])
		%pl.unHideNode((%female ? "fem" : "") @ "chest_blood_front");
	if (%pl.bloody["chest_back"])
		%pl.unHideNode((%female ? "fem" : "") @ "chest_blood_back");

	%pl.setFaceName(%faceName);
	%pl.setDecalName(%decalName);
	
	%pl.setNodeColor("headskin",%skinColor);
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
	
	%pl.setNodeColor("rhand",%skinColor);
	%pl.setNodeColor("lhand",%skinColor);
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

package _temp_DespairPlayerPackage
{
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
		Parent::onCollision(%this, %obj, %col, %velocity, %speed);
		if (isObject(%col) && %col.getClassName() $= "Item" && %col.getDatablock().onPickUp(%col, %obj) && %obj.client.miniGame == $defaultMiniGame)
			%col.delete();
	}
};
activatePackage(_temp_DespairPlayerPackage);