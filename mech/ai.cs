$RPG::AISpeed = 1000;
$RPG::AIDebugLoop = false;

//RPGAIGroup_Init()
//Creates & initialises the RPGAIGroup object
//Returns a SimGroup
function RPGAIGroup_Init()
{
	if(isObject(RPGAIGroup))
		return RPGAIGroup;

	$RPG::AIGroup = new SimGroup(RPGAIGroup);

	return $RPG::AIGroup;
}

//RPGAI_New(%datablock[, %transform][, %outfit][, %debug])
//Creates a new AIPlayer suitable for RPG use
//%datablock:	PlayerData datablock for the new AIPlayer
//%transform:	Transform for the new AIPlayer (defaults to 0 0 0 0 0 0 0)
//%outfit	:	Outfit file for the new AIPlayer
//%debug	:	Sets the debug flag on the AIPlayer
//Returns -1 or a new AIPlayer
function RPGAI_New(%datablock, %transform, %outfit, %debug)
{
	if(!DataBlockGroup.isMember(%datablock) || %datablock.getClassName() !$= "PlayerData")
		return -1;

	if(!isObject(RPGAIGroup))
		RPGAIGroup_Init();

	if(%transform $= "")
		%transform = "0 0 0 0 0 0 0";

	%this = new AIPlayer()
			{
				datablock = %datablock;
				// client = CreateAIClient();
				position = "0 0 0";
				rotation = "0 0 0 0";

				rpgAI = true;
				behaviours = 0;

				debug = %debug;
			};
	missionCleanup.add(%this);
	RPGAIGroup.add(%this);

	%this.setTransform(%transform);
	%this.applyOutfit(%outfit);
	%this.client.player = %this;

	%this.aiOrigin = %this.getPosition();

	if(%datablock.aiBehaviours)
	{
		for(%i = 0; %i < %datablock.aiBehaviours; %i++)
		{
			%name = %datablock.aiBehaviour[%i];
			for(%j = 0; %j < 16; %j++)
				%a[%j] = %datablock.aiBehaviourArg[%i, %j];

			%this.RPG_addBehaviour(%name, %a0, %a1, %a2, %a3, %a4, %a5, %a6, %a7, %a8, %a9, %a10, %a11, %a12, %a13, %a14, %a15);
		}
	}

	%this.RPG_loop();

	return %this;
}

//AIPlayer::RPG_getLoopSpeed(%this)
//Returns the speed at which the AIPlayer's loop iterates
//%this		:	AIPlayer object
//Returns a time in MS
function AIPlayer::RPG_getLoopSpeed(%this)
{
	if(%this.loopSpeed > 0)
		return %this.loopSpeed;

	%db = %this.getDatablock();
	if(%db.aiLoopSpeed > 0)
		return %db.aiLoopSpeed;

	return $RPG::AISpeed;
}

//AIPlayer::RPG_addBehaviour(%this, %name[, %a0][, %a1] ... [, %a15])
//Adds a behaviour to the AIPlayer
//%this		:	AIPlayer object
//%name		:	Name of the behaviour to add (Must be a method of AIPlayer)
//%a0-%a15	:	Arguments for the behaviour
//Returns a boolean
function AIPlayer::RPG_addBehaviour(%this, %name, %a0, %a1, %a2, %a3, %a4, %a5, %a6, %a7, %a8, %a9, %a10, %a11, %a12, %a13, %a14, %a15)
{
	if(!%this.rpgAI)
		return false;

	if(!isFunction(AIPlayer, %name))
	{
		echo("AIPlayer::RPG_addBehaviour - Failed to add non-existent behaviour \'" @ %name @ "\' to AIPlayer" SPC %this);
		return false;
	}

	if(%this.debug)
	{
		echo("Adding behaviour to AIPlayer" SPC %this SPC "...");
		echo("   +-Behaviour name is \'" SPC %name SPC "\'");
	}

	if((%idx = %this.behaviourIdx[%name]) !$= "")
	{
		if(%this.debug)
			echo("   +-Behaviour already on AI, modifying arguments");

		for(%i = 0; %i < 16; %i++)
		{
			%diff = (%this.behaviourArg[%idx, %i] !$= %a[%i]);
			%this.behaviourArg[%idx, %i] = %a[%i];
			if(%this.debug && %a[%i] !$= "")
				echo("   +-" @ (%diff ? "new " : "") @ "a" @ %i SPC " : " SPC %a[%i]);
		}
		%this.behaviourActive[%idx] = true;

		return true;
	}

	%this.behaviour[%this.behaviours] = %name;
	for(%i = 0; %i < 16; %i++)
	{
		%this.behaviourArg[%this.behaviours, %i] = %a[%i];
		if(%this.debug && %a[%i] !$= "")
			echo("   +-a" @ %i SPC " : " SPC %a[%i]);
	}
	%this.behaviourActive[%name] = true;

	%this.behaviourIdx[%name] = %this.behaviours;

	%this.behaviours++;

	return true;
}

