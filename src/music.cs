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

function ServerStopSong()
{
	if(isObject(ServerMusic))
	{
		ServerMusic.delete();
	}
}

function ServerPlaySong(%profile, %loop)
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
		isLooping = %loop;
	};
}