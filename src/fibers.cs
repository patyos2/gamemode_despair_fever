datablock StaticShapeData(fiberDecal)
{
	shapeFile = $Despair::Path @ "res/shapes/strand.dts";
	canClean = true;
};

function Player::spawnFiber(%this)
{
	%char = %this.character;
	%app = %char.appearance;
	%shirtColor = getField(%app, 4);
	%pantsColor = getField(%app, 5);
	%shoesColor = getField(%app, 6);
	%hairColor = getField(%app, 7);
	if(isObject(%this.getMountedImage(1)) && %this.getMountedImage(1).item.hideAppearance)
		%hideApp = true;
	if(isObject(%hat = %this.tool[%this.hatSlot]) && isObject(%this.getMountedImage(2)) && %this.getMountedImage(2) == nameToID(%hat.image))
	{
		if(%hat.hideHair)
			%hairColor = "0.25 0.25 0.25 1";

		if(%hideApp)
		{
			%shoesColor = "0.25 0.25 0.25 1";
			%shirtColor = "0.05 0.05 0.08 1";
			%pantsColor = "0.05 0.05 0.08 1";
		}
	}

	%colors = %shirtColor TAB %pantsColor TAB %shoesColor TAB %hairColor;
	%color = getField(%colors, getRandom(0, getFieldCount(%colors) - 1));

	%pos = vectorAdd(%this.getPosition(), "0 0 1");
	%ray = containerRayCast(%pos, VectorSub(%pos, "0 0 5"), $SprayBloodMask); //Fibers fall
	if(%ray)
	{
		%decal = spawnDecalFromRayCast(fiberDecal, %ray);
		%decal.color = %color;
		%decal.setNodeColor("ALL", %decal.color);
		%size = 0.5 + (getRandom() * 0.5);
		%decal.setScale(%size SPC %size SPC 1);
	}
}