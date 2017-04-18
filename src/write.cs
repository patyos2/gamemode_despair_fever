function serverCmdWrite(%client, %a1, %a2, %a3, %a4, %a5, %a6, %a7, %a8, %a9, %a10, %a11, %a12, %a13, %a14, %a15, %a16, %a17, %a18, %a19, %a20, %a20, %a22, %a23, %a24)
{
	if(!isObject(%player = %client.player))
		return;

	%text = %a1;
	for (%i=2; %i<=24; %i++)
		%text = %text SPC %a[%i];
	%text = trim(stripMLControlChars(%text));
	if (%text $= "")
		return;

	if (%player.tool[%player.currTool] == nameToID(PaperItem))
	{
		//wow
		return;
	}

	//if %player is in critical health
	//	...do stuff
	//	return;

	%a = %obj.getEyePoint();
	%b = vectorAdd(%a, vectorScale(%obj.getEyeVector(), 6));

	%mask =
		$TypeMasks::FxBrickObjectType |
		$TypeMasks::ItemObjectType;

	%ray = containerRayCast(%a, %b, %mask, %obj);

	if(isObject(%ray))
	{
		if (%ray.getType() & $TypeMasks::ItemObjectType)
		{
			if (%ray.getDataBlock() == nameToID(PaperItem))
			{
				//wew
			}
		}
		else
		{
			//GRAFFITI
		}
	}
}