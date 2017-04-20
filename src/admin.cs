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
			%text[%a++] = "\c3["@ (%target.attackTime[%i] - $defaultMiniGame.lastResetTime) / 1000 @ " seconds after roundstart], \c6Attacker\c3:" SPC %target.attackCharacter[%i].clientName;
		}
		for (%i=1; %i<=%a; %i++)
			messageClient(%client, '', %text[%i]);
	}
	else
	{
		messageClient(%client, '', '\c5Player not found');
	}
}