//AIPlayer::RPG_setBehaviourActive(%this, %name, %val)
//Changes the active state of a behaviour on the AI.
//%this		:	AIPlayer object
//%name		:	Name of the behaviour
//%val		:	Boolean to set the active state to
//Returns a boolean
function AIPlayer::RPG_setBehaviourActive(%this, %name, %val)
{
	%this.behaviourActive[%name] = (%val || 0);
}

//AIPlayer::RPG_loop(%this)
//Looping function to control AI
//%this		:	AIPlayer object
//Returns a schedule
function AIPlayer::RPG_loop(%this)
{
	if(isEventPending(%this.loop))
		cancel(%this.loop);

	%actions = 0;
	for(%i = 0; %i < %this.behaviours; %i++)
	{
		%n = %this.behaviour[%i];
		if(!%this.behaviourActive[%n])
			continue;

		for(%j = 0; %j < 16; %j++)
			%a[%j] = %this.behaviourArg[%i, %j];

		%this.call(%n, %a0, %a1, %a2, %a3, %a4, %a5, %a6, %a7, %a8, %a9, %a10, %a11, %a12, %a13, %a14, %a15);
		%actions++;
	}

	%this.loop = %this.scheduleNoQuota(%this.RPG_getLoopSpeed(), RPG_loop);

	%this.RPG_onLoop(%actions);
}

//ShapeBase::getPositionXY(%this)
//Returns the position of an object with Z axis at zero
//%this		:	ShapeBase object
//Returns a Vector3f
function ShapeBase::getPositionXY(%this)
{
	return getWords(%this.getPosition(), 0, 1) SPC 0;
}

//AIPlayer::RPG_onDamage(%this, %obj, %data, %pos, %dam, %type)
//Callback for when an AI is damaged.
//%this		:	Damaged AIPlayer object
//%obj		:	Damaging object
//%data		:	Datablock of the AIPlayer object
//%pos		:	Position of the damage
//%dam		:	Amount of damage inflicted
//%type		:	Damage type inflicted
function AIPlayer::RPG_onDamage(%this, %obj, %data, %pos, %dam, %type)
{
	if(%this.debug)
	{
		echo("AI Damage -" SPC %this SPC "by" SPC (%class = %obj.getClassName()) SPC %obj);
		echo("   +-data" SPC %data SPC "... pos" SPC %pos SPC "... dam" SPC %dam SPC "... type" SPC %type);
		switch$(%class)
		{
			case "Projectile":
				echo("   +-player" SPC %obj.sourceObject);
				echo("   +-client" SPC %obj.client SPC "... player" SPC %obj.client.player);
			case "Player":
				echo("   +-client" SPC %obj.client);
		}
	}

	%db = %this.getDatablock();
	if(%db.aiCall_Damage)
		%db.RPG_onDamage(%this, %obj, %data, %pos, %dam, %type);

	%this.bLastDamaged = $Sim::Time;
}

