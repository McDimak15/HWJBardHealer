using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace HWJBardHealer.Content.Projectiles.Thrower
{
    public class RaveralShard : ModProjectile
    {
        public override string Texture => "ContinentOfJourney/Projectiles/MeatPot";

        private int bounces = 3;

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;

            Projectile.friendly = true;
            Projectile.penetrate = -1;

            Projectile.timeLeft = 400;
            Projectile.DamageType = DamageClass.Throwing;

            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;
        }

        public override void AI()
        {
            Projectile.velocity.Y += 0.35f;

            Projectile.rotation += Projectile.velocity.X * 0.15f;

            if (Main.rand.NextBool(3))
            {
                Dust d = Dust.NewDustDirect(
                    Projectile.position,
                    Projectile.width,
                    Projectile.height,
                    DustID.FireworkFountain_Pink
                );
                d.noGravity = true;
                d.scale = Main.rand.NextFloat(0.8f, 1.3f);
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            bounces--;

            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);

            for (int i = 0; i < 6; i++)
            {
                Dust.NewDustDirect(
                    Projectile.position,
                    Projectile.width,
                    Projectile.height,
                    DustID.FireworkFountain_Pink,
                    Scale: 1f
                ).noGravity = true;
            }

            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = -oldVelocity.X * 0.75f;

            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y * 0.75f;

            return bounces < 0;
        }
    }
}
