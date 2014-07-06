$BIOME_P = (1 / 32) SPC 8 SPC 0.5 SPC 0 SPC 16 SPC 1;
$BIOME_M = (1 / 16) SPC 8 SPC 0.75 SPC 0 SPC 24 SPC 1;
$BIOME_R = (1 / 16) SPC 8 SPC 0.5 SPC 0 SPC 10 SPC 1;
$BIOME_F = (1 / 64) SPC 4 SPC 0.25 SPC 0 SPC 14 SPC 1;
$BIOME_L = (1 / 16) SPC 8 SPC 0.5 SPC 0 SPC 8 SPC 1;

function SimplexNoise(%seed)
{
	%g3 = "1 1 0\t-1 1 0\t1 -1 0\t-1 -1 0\t1 0 1\t-1 0 1\t1 0 -1\t-1 0 -1\t0 1 1\t0 -1 1\t0 1 -1\t0 -1 -1";
	// %p = "151\t160\t137\t91\t90\t15" TAB
	// "131\t13\t201\t95\t96\t53\t194\t233\t7\t225\t140\t36\t103\t30\t69\t142\t8\t99\t37\t240\t21\t10\t23" TAB
	// "190\t6\t148\t247\t120\t234\t75\t0\t26\t197\t62\t94\t252\t219\t203\t117\t35\t11\t32\t57\t177\t33" TAB
	// "88\t237\t149\t56\t87\t174\t20\t125\t136\t171\t168\t68\t175\t74\t165\t71\t134\t139\t48\t27\t166" TAB
	// "77\t146\t158\t231\t83\t111\t229\t122\t60\t211\t133\t230\t220\t105\t92\t41\t55\t46\t245\t40\t244" TAB
	// "102\t143\t54\t65\t25\t63\t161\t1\t216\t80\t73\t209\t76\t132\t187\t208\t89\t18\t169\t200\t196" TAB
	// "135\t130\t116\t188\t159\t86\t164\t100\t109\t198\t173\t186\t3\t64\t52\t217\t226\t250\t124\t123" TAB
	// "5\t202\t38\t147\t118\t126\t255\t82\t85\t212\t207\t206\t59\t227\t47\t16\t58\t17\t182\t189\t28\t42" TAB
	// "223\t183\t170\t213\t119\t248\t152\t2\t44\t154\t163\t70\t221\t153\t101\t155\t167\t43\t172\t9" TAB
	// "129\t22\t39\t253\t19\t98\t108\t110\t79\t113\t224\t232\t178\t185\t112\t104\t218\t246\t97\t228" TAB
	// "251\t34\t242\t193\t238\t210\t144\t12\t191\t179\t162\t241\t81\t51\t145\t235\t249\t14\t239\t107" TAB
	// "49\t192\t214\t31\t181\t199\t106\t157\t184\t84\t204\t176\t115\t121\t50\t45\t127\t4\t150\t254" TAB
	// "138\t236\t205\t93\t222\t114\t67\t29\t24\t72\t243\t141\t128\t195\t78\t66\t215\t61\t156\t180";
 
	%obj = new ScriptObject()
			{
				class = "SimplexNoise";
				p = %p;
			};
	// for(%i = 0; %i < 512; %i++)
	// 	%obj.perm[%i] = getField(%p, %i & 255);
	%ct = getFieldCount(%g3);
	for(%i = 0; %i < %ct; %i++)
		%obj.grad3[%i] = getField(%g3, %i);
	%obj.setSeed(%seed);
	return %obj;
}

