//Blood event
registerOutputEvent(Player, setBloody, "list ALL 0 chest 1 left_hand 2 right_hand 3 left_shoe 4 right_shoe 5 head 6" TAB "list ALL 0 front 1 back 2" TAB "bool 0", 1);
function Player::setBloody(%this, %type, %dir, %bool, %client)
{
	switch(%type)
	{
		case 1:
			if (%dir == 1)
				%this.bloody["chest_front"] = %bool;
			else if (%dir == 2)
				%this.bloody["chest_back"] = %bool;
			else
			{
				%this.bloody["chest_front"] = %bool;
				%this.bloody["chest_back"] = %bool;
			}
		case 2:
			%this.bloody["lhand"] = %bool;
		case 3:
			%this.bloody["rhand"] = %bool;
			if(isObject(%this.getMountedImage(0)) && (%props = %this.getItemProps()).bloody)
			{
				%props.bloody = 0;
				%this.updateBloody = 1;
				%this.unMountImage(0); %this.schedule(32, mountImage, %image, 0); //update blood
			}
		case 4:
			%this.bloody["lshoe"] = %bool;
			if(!%bool)
				%this.bloodyFootprints = 0;
		case 5:
			%this.bloody["rshoe"] = %bool;
			if(!%bool)
				%this.bloodyFootprints = 0;
		case 6:
			%this.bloody["head"] = %bool;
		default:
			%this.bloody["lshoe"] = %bool;
			%this.bloody["rshoe"] = %bool;
			%this.bloody["lhand"] = %bool;
			%this.bloody["rhand"] = %bool;
			if(isObject(%this.getMountedImage(0)) && (%props = %this.getItemProps()).bloody)
			{
				%props.bloody = 0;
				%this.updateBloody = 1;
				%this.unMountImage(0); %this.schedule(32, mountImage, %image, 0); //update blood
			}
			%this.bloody["chest_front"] = %bool;
			%this.bloody["chest_back"] = %bool;
			%this.bloody["chest_lside"] = %bool;
			%this.bloody["chest_rside"] = %bool;
			%this.bloody["head"] = %bool;
			if(!%bool)
				%this.bloodyFootprints = 0;
	}
	if (isObject(%client))
		%client.applyBodyParts();
}