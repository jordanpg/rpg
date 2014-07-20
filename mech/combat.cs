$RPG::Hitnumbers = true;
$RPG::Hitnumbers::MaxPerEntity = 10;
$RPG::Hitnumbers::Fadeout = 0.05;
$RPG::Hitnumbers::Speed = 33;
$RPG::Hitnumbers::Lifetime = 1.5;
$RPG::Hitnumbers::MaxVel = "0.5 0.5 1.5";
$RPG::Hitnumbers::MinVel = "-0.5 -0.5 0.2";
$RPG::Hitnumbers::Gravity = 0.03;
$RPG::Hitnumbers::Drag = 0.75;
$RPG::Hitnumbers::Dist = 50;

addDamageType("RPGGeneric", '<bitmap:base/client/ui/CI/generic> %1', '%2 <bitmap:base/client/ui/CI/generic> %1', 0.2, 1);
addDamageType("RPGMaterial", '<bitmap:base/client/ui/CI/hammer> %1', '%2 <bitmap:base/client/ui/CI/hammer> %1', 0.2, 1);
addDamageType("RPGMagic", '<bitmap:base/client/ui/CI/skull> %1', '%2 <bitmap:base/client/ui/CI/skull> %1', 0.2, 1);

function rpgCanDamage(%obj, %obj2)
{
	if(%obj2.rpgAI || %obj.rpgAI)
		return true;

	return false;
}

function addRPGDamageType(%name, %type, %tags, %atags, %dtags)
{
	$RPG::DamageType[%name] = %type;
	$RPG::DamageATags[%name] = %atags;
	$RPG::DamageDTags[%name] = %dtags;
	$RPG::DamageTags[%name] = %tags;
}

addRPGDamageType("Generic", $DamageType::RPGGeneric);
addRPGDamageType("GenericMaterial", $DamageType::RPGMaterial);
addRPGDamageType("GenericMagic", $DamageType::RPGMagic);

function rpgSolveTags(%ctA, %ctD, %att, %def, %dam, %dtype)
{
	//solve attack effects first
	%ct = getFieldCount(%att);
	for(%i = 0; %i < %ct; %i++)
	{
		%w = false;

		%tag = getField(%att, %i);

		%type = firstWord(%tag);
		%ops = restWords(%tag);

		if(%type !$= "ALL" && striPos(%ctD, %type) == -1)
			continue;

		%n = eval("%r = %dam" SPC %ops @ ";" NL "%w = true;" NL "return %r;");
		if(!%w)
			continue;
		%dam = %n;
	}

	//solve defence effects next
	%ct = getFieldCount(%def);
	for(%i = 0; %i < %ct; %i++)
	{
		%w = false;

		%tag = getField(%def, %i);

		%type = firstWord(%tag);
		%ops = restWords(%tag);

		if(%type !$= "ALL" && striPos(%ctA, %type) == -1 && striPos($RPG::DamageTags[%dtype], %type) == -1)
			continue;

		%n = eval("%r = %dam" SPC %ops @ ";" NL "%w = true;" NL "return %r;");
		if(!%w)
			continue;
		%dam = %n;
	}

	return %dam;
}

function rpgDamage(%obj, %obj2, %base, %type)
{
	if($RPG::DamageType[%type] $= "")
		%type = "Generic";

	%r = minigameCanDamage(%obj, %obj2);
	if(%r != 1)
		return;

	%damage = %base;

	%att = %obj.getAttackTags(%type);
	%def = %obj2.getDefenceTags(%type);
	%c1 = %obj.getCharacterTags();
	%c2 = %obj2.getCharacterTags();

	%damage = rpgSolveTags(%c1, %c2, %att, %def, %base, %type);

	if(%obj.rpgDebug || %obj2.rpgDebug)
	{
		echo("Damage from" SPC %obj SPC "to" SPC %obj2);
		echo("   +-damage" SPC %base SPC "-->" SPC %damage);
		echo("   +-attack" SPC %att);
		echo("   +-defence" SPC %def);
		echo("   +-char1" SPC %c1);
		echo("   +-char2" SPC %c2);
	}

	%obj2.damage(%obj, %obj2.getTransform(), %damage, $RPG::DamageType[%type]);
}

function Player::buildCharacterTags(%this)
{
	return %this.cTags;
}

function Player::buildAttackTags(%this, %type)
{
	%us = %this.aTags;
	%type = $RPG::DamageATags[%type];

	return trim(%us TAB %type);
}

function Player::buildDefenceTags(%this, %type)
{
	%us = %this.dTags;
	%type = $RPG::DamageDTags[%type];

	return trim(%us TAB %type);
}

function Player::getCharacterTags(%this)
{
	%tags = %this.buildCharacterTags();

	return %tags;
}

function Player::getAttackTags(%this, %type)
{
	%tags = %this.buildAttackTags(%type);

	%ops = "*/ +-";
	%new = "";

	%ct = getFieldCount(%tags);
	for(%o = 0; %o < 2; %o++)
	{
		%op = getWord(%ops, %o);

		for(%i = 0; %i < %ct; %i++)
		{
			%tag = getField(%tags, %i);
			if(stripChars(%tag, %op) $= %tag)
				continue;

			%new = %new TAB %tag;
		}
	}

	return trim(%new);
}

