$RPG::DataDir = "config/server/RPG/data/";
$RPG::AutoSave = (60000 * 5);

//RPGData_Init()
//Creates and initialises the RPGDataGroup object, which holds all RPGData objects.
//Returns the RPGDataGroup object
function RPGData_Init()
{
	if(isObject(RPGDataGroup))
		return RPGDataGroup;

	$RPG::Data = new ScriptGroup(RPGDataGroup);
	$RPG::Data.autoSaveTick();

	return $RPG::Data;
}

//RPGDataGroup::addPlayer(%this, %client[, %noimport])
//Creates a new RPGData object or returns an existing one.
//%this		:	RPGDataGroup object
//%client	:	The relevant GameConnection object
//%noimport	:	Flag to avoid importing or creating data on the RPGData object
//Returns -1 or an RPGData object
function RPGDataGroup::addPlayer(%this, %client, %noimport)
{
	if(!isObject(%client))
		return -1;

	%this.schedule(0, updatePlayers);
	%id = %client.getBLID();
	if(isObject(%this.data[%id]))
		return %this.data[%id];

	%data = new ScriptObject("RPGData_" @ %id)
			{
				class = "RPGData";
				client = %client;
				bl_id = %id;
				brickGroup = getBrickGroupFromObject(%client);

				vals = 0;
			};
	%this.data[%id] = %data;
	%data.brickGroup.rpgData = %data;
	%this.add(%data);

	if(!%noimport)
	{
		%r = %data.import();

		if(%r)
			echo("Loaded RPG Data for BL_ID" SPC %id SPC "successfully!");
		else
		{
			%dir = RPGDir("misc/default.txt");
			%r = %data.import(%dir);

			if(%r)
				echo("Created default RPG Data for BL_ID" SPC %id SPC "successfully!");
			else
				error("!!!!!!!!!!!!!!!!Something\'s up!!!!!!!!!!!!!!!!" SPC %dir SPC %id SPC %data);
		}
	}

	return %data;
}

//findRPGData(%bl_id[, %c])
//Finds an RPGData object by its related BL_ID
//%bl_id		:	BL_ID to look for
//%c			:	Flag to do a loop through the RPGDataGroup if the data is not found for a bug case.
//Returns -1 or an RPGData object
function findRPGData(%bl_id, %c)
{
	if(!isObject(RPGDataGroup))
		return;

	if(isObject(%d = RPGDataGroup.data[%bl_id]))
		return %d;
	else if(%c)
	{
		%ct = RPGDataGroup.getCount();
		for(%i = 0; %i < %ct; %i++)
		{
			%d = RPGDataGroup.getObject(%i);
			if(%d.bl_id == %bl_id)
				return %d;
		}
	}

	return -1;
}

//RPGDataGroup::updatePlayers(%this)
//Does nothing for the time being.
//%this		:	RPGDataGroup object
function RPGDataGroup::updatePlayers(%this)
{
	//pass for now
}

//GameConnection::applyRPGData(%this)
//Creates an RPGData object for the client and returns it.
//%this		:	GameConnection object
//Returns an RPGData object
function GameConnection::applyRPGData(%this)
{
	if(!isObject(RPGDataGroup))
		RPGData_Init();

	%this.rpgData = RPGDataGroup.addPlayer(%this);
}

//RPGData::hasVal(%this, %name[, %c])
//Returns true or false if the RPGData object has the specified variable.
//%this		:	RPGData object
//%name		:	Name of the value
//%c		:	Flag to loop through the RPGData's values in a bug case
//Returns a boolean
function RPGData::hasVal(%this, %name, %c)
{
	%name = firstWord(%name);
	if(%this.vali[%name] !$= "")
		return true;

	if(%c)
	{
		for(%i = 0; %i < %this.vals; %i++)
		{
			if(%this.val[%i] $= %name)
				return true;
		}
	}

	return false;
}

//RPGData::addVal(%this, %name)
//Adds a value to the RPGData object
//%this		:	RPGData object
//%name		:	Name of the value to add
//Returns a boolean
function RPGData::addVal(%this, %name)
{
	%name = firstWord(%name);
	if(%name $= "")
		return false;

	if(%this.hasVal(%name))
		return false;

	%this.val[%this.vals] = %name;
	%this.vali[%name] = %this.vals;
	%this.valv[%this.vals] = "";
	%this.vals++;
	return true;
}

//RPGData::setVal(%this, %name, %val)
//Sets a value on the RPGData object
//%this		:	RPGData object
//%name		:	Name of the value to set
//%val		:	New state of the value
//Returns a boolean
function RPGData::setVal(%this, %name, %val)
{
	%name = firstWord(%name);
	if(%name $= "")
		return;

	if(!%this.hasVal(%name))
		%this.addVal(%name);

	%i = %this.vali[%name];
	%this.valv[%i] = %val;
	return true;
}

