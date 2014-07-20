datablock PlayerData(PlayerDumbAI : PlayerStandardArmor)
{
	uiName = "";

	aiBehaviour0 = bShimmy;
	aiBehaviourArg0_0 = 10;
	aiBehaviourArg0_1 = 0.5;

	aiBehaviour1 = bLeapFrog;
	aiBehaviourArg1_0 = 1;

	aiBehaviour2 = bEmoteSpam;

	aiBehaviours = 3;
};

datablock PlayerData(PlayerButtAI : PlayerDumbAI)
{
	aiBehaviour3 = bAButt;
	aiBehaviourArg3_0 = 3;

	aiBehaviours = 4;
};