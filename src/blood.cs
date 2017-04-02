//Datablocks at the bottom
$SprayBloodMask = $TypeMasks::FxBrickObjectType
				| $TypeMasks::TerrainObjectType;
$SprayBloodGravity = VectorScale("0 0 -5", 0.05);
$SprayBloodDrag = 0.05 * 1;

function sprayBloodTick(%position, %velocity, %i)
{
	%next = VectorAdd(%position, %velocity);
	%ray = containerRayCast(%position, %next, $SprayBloodMask);

	// schedule(6000, 0, freeLine, drawLine("", %position, %next, 1 SPC (%i / 19) @ " 0 1"));

	if (%ray)
	{
		%rayNormal = getWords(%ray, 4, 6);
		%rayPosition = getWords(%ray, 1, 3);
		%rayPosition = VectorAdd(%rayPosition, VectorScale(%rayNormal, 0.01));
		%size = 0.6 + 0.4 * getRandom();
		%color = 0.75 + 0.1 * getRandom() @ " 0 0 1";

		%angle = mATan(
			getWord(%ray, 1) - getWord(%position, 0),
			getWord(%ray, 2) - getWord(%position, 1));

		// spawnDecal(PlaneShape, %rayPosition, %rayNormal, %size, %color);
		// spawnDecal(BloodDecalShape @ getRandom(1, 2), %rayPosition, %rayNormal, %size, %color);
		// play3D(BloodSplatSound @ 1, %rayPosition);

		if (%i >= 5)
			%node = blood @ getRandom(3, 4);
		else
			%node = blood2; //blood @ getRandom(1, 2);

		%decal = spawnDecal(NewBloodDecal, %rayPosition, %rayNormal, %size, %color, %angle, %i < 5 ? "blood" : "", %i >= 5);
		%decal.hideNode("ALL");
		%decal.unHideNode(%node);
		if(getRandom() < 0.45)
		serverPlay3d(BloodSplat @ getRandom(1,3), %rayPosition);
		return;
	}

	%velocity = VectorSub(%velocity, VectorScale(%velocity, $SprayBloodDrag));
	%velocity = VectorAdd(%velocity, $SprayBloodGravity);
	
	if (%i < 40)
		schedule(50, 0, sprayBloodTick, %next, %velocity, %i + 1);
}

function sprayBlood(%position, %velocity)
{
	sprayBloodTick(%position, VectorScale(%velocity, 0.05));
}

$SprayBloodHitAngle = 0.0981748;
$SprayBloodHitAngleWide = 0.785398;
$SprayBloodHitAngleGush = 0.785398;

function sprayBloodFromHit(%position, %velocity)
{
	sprayBlood(%position, rotateVector(%velocity,
		getRandomScalar($SprayBloodHitAngle), getRandomScalar($SprayBloodHitAngle)));
	sprayBlood(%position, rotateVector(%velocity,
		getRandomScalar($SprayBloodHitAngle), getRandomScalar($SprayBloodHitAngle)));
	%velocity = VectorScale(%velocity, -0.1);
	sprayBlood(%position, rotateVector(%velocity,
		getRandomScalar($SprayBloodHitAngle), getRandomScalar($SprayBloodHitAngle)));
	sprayBlood(%position, rotateVector(%velocity,
		getRandomScalar($SprayBloodHitAngle), getRandomScalar($SprayBloodHitAngle)));
}

function sprayBloodWide(%position, %velocity)
{
	sprayBlood(%position, rotateVector(VectorScale(%velocity, 0.5 + getRandom()),
		getRandomScalar($SprayBloodHitAngleWide), getRandomScalar($SprayBloodHitAngleWide)));
	sprayBlood(%position, rotateVector(VectorScale(%velocity, 0.5 + getRandom()),
		getRandomScalar($SprayBloodHitAngleWide), getRandomScalar($SprayBloodHitAngleWide)));
	sprayBlood(%position, rotateVector(VectorScale(%velocity, 0.5 + getRandom()),
		getRandomScalar($SprayBloodHitAngleWide), getRandomScalar($SprayBloodHitAngleWide)));
	sprayBlood(%position, rotateVector(VectorScale(%velocity, 0.5 + getRandom()),
		getRandomScalar($SprayBloodHitAngleWide), getRandomScalar($SprayBloodHitAngleWide)));
	sprayBlood(%position, rotateVector(VectorScale(%velocity, 0.5 + getRandom()),
		getRandomScalar($SprayBloodHitAngleWide), getRandomScalar($SprayBloodHitAngleWide)));
}