function SimplexNoise::setSeed(%this, %seed)
{
	if(!%seed)
		%seed = 0;

	if(%seed $= %this.seed)
		return;

	%p = "151\t160\t137\t91\t90\t15" TAB
	"131\t13\t201\t95\t96\t53\t194\t233\t7\t225\t140\t36\t103\t30\t69\t142\t8\t99\t37\t240\t21\t10\t23" TAB
	"190\t6\t148\t247\t120\t234\t75\t0\t26\t197\t62\t94\t252\t219\t203\t117\t35\t11\t32\t57\t177\t33" TAB
	"88\t237\t149\t56\t87\t174\t20\t125\t136\t171\t168\t68\t175\t74\t165\t71\t134\t139\t48\t27\t166" TAB
	"77\t146\t158\t231\t83\t111\t229\t122\t60\t211\t133\t230\t220\t105\t92\t41\t55\t46\t245\t40\t244" TAB
	"102\t143\t54\t65\t25\t63\t161\t1\t216\t80\t73\t209\t76\t132\t187\t208\t89\t18\t169\t200\t196" TAB
	"135\t130\t116\t188\t159\t86\t164\t100\t109\t198\t173\t186\t3\t64\t52\t217\t226\t250\t124\t123" TAB
	"5\t202\t38\t147\t118\t126\t255\t82\t85\t212\t207\t206\t59\t227\t47\t16\t58\t17\t182\t189\t28\t42" TAB
	"223\t183\t170\t213\t119\t248\t152\t2\t44\t154\t163\t70\t221\t153\t101\t155\t167\t43\t172\t9" TAB
	"129\t22\t39\t253\t19\t98\t108\t110\t79\t113\t224\t232\t178\t185\t112\t104\t218\t246\t97\t228" TAB
	"251\t34\t242\t193\t238\t210\t144\t12\t191\t179\t162\t241\t81\t51\t145\t235\t249\t14\t239\t107" TAB
	"49\t192\t214\t31\t181\t199\t106\t157\t184\t84\t204\t176\t115\t121\t50\t45\t127\t4\t150\t254" TAB
	"138\t236\t205\t93\t222\t114\t67\t29\t24\t72\t243\t141\t128\t195\t78\t66\t215\t61\t156\t180";

	for(%i = 0; %i < 512; %i++)
	{
		%shift = (%i + %seed) % 512;
		%this.perm[%i] = getField(%p, %shift & 255);
	}

	%this.seed = %seed;
}

function SimplexNoise::noise2d(%this, %x, %y, %seed)
{
	if(%seed !$= "")
		%this.setSeed(%seed);
	%sq3 = mPow(3, 0.5);
	%f2 = 0.5 * (%sq3 - 1);
	%s = (%x + %y) * %f2;
	%i = mFloor(%x + %s);
	%j = mFloor(%y + %s);

	%g2 = (3 - %sq3) / 6;
	%t = (%i + %j) * %g2;
	%x0 = %x - (%i - %t);
	%y0 = %y - (%j - %t);

	if(%x0 > %y0)
	{
		%i1 = 1;
		%j1 = 0;
	}
	else
	{
		%i1 = 0;
		%j1 = 1;
	}

	%x1 = %x0 - %i1 + %g2;
	%y1 = %y0 - %j1 + %g2;
	%x2 = %x0 - 1 + 2 * %g2;
	%y2 = %y0 - 1 + 2 * %g2;

	%ii = %i & 255;
	%jj = %j & 255;
	%gi0 = %this.perm[%ii + %this.perm[%jj]] % 12;
	// echo(1 SPC %gi0);
	%gi1 = %this.perm[%ii + %i1 + %this.perm[%jj + %j1]] % 12;
	// echo(2 SPC %gi1);
	%gi2 = %this.perm[%ii + 1 + %this.perm[%jj + 1]] % 12;
	// echo(3 SPC %gi2);

	%t0 = 0.5 - %x0 * %x0 - %y0 * %y0;
	if(%t0 < 0)
		%n0 = 0;
	else
	{
		%t0 = %t0 * %t0;
		%n0 = %t0 * %t0 * simp_dot2d(%this.grad3[%gi0], %x0, %y0);
	}

	%t1 = 0.5 - %x1 * %x1 - %y1 * %y1;
	if(%t1 < 0)
		%n1 = 0;
	else
	{
		%t1  = %t1 * %t1;
		%n1 = %t1 * %t1 * simp_dot2d(%this.grad3[%gi1], %x1, %y1);
	}

	%t2 = 0.5 - %x2 * %x2 - %y2 * %y2;
	if(%t2 < 0)
		%n2 = 0;
	else
	{
		%t2 = %t2 * %t2;
		%n2 = %t2 * %t2 * simp_dot2d(%this.grad3[%gi2], %x2, %y2);
	}

	return 70 * (%n0 + %n1 + %n2);
}

