function RPGAbility::aActivate(%this, %obj, %slot)
{
	if(!isObject(%obj) || (%obj.getClassName() !$= "Player" && %obj.getClassName() !$= "AIPlayer")) //do we even need AI to do this? i guess so.
		return false;

	%id = nameToID(%this);

	if($Sim::Time <= %obj.aCooldownEnd[%id] || %obj.aCooldownEnd[%id] < 0)
	{
		%this.onCooldownFail(%obj, %slot);
		return;
	}

	if(isObject(%ability = %obj.getActiveAbility(%slot)))
		%ability.aDeactivate(%obj, %slot);

	%obj.ability[%slot] = %id;
	%obj.abilityPhase[%slot] = 0;
	%obj.abilitySlot[%id] = %slot;
	%obj.setAbilityCooldown(%this, %this.abilityCooldown);

	%this.aPhase(%obj, 0);

	%this.onActive(%obj, %slot);

	return true;
}

function RPGAbility::aDeactivate(%this, %obj, %slot)
{
	if(!isObject(%obj) || (%obj.getClassName() !$= "Player" && %obj.getClassName() !$= "AIPlayer"))
		return false;

	%id = nameToID(%this);
	// echo("bloop" SPC %this SPC %id SPC %obj.ability[%slot]);
	if(%obj.ability[%slot] != %id)
		return false;

	// echo("blop");
	%obj.abilitySlot[%id] = "";
	%obj.ability[%slot] = "";
	%obj.abilityPhase[%slot] = "";
	%obj.abilityTransition[%slot] = "";

	%this.aPhase(%obj, -1);

	%this.onInactive(%obj, %slot);

	return true;
}

function RPGAbility::aCallPhase(%this, %obj, %phase)
{
	%slot = %obj.abilitySlot[nameToID(%this)];
	if(%slot $= "")
		return false;

	%func = %this.phase[%phase];
	if(!isFunction(%this.getName(), %func))
	{
		warn("RPGAbility::aCallPhase - Could not find function for phase" SPC %phase);
		return false;
	}

	%this.call(%func, %obj, %slot);

	return true;
}

function RPGAbility::aPhase(%this, %obj, %phase)
{
	%slot = %obj.abilitySlot[nameToID(%this)];
	if(%slot $= "")
		return false;

	// echo("yee" SPC %phase);
	if(%phase < 0)
	{
		%this.aDeactivate(%obj, %slot);
		return true;
	}

	if(%this.phase[%phase] $= "")
	{
		warn("RPGAbility::aPhase - Phase" SPC %phase SPC "for ability" SPC %this.getName() SPC "does not exist!");
		return false;
	}

	if(!isObject(%obj) || (%obj.getClassName() !$= "Player" && %obj.getClassName() !$= "AIPlayer")) //do we even need AI to do this? i guess so.
		return false;

	%obj.abilityPhase[%slot] = %phase;

	if(%this.phaseCall[%phase])
		%this.aCallPhase(%obj, %phase);

	%this.aPhaseTransition(%obj, %phase);

	%this.onPhase(%obj, %slot, %phase);

	return true;
}

function RPGAbility::aNextPhase(%this, %obj)
{
	if(!isObject(%obj) || (%obj.getClassName() !$= "Player" && %obj.getClassName() !$= "AIPlayer")) //do we even need AI to do this? i guess so.
		return false;

	%slot = %obj.abilitySlot[nameToID(%this)];
	if(%slot $= "")
		return false;

	if(isEventPending(%obj.abilityTransition[%slot]))
		cancel(%obj.abilityTransition[%slot]);

	%phase = %obj.abilityPhase[%slot];
	
	%next = %this.phaseNext[%phase];
	if(%next $= "")
		return false;

	%s = %this.aPhase(%obj, %next);
}