function sprayBloodGush(%position, %velocity)
{
	sprayBlood(%position, rotateVector(VectorScale(%velocity, 0.5 + getRandom()),
		getRandomScalar($SprayBloodHitAngleGush), getRandomScalar($SprayBloodHitAngleGush)));
	sprayBlood(%position, rotateVector(VectorScale(%velocity, 0.5 + getRandom()),
		getRandomScalar($SprayBloodHitAngleGush), getRandomScalar($SprayBloodHitAngleGush)));
	sprayBlood(%position, rotateVector(VectorScale(%velocity, 0.5 + getRandom()),
		getRandomScalar($SprayBloodHitAngleGush), getRandomScalar($SprayBloodHitAngleGush)));
	sprayBlood(%position, rotateVector(VectorScale(%velocity, 0.5 + getRandom()),
		getRandomScalar($SprayBloodHitAngleGush), getRandomScalar($SprayBloodHitAngleGush)));
	sprayBlood(%position, rotateVector(VectorScale(%velocity, 0.5 + getRandom()),
		getRandomScalar($SprayBloodHitAngleGush), getRandomScalar($SprayBloodHitAngleGush)));
	%velocity = VectorScale(%velocity, -1);
	sprayBlood(%position, rotateVector(VectorScale(%velocity, 0.5 + getRandom()),
		getRandomScalar($SprayBloodHitAngleGush), getRandomScalar($SprayBloodHitAngleGush)));
	sprayBlood(%position, rotateVector(VectorScale(%velocity, 0.5 + getRandom()),
		getRandomScalar($SprayBloodHitAngleGush), getRandomScalar($SprayBloodHitAngleGush)));
}

//TODO: Redo this function so it allows multiple "corpse drag" pools
function updateCorpseBloodPool(%player, %offset, %slowTime, %size)
{
	if (VectorLen(%player.getVelocity()) < 1)
	{
		if (%player.pool $= "")
		{
			%a = VectorAdd(%player.position, "0 0 0.1");
			%a = VectorAdd(%a, MatrixMulVector("0 0 0 " @ getWords(%player.getTransform(), 3, 6), %offset));
			%b = VectorSub(%a, "0 0 1");
			%ray = containerRayCast(%a, %b, $TypeMasks::FxBrickObjectType);
			// drawLine("", %a, %b, "1 0 1 1");
			if (!%ray)
				return;
			%player.pool = spawnDecalFromRayCast(NewBloodDecal, %ray);
			%player.pool.noUnclutter = true;
			%player.pool.hideNode("ALL");
			%player.pool.unHideNode("blood5");
			%player.pool.setNodeColor("blood5", 0.6 + 0.2 * getRandom() @ " 0 0 1");
		}

		if (!isObject(%player.pool))
			return;
		
		%player.pool.setScale(%size SPC %size SPC %size);
		%add = 0.025;
		if (%size >= 4)
			%add *= 1 - (%size - 4) / %slowTime;
		if (%add < 0.0005)
			return;
		%size += %add;
	}

	schedule(30, %player, updateCorpseBloodPool, %player, %offset, %slowTime, %size);
}

//
//Datablocks
//

