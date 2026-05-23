using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace HWJBardHealer.Content.Projectiles.Thrower
{
    public class RaveralProj : ModProjectile
    {
        public override string Texture => "HWJBardHealer/Content/Weapons/Thrower/Raveral";

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.scale = 1.55f;

            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;

            Projectile.DamageType = DamageClass.Throwing;

            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            for (int i = 0; i < 10; i++)
            {
                Dust d = Dust.NewDustDirect(
                    Projectile.Center - Vector2.One * 6f,
                    12, 12,
                    DustID.FireworkFountain_Red
                );

                d.velocity = Main.rand.NextVector2Circular(2f, 2f) + Projectile.velocity * 0.15f;
                d.noGravity = true;
                d.scale = Main.rand.NextFloat(1f, 1.6f);
            }

            SoundEngine.PlaySound(SoundID.Item8 with { Volume = 0.7f, Pitch = 0.15f }, Projectile.Center);
        }

        public override void AI()
        {
            Projectile.velocity.Y += 0.4f;

            if (Projectile.velocity.LengthSquared() > 0.2f)
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            Lighting.AddLight(Projectile.Center, 0.2f, 0.3f, 0.9f);

            if (Main.rand.NextBool(2))
            {
                Dust d = Dust.NewDustDirect(
                    Projectile.position,
                    Projectile.width,
                    Projectile.height,
                    DustID.FireworkFountain_Pink
                );
                d.velocity = Projectile.velocity * 0.15f;
                d.noGravity = true;
                d.scale = Main.rand.NextFloat(0.8f, 1.2f);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SpawnBlueExplosion(damageDone);
            ExplodeIntoShards(target); 
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            SpawnBlueExplosion(Projectile.damage);
            ExplodeIntoShards(); 
            return true;
        }

        private void SpawnBlueExplosion(int damageDone)
        {
            for (int i = 0; i < 35; i++)
            {
                Dust d = Dust.NewDustDirect(
                    Projectile.Center - new Vector2(20, 20),
                    40, 40,
                    DustID.BlueTorch
                );

                d.velocity = Main.rand.NextVector2Circular(6f, 6f);
                d.scale = Main.rand.NextFloat(1.3f, 1.9f);
                d.noGravity = true;
            }

            SoundEngine.PlaySound(SoundID.Item14 with { Pitch = 0.3f }, Projectile.Center);
        }

        private void ExplodeIntoShards()
        {
            ExplodeIntoShards(null);
        }

        private void ExplodeIntoShards(NPC target)
        {
            if (Projectile.localAI[0] == 1f)
                return;

            Projectile.localAI[0] = 1f;

            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);

            Vector2 explosionBasePos = (target != null) ? (target.Center - new Vector2(0, 25)) : (Projectile.Center - new Vector2(0, 25));

            Projectile.NewProjectile(
                Projectile.GetSource_FromThis(),
                explosionBasePos,
                Vector2.Zero,
                ModContent.ProjectileType<RaveralExplosionBlue>(),
                (int)(Projectile.damage * 0.9f),
                2f,
                Projectile.owner
            );

            for (int i = 0; i < 20; i++)
            {
                Dust d = Dust.NewDustDirect(
                    Projectile.position,
                    Projectile.width,
                    Projectile.height,
                    DustID.FireworkFountain_Pink
                );
                d.velocity = Main.rand.NextVector2Circular(4f, 4f);
                d.noGravity = true;
            }

            int count = Main.rand.Next(3, 6);
            for (int i = 0; i < count; i++)
            {
                float angle = MathHelper.ToRadians(Main.rand.NextFloat(-30f, 30f));
                Vector2 vel = new Vector2(0, -7f).RotatedBy(angle);

                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    Projectile.Center,
                    vel,
                    ModContent.ProjectileType<RaveralShard>(),
                    Projectile.damage / 2,
                    Projectile.knockBack,
                    Projectile.owner
                );
            }
        }
    }
}
