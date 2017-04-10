datablock StaticShapeData(DespairStand)
{
	shapeFile = $Despair::Path @ "res/shapes/stand.dts";
	canDismount = false;
};
datablock StaticShapeData(DespairMemorial)
{
	shapeFile = $Despair::Path @ "res/shapes/memorial.dts";
};

function createShape(%data, %position, %rotation, %scale, %color)
{
	if (%color $= "") %color = "0.5 0.3 0.1 1";
	%shape = new StaticShape()
	{
		datablock = %data;
		position = %position;
		rotation = %rotation;
		scale = %scale;
	};
	%shape.setNodeColor("ALL", %color);
	return %shape;
}

function createCourtroom()
{
	if (isObject(CourtroomGroup))
		CourtroomGroup.deleteAll();
	else
		new SimGroup(CourtroomGroup);
	%oldInstantGroup = $instantGroup;
	$instantGroup = CourtroomGroup;

	$stand0 = createShape(DespairStand,  "0 0 -300");
	$stand1 = createShape(DespairStand,  "0 0 -300");
	$stand2 = createShape(DespairStand,  "0 0 -300");
	$stand3 = createShape(DespairStand,  "0 0 -300");
	$stand4 = createShape(DespairStand,  "0 0 -300");
	$stand5 = createShape(DespairStand,  "0 0 -300");
	$stand6 = createShape(DespairStand,  "0 0 -300");
	$stand7 = createShape(DespairStand,  "0 0 -300");
	$stand8 = createShape(DespairStand,  "0 0 -300");
	$stand9 = createShape(DespairStand,  "0 0 -300");
	$stand10 = createShape(DespairStand, "0 0 -300");
	$stand11 = createShape(DespairStand, "0 0 -300");
	$stand12 = createShape(DespairStand, "0 0 -300");
	$stand13 = createShape(DespairStand, "0 0 -300");
	$stand14 = createShape(DespairStand, "0 0 -300");
	$stand15 = createShape(DespairStand, "0 0 -300");

	$memorial0 = createShape(DespairMemorial,  "0 0 -300");
	$memorial1 = createShape(DespairMemorial,  "0 0 -300");
	$memorial2 = createShape(DespairMemorial,  "0 0 -300");
	$memorial3 = createShape(DespairMemorial,  "0 0 -300");
	$memorial4 = createShape(DespairMemorial,  "0 0 -300");
	$memorial5 = createShape(DespairMemorial,  "0 0 -300");
	$memorial6 = createShape(DespairMemorial,  "0 0 -300");
	$memorial7 = createShape(DespairMemorial,  "0 0 -300");
	$memorial8 = createShape(DespairMemorial,  "0 0 -300");
	$memorial9 = createShape(DespairMemorial,  "0 0 -300");
	$memorial10 = createShape(DespairMemorial, "0 0 -300");
	$memorial11 = createShape(DespairMemorial, "0 0 -300");
	$memorial12 = createShape(DespairMemorial, "0 0 -300");
	$memorial13 = createShape(DespairMemorial, "0 0 -300");
	$memorial14 = createShape(DespairMemorial, "0 0 -300");
	$memorial15 = createShape(DespairMemorial, "0 0 -300");

	$instantGroup = %oldInstantGroup;
}
if (!isObject(MissionCleanup))
	schedule("0", "0", "createCourtroom");

function courtPlayers()
{
	%charCount = 8;//GameCharacters.getCount();
	for (%i = 0; %i < 16; %i++)
		$stand[%i].setTransform("0 0 -300");
	
	%radius = getMax(2, %charCount * 0.5);
	for (%i = 0; %i < %charCount; %i++)
	{
		%angle = $m2pi * (%i / %charCount);
		%x = mCos(%angle) * %radius;
		%y = 8 + mSin(%angle) * %radius;
		$stand[%i].setTransform(%x SPC %y SPC "100 0 0 -1" SPC (%angle + $piOver2));
		$memorial[%i].setTransform($stand[%i].getSlotTransform(0));
		//%character = GameCharacters.getObject(%i);
		//%player = %character.player;
	}
}