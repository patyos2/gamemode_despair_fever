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