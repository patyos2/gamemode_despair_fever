datablock AudioProfile(VoicePain1Male) {
	fileName = $Despair::Path @ "res/sounds/voice/male_pain-01.wav";
	description = audioClose3D;
	preload = true;
};
datablock AudioProfile(VoicePain2Male) {
	fileName = $Despair::Path @ "res/sounds/voice/male_pain-02.wav";
	description = audioClose3D;
	preload = true;
};
datablock AudioProfile(VoicePain3Male) {
	fileName = $Despair::Path @ "res/sounds/voice/male_pain-03.wav";
	description = audioClose3D;
	preload = true;
};
$painCount["male"] = 3;

datablock AudioProfile(VoiceDeath1Male) {
	fileName = $Despair::Path @ "res/sounds/voice/male_Die-01.wav";
	description = audioClose3D;
	preload = true;
};
datablock AudioProfile(VoiceDeath2Male) {
	fileName = $Despair::Path @ "res/sounds/voice/male_Die-02.wav";
	description = audioClose3D;
	preload = true;
};
datablock AudioProfile(VoiceDeath3Male) {
	fileName = $Despair::Path @ "res/sounds/voice/male_Die-03.wav";
	description = audioClose3D;
	preload = true;
};
datablock AudioProfile(VoiceDeath4Male) {
	fileName = $Despair::Path @ "res/sounds/voice/male_Die-04.wav";
	description = audioClose3D;
	preload = true;
};
$deathCount["male"] = 4;

datablock AudioProfile(VoiceShock1Male) {
	fileName = $Despair::Path @ "res/sounds/voice/male_shock-01.wav";
	description = AudioDefault3d;
	preload = true;
};
datablock AudioProfile(VoiceShock2Male) {
	fileName = $Despair::Path @ "res/sounds/voice/male_shock-02.wav";
	description = AudioDefault3d;
	preload = true;
};
datablock AudioProfile(VoiceShock3Male) {
	fileName = $Despair::Path @ "res/sounds/voice/male_shock-03.wav";
	description = AudioDefault3d;
	preload = true;
};
datablock AudioProfile(VoiceShock4Male) {
	fileName = $Despair::Path @ "res/sounds/voice/male_shock-04.wav";
	description = AudioDefault3d;
	preload = true;
};
datablock AudioProfile(VoiceShock5Male) {
	fileName = $Despair::Path @ "res/sounds/voice/male_shock-05.wav";
	description = AudioDefault3d;
	preload = true;
};
$shockCount["male"] = 5;


datablock AudioProfile(VoicePain1Female) {
	fileName = $Despair::Path @ "res/sounds/voice/female_pain-01.wav";
	description = audioClose3D;
	preload = true;
};
datablock AudioProfile(VoicePain2Female) {
	fileName = $Despair::Path @ "res/sounds/voice/female_pain-02.wav";
	description = audioClose3D;
	preload = true;
};
datablock AudioProfile(VoicePain3Female) {
	fileName = $Despair::Path @ "res/sounds/voice/female_pain-03.wav";
	description = audioClose3D;
	preload = true;
};
$painCount["female"] = 3;

datablock AudioProfile(VoiceDeath1Female) {
	fileName = $Despair::Path @ "res/sounds/voice/Female_Pain-04.wav";
	description = audioClose3D;
	preload = true;
};
datablock AudioProfile(VoiceDeath2Female) {
	fileName = $Despair::Path @ "res/sounds/voice/Female_Pain-05.wav";
	description = audioClose3D;
	preload = true;
};
datablock AudioProfile(VoiceDeath3Female) {
	fileName = $Despair::Path @ "res/sounds/voice/Female_Pain-06.wav";
	description = audioClose3D;
	preload = true;
};
$deathCount["female"] = 3;

datablock AudioProfile(VoiceShock1Female) {
	fileName = $Despair::Path @ "res/sounds/voice/Female_shock-01.wav";
	description = AudioDefault3d;
	preload = true;
};
datablock AudioProfile(VoiceShock2Female) {
	fileName = $Despair::Path @ "res/sounds/voice/Female_shock-02.wav";
	description = AudioDefault3d;
	preload = true;
};
datablock AudioProfile(VoiceShock3Female) {
	fileName = $Despair::Path @ "res/sounds/voice/Female_shock-03.wav";
	description = AudioDefault3d;
	preload = true;
};
$shockCount["female"] = 3;

datablock AudioProfile(VoiceCheese1Female) {
	fileName = $Despair::Path @ "res/sounds/voice/Female_Death-01.wav";
	description = audioClose3D;
	preload = true;
};
datablock AudioProfile(VoiceCheese2Female) {
	fileName = $Despair::Path @ "res/sounds/voice/Female_Death-02.wav";
	description = audioClose3D;
	preload = true;
};

function Player::playShock(%player)
{
	%gender = %player.gender;
	if(%gender $= "")
		%gender = "male";
	%player.playAudio(0, VoiceShock @ getRandom(1,$shockCount[%gender]) @ %gender);
}

function serverCmdAlarm(%client)
{
	if(isObject(%player = %client.player))
	{
		%scream = false;

		%center = %player.getEyePoint();
		initContainerRadiusSearch(%center, 128, $TypeMasks::PlayerObjectType | $TypeMasks::CorpseObjectType | $TypeMasks::StaticShapeObjectType);
		while (isObject(%obj = containerSearchNext()))
		{
			if(%obj == %player)
				continue;
			%point = %obj.getPosition();
			if(%obj.getType() & $TypeMasks::PlayerObjectType)
				%point = %obj.getEyePoint();
			%ray = containerRayCast(%center, %point, $TypeMasks::FxBrickObjectType, %player);
			if(!isObject(%ray) && %player.isWithinView(%point) && (%obj.getDataBlock().isBlood || %obj.isDead || %obj.bloody || (isObject(%img = %obj.getMountedImage(0)) && %img.isWeapon)))
			{
				%scream = true;
				break;
			}
		}

		if (!%scream || $Sim::Time - %player.lastScream < 3)
			return;
		//todo: screaming when seeing fucked up shit
		%player.lastScream = $Sim::Time;
		%player.playShock();
	}
}

package DespairVoice
{
	function Player::playPain(%player)
	{
		%gender = %player.gender;
		if(%gender $= "")
			%gender = "male";
		%player.stopAudio(0);
		%player.playAudio(0, VoicePain @ getRandom(1,$painCount[%gender]) @ %gender);
	}
	function Player::playDeathCry(%player)
	{
		%gender = %player.gender;
		if(%gender $= "")
			%gender = "male";
		%player.stopAudio(0);
		%player.playAudio(0, VoiceDeath @ getRandom(1,$deathCount[%gender]) @ %gender);
	}
};
activatePackage("DespairVoice");