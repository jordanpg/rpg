$rep = "0 1 2 3 4 5 6 7 8 9 ";
$lookstr = "";
for(%i = 0; %i < 18; %i++)
	$lookstr = $lookstr @ $rep;

function Player::lookTest(%this, %size)
{
	if(isEventPending(%this.lookTest))
		cancel(%this.lookTest);

	%ang = mFloatLength(%this.getLookHoriz(), 0);

	%end = %ang + %size;
	if(%end > 360)
		%end = %end - 360;
	for(%i = %ang; %i < 360; %i++)
	{
		%str = %str @ getSubStr($lookstr, %i, 1);
		if(%i == %end)
			break;
		if(%i == 359)
			%i = 0;
	}
	%this.client.centerPrint("\c6" @ trim(%str), 3);

	%this.lookTest = %this.schedule(100, lookTest, %size);
}