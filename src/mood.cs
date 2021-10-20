$Despair::Mood::Tick = 60000; //1 minute

function getMoodSmiley(%mood)
{
	%smiley = ":|";
	switch$ (%mood)
	{
		case "overjoyed":
			%smiley = "\c2:D";
		case "happy":
			%smiley = "\c2:)";
		case "neutral":
			%smiley = "\c6:l";
		case "sad":
			%smiley = "\c1:(";
		case "depressed":
			%smiley = "\c7:(";
	}
	return %smiley;
}

function getMoodColor(%mood)
{
	%color = "\c0";
	switch$ (%mood)
	{
		case "overjoyed":
			%color = "\c2";
		case "happy":
			%color = "\c2";
		case "neutral":
			%color = "\c6";
		case "sad":
			%color = "\c1";
		case "depressed":
			%color = "\c7";
	}
	return %color;
}

function getMoodName(%state)
{
	%text = "\c0";
	if (%state >= 22)
		%text = "overjoyed";
    else if (%state >= 17)
		%text = "happy";
    else if (%state >= 13)
		%text = "neutral";
    else if (%state >= 8)
		%text = "sad";
    else
		%text = "depressed";

	return %text;
}