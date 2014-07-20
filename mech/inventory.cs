$RPG::Inventory::BaseSlots = 25;

function RPGData::applyInventory(%this, %slots)
{
	%s = 0;
	for(%i = 0; %i < %slots; %i++)
	{
		%n = "inv" @ %i;

		if(%this.hasVal(%n))
			continue;

		%this.setVal(%n, "NONE\t0");
		%s++;
	}

	%ct = %this.getVal("invSize");
	if(%slots > %ct)
		%this.setVal("invSize", %slots);

	return %s;
}

function RPGData::setInv(%this, %slot, %val, %num)
{
	if(%slot < 0)
		return;

	%n = "inv" @ (%slot + 0);

	%this.setVal(%n, %val TAB %num);
}

function RPGData::searchInv(%this, %val)
{
	%ct = %this.getVal("invSize");
	for(%i = 0; %i < %ct; %i++)
	{
		%v = %this.getVal("inv" @ %i);
		if(%v $= %val)
			return %i;
	}
	return -1;
}

function RPGData::getInv(%this, %slot)
{
	if(%slot < 0)
		return;

	%n = "inv" @ (%slot + 0);

	%i = %this.getVal(%n);

	if(%i $= "")
		return -1;

	return %i;
}

function RPGData::getInvContent(%this, %slot)
{
	%i = %this.getInv(%slot);

	return getField(%i, 0);
}

function RPGData::getInvNumber(%this, %slot)
{
	%i = %this.getInv(%slot);

	return getField(%i, 1);
}

function RPGData::listInventory(%this)
{
	%ct = %this.getVal("invSize");
	for(%i = 0; %i < %ct; %i++)
	{
		%v = %this.getVal("inv" @ %i);
		echo("SLOT" SPC %i SPC ":" SPC strReplace(%v, "\t", ", "));
	}
	return -1;
}