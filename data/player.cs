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

	aiCall_Death = true;
};

datablock PlayerData(PlayerButtAI : PlayerDumbAI)
{
	aiBehaviour3 = bAButt;
	aiBehaviourArg3_0 = 3;

	aiBehaviours = 4;
};

function PlayerDumbAI::RPG_onDeath(%this, %obj, %killer, %data, %pos, %dam, %type)
{
	RPGAI_New(PlayerDumbAI);

	if(isObject(%killer.client))
		%killer.client.bottomPrint("\c6Shame on you. He wasn\'t hurtin\' nobody. :(", 3, 1);
 }