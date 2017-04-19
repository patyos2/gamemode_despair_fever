function GameConnection::examineObject(%client, %col)
{
	if(!isObject(%player = %client.player))
		return;

	%text = "<font:cambria:32><color:FFFFFF>";

	if(%col.getType() & ($TypeMasks::playerObjectType | $TypeMasks::CorpseObjectType))
	{
		%name = %col.character.name;
		if(isObject(%hat = %col.tool[%col.hatSlot]) && %hat.disguise && isObject(%col.getMountedImage(2)) && %col.getMountedImage(2) == nameToID(%hat.image))
			%name = "Unknown";

		%text = %text @ "This is \c3" @ %name;
		%gender = %col.character.gender;
		if (%col.isDead)
		{
			%text = %text @ "\n\c0" @ (%gender $= "female" ? "She's" : "He's") @ " dead.";
			if(!%client.killer)
			{
				DespairCheckInvestigation(%player, %col);
			}
		}
		else
		{
			if(%col.unconscious)
			{
				%text = %text @ "\n\c5" @ (%gender $= "female" ? "She's" : "He's") @ " sleeping.";
			}
			
			if(%col.bloody)
			{
				%text = %text @ "\n\c0" @ (%gender $= "female" ? "She's" : "He's") @ " bloody.";
			}
		}		
	}

	if(%col.getType() & $TypeMasks::itemObjectType)
	{
		%name = %col.getDataBlock().uiName;
		%props = %col.itemProps;

		if(%props.class $= "PaperProps")
		{
			%name = %props.name;
			%b = %props.contents;
		}
		if(%props.class $= "KeyProps")
		{
			%name = %props.name;
		}
		if(%props.class $= "MeleeProps")
		{
			if(%props.bloody)
				%b = "\n\c0It's bloody.";
		}
		%text = %text @ "This is \c3" @ %name @ "\n\c6" @ %b;
	}

	commandToClient(%client, 'CenterPrint', %text, 3);
}