function Player::getDefenceTags(%this, %type)
{
	%tags = %this.buildDefenceTags(%type);

	%ops = "*/ +-";
	%new = "";

	%ct = getFieldCount(%tags);
	for(%o = 0; %o < 2; %o++)
	{
		%op = getWord(%ops, %o);

		for(%i = 0; %i < %ct; %i++)
		{
			%tag = getField(%tags, %i);
			if(stripChars(%tag, %o) !$= %tag)
				continue;

			%new = %new TAB %tag;
		}
	}

	return trim(%new);
}

package RPG_Combat
{
	function minigameCanDamage(%obj, %obj2)
	{
		if($RPG::Enabled)
		{
			%r = rpgCanDamage(%obj, %obj2);
			if(%r) return 1;
		}
			
		return parent::minigameCanDamage(%obj, %obj2);
	}

	function Armor::onAdd(%this, %obj)
	{
		parent::onAdd(%this, %obj);

		%obj.cTags = trim(%this.rpgCTags TAB (%obj.getClassName() $= "Player" ? "PLAYER" : "AI"));
		%obj.aTags = %this.rpgATags;
		%obj.dTags = %this.rpgDTags;
	}
};
activatePackage(RPG_Combat);


package RPG_Hitnumbers
{
	function Player::addHitnumber(%this, %val)
	{
		if(!isObject(GlobalHitNumberGroup))
			new SimGroup(GlobalHitNumberGroup);

		%g = %this.hitNumbers;
		if(!isObject(%this.hitNumbers))
		{
			%g = %this.hitNumbers = new SimGroup()
									{
										hitNumbers = true;
									};

			GlobalHitNumberGroup.add(%g);
		}

		if(%g.getCount() > $RPG::Hitnumbers::MaxPerEntity)
			%g.getObject(0).delete();

		%max = %this.getDatablock().maxDamage;
		if(%max == 0)
			%perc = 0;
		else
			%perc = %val / %max;

		%col = HSVtoRGB(%perc-0.1, 1, 1);

		%shape = new StaticShape()
				{
					datablock = RPGEmptyShape;
					position = %this.getPosition();

					hitNumber = true;
					owner = %this;
					val = %val;
					velocity = getRandomVect($RPG::Hitnumbers::MaxVel, $RPG::Hitnumbers::MinVel);
					spawnTime = $Sim::Time;
					fade = 1;
				};
		%g.add(%shape);

		%shape.setShapeName(%val);
		%shape.setShapeNameColor(%col);
		%shape.setShapeNameDistance($RPG::Hitnumbers::Dist);

		GlobalHitNumberGroup.hitNumberTick();

		return %shape;
	}

	function SimGroup::hitNumberTick(%this)
	{
		if(isEventPending(%this.tick))
			cancel(%this.tick);

		%ct = %this.getCount();

		if(%ct == 0)
			return;

		for(%i = 0; %i < %this.getCount(); %i++)
		{
			%obj = %this.getObject(%i);

			if(!isObject(%obj)) continue;
			
			%obj.hitNumber();
		}

		%this.tick = %this.schedule($RPG::Hitnumbers::Speed, hitNumberTick);
	}

	function SimGroup::hitNumber(%this)
	{
		%ct = %this.getCount();

		if(%ct == 0)
		{
			%this.delete();
			return;
		}

		for(%i = 0; %i < %this.getCount(); %i++)
		{
			%obj = %this.getObject(%i);

			if(!isObject(%obj)) continue;

			%obj.hitNumber();
		}
	}

	function StaticShape::hitNumber(%this)
	{
		if(!%this.hitNumber)
			return;

		%this.position = VectorAdd(%this.getPosition(), %this.velocity);

		%this.velocity = VectorSub(VectorScale(%this.velocity, $RPG::Hitnumbers::Drag), "0 0" SPC $RPG::Hitnumbers::Gravity);

		if($Sim::Time - %this.spawnTime > $RPG::Hitnumbers::Lifetime)
		{
			%dist = %this.fade * $RPG::Hitnumbers::Dist;
			%this.setShapeNameDistance(%dist);

			%this.fade -= $RPG::Hitnumbers::Fadeout;
		}

		if(%this.fade <= 0)
		{
			%this.delete();
			return;
		}

		%this.setTransform(%this.position);
	}

	function Armor::damage(%this, %obj, %source, %pos, %dam, %type)
	{
		%obj.addHitnumber(%dam);

		parent::damage(%this, %obj, %source, %pos, %dam, %type);
	}

	function Armor::onRemove(%this, %obj)
	{
		if(isObject(%this.hitNumbers))
			%this.hitNumbers.delete();

		parent::onRemove(%this, %obj);
	}
};
if($RPG::Hitnumbers)
	activatePackage(RPG_Hitnumbers);

function RPG_SetHitnumbers(%val)
{
	if(%val)
	{
		$RPG::Hitnumbers = true;
		echo("RPG Hitmarkers turned ON");

		activatePackage(RPG_Hitnumbers);
	}
	else
	{
		$RPG::Hitnumbers = false;
		echo("RPG Hitnumbers turned OFF");

		deactivatePackage(RPG_Hitnumbers);
	}
}