//RPGData::getVal(%this, %name)
//Returns the specified value on the RPGData object
//%this		:	RPGData object
//%name		:	Name of the relevant value
//Returns a string
function RPGData::getVal(%this, %name)
{
	%name = firstWord(%name);
	if(!%this.hasVal(%name))
		return "";

	%i = %this.vali[%name];
	return %this.valv[%i];
}

//RPGData::clearVals(%this)
//Clears the values on the RPGData object
//%this		:	RPGData object
//Returns 0
function RPGData::clearVals(%this)
{
	for(%i = %this.vals-1; %i >= 0; %i--)
	{
		%n = %this.val[%i];
		%this.vali[%n] = "";
		%this.valv[%i] = "";
		%this.val[%i] = "";
	}
	%this.vals = 0;
}

//RPGData::modVal(%this, %name, %op[, %over])
//Modifies a value on the RPGData object
//%this		:	RPGData object
//%name		:	Name of the value to modify
//%op		:	Operation string (e.g. "+ 1"; %1 will be replaced with the current value)
//%over		:	Don't put the value before the operator, allowing for function calls (e.g. "mFloatLength(%1, 0)")
//Returns the new value
function RPGData::modVal(%this, %name, %op, %over)
{
	%name = firstWord(%name);
	if(!%this.hasVal(%name))
		return "";

	%i = %this.vali[%name];
	%v = %this.valv[%i];
	%op = strReplace(%op, "%1", %v);
	%nv = eval("return" SPC (!%over ? %v SPC %op : %op) @ ";");
	if(%nv $= "")
		return "error";

	%this.setVal(%name, %nv);
	return %nv;
}

//RPGData::export(%this[, %path])
//Saves the RPGData object's fields to a file
//%this		:	RPGData object
//%path		:	File path (defaults to "$RPG::DataDir/bl_id.txt")
//Returns a boolean
function RPGData::export(%this, %path)
{
	if(%path $= "" || !isWriteableFileName(%path))
		%path = $RPG::DataDir @ %this.bl_id @ ".txt";

	%file = new FileObject();
	%file.openForWrite(%path);
	for(%i = 0; %i < %this.vals; %i++)
	{
		%n = %this.val[%i];
		%v = %this.getVal(%n);

		%line = %n TAB %v;
		%file.writeLine(%line);
	}
	%file.close();
	%file.delete();

	return true;
}

//RPGData::import(%this[, %path])
//Imports values to the RPGData object from a file
//%this		:	RPGData object
//%path		:	File path (defaults to "$RPG::DataDir/bl_id.txt")
//Returns a boolean
function RPGData::import(%this, %path)
{
	if(!isFile(%path))
	{
		%path = $RPG::DataDir @ %this.bl_id @ ".txt";
		if(!isFile(%path))
			return false;
	}

	%file = new FileObject();
	%file.openForRead(%path);
	while(!%file.isEOF())
	{
		%line = %file.readLine();

		%n = getField(%line, 0);
		%v = getFields(%line, 1, getFieldCount(%line)-1);

		%this.setVal(%n, %v);
	}
	%file.close();
	%file.delete();

	return true;
}

//getRPGDataFromObject(%this)
//Returns the RPGData associated with the given object
//%this		:	The object to check
//Returns -1 or an RPGData object
function getRPGDataFromObject(%this)
{
	switch$(%this.getClassName())
	{
		case "Player":
			if(isObject(%this.client))
				return getRPGDataFromObject(%this.client);
		default: 
			return (isObject(%this.rpgData) ? %this.rpgData : -1);
	}
	return -1;
}

//RPGDataGroup::autoSaveTick(%this)
//Looping function to auto-save RPGData objects
//%this		:	RPGDataGroup object
//Returns a schedule
function RPGDataGroup::autoSaveTick(%this)
{
	if(isEventPending(%this.autoSave))
		cancel(%this.autoSave);

	%ct = %this.getCount();
	for(%i = 0; %i < %ct; %i++)
		%this.getObject(%i).export();

	%this.autoSave = %this.schedule($RPG::AutoSave, autoSaveTick);
}

package RPG_Data
{
	function GameConnection::autoAdminCheck(%this)
	{
		%r = parent::autoAdminCheck(%this);

		if($RPG::Enabled)
			%this.applyRPGData();

		return %r;
	}

	function GameConnection::onClientLeaveGame(%this)
	{
		parent::onClientLeaveGame(%this);

		%data = getRPGDataFromObject(%this);
		if(isObject(%data))
			%data.export();
	}
};
activatePackage(RPG_Data);