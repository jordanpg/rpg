if(isObject(TestAbility))
	TestAbility.delete();
new ScriptObject(TestAbility)
{
	class = "RPGAbility";

	abilityCooldown = 1;
	abilityName = "Test";

	phase0 = "Init";
	phaseCall0 = true;
	phaseTime0 = 0;
	phaseNext0 = 1;

	phase1 = "Action";
	phaseCall1 = true;
	phaseTime1 = 1000;
	phaseNext1 = -1;

	phases = 2;

	debug = true;
};

function TestAbility::Init(%this, %obj, %slot)
{
	echo("Test ability activated!");
	echo("  +-object" SPC %obj SPC "... slot" SPC %slot);
}

function TestAbility::Action(%this, %obj, %slot)
{
	echo("Test ability action!");
	echo("  +-object" SPC %obj SPC "... slot" SPC %slot);
}



addRPGDamageType("Sword Spin", $DamageType::RPGMaterial, "MATERIAL	AoE	SPECIAL	SWORD	MELEE	MATMELEE");

$RPG::Ability::SwordSpinCt = 2;
$RPG::Ability::SwordSpinStep = 15;
$RPG::Ability::SwordSpinRadius = (2 * 0.025);
$RPG::Ability::SwordSpinHitRange = 0.05;
$RPG::Ability::SwordSpinDamage = 50;
$RPG::Ability::SwordSpinDamageTimeout = (500 / 1000);
if(isObject(SwordSpinAbility))
	SwordSpinAbility.delete();
new ScriptObject(SwordSpinAbility)
{
	class = "RPGAbility";

	abilityCooldown = 1;
	abilityName = "Spin Attack";

	phase0 = "Init";
	phaseCall0 = true;
	phaseTime0 = 0;
	phaseNext0 = 1;

	phase1 = "Action";
	phaseCall1 = true;
	phaseTime1 = 20;
	phaseNext1 = 1;

	phases = 2;
};

function SwordSpinAbility::Init(%this, %obj, %slot)
{
	if(isObject(%obj.aShape[%slot]))
		%obj.aShape[%slot].delete();

	%obj.aShape[%slot] = new StaticShape()
							{
								datablock = RPGSwordShapeData;
								player = %obj;
							};
	%obj.aShape[%slot].setNodeColor("ALL", swordImage.colorShiftColor);
}

function SwordSpinAbility::Action(%this, %obj, %slot)
{
	%shape = %obj.aShape[%slot];

	if(%shape.cAngle $= "")
	{
		%shape.cAngle = 0;
		%shape.cSpins = 0;
	}

	%x = $RPG::Ability::SwordSpinRadius * mRadToDeg(mCos(mDegToRad(%shape.cAngle)));
	%y = $RPG::Ability::SwordSpinRadius * mRadToDeg(mSin(mDegToRad(%shape.cAngle)));

	%pos = VectorAdd(%obj.getHackPosition(), %x SPC %y SPC 0);
	%rot = eulerToAxis("0 90" SPC %shape.cAngle);
	%shape.setTransform(%pos SPC %rot);

	%shape.cAngle += $RPG::Ability::SwordSpinStep;
	%shape.cSpins += ($RPG::Ability::SwordSpinStep / 360);

	if(%shape.cSpins >= $RPG::Ability::SwordSpinCt)
	{
		%this.aPhase(%obj, -1);
		return;
	}

	if(%shape.cAngle >= 360)
	{
		%shape.cAngle -= 360;
		%shape.cCompleteSpins++;
	}

	initContainerRadiusSearch(%pos, $RPG::Ability::SwordSpinHitRange, $TypeMasks::PlayerObjectType);
	while(isObject(%o = containerSearchNext()))
	{
		if(nameToID(%o) == nameToID(%obj))
			continue;

		if(rpgCanDamage(%obj, %o) == 1 && $Sim::Time - %o.aSpinLast > $RPG::Ability::SwordSpinDamageTimeout)
		{
			rpgDamage(%obj, %o, $RPG::Ability::SwordSpinDamage, "Sword Spin");
			%o.aSpinLast = $Sim::Time;
		}
	}
}

function SwordSpinAbility::onInactive(%this, %obj, %slot)
{
	if(isObject(%obj.aShape[%slot]))
	{
		%obj.aShape[%slot].delete();
		%obj.aShape[%slot] = "";
	}
	parent::onInactive(%this, %obj, %slot);
}

function serverCmdSpinDebug(%this)
{
	if(!isObject(%this.player))
		return;

	SwordSpinAbility.aActivate(%this.player, 0);
}