//AIPlayer::RPG_onDeath(%this, %obj, %data, %pos, %dam, %type)
//Callback for when an AI dies.
//%this		:	Damaged AIPlayer object
//%obj		:	Damaging object
//%data		:	Datablock of the AIPlayer object
//%pos		:	Position of the damage
//%dam		:	Amount of damage inflicted
//%type		:	Damage type inflicted
function AIPlayer::RPG_onDeath(%this, %obj, %data, %pos, %dam, %type)
{
	if(%this.debug)
	{
		echo("AI Death -" SPC %this SPC "by" SPC (%class = %obj.getClassName()) SPC %obj);
		echo("   +-data" SPC %data SPC "... pos" SPC %pos SPC "... dam" SPC %dam SPC "... type" SPC %type);
		switch$(%class)
		{
			case "Projectile":
				echo("   +-player" SPC %obj.sourceObject);
				echo("   +-client" SPC %obj.client SPC "... player" SPC %obj.client.player);
			case "Player":
				echo("   +-client" SPC %obj.client);
		}
	}

	%db = %this.getDatablock();
	if(%db.aiCall_Death)
		%db.RPG_onDeath(%this, %obj, %data, %pos, %dam, %type);
}

//AIPlayer::RPG_onRemove(%this)
//Callback for when an AI is removed
//%this		:	Removed AIPlayer object
function AIPlayer::RPG_onRemove(%this)
{
	if(%this.debug)
		echo("AI Removal -" SPC %this SPC "at" SPC getSimTime() SPC "ms");

	%db = %this.getDatablock();
	if(%db.aiCall_Remove)
		%db.RPG_onRemove(%this);
}

//AIPlayer::RPG_onLoop(%this, %actions)
//Callback for when an AI loops
//%this		:	Relevant AIPlayer object
//%actions	:	Number of successful behaviour calls
function AIPlayer::RPG_onLoop(%this, %actions)
{
	if(%this.debug && $RPG::AIDebugLoop)
	{
		echo("AI Loop -" SPC %this SPC $Sim::Time);
		if(%actions)
			echo("   +-actions" SPC %actions);
	}

	%db = %this.getDatablock();
	if(%db.aiCall_Loop)
		%db.RPG_onLoop(%this, %actions);
}

//AIPlayer::RPG_onActivate(%this, %obj, %ray)
//Callback for whan an AI is activated
//%this		:	Activated AIPlayer object
//%obj		:	Activating Player object
//%ray		:	Raycast associated with the activation
function AIPlayer::RPG_onActivate(%this, %obj, %ray)
{
	if(%this.debug)
	{
		echo("AI Activation -" SPC %this SPC "by" SPC %obj);
		echo("   +-raycast" SPC %ray);
	}

	%db = %this.getDatablock();
	if(%db.aiCall_Activate)
		%db.RPG_onActivate(%this, %obj, %ray);

	%this.bLastActivated = $Sim::Time;
}

package RPG_AI
{
	function Armor::onRemove(%this, %obj)
	{
		if(%obj.rpgAI)
			%obj.RPG_onRemove();

		// echo("burt");

		parent::onRemove(%this, %obj);
	}

	function Armor::damage(%this, %obj, %source, %pos, %dam, %type)
	{
		parent::damage(%this, %obj, %source, %pos, %dam, %type);

		if(%obj.rpgAI)
		{
			%obj.RPG_onDamage(%source, %this, %pos, %dam, %type);

			if(%obj.getState() $= "Dead")
				%obj.RPG_onDeath(%source, %this, %pos, %dam, %type);
		}
	}

	function Player::activateStuff(%this)
	{
		if(%this.rpgAI && !%this.getDatablock().aiCanActivate)
			return;

		parent::activateStuff(%this);

		// echo("dangus");
		%ray = %this.eyeCast(5, $TypeMasks::PlayerObjectType | $TypeMasks::FxBrickObjectType, %this);
		%obj = firstWord(%ray);
		// echo(%ray NL %obj);
		if(isObject(%obj) && %obj.rpgAI)
		{
			// echo("ya");
			%obj.RPG_onActivate(%this, %ray);
		}
	}
};
activatePackage(RPG_AI);

//AI BEHAVIOURS

//AIPlayer::bBunnyHop(%this)
//Behaviour method causing the AI to jump continuously when $bunnyHop is set to true
//%this		:	AIPlayer object
function AIPlayer::bBunnyHop(%this)
{
	if($bunnyHop)
		%this.setJumping(true);
	else
		%this.setJumping(false);
}

