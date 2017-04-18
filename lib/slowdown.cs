function Player::setSpeedScale(%obj, %scale)
{
	%db = %obj.getDataBlock();

	%obj.setMaxForwardSpeed(%db.maxForwardSpeed * %scale);
	%obj.setMaxBackwardSpeed(%db.maxBackwardSpeed * %scale);
	%obj.setMaxSideSpeed(%db.maxSideSpeed * %scale);
	
	%obj.setMaxCrouchForwardSpeed(%db.maxForwardCrouchSpeed * %scale);
	%obj.setMaxCrouchBackwardSpeed(%db.maxBackwardCrouchSpeed * %scale);
	%obj.setMaxCrouchSideSpeed(%db.maxSideCrouchSpeed * %scale);
}