function rpgCanDamage(%obj, %obj2)
{
	if(%obj2.rpgAI || %obj.rpgAI)
		return true;

	return false;
}

function addRPGDamageType(%name, %type, %ops)
{
	$RPG::DamageType[%name] = %type;
	$RPG::DamageOps[%name] = %ops;
}

function rpgDamage(%obj, %obj2, %type, %base)
{
	if($RPG::DamageType[%type] !$= "")
		%type = $RPG::DamageType[%type];

	%r = minigameCanDamage(%obj, %obj2);
	if(%r != 1)
		return;

	%damage = %base;

	%obj2.damage(%obj, %obj2.getTransform(), %damage, %type);
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

		%obj.cTags = %this.rpgCTags TAB (%obj.getClassName() $= "Player" ? "PLAYER" : "AI");
		%obj.aTags = %this.rpgATags;
		%obj.dTags = %this.rpgDTags;
	}
};
activatePackage(RPG_Combat);