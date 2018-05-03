//Base
$Despair::DayLength = 480; //8 minutes for a full cycle
$Despair::InvestigationLength = 420; //7 mins
$Despair::InvestigationExtraLength = 90; //+1:30 mins for every new body
$Despair::CritThreshold = -150; //How much negative health can the player take before dying

$Despair::DisableWepsInvest = true; //Disable weapons when investgation starts?
$Despair::DisableWepsTimer = 30; //How many seconds until the weps are disabled.
//Trial
$Despair::TrialChatDelay = 0.75; //Chat delay in seconds to prevent spamming
$Despair::DiscussPeriod = 420; //7 mins
$Despair::DiscussExtraLength = 90; //+1:30 mins for every body in trial
$Despair::MangleTimer = 900; //15 minutes if a mangled body turns up for discusison phase
$Despair::CanForceVote = 60; //1 min has to pass before you can forcevote
$Despair::CanForceTrial = 240; //4 mins have to pass before you can force trial
$Despair::MissingLength = 120; //2 mins until body announcement is made automatically
//Sleep
$Despair::SleepOverjoyed = 60;
$Despair::SleepHappy = 70;
$Despair::SleepSad = 85;
$Despair::SleepDepressed = 90;
$Despair::SleepDefault = 80;

//Mood
$Despair::Mood::Enabled = true;
//Traits
$Despair::Traits::Enabled = true;

$map = "school";
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
	$Despair::RoomCount = 22;

	$roomNum[1] = "11 Condo";
	$roomNum[2] = "12 Condo";
	$roomNum[3] = "13 Condo";
	$roomNum[4] = "14 Condo";
	$roomNum[5] = "15 Condo";
	$roomNum[6] = "16 Condo";
	$roomNum[7] = "21 Condo";
	$roomNum[8] = "22 Condo";
	$roomNum[9] = "23 Condo";
	$roomNum[10] = "24 Condo";
	$roomNum[11] = "25 Condo";

	$roomNum[12] = "11 Xing";
	$roomNum[13] = "12 Xing";
	$roomNum[14] = "13 Xing";
	$roomNum[15] = "14 Xing";
	$roomNum[16] = "15 Xing";
	$roomNum[17] = "16 Xing";
	$roomNum[18] = "21 Xing";
	$roomNum[19] = "22 Xing";
	$roomNum[20] = "23 Xing";
	$roomNum[21] = "24 Xing";
	$roomNum[22] = "25 Xing";
}