using Microsoft.Xna.Framework;
using System;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace TestMod;

public class ForbiddenFruit : ModProjectile
{
    public override void SetStaticDefaults()
    {
        Main.projFrames[Projectile.type] = 2;
    }

    public override void SetDefaults()
    {
        Projectile.width = 36;
        Projectile.height = 46;
        Projectile.DamageType = DamageClass.Generic;
        Projectile.friendly = true;
        Projectile.timeLeft = 180;
    }

    public override void AI()
    {
        Projectile.frameCounter++;
        if (Projectile.frameCounter == 5)
        {
            Projectile.frameCounter = 0;
            Projectile.frame = (Projectile.frame + 1) % 2;
        }

        var target = Projectile.FindTargetWithinRange(800);
        if (target is not null && (Projectile.ai[0] == -1 || !Main.npc[(int)Projectile.ai[0]].active))
            Projectile.ai[0] = target.whoAmI;
        else
        {
            Projectile.ai[0] = -1;
        }

        Projectile.velocity.Y += 0.2f;

        Projectile.direction = Projectile.spriteDirection = -Math.Sign(Projectile.velocity.X);
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        if (Projectile.ai[0] < 0 || !Main.npc.Any(t => t.active && Array.IndexOf(Main.npc, t) == (int)Projectile.ai[0]))
            return false;

        Projectile.velocity += Projectile.DirectionTo(Main.npc[(int)Projectile.ai[0]].Center) * 3;
        Projectile.velocity.Y -= 4;

        return false;
    }
}