function SimplexNoise::noise3d(%this, %x, %y, %z, %seed)
{
	if(%seed !$= "")
		%this.setSeed(%seed);
	%f3 = (1 / 3);
	%s = (%x + %y + %z) * %f3;
	%i = mFloor(%x + %s);
	%j = mFloor(%y + %s);
	%k = mFloor(%z + %s);
 
	%g3 = (1 / 6);
	%t = (%i + %j + %k) * %g3;
	%x0 = %x - (%i - %t);
	%y0 = %y - (%j - %t);
	%z0 = %z - (%k - %t);
 
	if(%x0 >= %y0)
	{
		if(%y0 >= %z0)
		{
			%i1 = 1;
			%j1 = 0;
			%k1 = 0;
			%i2 = 1;
			%j2 = 1;
			%k2 = 0;
		}
		else if(%x0 >= %z0)
		{
			%i1 = 1;
			%k1 = 0;
			%j1 = 1;
			%i2 = 0;
			%k2 = 1;
			%j2 = 1;
		}
		else
		{
			%i1 = 0;
			%k1 = 0;
			%j1 = 1;
			%i2 = 1;
			%k2 = 0;
			%j2 = 1;
		}
	}
	else
	{
		if(%y0 < %z0)
		{
			%i1 = 1;
			%j1 = 0;
			%k1 = 0;
			%i2 = 1;
			%j2 = 1;
			%k2 = 0;
		}
		else if(%x0 < %z0)
		{
			%i1 = 0;
			%k1 = 1;
			%j1 = 0;
			%i2 = 0;
			%k2 = 1;
			%j2 = 1;
		}
		else
		{
			%i1 = 0;
			%k1 = 1;
			%j1 = 0;
			%i2 = 1;
			%k2 = 1;
			%j2 = 0;
		}
	}
 
	%x1 = %x0 - %i1 + %g3;
	%y1 = %y0 - %j1 + %g3;
	%z1 = %z0 - %k1 + %g3;
	%x2 = %x0 - %i2 + 2*%g3;
	%y2 = %y0 - %j2 + 2*%g3;
	%z2 = %z0 - %k2 + 2*%g3;
	%x3 = %x0 - 1 + 3*%g3;
	%y3 = %y0 - 1 + 3*%g3;
	%z3 = %z0 - 1 + 3*%g3;
 
	%ii = %i & 255;
	%jj = %j & 255;
	%kk = %k & 255;
	%gi0 = %this.perm[%ii + %this.perm[%jj + %this.perm[%kk]]] % 12;
	%gi1 = %this.perm[%ii + %i1 + %this.perm[%jj + %j1 + %this.perm[%kk + %k1]]] % 12;
	%gi3 = %this.perm[%ii + 1 + %this.perm[%jj + 1 + %this.perm[%kk + 1]]] % 12;
 
	%t0 = 0.5 - %x0*x0 - %y0*%y0 - %z0*%z0;
	if(%t0 < 0)
		%n0 = 0;
	else
	{
		%t0 = %t0 * %t0;
		%n0 = %t0 * %t0 * simp_dot(%this.grad3[%gi0], %x0, %y0, %z0);
	}
 
	%t1 = 0.5 - %x1 * %x1 - %y1 * %y1 - %z1 * %z1;
	if(%t1 < 0)
		%n1 = 0;
	else
	{
		%t1 = %t1 * %t1;
		%n1 = %t1 * %t1 * simp_dot(%this.grad3[%gi3], %x1, %y1, %z1);
	}
 
	%t2 = 0.5 - %x2 * %x2 - %y2 * %y2 - %z2 * %z2;
	if(%t2 < 0)
		%n2 = 0;
	else
	{
		%t2 = %t2 * %t2;
		%n2 = %t2 * %t2 * simp_dot(%this.grad3[%gi3], %x2, %y2, %z2);
	}
 
	%t3 = 0.5 - %x3 * %x3 - %y3 * %y3 - %z3 * %z3;
	if(%t3 < 0)
		%n3 = 0;
	else
	{
		%t3 = %t3 * %t3;
		%n3 = %t3 * %t3 * simp_dot(%this.grad3[%gi3], %x3, %y3, %z3);
	}
 
	return 32 * (%n0 + %n1 + %n2 + %n3);
}

