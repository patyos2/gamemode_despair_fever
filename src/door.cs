function fxDtsBrick::doorDamage(%brick, %damage)
{
	if (!%brick.getDatablock().isDoor || %brick.impervious)
		return;

	%brick.doorHits += %damage;
	if (%brick.doorHits >= %brick.doorMaxHits)
	{
		%brick.doorOpen(%brick.isCCW, %obj.client);
		%brick.lockState = false;
		%brick.broken = true;
	}
}

package DespairDoors
{
	function FxDTSBrick::door(%this, %state, %client)
	{
		if (%this.broken)
			%client.centerPrint("\c2The door lock is broken...", 2);
		else if (%this.lockId !$= "" && %this.lockState)
		{
			%client.centerPrint("\c2The door is locked.", 2);
			serverPlay3d(DoorJiggleSound, %this.getWorldBoxCenter(), 1);
		}
		else
			Parent::door(%this, %state, %client);
	}
};

activatePackage("DespairDoors");