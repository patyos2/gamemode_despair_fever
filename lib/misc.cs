function rotateVector(%vector, %yawDelta, %pitchDelta)
{
	%length = VectorLen(%vector);
	%vector = VectorNormalize(%vector);
	%x = getWord(%vector, 0);
	%y = getWord(%vector, 1);
	%z = getWord(%vector, 2);
	%yaw = mATan(%y, %x) + %yawDelta;
	%pitch = mATan(%z, mSqrt(%x * %x + %y * %y)) + %pitchDelta;
	%x = mCos(%pitch) * mCos(%yaw);
	%y = mCos(%pitch) * mSin(%yaw);
	%z = mSin(%pitch);
	return VectorScale(%x SPC %y SPC %z, %length);
}

function getRandomScalar(%magnitude)
{
	return (getRandom() * 2 - 1) * %magnitude;
}

function Player::getAimVector(%player)
{
	%fwd = %player.getForwardVector();
	%eye = %player.getEyeVector();
	
	%scale = vectorLen(setWord(%eye, 2, 0));

	return
		getWord(%fwd, 0) * %scale SPC
		getWord(%fwd, 1) * %scale SPC
		getWord(%eye, 2);
}

function commafy(%s)
{
    %i = %L = strlen(%s);
    while (%i-- >= 0)
        %o = (%i == 0 ? "" : ((%L - %i) % 3 ? "" : ",")) @ getSubStr(%s, %i, 1) @ %o;
    return %o;
}

datablock StaticShapeData(PrimitiveLineC)
{
	shapeFile = $Despair::Path @ "res/shapes/line.dts";
};

if (!isObject(LinePool))
	new SimSet(LinePool);

if (!isObject(PrimitiveSet))
	new SimSet(PrimitiveSet);

function freeLine(%ref)
{
	if (isObject(%ref))
	{
		%ref.setScale("0 0 0");
		%ref.setTransform("0 0 -100");
		LinePool.add(%ref);
	}
	return 0;
}

function drawLine(%ref, %a, %b, %color)
{
	if (!isObject(%ref))
	{
		if (%count = LinePool.getCount())
		{
			%ref = LinePool.getObject(%count - 1);
			LinePool.remove(%ref);
		}
		else
		{
			%ref = new StaticShape()
			{
				datablock = PrimitiveLineC;
			};
			
			PrimitiveSet.add(%ref);
		}
	}
	
	if (%ref.color !$= %color)
		%ref.setNodeColor("ALL", %ref.color = %color);
	
	%vector = VectorNormalize(VectorSub(%b, %a));
	%axis = VectorNormalize(VectorCross("0 1 0", %vector));
	%mag = mACos(VectorDot("0 1 0", %vector)) * -1;

	%ref.setTransform(VectorScale(VectorAdd(%a, %b), 0.5) SPC %axis SPC %mag);
	%ref.setScale("0.5 " @ VectorDist(%a, %b) @ " 0.5");

	return %ref;
}

function muffleText(%text, %prob)
{
	if (%text $= "")
		return;
	if (%prob $= "")
		%prob = 0.2;
	if (%prob <= 0)
		return %text;
	%result = %text;
	for (%i=0;%i<strlen(%text);%i++)
	{
		if (getSubStr(%text, %i, %i+1) $= " ") //space character
			continue;
		if (getRandom() < %prob)
			%result = getSubStr(%result, 0, %i) @ "#" @ getSubStr(%result, %i+1, strlen(%result));
	}
	return %result;
}

function naturalGrammarList(%list)
{
	%fields = getFieldCount(%list);

	if (%fields < 2) {
			return %list;
	}

	for (%i = 0; %i < %fields - 1; %i++) {
			%partial = %partial @ (%i ? ", " : "") @ getField(%list, %i);
	}

	return %partial SPC "and" SPC getField(%list, %fields - 1);
}