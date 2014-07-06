$RPG::Dir = "config/scripts/mod/GameMode_RPG/";
if(!isFile($RPG::Dir @ "server.cs"))
	$RPG::Dir = "Add-Ons/GameMode_RPG/";

function RPGDir(%add)
{
	return $RPG::Dir @ %add;
}

function RPG_Load()
{
	exec("./data/main.cs");
	exec("./dep/main.cs");
	exec("./mech/main.cs");
}
RPG_Load();