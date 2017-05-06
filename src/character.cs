function getRandomAppearance(%gender)
{
	%appearance =
		getRandomSkinColor() TAB
		getRandomFaceName(%gender) TAB
		getRandomDecalName() TAB
		getRandomHairName(%gender) TAB
		getRandomGenericColor() TAB
		getRandomPantsColor() TAB
		getRandomPantsColor() TAB
		getRandomHairColor();
	return %appearance;
}

function getRandomSkinColor()
{
	%index = -1;
	%color[%index++] = "0.956863 0.878431 0.784314";
	%color[%index++] = "1 0.878431 0.611765";
	%color[%index++] = "1 0.603922 0.423529";
	if (getRandom(1, 10) == 1) //rare color
		%color[%index++] = "0.392157 0.196078 0";
	%pick = getRandom(%index);
	%r = getmax(getmin(getWord(%color[%pick], 0) + 0.05 - getRandom() * 0.1, 1), 0);
	%g = getmax(getmin(getWord(%color[%pick], 1) + 0.05 - getRandom() * 0.1, 1), 0);
	%b = getmax(getmin(getWord(%color[%pick], 2) + 0.05 - getRandom() * 0.1, 1), 0);

	return %r SPC %g SPC %b SPC 1;
}

function getRandomHairColor()
{
	//Natural colors
	//Grays/"blues"
	%i = -1;
	%color[%i++] = "0.753 0.816 0.816";
	%color[%i++] = "0.439 0.502 0.565";
	%color[%i++] = "0.251 0.251 0.376";
	%color[%i++] = "0.125 0.063 0.188";
	//Browns (light)
	%color[%i++] = "0.816 0.816 0.69";
	%color[%i++] = "0.627 0.502 0.376";
	%color[%i++] = "0.376 0.251 0.251";
	%color[%i++] = "0.251 0.125 0.125";
	//Browns
	%color[%i++] = "0.878 0.69 0.376";
	%color[%i++] = "0.753 0.502 0.251";
	%color[%i++] = "0.627 0.314 0.125";
	%color[%i++] = "0.251 0 0";
	//Orange
	%color[%i++] = "1 0.502 0.251";
	%color[%i++] = "0.753 0.251 0";
	%color[%i++] = "0.502 0.125 0";
	%color[%i++] = "0.251 0.063 0";
	//Blonde
	%color[%i++] = "1 1 0.627";
	%color[%i++] = "1 0.753 0.251";
	%color[%i++] = "0.753 0.502 0";
	%color[%i++] = "0.502 0.251 0";
	//Ginger
	%color[%i++] = "1 0.251 0";
	%color[%i++] = "1 0.502 0";
	%color[%i++] = "1 0.878 0";
	//Dyed colors
	if (getRandom(1, 3) == 1) //sorta rarer
	{
		//Green
		%color[%i++] = "0.753 1 0";
		%color[%i++] = "0.251 0.753 0";
		%color[%i++] = "0 0.502 0.251";
		%color[%i++] = "0 0.251 0.251";
		//Teal
		%color[%i++] = "0 1 0.502";
		%color[%i++] = "0 0.753 0.502";
		%color[%i++] = "0 0.502 0.502";
		%color[%i++] = "0 0.251 0.376";
		//Blues
		%color[%i++] = "0.502 0.941 1";
		%color[%i++] = "0 0.753 1";
		%color[%i++] = "0 0.251 0.753";
		%color[%i++] = "0.125 0 0.502";
		//Purple
		%color[%i++] = "0.878 0.627 1";
		%color[%i++] = "0.753 0.251 1";
		%color[%i++] = "0.502 0 0.753";
		//Pinks
		%color[%i++] = "1 0.753 0.753";
		%color[%i++] = "1 0.376 0.502";
		%color[%i++] = "0.753 0 0.376";
		%color[%i++] = "0.502 0 0.376";
	}

	return %color[getRandom(0, %i)] SPC 1;
}

