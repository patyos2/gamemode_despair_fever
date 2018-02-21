//Base
$Despair::DayLength = 360; //6 minutes for a full cycle
$Despair::InvestigationLength = 360; //6 mins
$Despair::InvestigationExtraLength = 90; //+1:30 mins for every new body
$Despair::CritThreshold = -150; //How much negative health can the player take before dying
//Trial
$Despair::DiscussPeriod = 300; //5 mins
$Despair::CanForceVote = 60; //1 min has to pass before you can forcevote
$Despair::CanForceTrial = 300; //5 mins have to pass before you can force trial
$Despair::MissingLength = 120; //2 mins until body announcement is made automatically

$map = "motel";
//MOTEL MAP PREFS:
if($map $= "motel")
{
	$mapCenter = "-87.7265 -20.5779 0.20202";
	$boundaries = 170;
	$Despair::RoomCount = 22;
	//male block
	$roomNum[1] = "16";
	$roomNum[2] = "17";
	$roomNum[3] = "18";
	$roomNum[4] = "26";
	$roomNum[5] = "27";
	$roomNum[6] = "28";

	//female block
	$roomNum[7] = "11";
	$roomNum[8] = "12";
	$roomNum[9] = "13";
	$roomNum[10] = "14";
	$roomNum[11] = "15";
	$roomNum[12] = "21";
	$roomNum[13] = "22";
	$roomNum[14] = "23";
	$roomNum[15] = "24";
	$roomNum[16] = "25";

	//extra rooms
	$roomNum[17] = "A";
	$roomNum[18] = "B";
	$roomNum[19] = "C";
	$roomNum[20] = "D";
	$roomNum[21] = "E";
	$roomNum[22] = "F";
}
//MANSION MAP PREFS:
if($map $= "mansion")
{
	$Despair::RoomCount = 14;
	//underground
	$roomNum[1] = "B1";
	$roomNum[2] = "B2";
	$roomNum[3] = "B3";
	$roomNum[4] = "B4";
	$roomNum[5] = "B5";
	$roomNum[6] = "B6";
	$roomNum[7] = "B7";

	//aboveground
	$roomNum[8] = "Shack";
	$roomNum[9] = "F2";
	$roomNum[10] = "F3";
	$roomNum[11] = "F4";
	$roomNum[12] = "F5";
	$roomNum[13] = "F6";
	$roomNum[14] = "F7";
}
//SCHOOL MAP PREFS:
if($map $= "school")
{
	$Despair::RoomCount = 16;

	$roomNum[1] = "1";
	$roomNum[2] = "2";
	$roomNum[3] = "3";
	$roomNum[4] = "4";
	$roomNum[5] = "5";
	$roomNum[6] = "6";
	$roomNum[7] = "7";
	$roomNum[8] = "8";
	$roomNum[9] = "9";
	$roomNum[10] = "10";
	$roomNum[11] = "11";
	$roomNum[12] = "12";
	$roomNum[13] = "13";
	$roomNum[14] = "14";
	$roomNum[15] = "15";
	$roomNum[16] = "16";
}