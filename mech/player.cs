//Do a raycast to see what the player is looking at.
function Player::eyeCast(%this, %r, %t, %e0, %e1, %e2, %e3, %e4)
{
	%eye = %this.getEyePoint();
	%vec = VectorNormalize(%this.getEyeVector());
	%end = VectorAdd(%eye, VectorScale(%vec, %r));
	return containerRayCast(%eye, %end, %t, %e0, %e1, %e2, %e3, %e4);
}

//Get the normal from a raycast.
function getRayNormal(%ray)
{
	%nX = mFloatLength(getWord(%ray, 4), 0);
	%nY = mFloatLength(getWord(%ray, 5), 0);
	%nZ = mFloor(getWord(%ray, 6));
	return %nX SPC %nY SPC %nZ;
}

function Player::getLookVert(%this)
{
	return getVertAngle(%this.getEyeTransform());
}

function Player::getLookHoriz(%this)
{
	return getHorizAngle(%this.getEyeTransform());
}

function StaticShape::circlePlayer(%this, %s, %obj, %radius, %step, %st)
{
	if(isEventPending(%this.circle))
		cancel(%this.circle);

	if(!isObject(%obj) || %s <= 0)
		return;

	if(%this.cAngle $= "" || %st !$= "")
		%this.cAngle = %st;

	%x = (%radius * 0.025) * mRadToDeg(mCos(mDegToRad(%this.cAngle)));
	%y = (%radius * 0.025) * mRadToDeg(mSin(mDegToRad(%this.cAngle)));

	%pos = VectorAdd(%obj.getHackPosition(), %x SPC %y SPC 0);
	%rot = eulerToAxis("0 90" SPC %this.cAngle);
	%this.setTransform(%pos SPC %rot);

	%this.cAngle += %step;
	if(%this.cAngle >= 360)
		%this.cAngle -= 360;

	%this.circle = %this.schedule(%s, circlePlayer, %s, %obj, %radius, %step);
}

function ParticleEmitterNode::circlePlayer(%this, %s, %obj, %radius, %step, %st)
{
	if(isEventPending(%this.circle))
		cancel(%this.circle);

	if(!isObject(%obj) || %s <= 0)
		return;

	if(%this.cAngle $= "" || %st !$= "")
		%this.cAngle = %st;

	%x = (%radius * 0.025) * mRadToDeg(mCos(mDegToRad(%this.cAngle)));
	%y = (%radius * 0.025) * mRadToDeg(mSin(mDegToRad(%this.cAngle)));

	%pos = VectorAdd(%obj.getHackPosition(), %x SPC %y SPC 0);
	%rot = eulerToAxis("90" SPC %this.cAngle+90 SPC 0);
	%this.setTransform(%pos SPC %rot);
	%this.inspectPostApply();

	%this.cAngle += %step;
	if(%this.cAngle >= 360)
		%this.cAngle -= 360;

	%this.circle = %this.schedule(%s, circlePlayer, %s, %obj, %radius, %step);
}