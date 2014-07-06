function battleGroup_New()
{
	if(isObject(BattleGroup))
		return BattleGroup;

	%this = new SimGroup(BattleGroup);
	return %this;
}

function battleSO_New(%label, %solver, %players)
{
	if(!isObject(%solver) || %solver.getClassName() !$= "ScriptObject" || %solver.class !$= "BattleSolverSO")
		return -1;

	if(!isObject(BattleGroup))
		battleGroup_New();

	%this = new ScriptObject("battle" @ %label)
			{
				class = "BattleSO";
				solver = %solver;

				label = %label;
				players = 0;
			};
	%this.addPlayers(%players);

	return %this;
}

function BattleSO::addPlayers(%this, %list)
{
	if((%ct = getFieldCount(%list)) < 1)
		return 0;

	for(%i = 0; %i < %ct; %i++)
	{
		%ent = getField(%list, %i);
		if(!isObject(%ent) || %ent.getClassName() !$= "GameConnection")
			continue;

		%this.player[%this.players] = %ent.getID();
		%this.players++;
	}

	return %this.players;
}

function BattleSO::hasPlayer(%this, %client)
{
	if(!isObject(%client) || %client.getClassName() !$= "GameConnection")
		return false;

	%id = %client.getID();
	for(%i = 0; %i < %this.players; %i++)
	{
		%p = %this.player[%i];
		if(!isObject(%p))
			continue;

		if(%p == %id)
			return true;
	}
	return false;
}

function BattleSO::getPlayerCount(%this)
{
	return %this.players;
}

function BattleSO::getPlayer(%this, %i)
{
	return %this.player[%i];
}

//RPG TURNBASED