function simp_dot2d(%g, %x, %y)
{
	%g0 = getWord(%g, 0) * %x;
	%g1 = getWord(%g, 1) * %y;
	return %g0 + %g1;
}

function simp_dot(%g, %x, %y, %z)
{
	%g0 = getWord(%g, 0) * %x;
	%g1 = getWord(%g, 1) * %y;
	%g2 = getWord(%g, 2) * %z;
	return %g0 + %g1 + %g2;
}

function sHeight(%n, %x, %y, %seed, %freq, %iter, %persist, %low, %high, %scale)
{
	%maxAmp = 0;
	%amp = 1;
	%noise = 0;

	for(%i = 0; %i < %iter; %i++)
	{
		%noise += (1 - mAbs(%n.noise2d(%x * %freq, %y * %freq, %seed))) * %amp;
		%maxAmp += %amp;
		%amp *= %persist;
		%freq *= 2;
		// echo(%noise);
	}

	%noise /= %maxAmp;
	%noise = %noise * (%high - %low) / 2 + (%high + %low) / 2;
	return %noise * %scale;	
}

function angleToRot(%id)
{
	switch(%id)
	{
		case 0: %rotation = "1 0 0 0";
		case 1: %rotation = "0 0 1 90";
		case 2: %rotation = "0 0 1 180";
		case 3: %rotation = "0 0 -1 90";
		default: %rotation = "1 0 0 0";
	}
	return %rotation;
}

function simpBrick(%db, %pos, %client, %colour, %angle, %print)
{
	%brick = new fxDTSBrick()
			{
				datablock = %db;
				position = %pos;
				rotation = angleToRot(%angle);

				angleID = %angle;
				colorID = %colour;
				colorFXID = 0;
				shapeFXID = 0;
				brickGroup = %client.brickGroup;
				client = %client;
				printID = (%printID !$= "" ? %printID : 85);

				isBaseplate = true;
				isPlanted = true;
			};
	%brick.schedule(0, plant);
	%brick.brickGroup.add(%brick);
}

function Terrain_New(%this)
{
	%this = new ScriptGroup(Terrain)
			{
				noise = SimplexNoise();
			};
	return %this;
}

function Terrain::newMap(%this, %name, %keep)
{
	if(isObject(%this.map[%name]))
	{
		if(%keep)
			return %this.map[%name];
		%this.map[%name].delete();
	}

	%map = new ScriptObject("terrainMap_" @ %name)
			{
				class = "TerrainMap";
				name = %name;
				parent = %this;
			};
	%this.map[%name] = %map;
	%this.add(%map);
	return %map;
}

function terrainMap::fill(%this, %minX, %minY, %maxX, %maxY, %seed, %freq, %iter, %persist, %low, %high, %scale)
{
	for(%y = %minY; %y <= %maxY; %y++)
	{
		for(%x = %minX; %x <= %maxX; %x++)
			%this.map[%x, %y] = sHeight(%this.parent.noise, %x, %y, %seed, %freq, %iter, %persist, %low, %high, %scale);
	}
}

function terrainMap::debugMap(%this, %minX, %minY, %maxX, %maxY, %db, %col)
{
	%sX = %db.brickSizeX * 0.5;
	%sY = %db.brickSizeY * 0.5;
	%sZ = %db.brickSizeZ * 0.2;

	%i++;
	for(%y = %minY; %y <= %maxY; %y++)
	{
		for(%x = %minX; %x <= %maxX; %x++)
		{
			%height = %this.map[%x, %y];

			%pos = (%x * %sX) SPC (%y * %sY) SPC (%height * %sZ);
			schedule(%i++ * 33, 0, simpBrick, %db, %pos, localClientConnection, %col);
		}
	}
}

