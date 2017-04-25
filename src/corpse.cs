datablock AudioProfile(BodyPickUpSound1)
{
	fileName = $Despair::Path @ "res/sounds/gore/bodypickup1.wav";
	description = audioClosest3D;
	preload = true;
};
datablock AudioProfile(BodyPickUpSound2)
{
	fileName = $Despair::Path @ "res/sounds/gore/bodypickup2.wav";
	description = audioClosest3D;
	preload = true;
};
datablock AudioProfile(BodyPickUpSound3)
{
	fileName = $Despair::Path @ "res/sounds/gore/bodypickup3.wav";
	description = audioClosest3D;
	preload = true;
};

datablock AudioProfile(BodyDropSound1)
{
	fileName = $Despair::Path @ "res/sounds/gore/bodyDrop1.wav";
	description = audioClosest3D;
	preload = true;
};
datablock AudioProfile(BodyDropSound2)
{
	fileName = $Despair::Path @ "res/sounds/gore/bodyDrop2.wav";
	description = audioClosest3D;
	preload = true;
};
datablock AudioProfile(BodyDropSound3)
{
	fileName = $Despair::Path @ "res/sounds/gore/bodyDrop3.wav";
	description = audioClosest3D;
	preload = true;
};


function Player::findCorpseRayCast(%obj)
{
	%a = %obj.getEyePoint();
	%b = vectorAdd(vectorScale(%obj.getEyeVector(), 5), %a);
	%ray = containerRayCast(%a, %b, $TypeMasks::fxBrickObjectType | $TypeMasks::playerObjectType, %obj);
	if(%ray)
		%b = getWords(%ray, 1, 3);
	%center = vectorScale(vectorAdd(%a, %b), 0.5); //Get middle of raycast
	%length = vectorDist(%a, %b) / 2;

	if(%ray && %ray.getType() & $TypeMasks::playerObjectType)
	{
		if(%ray.isBody)
			return %ray;
	}

	%maxdist = 1; //how fatass our fat raycast is
	initContainerRadiusSearch(%center, %length + %maxdist, $TypeMasks::CorpseObjectType); //Scale radius search so it searches the entirety of raycast
	while (isObject(%col = containerSearchNext()))
	{
		if (!%col.isDead)
			continue;
		%p = %col.getHackPosition();
		%ab = vectorSub(%b, %a);
		%ap = vectorSub(%p, %a);

		%project = vectorDot(%ap, %ab) / vectorDot(%ab, %ab); //Projection, aka "check against closest point on raycast" or something.

		if (%project < 0 || %project > 1)
			continue;

		%j = vectorAdd(%a, vectorScale(%ab, %project));
		%distance = vectorDist(%p, %j);
		if (%distance <= %maxdist) //Give 'em the corpse!
		{
			return %col;
		}
	}
	return 0;
}

function Player::carryTick(%this)
{
	cancel(%this.carrySchedule);
	%player = %this.carryPlayer;
	if (!%this.isBody)
	{
		%this.lastTosser = %player;
		%this.carryEnd = $Sim::Time;
		%this.carryPlayer = 0;
		%player.carryObject = 0;
		%player.playThread(2, "root");
		ServerPlay3D("BodyDropSound" @ getRandom(1, 3), %this.getPosition());
		return;
	}

	if(%this.isDead && getRandom() < 0.45 && %this.pools < 1000) //blood
	{
		updateCorpseBloodPool(%this.getPosition());
		%this.pools++;
	}

	if (!isObject(%player) || %player.getState() $= "Dead")
	{
		%this.lastTosser = %player;
		%this.carryEnd = $Sim::Time;
		%this.carryPlayer = 0;
		%player.carryObject = 0;
		ServerPlay3D("BodyDropSound" @ getRandom(1, 3), %this.getPosition());
		return;
	}
	if (%player.getMountedImage(0))
	{
		%player = %this.carryPlayer;
		%this.lastTosser = %player;
		%this.carryPlayer = 0;
		%player.carryObject = 0;
		ServerPlay3D("BodyDropSound" @ getRandom(1, 3), %this.getPosition());
		return;
	}
	%eyePoint = %player.getEyePoint();
	%normal = %player.getAimVector();
	%eyeVector = getWords(%normal, 0, 1) SPC (getWord(%normal, 2) * 0.5) - 0.5;

	%center = %this.getPosition();
	%target = vectorAdd(%eyePoint, vectorScale(%eyeVector, 3));

	if (vectorDist(%center, %target) > 4)
	{
		%this.carryPlayer = 0;
		%player.carryObject = 0;
		%player.playThread(2, "root");
		return;
	}

	%vel = vectorScale(vectorSub(%target, %center), 4);

	%this.setVelocity(%vel);

	%rot = getWords(%player.getTransform(), 3, 7);
	if(%rot !$= getWords(%this.getTransform(), 3, 7))
		%this.setTransform(getWords(%this.getTransform(), 0, 2) SPC %rot);
	%this.carrySchedule = %this.schedule(1, "carryTick");
}

package DespairCorpses
{
	function Armor::onTrigger(%this, %obj, %slot, %state)
	{
		if(%slot == 0 && !isObject(%obj.getMountedImage(0)))
		{
			%item = %obj.carryObject;
			if (isObject(%item) && isEventPending(%item.carrySchedule) && %item.carryPlayer $= %obj)
			{
				%time = $Sim::Time - %item.carryStart;
				cancel(%item.carrySchedule);
				%item.lastTosser = %obj;
				%item.carryEnd = $Sim::Time;
				%item.carryPlayer = 0;
				ServerPlay3D("BodyDropSound" @ getRandom(1, 3), %item.getPosition());
				%obj.carryObject = 0;
				%obj.playThread(2, "root");
			}
			if(%state && isObject(%col = %obj.findCorpseRayCast()))
			{
				if ($Sim::Time - %obj.lastBodyClick < 0.3 && ($investigationStart $= "" || !%col.isDead))
				{
					if (isEventPending(%col.carrySchedule) && isObject(%col.carryPlayer))
						%col.carryPlayer.playThread(2, "root");
					%obj.carryObject = %col;
					%col.carryPlayer.carryObject = 0;
					%col.carryPlayer = %obj;
					%col.carryStart = $Sim::Time;
					%col.carryTick();
					ServerPlay3D("BodyPickUpSound" @ getRandom(1, 3), %col.getPosition());
					if (%col.bloody["chest_front"] || %col.bloody["chest_back"])
					{
						%obj.bloody["rhand"] = true;
						%obj.bloody["lhand"] = true;
						%player.bloodyWriting = 2;
					}
					if (isObject(%obj.client))
					{
						%obj.client.applyBodyParts();
						%obj.client.applyBodyColors();
					}
					%obj.playThread(2, "armReadyBoth");
				}
				else
				{
					if(isObject(%obj.client))
						%obj.client.examineObject(%col);
					%obj.lastBodyClick = $sim::time;
				}
				return; //so we don't call anything else
			}
		}
		Parent::onTrigger(%this, %obj, %slot, %state);
	}
};
activatePackage(DespairCorpses);