function RPGAbility::aPhaseTransition(%this, %obj, %phase)
{
	%slot = %obj.abilitySlot[nameToID(%this)];
	if(%slot $= "")
		return false;

	if(isEventPending(%obj.abilityTransition[%slot]))
		cancel(%obj.abilityTransition[%slot]);

	if(%this.phaseNext[%phase] $= "")
	{
		warn("RPGAbility::aPhaseTransition - No transition defined for phase" SPC %phase);
		return false;
	}

	if(!isObject(%obj) || (%obj.getClassName() !$= "Player" && %obj.getClassName() !$= "AIPlayer")) //do we even need AI to do this? i guess so.
		return false;

	%time = %this.phaseTime[%phase];
	if(%time < 0)
		return true; //negative number means we don't want to handle transitions automatically

	%obj.abilityTransition[%slot] = %this.scheduleNoQuota(%time, aNextPhase, %obj);
	// echo("tranasdg");

	return isEventPending(%obj.abilityTransition[%slot]);
}

function RPGAbility::onAdd(%this)
{
	echo("uh");
	if(%this.phases <= 0)
	{
		warn("RPGAbility::onAdd -" SPC %this SPC "has no phases and will not work, removing...");
		%this.delete();
		return;
	}

	if(!isObject(RPGAbilityGroup))
		new SimGroup(RPGAbilityGroup);

	RPGAbilityGroup.add(%this);
}

function RPGAbility::onActive(%this, %obj, %slot)
{
	if(%this.debug || %obj.rpgDebug)
		echo("Ability active -" SPC %this SPC "on" SPC %obj SPC "in slot" SPC %slot);

	//pass
}

function RPGAbility::onPhase(%this, %obj, %slot, %phase)
{
	if(%this.debug || %obj.rpgDebug)
		echo("Ability phase" SPC %phase SPC "-" SPC %this SPC "on" SPC %obj SPC "in slot" SPC %slot);

	//pass
}

function RPGAbility::onInactive(%this, %obj, %slot)
{
	if(%this.debug || %obj.rpgDebug)
		echo("Ability inactive -" SPC %this SPC "on" SPC %obj SPC "in slot" SPC %slot);

	//pass
}

function RPGAbility::onCooldownFail(%this, %obj, %slot)
{
	if(%this.debug || %obj.rpgDebug)
		echo("Ability uncool -" SPC %this SPC "on" SPC %obj SPC "in slot" SPC %slot);

	//pass
}

function RPGAbility::onCooldownEnd(%this, %obj, %slot)
{
	if(%this.debug || %obj.rpgDebug)
		echo("Ability cooled -" SPC %this SPC "on" SPC %obj SPC "in slot" SPC %slot);

	//pass
}

function RPGAbility::onCooldownStart(%this, %obj, %slot, %time)
{
	if(%this.debug || %obj.rpgDebug)
		echo("Ability heated -" SPC %this SPC "on" SPC %obj SPC "in slot" SPC %slot SPC "for" SPC %time SPC "seconds");

	//pass
}

function isValidAbility(%obj)
{
	if(!isObject(%obj))
		return false;

	if(%obj.getClassName() !$= "ScriptObject")
		return false;

	if(%obj.class !$= "RPGAbility" && %obj.superClass !$= "RPGAbility")
		return false;

	return true;
}

function Player::activateAbility(%this, %obj, %slot)
{
	if(!isValidAbility(%obj))
		return false;

	return %obj.aActivate(%this, %slot);
}

function Player::getActiveAbility(%this, %slot)
{
	return (isObject(%this.ability[%slot]) ? %this.ability[%slot] : -1);
}

function Player::getAbilitySlot(%this, %obj)
{
	if(!isValidAbility(%obj))
		return -1;

	%id = nameToID(%obj);

	%slot = %this.abilitySlot[%id];
	if(%slot !$= "")
		return %slot;

	return -1;
}

function Player::setAbilityCooldown(%this, %obj, %time, %slot)
{
	if(!isValidAbility(%obj))
		return false;

	%id = nameToID(%obj);

	if(%slot $= "")
		%slot = %this.getAbilitySlot(%id);

	%this.aCooldownStart[%id] = $Sim::Time;
	%this.aCooldownEnd[%id] = (%time > 0 ? $Sim::Time + %time : -1);

	if(%time > 0)
		%this.aCooldown[%id] = %obj.schedule(%time * 1000, onCooldownEnd, %this, %slot);

	%obj.onCooldownStart(%this, %slot, %time);

	return true;
}