function terrainMap::smoothMap(%this, %minX, %minY, %maxX, %maxY, %size)
{
	for(%y = %minY; %y <= %maxY; %y++)
	{
		%startY = %y - %size;
		%endY = %y + %size;
		for(%x = %minX; %x <= %maxX; %x++)
		{
			// echo(%x SPC %y);
			%ct = 0;
			%total = 0;

			%startX = %x - %size;
			%endX = %x + %size;
			for(%x1 = %startX; %x1 <= %endX; %x1++)
			{
				if(%x1 < %minX || %x1 > %maxX)
					continue;


				for(%y1 = %startY; %y1 <= %endY; %y1++)
				{
					if(%y1 < %minY || %y1 > %maxY)
						continue;

					// echo(%x SPC %y TAB %x1 SPC %y1);
					%total += %this.map[%x1, %y1];
					%ct++;
				}
			}

			if(%ct != 0 && %total != 0)
			{
				%old = %this.map[%x, %y];
				%this.map[%x, %y] = %total / %ct;
				// echo(%old SPC "-s->" SPC %this.map[%x, %y]);
			}
		}
	}
}

function terrainMap::smooth(%this, %minX, %minY, %maxX, %maxY, %size, %iter)
{
	for(%i = 0; %i < %iter; %i++)
		%this.smoothMap(%minX, %minY, %maxX, %maxY, %size);
}

function terrainMap::genChunk(%this, %s, %cX, %cY, %seed, %freq, %iter, %persist, %low, %high, %scale)
{
	if(%s < 1)
		return;

	%sX = %cX * %s;
	%eX = %sX + (%s - 1);
	%sY = %cY * %s;
	%eY = %sY + (%s - 1);

	%this.fill(%sX, %sY, %eX, %eY, %seed, %freq, %iter, %persist, %low, %high, %scale);
}

function terrainMap::loadBlueprint(%this, %f, %size, %seed)
{
	if(!isFile(%f))
		return;

	%file = new FileObject();
	%r = %file.openForRead(%f);
	if(!%r)
		return;

	%m = new ScriptObject(tempstore){ lines = 0; };
	while(!%file.isEOF())
	{
		%line = %file.readLine();
		%m.line[%m.lines] = %line;
		%m.lines++;
	}
	%file.close();
	%file.delete();

	%l = %m.lines - 1;
	for(%i = %l; %i >= 0; %i--)
	{
		%line = %m.line[%i];
		%len = strLen(%line);
		for(%j = 0; %j < %len; %j++)
		{
			%char = getSubStr(%line, %j, 1);
			%biome = $BIOME_[%char];
			%freq = getWord(%biome, 0);
			%iter = getWord(%biome, 1);
			%persist = getWord(%biome, 2);
			%low = getWord(%biome, 3);
			%high = getWord(%biome, 4);
			%scale = getWord(%biome, 5);

			%this.genChunk(%size, %j, %l - %i, %seed, %freq, %iter, %persist, %low, %high, %scale);
		}
	}
}

function buildSeaLevel(%level, %minX, %minY, %maxX, %maxY, %db, %col)
{
	%sX = %db.brickSizeX * 0.5;
	%sY = %db.brickSizeY * 0.5;
	%sZ = %db.brickSizeZ * 0.2;
	%h = (%level * %sZ);

	%i++;
	for(%y = %minY; %y <= %maxY; %y++)
	{
		for(%x = %minX; %x <= %maxX; %x++)
		{
			%pos = (%x * %sX) SPC (%y * %sY) SPC %h;
			schedule(%i++ * 33, 0, simpBrick, %db, %pos, localClientConnection, %col);
		}
	}
}

function terrainMap::buildSeaLevel(%this, %level, %minX, %minY, %maxX, %maxY, %db, %col)
{
	%sX = %db.brickSizeX * 0.5;
	%sY = %db.brickSizeY * 0.5;
	%sZ = %db.brickSizeZ * 0.2;
	%h = (%level * %sZ);

	%i++;
	for(%y = %minY; %y <= %maxY; %y++)
	{
		for(%x = %minX; %x <= %maxX; %x++)
		{
			if(%this.map[%x, %y] > %level)
				continue;

			%pos = (%x * %sX) SPC (%y * %sY) SPC %h;
			schedule(%i++ * 33, 0, simpBrick, %db, %pos, localClientConnection, %col);
		}
	}
}

