using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace HWJBardHealer.Content.Projectiles.Healer
{
    public class RodScytheBeam : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.DeathLaser);
            AIType = ProjectileID.DeathLaser;

            Projectile.timeLeft = 120;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.alpha = 0;
        }

        public override void AI()
        {
            // Homing
            float homingSpeed = 12f;
            float homingRange = 800f;
            NPC closest = null;
            float closestDist = homingRange;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && !npc.friendly && !npc.dontTakeDamage)
                {
                    float dist = Vector2.Distance(npc.Center, Projectile.Center);
                    if (dist < closestDist)
                    {
                        closestDist = dist;
                        closest = npc;
                    }
                }
            }

            if (closest != null)
            {
                Vector2 direction = closest.Center - Projectile.Center;
                direction.Normalize();
                direction *= homingSpeed;

                Projectile.velocity = (Projectile.velocity * 20f + direction) / 21f;
            }

            Projectile.rotation = Projectile.velocity.ToRotation();

            Lighting.AddLight(Projectile.Center, 0.9f, 0.8f, 0.2f);
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (target.friendly) return false;
            return base.CanHitNPC(target);
        }

        public override bool CanHitPlayer(Player target) => false;
    }
}
