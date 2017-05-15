//This is a script file for /help and /rules commands
function serverCmdHelp(%this, %cat)
{
	%cat = strLwr(%cat);
	switch$ (%cat)
	{
		case "1" or "gamemode":
			%text[%count++] = "\c3[GAMEMODE]";
			%text[%count++] = " \c6The first day is the day without the killer. However, as soon as night strikes, the culprit is picked.";
			%text[%count++] = " \c6Killer's objective is to murder someone and get away with it. They can only kill up to three people (two if investigation period has started).";
			%text[%count++] = " \c6Once the killer does their job, they have to blend in and try to seem least suspicious.";
			%text[%count++] = " \c6A body discovery announcement is initated when at least two people examine/scream at the body.";
			%text[%count++] = " \c6After a body has been discovered, investigation period starts.";
			%text[%count++] = " \c6During investigation, the crime scene is \c3\"frozen\"\c6 - Nobody can modify the state of the scene and nobody will be able to use weapons.";
			%text[%count++] = " \c6After investigation period ends, the trial period starts.";
			%text[%count++] = " \c6In trial, everyone is teleported to the courtroom where you are given time to discuss everyone's alibis and possible suspects.";
			%text[%count++] = " \c6After trial period is over, it's time to vote the most suspicious person.";
			%text[%count++] = " \c6If majority vote is correct and everyone votes the killer, the killer dies and everyone else lives.";
			%text[%count++] = " \c6However, if the vote is a tie, people didn't vote or majority vote is wrong, the killer wins and everyone else is executed.";
			%text[%count++] = " \c6Trial period is the climax of the round - make sure to pay close attention and figure out the culprit!";
			%text[%count++] = " \c5Page Up to read the above.";
		case "2" or "chat":
			%text[%count++] = "\c3[CHAT]";
			%text[%count++] = " \c6Normal chat is local/IC (In-Character) chat. Put @ before your message to whisper (like so: \c3\"@whisper\"\c6)";
			//%text[%count++] = " \c6You can also say /me *action* to do an IC action, like \c3\"John Doe grabs a weapon.\"";
			%text[%count++] = " \c6To use Killer-to-Admin chat as the killer, use \c3TEAM CHAT\c6 (Default key: \c3\"Y\"\c6)";
			%text[%count++] = " \c6Please follow the chat etiquette defined in the /rules.";
			%text[%count++] = " \c5Page Up to read the above.";
		case "3" or "killer":
			%text[%count++] = "\c3[KILLER]";
			%text[%count++] = " \c6You can become the killer at first night or by killing the killer.";
			%text[%count++] = " \c6Once you become the killer, you have to kill someone and get away with it. Killing people in public or having a killing spree is completely discouraged.";
			%text[%count++] = " \c6You can only kill so much. When you hit an arbitrary kill limit, your weapon will be disabled and you will be unable to kill.";
			%text[%count++] = " \c6You can clean up the blood with a mop. To wash blood off yourself, you can use sinks, showers, water pumps, buckets or anything of the like.";
			%text[%count++] = " \c6Sinks only clean your hands and head. Showers, buckets, water pumps clean you head to toe.";
			%text[%count++] = " \c6To clean your weapon, take it out when washing and use \c3\"Paint Key\"\c6 to activate events.";
			%text[%count++] = " \c6You also have a number of cool abilities!";
			%text[%count++] = " \c6You can sprint with \c3\"JET\" \c6key (default\c3: \"RightClick\"\c6)";
			%text[%count++] = " \c6When holding a sleeping person, you can hold \c3\"JET\" \c6key to choke them out! This is a bloodless and silent way to kill someone.";
			%text[%count++] = " \c6You can hear nearby heartbeats with \c3\"LIGHT\" \c6key (default\c3: \"R\"\c6)";
			%text[%count++] = " \c6And, since you will \c3never get tired\c6, you can fake \c3/sleep\c6 and get up any time!";
			%text[%count++] = " \c6Being a killer is very hard. Make sure to plan, but don't take your time!";
			%text[%count++] = " \c5Page Up to read the above.";
		case "4" or "mechanics":
			%text[%count++] = "\c3[MECHANICS]";
			%text[%count++] = " \c6To unlock/lock the door with the key, press \c3\"FIRE\"\c6 and \c3\"JET\"\c6 keys (default\c3: \"LeftClick\", \"RightClick\"\c6) respectively.";
			%text[%count++] = " \c6You can sleep with /sleep if you're sleepy, tired or exhausted. It's highly reccomeneded to go to sleep early, otherwise you will have to sleep way more often!";
			%text[%count++] = " \c6Dormitory rooms are the \c3best place to sleep\c6. They are your little safe havens. Make sure to lock the door!";
			%text[%count++] = " \c6Press \c3\"LIGHT\"\c6 key (default\c3: \"R\"\c6) when aiming at a door to knock, and when aiming at the corpse to loot it.";
			%text[%count++] = " \c6To navigate the inventory menu, scroll w/ mouse and click to take something out. To plant something on the corpse, drop an item when looking at it.";
			%text[%count++] = " \c3\"Hold-click\" and move your mouse\c6 to carry around a corpse.";
			%text[%count++] = " \c6You can \c3Scream\c6 with \c3\"ALARM\"\c6 key (default\c3: \"H\"\c6). However, you must be looking at something screwed up, like blood or a body.";
			%text[%count++] = " \c6Every morning, \c3papers\c6 will spawn. These papers will range from \c3trash\c6, \c3gameplay tips\c6 and \c3news articles\c6.";
			%text[%count++] = " \c6Daily News papers will contain \c3IMPORTANT INFORMATION\c6 giving you a tip on the killer's identity! Be sure to seek them out!";
			%text[%count++] = " \c6There are \c0NO NON-LETHAL WEAPONS\c6 in this gamemode. Everything will kill.";
			%text[%count++] = " \c5Page Up to read the above.";
		case "5" or "combat":
			%text[%count++] = "\c3[COMBAT]";
			%text[%count++] = " \c6You can find melee weapons all over the map.";
			%text[%count++] = " \c6EVERYTHING can \c3BREAK DOWN DOORS\c6, however, it takes a while to do so and it's incredibly loud, and effectiveness varies on the weapon.";
			//%text[%count++] = " \c6Attacks from behind have multiplied damage. Use this to your advantage!";
			%text[%count++] = " \c6If you fall in combat, you will enter \c0Critical Health\c6. In that state, your speaking range will be reduced and you will not able to scream as much.";
			%text[%count++] = " \c0You cannot be saved\c6 from that state. However, you can leave your last message with \c3/write [msg]\c6! Be sure to be aiming at a surface.";
			%text[%count++] = " \c6There are plans to expand the combat system, so stay tuned for updates!";
			%text[%count++] = " \c5Page Up to read the above.";
		case "6" or "rules":
			%text[%count++] = "\c3[RULES]";
			%text[%count++] = " \c31\c6. \c0Don't be a dick!\c6 We're all here to have fun. If you block doorways, lock people in your rooms or break room doors as a non-killer w/o a reason you will be banned.";
			%text[%count++] = " \c32\c6. \c0Do not gamethrow!\c6 This means don't play at expense of others, deliberately act extra suspicious or pretend to be the culprit.";
			%text[%count++] = " \c33\c6. \c0Follow chat etiquette!\c6 Don't reference people by their in-game names, don't spam chat and please don't abuse emoticons.";
			%text[%count++] = " \c34\c6. \c0Don't freekill!\c6 It's really obvious to admins when you freekill. We will figure if it's self-defence or not, but expect to be banned if you kill someone without a reason.";
			%text[%count++] = " \c35\c6. \c0Don't metagame!\c6 Do not relay in-game information to others through out-of-game means! If we determine you are metagaming you will be banned.";
			%text[%count++] = " \c36\c6. \c0Don't ERP (Erotic RolePlay)!\c6 It is obnoxious as fuck and serves no purpose other than to get some preteens' dick wet and annoy everyone else.";
			%text[%count++] = " \c0    EXAMPLES PROVIDED IN THIS LIST ARE ONLY EXAMPLES. THEY DO NOT ENCAPSULATE THE FULL EXTENT OF THE RULE'S EFFECT.";
			//%text[%count++] = " \c3If someone is breaking the rules, use /report *message* to get an admin's attention!";
			%text[%count++] = " \c5Page Up to read the above.";
		case "7" or "commands":
			%text[%count++] = "\c3[COMMANDS]";
			%text[%count++] = " \c6/keepcharacter \c7- \c6Enable or disable character persistance (if you survive a round, you keep your character for the next)";
			%text[%count++] = " \c6/forcevote \c7- \c6Skip the discussion phase and get straight to the voting (Trial)";
			if (%this.isAdmin)
			{
				%text[%count++] = "\c3[ADMIN]";
				%text[%count++] = " \c6Team Chat \c7- \c6Talk through admin-only chat";
				%text[%count++] = " \c3@\c6 before Team Chat e.g. \c3@message \c7- \c6Talk to the killer directly (cannot see killer chat when alive)";
				%text[%count++] = " \c6/damageLogs \c3name \c7- \c6See what damage has been done to \c3name";
				%text[%count++] = " \c6/whoIs \c3name \c7- \c6Find out who \c3name\c6's in-game alias is";
				%text[%count++] = " \c6/spectate \c7- \c6Become a spectator to be excluded from rounds";
				%text[%count++] = " \c6/showroles \c7- \c6See who's killer or innocent when spectating";
				%text[%count++] = " \c6/announce \c3message \c7- \c6Send a server-wide announcement";
			}
			%text[%count++] = " \c5Page Up to read the above.";
		default:
			%text[%count++] = "<font:impact:30>\c6Welcome to \c5Despair Fever\c6!";
			%text[%count++] = "\c6======";
			%text[%count++] = " \c6Available topics:";
			%text[%count++] = "   \c31\c6 - \c3gamemode\c6: How the gamemode progresses";
			%text[%count++] = "   \c32\c6 - \c3chat\c6: Chat functionality explained";
			%text[%count++] = "   \c33\c6 - \c3killer\c6: What to do if you're killer";
			%text[%count++] = "   \c34\c6 - \c3mechanics\c6: Game mechanics - locking doors, papers, sleeping, etc.";
			%text[%count++] = "   \c35\c6 - \c3combat\c6: Health and Melee explained";
			%text[%count++] = "   \c36\c6 - \c3rules\c6: Read this to avoid getting banned";
			%text[%count++] = "   \c37\c6 - \c3commands\c6: Various slash commands";
			%text[%count++] = "\c6======";
			%text[%count++] = "\c5Say \c3/help *category*\c5 for more info on certain topics.";
	}

	for (%i=1; %i<=%count; %i++)
		messageClient(%this, '', %text[%i]);
}

function serverCmdRules(%this)
{
	serverCmdHelp(%this, "rules");
}