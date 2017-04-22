//Contains all 2d sound effects as well as music
datablock AudioProfile(AnnouncementSound)
{
	fileName = $Despair::Path @ "res/sounds/dingdong.wav";
	description = audio2D;
	preload = true;
};

datablock AudioProfile(KillerJingleSound)
{
	fileName = $Despair::Path @ "res/sounds/dundun.wav";
	description = audio2D;
	preload = true;
};

datablock AudioProfile(BodyDiscoverySound1)
{
	fileName = $Despair::Path @ "res/music/BodyDiscoveryNoise1.ogg";
	description = audio2D;
	preload = true;
};

datablock AudioProfile(BodyDiscoverySound2)
{
	fileName = $Despair::Path @ "res/music/BodyDiscoveryNoise2.ogg";
	description = audio2D;
	preload = true;
};
datablock AudioProfile(MusicGameStart)
{
	fileName = $Despair::Path @ "res/music/gameStart.ogg";
	description = Audio2D;
	preload = true;
};
datablock AudioProfile(MusicInvestigationStart)
{
	fileName = $Despair::Path @ "res/music/investigationStart.ogg";
	description = Audio2D;
	preload = true;
};
datablock AudioProfile(MusicVoteStart)
{
	fileName = $Despair::Path @ "res/music/VoteStart.ogg";
	description = Audio2D;
	preload = true;
};
datablock AudioProfile(MusicOpeningPre)
{
	fileName = $Despair::Path @ "res/music/OpeningPre.ogg";
	description = Audio2D;
	preload = true;
};
datablock AudioProfile(MusicOpeningStatements)
{
	fileName = $Despair::Path @ "res/music/OpeningStatements.ogg";
	description = AudioLooping2D;
	preload = true;
};
datablock AudioProfile(MusicTrialDiscussion)
{
	fileName = $Despair::Path @ "res/music/TrialDiscussion.ogg";
	description = AudioLooping2D;
	preload = true;
};

datablock AudioProfile(MusicInvestigationIntro1)
{
	fileName = $Despair::Path @ "res/music/investigationintro1.ogg";
	description = Audio2D;
	preload = true;
	loopStart = 15688;
	loopProfile = MusicInvestigationLoop1;
};
datablock AudioProfile(MusicInvestigationLoop1)
{
	fileName = $Despair::Path @ "res/music/investigationloop1.ogg";
	description = AudioLooping2D;
	preload = true;
};

function ServerStopSong()
{
	if(isObject(ServerMusic))
	{
		cancel(ServerMusic.loopSchedule);
		ServerMusic.delete();
	}
}

function ServerPlaySong(%profile)
{
	ServerStopSong();
	new AudioEmitter(ServerMusic)
	{
		position = "0 0 0";
		profile = %profile;
		useProfileDescription = 1;
		description = "AudioLooping2D";
		type = "0";
		volume = "1.5";
		outsideAmbient = "1";
		ReferenceDistance = "4";
		maxDistance = "9001";
		isLooping = 0;
	};
	if(isObject(%profile.loopProfile))
		ServerMusic.loopSchedule = schedule(%profile.loopStart, 0, ServerPlaySong, %profile.loopProfile);
}