function Player::grabCorpse(%obj, %corpse)
{
	%obj.unMountImage(0);
	fixArmReady(%obj);
	%obj.mountObject(%corpse, 0);
	%obj.playThread(2, "ArmReadyBoth");
	%obj.heldCorpse = %corpse;
	%corpse.holder = %obj;
	%corpse.setTransform("0 0 -100 0 0 -1 -1.5709");
}

function Player::throwCorpse(%obj)
{
	if (!isObject(%corpse = %obj.heldCorpse))
		return 0;

	%obj.playThread(2, "shiftUp");
	%a = %obj.getEyePoint();
	%b = vectorAdd(vectorScale(%obj.getEyeVector(), 5), %a);
	%ray = containerRayCast(%a, %b, $TypeMasks::All ^ $TypeMasks::fxAlwaysBrickObjectType, %obj);
	if(%ray)
		%b = getWords(%ray, 1, 3);
	%velocity = vectorScale(%obj.getEyeVector(), vectorDist(%a, %b));
	%velocity = vectorAdd(%velocity, %obj.getVelocity()); //velocity inheritance
	%pos = vectorScale(vectorAdd(%a, %b), 0.5); //Get middle of raycast

	%obj.mountObject(%corpse, 8);
	%corpse.dismount();
	%corpse.setTransform(%pos);
	%corpse.setVelocity(%velocity);
	return 1;
}

function Player::dropCorpse(%obj)
{
	if (!isObject(%corpse = %obj.heldCorpse))
		return 0;

	%obj.playThread(2, "shiftDown");
	%a = %obj.getPosition();
	%b = vectorAdd(vectorScale(%obj.getForwardVector(), 3), %a);
	%ray = containerRayCast(%a, %b, $TypeMasks::All ^ $TypeMasks::fxAlwaysBrickObjectType, %obj);
	if(%ray)
		%b = getWords(%ray, 1, 3);
	%pos = vectorScale(vectorAdd(%a, %b), 0.5); //Get middle of raycast

	%obj.mountObject(%corpse, 8);
	%corpse.dismount();
	%corpse.setTransform(%pos);
	%corpse.setVelocity(%obj.getVelocity());
	return 1;
}

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

package DespairCorpses
{
	function Player::mountImage(%this, %image, %slot, %loaded, %skintag)
	{
		parent::mountImage(%this, %image, %slot, %loaded, %skintag);
		if (%slot == 0 && isObject(%this.heldCorpse))
			%this.throwCorpse();
	}

	function Armor::onTrigger(%this, %obj, %slot, %state)
	{
		if(%slot == 0 && %state)
		{
			if (!%obj.dropCorpse())
			{
				if (isObject(%corpse = %obj.findCorpseRayCast()))
				{
					//DespairCheckInvestigation(%corpse); //Only screaming at corpses should initiate investigation period
				}
			}
		}

		if(%slot == 4 && %state)
		{
			if (!%obj.throwCorpse()) //No corpse to throw, try grabbing one instead
			{
				if (isObject(%corpse = %obj.findCorpseRayCast()))
				{
					//%obj.grabCorpse(%corpse);
				}
			}
		}
		Parent::onTrigger(%this, %obj, %slot, %state);
	}
};
activatePackage(DespairCorpses);