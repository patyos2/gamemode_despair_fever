datablock ItemData(PaperItem)
{
	shapeFile = $Despair::Path @ "res/shapes/items/Paper.dts";
	image = PaperImage;
	mass = 1;
	drag = 0.3;
	density = 0.2;
	elasticity = 0;
	friction = 1;
	doColorShift = true;
	colorShiftColor = "1 1 1 1";
	uiName = "Paper";
	canDrop = true;

	itemPropsClass = "PaperProps";
	itemPropsAlways = true;

	customPickupAlways = true;
	customPickupMultiple = true;
};

function PaperProps::onAdd(%this)
{
	%this.name = "Paper";
	%this.contents = ""; //Text
}

datablock ShapeBaseImageData(PaperImage)
{
	item = PaperItem;
	shapeFile = $Despair::Path @ "res/shapes/items/Paper.dts";
	doColorShift = true;
	colorShiftColor = "1 1 1 1";
	offset = "-0.2 0.17 0";
	eyeOffset = "0 0.7 -0.2";
	eyeRotation = eulerToMatrix("90 0 0");
	rotation = eulerToMatrix( "90 25 -45" );

	armReady = true;
};

function PaperItem::onUse(%this, %obj, %slot)
{
	%obj.unMountImage(0);
	%obj.mountImage(%this.image, 0);
	fixArmReady(%obj);
}

function PaperImage::onMount(%this, %obj, %slot)
{
	%props = %obj.getItemProps();
	%text = "(It's blank)";
	if(%props.contents !$= "")
		%text = %props.contents;
	if (isObject(%obj.client))
		commandToClient(%obj.client, 'CenterPrint', "<font:cambria:32>\c3" @ %props.name @ "\n<color:FFFFFF>" @ %text);
}

function PaperImage::onUnMount(%this, %obj, %slot)
{
	%props = %obj.getItemProps();
	if (isObject(%obj.client))
		commandToClient(%obj.client, 'ClearCenterPrint');
}

function getPaperEvidence(%character)
{
	%r = getRandom(1, 3);
	switch (%r)
	{
		case 1:
			%msg = "Investigation into robbery suspect reveals them to be";
			%decal = getField(%character.appearance, 2);
			switch$ (%decal)
			{
				case "Mod-Suit":
					%msg = %msg SPC "classy";
				case "Mod-Pilot":
					%msg = %msg SPC "a pilot";
				case "Mod-Army":
					%msg = %msg SPC "an US Army fanatic";
				case "Meme-Mongler":
					%msg = %msg SPC "a dinosaur enthusiast";
				case "Medieval-YARLY":
					%msg = %msg SPC "a fan of White Owls";
				case "Medieval-Rider":
					%msg = %msg SPC "a Riders fan";
				case "Medieval-ORLY":
					%msg = %msg SPC "a fan of Night Owls";
				case "Medieval-Lion":
					%msg = %msg SPC "a Lions fan";
				case "Medieval-Eagle":
					%msg = %msg SPC "an Eagles fan";
				case "Hoodie":
					%msg = %msg SPC "wearing a hoodie";
				case "Alyx":
					%msg = %msg SPC "a Black Mesa fan";
			}

		case 2:
			%msg = "Suspect of yesterday murder appears to be";
			if(%character.gender $= "male")
				%msg = %msg SPC "a male delinquent";
			else
				%msg = %msg SPC "a female student";

		case 3:
			%msg = "Initials of criminal revealed to be";
			%msg = %msg SPC getSubStr(getWord(%character.name, 0), 0, 1) @ "." @ getSubStr(getWord(%character.name, 1), 0, 1) @ ".";
	}
	return %msg;
}

function getPaperTrash()
{
	%high = -1;

	%choice[%high++] = "HONK HONK HONK HONK HONK HONK HONK HONK HONK HONK HONK HONK";
	%choice[%high++] = "(This seems to be a patent for \"joke tape\", a roll of scotch tape without any actual glue on it.)";
	%choice[%high++] = ""; //You can write on blank
	%choice[%high++] = "(It seems to be a stylized drawing of a video game character.)";
	%choice[%high++] = "Skeletons have invaded the USA, taking over the country and renaming it to USS, short for \"the United States of Skeletons\".";
	%choice[%high++] = "He doesnt love me... im not gona be with a fake bitch like him!!!";
	%choice[%high++] = "To do: to do! to do, to do, to do, to do, to dooooooo, dodododo";
	%choice[%high++] = "To do: cut someone";
	%choice[%high++] = "To do: find some source of wifi holy shit there isn't even cell service";
	%choice[%high++] = ":^)";
	%choice[%high++] = "die";
	%choice[%high++] = "i got killed in csgo and i am so fkin mad rn";

	return %choice[getRandom(%high)];
}

function getPaperTips()
{
	%high = -1;

	%choice[%high++] = "Paper evidence is only 60% accurate.";
	%choice[%high++] = "Spilled Coke on your shirt? Just throw a coat on - nobody will see your dirty shirt from under it!";
	%choice[%high++] = "Did you know you can look at yourself in the mirror? Shocking, right?";
	%choice[%high++] = "Blunt weapons leave less blood than sharp ones. Moreover, sharp weapons leave blood behind you, too!";
	%choice[%high++] = "You can disguise your name with hats or masks that cover your face!";
	%choice[%high++] = "If you see something suspicious, scream! If you scream you'll be heard much farther.";

	return %choice[getRandom(%high)];
}