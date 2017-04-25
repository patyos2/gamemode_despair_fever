function GameConnection::startViewingInventory(%this, %target, %maxtools)
{
	%player = %this.player;

	if (!isObject(%player))
		return;

	if (!isObject(%target))
		return;

	%player.unMountImage(0);
	%player.isViewingInventory = 1;
	%player.inventoryTarget = %target;
	%player.invMaxTools = %maxTools;
	%this.checkDistanceLoop(%target);
	%this.updateInventoryView();
}

function GameConnection::stopViewingInventory(%this)
{
	%player = %this.player;

	if (!isObject(%player))
		return;

	%player.isViewingInventory = 0;
	cancel(%this.inventoryDistanceSchedule);
	%this.updateInventoryView();
}

function GameConnection::updateInventoryView(%this)
{
	%player = %this.player;

	if (!isObject(%player) || !%player.isViewingInventory || !isObject(%target = %player.inventoryTarget))
	{
		if(isObject(%player))
		{
			//commandToClient(%this,'PlayGui_CreateToolHud',%player.getDataBlock().maxTools);
			for(%i=0; %i<%player.invMaxTools; %i++)
			{
				messageClient(%this, 'MsgItemPickup', '', %i, %player.tool[%i], 1);
			}
		}
		return;
	}

	if (vectorDist(%player.getPosition(), getWords(%target.getTransform(), 0, 2)) > 6)
	{
		%this.stopViewingInventory();
		return;
	}

	//commandToClient(%this,'PlayGui_CreateToolHud',%player.invMaxTools);
	for(%i=0; %i<%player.invMaxTools; %i++)
	{
		messageClient(%this, 'MsgItemPickup', '', %i, %target.tool[%i], 1);
	}
	//if(%player.currTool == -1)
	//	commandToClient(%this, 'SetActiveTool', %player.currTool);
}

function GameConnection::checkDistanceLoop(%this, %target)
{
	cancel(%this.checkDistanceLoop);
	if (!isObject(%player = %this.player) || !isObject(%target))
	{
		%this.stopViewingInventory();
		return;
	}
	if (vectorDist(%player.getPosition(), %target.getPosition()) > 6)
	{
		%this.stopViewingInventory();
		return;
	}
	%this.checkDistanceLoop = %this.schedule(500, checkDistanceLoop, %target);
}

package DespairInventory
{
	function serverCmdUseTool(%client, %slot)
	{
		if(isObject(%client.player) && %client.player.isViewingInventory)
		{
			%client.player.currTool = %slot;
			return;
		}
		Parent::serverCmdUseTool(%client, %slot);
	}
	function serverCmdUnUseTool(%client, %slot)
	{
		if(isObject(%client.player) && %client.player.isViewingInventory)
		{
			%client.stopViewingInventory();
		}
		Parent::serverCmdUseTool(%client, %slot);
	}
	function Armor::onTrigger(%this, %obj, %slot, %state)
	{
		if(%obj.isViewingInventory)
		{
			if(%slot == 0 && %state)
			{
				%obj.addTool(%obj.inventoryTarget.tool[%obj.currTool], %obj.inventoryTarget.itemProps[%obj.currTool], 0, 2);
				//Todo: proper itemProps support
				//remove item from corpse
				return;
			}
		}
		Parent::onTrigger(%this, %obj, %slot, %state);
	}
	function serverCmdDropTool(%client, %index)
	{
		//if looking at corpse, give them the thing
		Parent::serverCmdDropTool(%client, %index);
	}
};

activatePackage("DespairInventory");