function terrainMap::buildMap(%this, %minX, %minY, %maxX, %maxY, %db, %level, %sand, %m1, %m2)
{
	%sX = %db.brickSizeX * 0.5;
	%sY = %db.brickSizeY * 0.5;
	%sZ = %db.brickSizeZ * 0.2;
	%h = (%level * %sZ);

	%i++;
	for(%y = %minY; %y <= %maxY; %y++)
	{
		for(%x = %minX; %x <= %maxX; %x++)
		{
			%m = %this.map[%x, %y];
			%xy = (%x * %sX) SPC (%y * %sY);
			if(%m <= %level)
			{
				%pos = %xy SPC %h;
				schedule(%i++ * 33, 0, simpBrick, %db, %pos, localClientConnection, 33);
			}
			if(%m <= %level+%sand)
				%col = 21;
			else if(%m <= %level+%m1)
				%col = 2;
			else if(%m <= %level+%m2)
				%col = 6;
			else if(%m > %level+%m2)
				%col = 5;
			%pos = %xy SPC (%m * %sZ);
			schedule(%i++ * 33, 0, simpBrick, %db, %pos, localClientConnection, %col);
		}
	}
}

function Terrain::newCellMap(%this, %label)
{
	if(isObject(%this.cell[%name]))
	{
		if(%keep)
			return %this.cell[%name];
		%this.cell[%name].delete();
	}

	%map = new ScriptObject("terrainCellMap_" @ %name)
			{
				class = "TerrainCellMap";
				name = %name;
				parent = %this;
			};
	%this.cell[%name] = %map;
	%this.add(%map);
	return %map;
}

function TerrainCellMap::buildFromMap(%this, %map, %minX, %minY, %maxX, %maxY)
{
	if(%minX <= %this.minX)
		%this.minX = %minX;
	if(%maxX >= %this.maxX)
		%this.maxX = %maxX;
	if(%minY <= %this.minY)
		%this.minY = %minY;
	if(%maxY >= %this.maxY)
		%this.maxY = %maxY;

	for(%y = %minY; %y <= %maxY; %y++)
	{
		for(%x = %minX; %x <= %maxX; %x++)
		{
			%m = %map.map[%x, %y];
			%minZ = mFloor(%m);
			%maxZ = mFloor(%m);
			if(%maxZ >= %this.maxZ)
				%this.maxZ = %maxZ;
			if(%minZ <= %this.minZ)
				%this.minZ = %minZ;

			for(%z = %minZ; %z <= %maxZ; %z++)
				%this.cell[%x, %y, %z] = %m;
		}
	}
}

function TerrainCellMap::debugCells(%this, %snap, %db, %col, %minX, %minY, %minZ, %maxX, %maxY, %maxZ)
{
	if(%minX $= "")
		%minX = %this.minX;
	if(%maxX $= "")
		%maxX = %this.maxX;
	if(%minY $= "")
		%minY = %this.minY;
	if(%maxY $= "")
		%maxY = %this.maxY;
	if(%minZ $= "")
		%minZ = %this.minZ;
	if(%maxZ $= "")
		%maxZ = %this.maxZ;

	%sX = %db.brickSizeX * 0.5;
	%sY = %db.brickSizeY * 0.5;
	%sZ = %db.brickSizeZ * 0.2;

	for(%z = %minZ; %z <= %maxZ; %z++)
	{
		for(%y = %minY; %y <= %maxY; %y++)
		{
			for(%x = %minX; %x <= %maxX; %x++)
			{
				%m = %this.cell[%x, %y, %z];
				if(!%m)
					continue;

				%xy = (%x * %sX) SPC (%y * %sY);
				if(!%snap)
					%pos = %xy SPC (%m * %sZ);
				else
					%pos = %xy SPC (%z * %sZ);

				schedule(%i++ * 33, 0, simpBrick, %db, %pos, localClientConnection, %col);
			}
		}
	}
}

