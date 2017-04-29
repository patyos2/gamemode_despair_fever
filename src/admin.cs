function serverCmdWhoIs(%client, %a, %b)
{
	if (!%client.isAdmin)
		return;

	%search = trim(%a SPC %b);
	%charCount = GameCharacters.getCount();
	for (%i = 0; %i < %charCount; %i++)
	{
		%character = GameCharacters.getObject(%i);

		if (%search $= "" || striPos(%character.clientName, %search) != -1 || striPos(%character.name, %search) != -1)
		{
			messageClient(%client, '', "\c3" @ %character.clientName SPC "\c6is \c3" @ %character.name);
		}
	}
}
function serverCmdDamageLogs(%client, %a, %b)
{
	if (!%client.isAdmin)
		return;

	%search = trim(%a SPC %b);
	%charCount = GameCharacters.getCount();
	for (%i = 0; %i < %charCount; %i++)
	{
		%character = GameCharacters.getObject(%i);

		if (striPos(%character.clientName, %search) != -1 || striPos(%character.name, %search) != -1)
		{
			%target = %character.player;
			%name = %character.clientName;
		}
	}
	if (isObject(%target))
	{
		messageClient(%client, '', '\c5Damage logs for client \c3%1\c5:', %name);
		for (%i=1;%i<=%target.attackCount;%i++) //Parse attack logs for info
		{
			%text[%a++] = "\c3["@ (%target.attackTime[%i] - $defaultMiniGame.lastResetTime / 1000) @ " seconds after roundstart], \c6Type\c3:" SPC %target.attackType[%i] @ ", \c6Attacker\c3:" SPC %target.attackCharacter[%i].clientName;
		}
		for (%i=1; %i<=%a; %i++)
			messageClient(%client, '', %text[%i]);
	}
	else
	{
		messageClient(%client, '', '\c5Player not found');
	}
}

function serverCmdSpectate(%this, %tog)
{
	if(!%this.isAdmin)
		return;
	%this.spectating = %tog;
	if(%tog $= "")
		%this.spectating = !%this.spectating;
	messageClient(%this, '', '\c5You are \c6%1 spectating.', %this.spectating ? "now" : "no longer");
	if(isObject(%this.player) && %this.spectating)
	{
		%this.camera.setMode("Corpse", %this.player);
		%this.setControlObject(%this.camera);
		%this.camera.setControlObject(%this.camera);
		%this.player.delete(); //Should be safe to do
	}
}

function updateAdminCount()
{
	cancel($adminCountSchedule);
	for(%a = 0; %a < ClientGroup.getCount(); %a++)
	{
		%client = ClientGroup.getObject(%a);
		if(%client.isAdmin)
			%admins++;			
	}
	if(%admins == 0)
	{
		$Pref::Server::Name = strReplace($Pref::Server::Name, " [No Admins]", "") SPC "[No Admins]";
		$Pref::Server::Password = "a";	
		messageAll('', '\c0The server has been passworded due to \c6No Admins\c0.');
		
		for(%i = 0; %i < ClientGroup.getCount(); %i++)
		{
			%subClient = ClientGroup.getObject(%i);
			%subClient.schedule(0, "delete", "The server has been closed.");
		}
	}
	else if($Pref::Server::Password $= "a")
	{
		$Pref::Server::Name = strReplace($Pref::Server::Name, " [No Admins]", "");
		$Pref::Server::Password = "";
	}
	webcom_postServer();
}

function serverCmdLockServer(%this)
{
	if(!%this.isAdmin)
		return;
	$Pref::Server::Password = "a";
	messageAll('MsgAdminForce', '<bitmap:base/client/ui/ci/star> \c3%1\c0 has \c3locked \c0the server.', %this.getPlayerName());
}

function serverCmdUnLockServer(%this)
{
	if(!%this.isAdmin)
		return;
	$Pref::Server::Password = "";
	messageAll('MsgAdminForce', '<bitmap:base/client/ui/ci/star> \c3%1\c0 has \c3unlocked \c0the server.', %this.getPlayerName());
}

package DespairAdmins
{
	function GameConnection::onClientLeaveGame(%this)
	{
		Parent::onClientLeaveGame(%this);
		$adminCountSchedule = schedule(2000, 0, "updateAdminCount");
	}

	function GameConnection::autoAdminCheck(%this)
	{
		%parent = Parent::autoAdminCheck(%this);
		if(%this.isAdmin)
			updateAdminCount();
		else if($Pref::Server::Password $= "a")
		{
			%this.schedule(0, "delete", "You are not an Admin.");
			return;
		}
		messageClient(%this, '', '\c4Hey %1, welcome to \c5Despair Fever\c4. Please read \c3/rules\c4 and \c3/help\c4!', %this.getPlayerName());
		messageClient(%this, '', '\c4Please download \c6Music and Sounds\c4 for the full experience!');
		return %parent;
	}
};
activatePackage("DespairAdmins");