//AIPlayer::bShimmy(%this, %radius, %tol)
//Behaviour method causing the AI to wander about their origin point by %radius TU, acknowledging a reached destination by comparing distance with %tol.
//%this		:	AIPlayer object
//%radius	:	Distance from the centre the AI may wander
//%tol		:	Determines how close the AI must be to the destination to have reached it
function AIPlayer::bShimmy(%this, %radius, %tol)
{
	if(%this.debug)
		echo("bShimmy - call with radius \'" SPC %radius SPC "\' and tolerance \'" SPC %tol SPC "\'");

	%pos = %this.getPositionXY();
	if(%this.bShimmy)
	{
		%dest = %this.getMoveDestination();
		%dist = VectorDist(%pos, %dest);
		if(%dist <= %tol)
		{
			if(%this.debug)
				echo("bShimmy - reached (" SPC %dest SPC ") at (" SPC %pos SPC ") with tolerance \'" SPC %tol SPC "\'");

			%this.bShimmy = false;
			%this.clearMoveDestination();
			%this.emote(loveImage);
			return;
		}
		else
			%this.setMoveDestination(%dest);
	}
	else
	{
		%origin = %this.aiOrigin;
		%originXY = getWords(%origin, 0, 1) SPC 0;
		%min = VectorSub(%originXY, %radius SPC %radius SPC 0);
		%max = VectorAdd(%originXY, %radius SPC %radius SPC 0);
		%dest = getRandomVect(%min, %max);
		if(%this.debug)
			echo("bShimmy - started action to (" SPC %dest SPC ") from (" SPC %min SPC "), (" SPC %max SPC ")");

		%this.bShimmy = true;
		%this.setMoveDestination(%dest);
	}
}

//AIPlayer::bLeapFrog(%this, %len)
//Behaviour causing the AI to attempt to jump over obstacles.
//%this		:	AIPlayer object
//%len		:	Length of the forward raycast and maximum distance from last position to set off the event
function AIPlayer::bLeapFrog(%this, %len)
{
	%dest = %this.getMoveDestination();
	if(VectorLen(%dest) <= 0)
	{
		%this.bLeapFrogLast = "";
		if(%this.bLeapFrog)
		{
			%this.bLeapFrog = false;
			%this.setJumping(false);
		}
		return;
	}

	%pos = %this.getPosition();
	%fwd = %this.getForwardVector();
	%add = VectorScale(%fwd, %len);
	%end = VectorAdd(%pos, %add);

	%ray = ContainerRayCast(%pos, %end, $TypeMasks::FxBrickObjectType | $TypeMasks::PlayerObjectType);
	if(%ray || VectorDist(%this.bLeapFrogLast, %pos) <= %len)
	{
		%this.setJumping(true);

		%this.bLeapFrog = true;
	}
	else if(%this.bLeapFrog)
	{
		%this.setJumping(false);

		%this.bLeapFrog = false;

		%this.spawnExplosion(winStarProjectile, "1 1 1");
	}

	%this.bLeapFrogLast = %pos;
}

//AIPlayer::bEmoteSpam(%this)
//Behaviour causing the AI to spam emotes when interacted with in different ways
//%this		:	AIPlayer object
function AIPlayer::bEmoteSpam(%this)
{
	%speed = (%this.RPG_getLoopSpeed() * 0.0015);
	// echo(%speed);

	//Confusion on damaged
	if(($Sim::Time - %this.bLastDamaged) <= %speed && %this.bEmoteReady)
	{
		%this.emote(WtfImage);
		%this.bEmoteReady = false;
		return;
	}

	//Alarm on activated
	if(($Sim::Time - %this.bLastActivated) <= %speed && %this.bEmoteReady)
	{
		%this.emote(AlarmProjectile);
		%this.bEmoteReady = false;
		return;
	}

	%this.bEmoteReady = true;
}

function AIPlayer::bAButt(%this, %radius)
{
	initContainerRadiusSearch(%this.getPosition(), %radius, $TypeMasks::PlayerObjectType);
	%us = nameToID(%this);
	while(isObject(%obj = containerSearchNext()))
	{
		if(rpgCanDamage(%this, %obj) != 1 || nameToID(%obj) == %us)
			continue;
		%found = true;
		break;
	}

	if(%found)
		%this.activateAbility(SwordSpinAbility, 0);
}