function GameConnection::startViewingInventory(%this, %target)
{
	%player = %this.player;

	if (!isObject(%player))
		return;

	if (!isObject(%target))
		return;

	%player.isViewingInventory = 1;
	%player.inventoryTarget = %target;
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

	if (!isObject(%player) || !%player.isViewingInventory || !isObject(%player.inventoryTarget))
	{
		if(isObject(%player))
			commandToClient(%this,'PlayGui_CreateToolHud',%player.getDataBlock().maxTools);
		return;
	}

	if (vectorDist(%player.getPosition(), getWords(%player.inventoryTarget.getTransform(), 0, 2)) > 6)
	{
		%this.stopViewingInventory();
		return;
	}
}

function GameConnection::checkDistanceLoop(%this, %target)
{
	cancel(%this.checkDistanceLoop);
	if (!isObject(%player = %this.player) || !isObject(%target))
	{
		%this.stopViewingInventory();
		return;
	}
	if (vectorDist(%player.getPosition(), getWords(%target.getTransform(), 0, 2)) > 6)
	{
		%this.stopViewingInventory();
		return;
	}
	%this.checkDistanceLoop = %this.schedule(500, checkDistanceLoop, %target);
}