function getRandomGenericColor()
{
	%color0 = "0.9 0 0";
	%color1 = "0.9 0 0";
	%color2 = "0.74902 0.180392 0.482353";
	%color3 = "0.388235 0 0.117647";
	%color4 = "0.133333 0.270588 0.270588";
	%color5 = "0 0.141176 0.333333";
	%color6 = "0.105882 0.458824 0.768627";
	%color7 = "1 1 1";
	%color8 = "0.0784314 0.0784314 0.0784314";
	%color9 = "0.92549 0.513726 0.678431";
	%color10 = "0 0.5 0.25";
	%color11 = "0.784314 0.921569 0.490196";
	%color12 = "0.541176 0.698039 0.552941";
	%color13 = "0.560784 0.929412 0.960784";
	%color14 = "0.698039 0.662745 0.905882";
	%color15 = "0.878431 0.560784 0.956863";
	%color16 = "0.888 0 0";
	%color17 = "1 0.5 0";
	%color18 = "0.99 0.96 0";
	%color19 = "0.2 0 0.8";
	%color20 = "0 0.471 0.196";
	%color21 = "0 0.2 0.64";
	%color22 = "0.596078 0.160784 0.392157";
	%color23 = "0.55 0.7 1";
	%color24 = "0.85 0.85 0.85";
	%color25 = "0.1 0.1 0.1";
	%color26 = "0.9 0.9 0.9";
	%color27 = "0.75 0.75 0.75";
	%color28 = "0.5 0.5 0.5";
	%color29 = "0.2 0.2 0.2";
	%color30 = "0.901961 0.341176 0.0784314";

	return %color[getRandom(0, 30)] SPC 1;
}
function getRandomPantsColor()
{
	%color0 = "0.75 0.75 0.75";
	%color1 = "0.2 0.2 0.2";
	%color2 = "0.388 0 0.117";
	%color3 = "0.133 0.27 0.27";
	%color4 = "0 0.141 0.333";
	%color5 = "0.078 0.078 0.078";

	return %color[getRandom(0, 5)] SPC 1;
}


//Some faces are from winterbite face pack located here: https://www.dropbox.com/s/misn71lpawwi8le/Face_WinterBite.zip
function getRandomFaceName(%gender)
{
	%high = -1;
	%choice[%high++] = "smiley";
	if (%gender $= "male")
	{
		%choice[%high++] = "Male07Smiley";
		%choice[%high++] = "BrownSmiley";
		%choice[%high++] = "memeHappy";
		%choice[%high++] = "memeBlockMongler";
		// %choice[%high++] = "smileyBlonde";
		%choice[%high++] = "smileyCreepy";
		//Winterbite faces:
		%choice[%high++] = "smileyST";
		%choice[%high++] = "kleinerSmiley";
		%choice[%high++] = "kleinerSmiley2ST";
		%choice[%high++] = "kleinerSmiley2";
	}
	else
	{
		//Winterbite faces:
		%choice[%high++] = "smileyfST";
		%choice[%high++] = "smileyfCreepy";
		%choice[%high++] = "smileyf";
		%choice[%high++] = "KleinerfSmileysST";
		%choice[%high++] = "KleinerfSmiley";
	}

	return %choice[getRandom(%high)];
}

function getRandomDecalName()
{
	%high = -1;

	%choice[%high++] = "";
	%choice[%high++] = "Mod-Suit";
	%choice[%high++] = "Mod-Pilot";
	%choice[%high++] = "Mod-Army";
	%choice[%high++] = "Meme-Mongler";
	%choice[%high++] = "Medieval-YARLY";
	%choice[%high++] = "Medieval-Rider";
	%choice[%high++] = "Medieval-ORLY";
	%choice[%high++] = "Medieval-Lion";
	%choice[%high++] = "Medieval-Eagle";
	%choice[%high++] = "Hoodie";
	%choice[%high++] = "Alyx";

	return %choice[getRandom(%high)];
}