function TerrainCellMap::solveModTer(%this, %x, %y, %z, %size)
{
	%us = %this.cell[%x, %y, %z];
	if(!%us)
	{
		// echo("fail 0");
		return -1;
	}
	%z++; //modter is placed on top of the brick
	%centre = %this.cell[%x, %y, %z];
	if(%centre)
	{
		// echo("fail 1");
		return -1;
	}
	%up = %this.cell[%x, %y, %z+1];
	if(%up)
	{
		// echo("fail 2");
		return -1;
	}
	%down = %this.cell[%x, %y, %z-1];
	if(!%down)
	{
		// echo("fail 3");
		return -1;
	}

	%ct = 0;
	%hl = 0;
	%hr = 0;
	%hf = 0;
	%hb = 0;
	%left = %this.cell[%x-1, %y, %z];
	if(%left)
	{
		%hl = 1;
		%ct++;
	}
	%right = %this.cell[%x+1, %y, %z];
	if(%right)
	{
		%hr = 1;
		%ct++;
	}
	%fwd = %this.cell[%x, %y+1, %z];
	if(%fwd)
	{
		%hf = 1;
		%ct++;
	}
	%back = %this.cell[%x, %y-1, %z];
	if(%back)
	{
		%hb = 1;
		%ct++;
	}
	%a1  = %hr @ %hb @ %hl @ %hf; //Ordered so that a strPos will give the proper angleID to face the brick.
	if(%ct == 4)
		return "Brick" @ %size @ "Cube1Data 0"; //Just fill in the hole.
	else if(%ct == 3)
		return "Brick" @ %size @ "Ramp1Data" SPC strPos(%a1, 1, getRandom(0, 2)); //Make a ramp facing a random block.
	else if(%ct == 1)
		return "Brick" @ %size @ "Ramp1Data" SPC strStr(%a1, 1); //Make a ramp facing the block.
	else if(%ct == 2)
	{
		switch$(%a1)
		{
			case "1001": return "Brick" @ %size @ "CornerB1Data 0";
			case "1100": return "Brick" @ %size @ "CornerB1Data 1";
			case "0110": return "Brick" @ %size @ "CornerB1Data 2";
			case "0011": return "Brick" @ %size @ "CornerB1Data 3";
			default: return "Brick" @ %size @ "Cube1Data 0";
		}
	}

	%ct2 = 0;
	%h0 = 0;
	%h1 = 0;
	%h2 = 0;
	%h3 = 0;
	%xy0 = %this.cell[%x+1, %y+1, %z];
	if(%xy0)
	{
		%h0 = 1;
		%ct2++;
	}
	%xy1 = %this.cell[%x+1, %y-1, %z];
	if(%xy1)
	{
		%h1 = 1;
		%ct2++;
	}
	%xy2 = %this.cell[%x-1, %y-1, %z];
	if(%xy2)
	{
		%h2 = 1;
		%ct2++;
	}
	%xy3 = %this.cell[%x-1, %y+1, %z];
	if(%xy3)
	{
		%h3 = 1;
		%ct2++;
	}
	%a2 = %h0 @ %h1 @ %h2 @ %h3;
	if(%ct2 > 2)
		return "Brick" @ %size @ "Cube1Data 0"; //Just fill in the hole.
	else if(%ct2 == 2)
		return -1;
	else if(%ct2 == 1)
		return "Brick" @ %size @ "CornerA1Data" SPC strStr(%a2, 1);

	return -1;
}

function TerrainCellMap::debugModTer(%this, %col, %size, %minX, %minY, %minZ, %maxX, %maxY, %maxZ)
{
	if(!isObject("Brick" @ %size @ "Cube1Data"))
		return;
	if(%minX $= "")
		%minX = %this.minX;
	if(%maxX $= "")
		%maxX = %this.maxX;
	if(%minY $= "")
		%minY = %this.minY;
	if(%maxY $= "")
		%maxY = %this.maxY;
	if(%minZ $= "")
		%minZ = %this.minZ;
	if(%maxZ $= "")
		%maxZ = %this.maxZ;

	%sX = (%size / 2);
	%sY = (%size / 2);
	%sZ = (%size / 2);

	for(%z = %minZ; %z <= %maxZ; %z++)
	{
		for(%y = %minY; %y <= %maxY; %y++)
		{
			for(%x = %minX; %x <= %maxX; %x++)
			{
				%m = %this.cell[%x, %y, %z];
				if(!%m)
					continue;

				%ter = %this.solveModTer(%x, %y, %z, %size);
				echo(%ter);
				if(%ter == -1)
					continue;

				%db = firstWord(%ter);
				%rot = getWord(%ter, 1);

				%xy = (%x * %sX) SPC (%y * %sY);

				%pos = %xy SPC ((%z+1) * %sZ);

				schedule(%i++ * 33, 0, simpBrick, %db, %pos, localClientConnection, %col, %rot);
			}
		}
	}
}