//sound
datablock AudioProfile(BloodSplat1)
{
	fileName = $Despair::Path @ "res/sounds/gore/blood_splat1.wav";
	description = audioClosest3D;
	preload = true;
};
datablock AudioProfile(BloodSplat2)
{
	fileName = $Despair::Path @ "res/sounds/gore/blood_splat2.wav";
	description = audioClosest3D;
	preload = true;
};
datablock AudioProfile(BloodSplat3)
{
	fileName = $Despair::Path @ "res/sounds/gore/blood_splat3.wav";
	description = audioClosest3D;
	preload = true;
};

//everything else
datablock staticShapeData(NewBloodDecal)
{
	shapeFile = $Despair::Path @ "res/shapes/newblood.dts";
	decalCombine = "blood";
};

datablock ParticleData(CubeBlood23Particle)
{
	dragCoefficient = 0;
	gravityCoefficient = 0;
	inheritedVelFactor = 0;
	constantAcceleration = 0;
	lifetimeMS = 150;
	lifetimeVarianceMS = 0;
	textureName = "base/data/particles/dot";
	spinSpeed = 0;
	spinRandomMin = 0;
	spinRandomMax = 0;
	colors[0] = "0.8 0 0 1";
	colors[1] = "0.8 0 0 0";
	sizes[0] = 0.1;
	sizes[1] = 0.1;
	useInvAlpha = true;
};

datablock ParticleEmitterData(CubeBlood23Emitter)
{
	ejectionPeriodMS = 1;
	periodVarianceMS = 0;
	ejectionVelocity = 0;
	velocityVariance = 0;
	ejectionOffset = 0;
	thetaMin = -180;
	thetaMax = 180;
	phiReferenceVel = 0;
	phiVariance = 360;
	overrideAdvance = false;
	particles = "CubeBlood23Particle";
};

datablock ParticleData(smallBlood3Particle)
{
	dragCoefficient = 0;
	gravityCoefficient = 0;
	inheritedVelFactor = 0;
	constantAcceleration = 0;
	lifetimeMS         = 250;
	lifetimeVarianceMS = 200;
	textureName = "base/data/particles/dot";
	spinSpeed     = 0;
	spinRandomMin = 0;
	spinRandomMax = 0;
	colors[0] = "0.6 0 0 1";
	colors[1] = "0.5 0 0 0.9 ";
	colors[2] = "0.4 0 0 0";
	sizes[0] = 0.06;
	sizes[1] = 0.09;
	sizes[2] = 0.04;
	times[1] = 0.5;
	times[2] = 1;
	useInvAlpha = true;
};

datablock ParticleEmitterData(smallBlood3Emitter)
{
	ejectionPeriodMS = 1;
	periodVarianceMS = 0;
	ejectionVelocity = 0;
	velocityVariance = 0;
	ejectionOffset   = 0;
	thetaMin = 0;
	thetaMax = 0;
	phiReferenceVel = 0;
	phiVariance     = 0;
	overrideAdvance = false;
	lifetimeMS = 1000;
	particles = "smallBlood3Particle";
};

datablock DebrisData(CubeDebris)
{
	shapeFile = $Despair::Path @ "res/shapes/gorecube.dts";
	lifetime = 10;
	minSpinSpeed = -200;
	maxSpinSpeed = 200;
	useRadiusMass = true;
	baseRadius = 0.1;
	elasticity = 0.5;
	friction = 0.2;
	numBounces = 10; // 3
	staticOnMaxBounce = true;
	fade = false;
	gravModifier = 1;
	emitters[0] = smallBlood3Emitter;
};

datablock explosionData(CubeHighExplosion)
{
	lifetimeMS = 25;
	particleEmitter = CubeBlood23Emitter;
	particleDensity = 25;
	particleRadius = 0.2;
	debris = CubeDebris;
	debrisNum = 8;
	debrisNumVariance = 1;
	debrisPhiMin = 0;
	debrisPhiMax = 360;
	debrisThetaMin = -180;
	debrisThetaMax = 180;
	debrisVelocity = 6;
	debrisVelocityVariance = 2;
};

datablock projectileData(cubeHighExplosionProjectile)
{
	explosion = cubeHighExplosion;
};