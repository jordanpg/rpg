datablock ParticleData(SwordSpinTrailParticle)
{
	dragCoefficient = 0.5;
	gravityCoefficient = 0.0;
	inheritedVelFactor = 0;
	constantAcceleration = 0;
	lifetimeMS = 70;
	lifetimeVarianceMS = 0;
	textureName = "base/data/particles/dot";
	spinSpeed = 0;
	spinRandomMin = 0;
	spinRandomMax = 0;
	colors[0] = "0.5 0.5 0.5 0.8";
	colors[1] = "0.5 0.5 0.5 0.6";
	colors[2] = "0.35 0.35 0.35 0.4";
	sizes[0] = 0.2;
	sizes[1] = 0.2;
	sizes[2] = 0.2;
	times[0] = 0.0;
	times[1] = 0.5;
	times[2] = 1.0;
	useInvAlpha = false;
};

datablock ParticleEmitterData(SwordSpinTrailEmitter)
{
	uiName = "Sword Spin Trail";
	lifetimeMS = 70;
	ejectionPeriodMS = 10;
	periodVarianceMS = 0;
	ejectionVelocity = 30;
	velocityVariance = 0;
	ejectionOffset = 0;
	thetaMin = 0;
	thetaMax = 0;
	phiReferenceVel = 0;
	phiVariance = 0;
	overrideAdvance = false;
	useEmitterColors = false;
	orientParticles = false;
	particles = "SwordSpinTrailParticle";

	orientOnVelocity = true;
};