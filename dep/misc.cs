function shapeBase::getAxisRot(%this)
{
	return getWords(%this.getTransform(), 3, 6);
}

function shapeBase::getEulerRot(%this)
{
	%rot = getWords(%this.getTransform(), 3, 6);
	return axisToEuler(%rot);
}

function shapeBase::setAxisRot(%this, %rot)
{
	%trans = %this.getPosition() SPC %rot;
	%this.setTransform(%trans);
}

function shapeBase::setEulerRot(%this, %rot)
{
	%trans = %this.getPosition() SPC eulerToAxis(%rot);
	%this.setTransform(%trans);
}

function eulerToAxis(%euler) //Trader
{
	%euler = VectorScale(%euler,$pi / 180);
	%matrix = MatrixCreateFromEuler(%euler);
	return getWords(%matrix,3,6);
}

function axisToEuler(%axis) //Trader
{
	%angleOver2 = getWord(%axis,3) * 0.5;
	%angleOver2 = -%angleOver2;
	%sinThetaOver2 = mSin(%angleOver2);
	%cosThetaOver2 = mCos(%angleOver2);
	%q0 = %cosThetaOver2;
	%q1 = getWord(%axis,0) * %sinThetaOver2;
	%q2 = getWord(%axis,1) * %sinThetaOver2;
	%q3 = getWord(%axis,2) * %sinThetaOver2;
	%q0q0 = %q0 * %q0;
	%q1q2 = %q1 * %q2;
	%q0q3 = %q0 * %q3;
	%q1q3 = %q1 * %q3;
	%q0q2 = %q0 * %q2;
	%q2q2 = %q2 * %q2;
	%q2q3 = %q2 * %q3;
	%q0q1 = %q0 * %q1;
	%q3q3 = %q3 * %q3;
	%m13 = 2.0 * (%q1q3 - %q0q2);
	%m21 = 2.0 * (%q1q2 - %q0q3);
	%m22 = 2.0 * %q0q0 - 1.0 + 2.0 * %q2q2;
	%m23 = 2.0 * (%q2q3 + %q0q1);
	%m33 = 2.0 * %q0q0 - 1.0 + 2.0 * %q3q3;
	return mRadToDeg(mAsin(%m23)) SPC mRadToDeg(mAtan(-%m13, %m33)) SPC mRadToDeg(mAtan(-%m21, %m22));
}

function getRandomFloat( %min, %max )
{
	return %min + getRandom() * ( %max - %min );
}

//getRandomVect(%vectA, %vectB[, %len]);
//Returns a random vector between Vector3F %vectA and Vector3F %vectB.
//%len is an unsigned integer 0-5 for the allowed float length, defaults to two.
function getRandomVect(%vectA, %vectB)
{
	%xA = getWord(%vectA, 0);
	%yA = getWord(%vectA, 1);
	%zA = getWord(%vectA, 2);
	%xB = getWord(%vectB, 0);
	%yB = getWord(%vectB, 1);
	%zB = getWord(%vectB, 2);
	if(%xA != %xB)
		%x = getRandomFloat(%xA, %xB);
	else
		%x = %xA;

	if(%yA != %yB)
		%y = getRandomFloat(%yA, %yB);
	else
		%y = %xA;

	if(%zA != %zB)
		%z = getRandomFloat(%zA, %zB);
	else
		%z = %zA;
	return %x SPC %y SPC %z;
}

function getVertAngle(%trans)
{
	%rot = getWords(%trans, 3, 6);
	%euler = axisToEuler(%rot);
	%z = mAbs(getWord(%euler, 0) + 90);
	return mFloatLength(%z, 3);
}

function getHorizAngle(%trans)
{
	%rot = getWords(%trans, 3, 6);
	%euler = axisToEuler(%rot);
	%z = mAbs(getWord(%euler, 2) + 180);
	return mFloatLength(%z, 3);
}

function getRollAngle(%trans)
{
	%rot = getWords(%trans, 3, 6);
	%euler = axisToEuler(%rot);
	%z = mAbs(getWord(%euler, 1));
	return mFloatLength(%z, 3);
}

function SimGroup::hasNTObject(%bg, %nt)
{
	for(%i = 0; %i < %bg.NTNameCount; %i++)
	{
		if(%bg.NTName[%i] $= %nt)
			return %i;
	}
	return -1;
}

function SimGroup::ntCall(%bg, %nt, %func, %a1, %a2, %a3, %a4, %a5, %a6, %a7, %a8, %a9, %a10)
{
	if(!isObject(%bg))
		return false;
	if(%bg.hasNTObject(%nt) $= -1)
		return;
	for(%i = 0; %i < %bg.NTObjectCount[%nt]; %i++)
		%bg.NTObject[%nt, %i].call(%func, %a1, %a2, %a3, %a4, %a5, %a6, %a7, %a8, %a9, %a10);
}

// function simpBrick(%db, %pos, %client, %colour, %angle, %print)
// {
// 	%brick = new fxDTSBrick()
// 			{
// 				datablock = %db;
// 				position = %pos;
// 				rotation = angleToRot(%angle);

// 				angleID = %angle;
// 				colorID = %colour;
// 				colorFXID = 0;
// 				shapeFXID = 0;
// 				brickGroup = %client.brickGroup;
// 				client = %client;
// 				printID = (%printID !$= "" ? %printID : 85);

// 				isBaseplate = true;
// 				isPlanted = true;
// 			};
// 	%brick.schedule(0, plant);
// 	%brick.brickGroup.add(%brick);
// }

// function drawSphere(%origin, %r, %m, %n)
// {
// 	%end = %n-1;
// 	for(%m0 = 0; %m0 <= %m; %m0++)
// 	{
// 		%m1 = mSin($pi * %m0 / %m);
// 		%m2 = mCos($pi * %m0 / %m);

// 		for(%n0 = 0; %n0 <= %end; %n0++)
// 		{
// 			%n1 = mCos(2 * $pi * %n0 / %n);
// 			%n2 = mSin(2 * $pi * %n0 / %n);

// 			%pos = %r * (%m1 * %n1) SPC %r * (%m1 * %n2) SPC %r * %m2;

// 			simpBrick(Brick4xCubeData, VectorAdd(%origin, %pos), localClientConnection, 0, 0);
// 		}
// 	}
// }