function Player::findCorpseRayCast(%obj)
{
	%a = %obj.getEyePoint();
	%b = vectorAdd(vectorScale(%obj.getEyeVector(), 5), %a);
	%ray = containerRayCast(%a, %b, $TypeMasks::All ^ $TypeMasks::fxAlwaysBrickObjectType, %obj);
	if(%ray)
		%b = getWords(%ray, 1, 3);
	%center = vectorScale(vectorAdd(%a, %b), 0.5); //Get middle of raycast
	%length = vectorDist(%a, %b) / 2;

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
	if (!%this.isBody)
	{
		%player = %this.carryPlayer;
		%this.lastTosser = %player;
		%this.carryEnd = $Sim::Time;
		%this.carryPlayer = 0;
		%player.carryObject = 0;
		%player.playThread(2, "root");
		return;
	}
	%player = %this.carryPlayer;

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
		return;
	}
	if (%player.getMountedImage(0))
	{
		%player = %this.carryPlayer;
		%this.lastTosser = %player;
		%this.carryPlayer = 0;
		%player.carryObject = 0;
		return;
	}
	%eyePoint = %player.getEyePoint();
	%eyeVector = %player.getAimVector();

	%center = %this.getPosition();
	%target = vectorAdd(%eyePoint, vectorScale(%eyeVector, 3));
	%target = getWords(%target, 0, 1) SPC vectorScale(getWord(%target, 2), 0.5);

	if (vectorDist(%center, %target) > 4)
	{
		%this.carryPlayer = 0;
		%player.carryObject = 0;
		%player.playThread(2, "root");
		return;
	}

	%vel = vectorScale(vectorSub(%target, %center), 3);

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
		if(%slot == 0)
		{
			%item = %obj.carryObject;
			if (isObject(%item) && isEventPending(%item.carrySchedule) && %item.carryPlayer $= %obj)
			{
				%time = $Sim::Time - %item.carryStart;
				cancel(%item.carrySchedule);
				%item.lastTosser = %obj;
				%item.carryEnd = $Sim::Time;
				%item.carryPlayer = 0;
				%obj.carryObject = 0;
				%obj.playThread(2, "root");
			}
			if(%state && isObject(%col = %obj.findCorpseRayCast()))
			{
				if (isEventPending(%col.carrySchedule) && isObject(%col.carryPlayer))
					%col.carryPlayer.playThread(2, "root");
				%obj.carryObject = %col;
				%col.carryPlayer.carryObject = 0;
				%col.carryPlayer = %obj;
				%col.carryStart = $Sim::Time;
				%col.carryTick();
				if (%col.bloody["chest_front"] || %col.bloody["chest_back"])
				{
					%obj.bloody["rhand"] = true;
					%obj.bloody["lhand"] = true;
				}
				if (isObject(%obj.client))
				{
					%obj.client.applyBodyParts();
					%obj.client.applyBodyColors();
				}
				%obj.playThread(2, "armReadyBoth");
				return; //so we don't call anything else
			}
		}
		Parent::onTrigger(%this, %obj, %slot, %state);
	}
};
activatePackage(DespairCorpses);