//Blood event
registerOutputEvent(Player, setBloody, "list ALL 0 chest 1 hands 2 shoes 3 head 4" TAB "list ALL 0 front 1 back 2" TAB "bool 0", 1);
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
			%this.bloody["rhand"] = %bool;
			if(isObject(%image = %this.getMountedImage(0)) && (%props = %this.getItemProps()).bloody)
			{
				%props.bloody = 0;
				%this.updateBloody = 1;
				%this.unMountImage(0); %this.schedule(32, mountImage, %image, 0); //update blood
			}
		case 3:
			%this.bloody["lshoe"] = %bool;
			%this.bloody["rshoe"] = %bool;
			if(!%bool)
				%this.bloodyFootprints = 0;
		case 4:
			%this.bloody["head"] = %bool;
		default:
			%this.bloody["lshoe"] = %bool;
			%this.bloody["rshoe"] = %bool;
			%this.bloody["lhand"] = %bool;
			%this.bloody["rhand"] = %bool;
			if(isObject(%image = %this.getMountedImage(0)) && (%props = %this.getItemProps()).bloody)
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

//Set camera from brick to direction

registerOutputEvent("fxDTSBrick", "setCameraDir", "list North 0 East 1 South 2 West 3", 1);
function fxDTSBrick::setCameraDir(%brick, %dir, %client)
{
	%camera = %client.camera;
	if(!isObject(%camera))
		return;

	%pos = %brick.getPosition();

	switch (%dir)
	{
		case 0: // North
			%aa = "1 0 0 0";
		case 1: // East
			%aa = "0 0 1 1.57079";
		case 2: // South
			%aa = "0 0 1 3.14159";
		case 3: // West
			%aa = "0 0 1 -1.57079";
	}

	%camera.setTransform(%pos SPC %aa);
	%camera.setFlyMode();
	%camera.mode = "Observer";

	//client controls camera
	%client.setControlObject(%camera);
	%camera.setControlObject(%client.dummyCamera);
	%client.player.inCameraEvent = true;
}

package DespairEvents
{
	function Observer::onTrigger(%this, %obj, %trig, %tog)
	{
		%client = %obj.getControllingClient();

		if(isObject(%client.player) && %client.player.inCameraEvent && %tog)
		{
			%client.player.inCameraEvent = false;
			%client.setControlObject(%client.player);
			return;
		}

		parent::onTrigger(%this, %obj, %trig, %tog);
	}
};
activatePackage("DespairEvents");