function getRandomHairName(%gender)
{
	%high = -1;
	//Unisex hairs
	%choice[%high++] = "hair_wig";
	if (%gender $= "male") //Male hairs
	{
		%choice[%high++] = "hair_cornrows";
		%choice[%high++] = "hair_familiar";
		%choice[%high++] = "hair_layered";
		%choice[%high++] = "hair_messy";
		%choice[%high++] = "hair_neat";
		%choice[%high++] = "hair_suav";
		%choice[%high++] = "hair_mullet";
		%choice[%high++] = "hair_pompadour";
		%choice[%high++] = "hair_punk";
		%choice[%high++] = "hair_shaggy";
		%choice[%high++] = "hair_fabio";
	}
	else //Female hairs
	{
		%choice[%high++] = "hair_broad";
		%choice[%high++] = "hair_bunn";
		%choice[%high++] = "hair_headband";
		%choice[%high++] = "hair_ponytail";
		%choice[%high++] = "hair_daphne";
		%choice[%high++] = "hair_velma";
		%choice[%high++] = "hair_mahiru";
		%choice[%high++] = "hair_maya";
	}

	if (getRandom(1, 2) == 1) //rare hairs
	{
		%choice[%high++] = "hair_mohawk";
		%choice[%high++] = "hair_rocker";
	}
	return %choice[getRandom(%high)];
}


//todo: fix this to new system
function getRandomSpecialChar(%char)
{
	//%char[%chars++-1] = "Name\tChance\tGender\tSkin\tFace\tDecal\tHair\tShirt\tPants\tShoes\tHair";
	%char[%chars++-1] = "William T. Riker\t1\tM\t1 0.8 0.6 1\tMale07Smiley\tSpace-New\thair_messy\t0.7 0.1 0.1 1\t0.1 0.1 0.1 1\t0.1 0.1 0.1 1\t0.05 0.025 0 1";
	%char[%chars++-1] = "Mr. T\t1\tM\t0.4 0.2 0 1\tBrownSmiley\tMedieval-Tunic\thair_mohawk\t0 0 0 1\t0.8 0 0 1\t0.2 0.2 0.2 1\t0 0 0 1";
	%char[%chars++-1] = "Jamie Hyneman\t1\tM\t1 0.8 0.6 1\tJamie\tAAA-None\tscoutHat\t1 1 1 1\t0.2 0.2 0.2 1\t0.4 0.2 0 1\t0.1 0.1 0.1 1";
	%char[%chars++-1] = "Adam Savage\t1\tM\t1 0.8 0.6 1\tAdamSavage\tAAA-None\theadSkin\t0.2 0.2 0.2 1\t0.2 0.2 0.2 1\t0.4 0.2 0 1\t1 0.8 0.6 1";
	for (%i = 0; %i < %chars; %i++)
		%chance += getField(%char[%i], 1);
	%choose = getRandom() * %chance;
	for (%i = 0; %i < %chars; %i++)
	{
		%choose -= getField(%char[%i], 1);
		if (%choose <= 0)
		{
			%choose = %char[%i];
			break;
		}
	}
	%char.name = getField(%choose, 0);
	if(getField(%choose, 2) $= "M")
		%char.gender = "male";
	else
		%char.gender = "female";

	%char.appearance = getFields(%choose, 3, 10);
}

function getCharacterName(%char, %checkDisguised)
{
	if(%char.getClassName() $= "GameConnection")
		%char = %char.character;

	%name = %char.name;
	%player = %character.player;
	if(isObject(%player))
	{
		if(%checkDisguised && isObject(%hat = %player.tool[%player.hatSlot]) && %hat.disguise && isObject(%img = %player.getMountedImage(2)) && %img == nameToID(%hat.image))
			%name = "Unknown";
		if(%player.mangled)
			%name = "Unknown";
		if(%player.fakeName)
			%name = %player.fakeName;
	}
	return %name;
}