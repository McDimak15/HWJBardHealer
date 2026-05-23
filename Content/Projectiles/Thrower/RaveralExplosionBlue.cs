using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HWJBardHealer.Content.Projectiles.Thrower
{
    public class RaveralExplosionBlue : ModProjectile
    {
        public override string Texture =>
            "Terraria/Images/Projectile_" + ProjectileID.DD2ExplosiveTrapT3Explosion;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = Main.projFrames[ProjectileID.DD2ExplosiveTrapT3Explosion];
        }

        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 60;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 32;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Throwing;
            Projectile.scale = 0.9f;
        }

        public override void AI()
        {
            if (++Projectile.frameCounter >= 4)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;

                if (Projectile.frame >= Main.projFrames[Type])
                {
                    Projectile.Kill();
                }
            }

            Lighting.AddLight(Projectile.Center, 0.2f, 0.4f, 1.0f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;

            int frameHeight = tex.Height / Main.projFrames[Type];
            Rectangle frame = new Rectangle(0, Projectile.frame * frameHeight, tex.Width, frameHeight);

            Vector2 origin = frame.Size() / 2f;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;

            Color blueColor = new Color(80, 120, 255, 255);

            Main.EntitySpriteDraw(
                tex,
                drawPos,
                frame,
                blueColor,
                0f,
                origin,
                Projectile.scale,
                SpriteEffects.None,
                0
            );

            return false;
        }
    }
}
