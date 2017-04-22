datablock ItemData(PaperstackItem)
{
	shapeFile = $Despair::Path @ "res/shapes/items/Paperstack.dts";
	mass = 1;
	drag = 0.3;
	density = 0.2;
	elasticity = 0;
	friction = 1;
	doColorShift = true;
	colorShiftColor = "1 1 1 1";
	uiName = "Paper Stack";
	canDrop = true;
};

function PaperstackItem::onAdd(%this, %obj)
{
	parent::onAdd(%this, %obj);
	%obj.papers = 10;
}

function PaperstackItem::onPickUp(%this, %obj, %player)
{
	if(%player.addTool(PaperItem) == -1)
		return;
	%obj.papers--;
	if(%obj.papers <= 0)
	{
		%obj.delete();
		return;
	}
}

datablock ItemData(PaperItem)
{
	shapeFile = $Despair::Path @ "res/shapes/items/Paper.dts";
	image = PaperImage;
	mass = 0.5;
	drag = 7;
	density = 0;
	elasticity = 0;
	friction = 1;
	gravityMod = 0.5;
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
					%msg = %msg SPC "a wealthy progeny";
				case "Mod-Pilot":
					%msg = %msg SPC "an aspiring aviator";
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
			%a = getSubStr(getWord(%character.name, 0), 0, 1);
			%b = getSubStr(getWord(%character.name, 1), 0, 1);
			%rng = getRandom(0, 1);
			if(%rng == 0)
				%a = "#";
			if(%rng == 1)
				%b = "#";
			%msg = %msg SPC %a @ "." @ %b @ ".";
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

	%choice[%high++] = "Stuff you see in the news is sometimes inaccurate. Be sure to double-check!";
	%choice[%high++] = "Spilled Coke on your shirt? Just throw a coat on - nobody will see your dirty shirt from under it!";
	%choice[%high++] = "Be sure to admire yourself in the mirror by clicking it! There may be something on your face that you can't otherwise see.";
	%choice[%high++] = "A shower will not only completely clean you off, but will leave you feeling fresh and good about yourself!";
	%choice[%high++] = "Find a spooky mask to pull off a scary prank! With the mask on, they won't know it's you!";
	%choice[%high++] = "If you see something suspicious, scream! If you scream you'll be heard much farther.";
	%choice[%high++] = "Stick with someone who reflects your values! Otherwise you'll be in a constant state of internal conflict.";
	%choice[%high++] = "News sources are outdated! They don't report on any new developments.";

	return %choice[getRandom(%high)];
}