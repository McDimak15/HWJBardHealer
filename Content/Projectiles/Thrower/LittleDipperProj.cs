using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace HWJBardHealer.Content.Projectiles.Thrower
{
    public class LittleDipperProj : ModProjectile
    {
        public override string Texture => "HWJBardHealer/Content/Weapons/Thrower/LittleDipper";

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.DamageType = DamageClass.Throwing;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Player player = Main.player[Projectile.owner];

            // burst of dust at throw
            for (int d = 0; d < 12; d++)
            {
                Vector2 vel = Main.rand.NextVector2Circular(2f, 2f);
                Dust dust = Dust.NewDustDirect(Projectile.Center - Vector2.One * 6f, 12, 12, DustID.GoldFlame);
                dust.velocity = vel + Projectile.velocity * 0.2f;
                dust.noGravity = true;
                dust.scale = Main.rand.NextFloat(1.1f, 1.6f);
            }

            SoundEngine.PlaySound(SoundID.Item8 with { Volume = 0.7f, Pitch = -0.2f }, Projectile.Center);
        }

        public override void AI()
        {
            // light
            Lighting.AddLight(Projectile.Center, 0.3f, 0.4f, 0.1f);

            // Rotation
            if (Projectile.velocity.LengthSquared() > 0.1f)
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            // Trail 
            if (Main.rand.NextBool(2))
            {
                Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.GoldFlame);
                d.velocity = Projectile.velocity * 0.2f;
                d.noGravity = true;
                d.scale = Main.rand.NextFloat(0.8f, 1.2f);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SpawnSparkles(target, damageDone);

            // extra hit burst
            for (int i = 0; i < 20; i++)
            {
                Dust dust = Dust.NewDustDirect(target.position, target.width, target.height, DustID.GoldFlame);
                dust.velocity = Main.rand.NextVector2Circular(3f, 3f);
                dust.noGravity = true;
                dust.scale = Main.rand.NextFloat(1f, 1.5f);
            }

            SoundEngine.PlaySound(SoundID.Item27 with { Volume = 0.9f, PitchVariance = 0.2f }, target.Center);
        }

        // burst VFX
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            for (int i = 0; i < 16; i++)
            {
                Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.GoldFlame);
                d.velocity = Main.rand.NextVector2Circular(2.5f, 2.5f);
                d.noGravity = true;
            }

            SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
            return true;
        }

        private void SpawnSparkles(NPC target, int damageDone)
        {
            int count = 6;
            float radius = 60f;

            for (int i = 0; i < count; i++)
            {
                float angle = MathHelper.TwoPi / count * i;
                Vector2 offset = new Vector2(radius, 0).RotatedBy(angle);

                int sparkle = Projectile.NewProjectile(
                    Projectile.GetSource_OnHit(target),
                    target.Center + offset,
                    Vector2.Zero,
                    ModContent.ProjectileType<LittleDipperSparkle>(),
                    damageDone / 5,
                    0f,
                    Projectile.owner,
                    target.whoAmI,
                    i
                );

                Main.projectile[sparkle].ai[0] = target.whoAmI;
            }
        }
    }
}
