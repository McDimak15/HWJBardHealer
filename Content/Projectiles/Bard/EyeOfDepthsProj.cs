using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using ThoriumMod;
using ThoriumMod.Projectiles.Bard;

namespace HWJBardHealer.Content.Projectiles.Bard
{
    public class EyeOfDepthsProj : BardProjectile
    {
        public override string Texture => "CalamityMod/Particles/HollowCircleHardEdge";
        public override BardInstrumentType InstrumentType => BardInstrumentType.Electronic;

        public override void SetBardDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = ThoriumDamageBase<BardDamage>.Instance;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 1;
            Projectile.timeLeft = 60;
            Projectile.alpha = 255;
        }

        public override void AI()
        {
            // fade in/out
            if (Projectile.timeLeft < 15)
                Projectile.alpha = Math.Min(255, Projectile.alpha + 20);
            else
                Projectile.alpha = Math.Max(0, Projectile.alpha - 20);

            Projectile.velocity *= 1.04f;
            Projectile.scale *= 1.04f;

            Projectile.position += Projectile.velocity;

            Vector2 oldCenter = Projectile.Center;
            int newSize = (int)(40 * Projectile.scale);
            Projectile.width = newSize;
            Projectile.height = newSize;
            Projectile.Center = oldCenter;

            // face velocity
            if (Projectile.velocity.LengthSquared() > 0.1f)
                Projectile.rotation = Projectile.velocity.ToRotation();

            // bubble trail
            for (int i = 0; i < 2; i++)
            {
                int dust = Dust.NewDust(
                    Projectile.Center,
                    0, 0,
                    DustID.BubbleBurst_Blue,
                    Main.rand.NextFloat(-1.8f, 1.8f),
                    Main.rand.NextFloat(-1.8f, 1.8f),
                    100,
                    default,
                    Main.rand.NextFloat(0.9f, 1.4f)
                );
                Main.dust[dust].noGravity = true;
                Main.dust[dust].fadeIn = 1.2f;
            }

            Lighting.AddLight(Projectile.Center, 0.2f, 0.4f, 0.8f);
        }

        public override void BardOnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.damage = Math.Max(1, Projectile.damage - 4);

            // bubbles
            for (int i = 0; i < 20; i++)
            {
                Vector2 vel = Main.rand.NextVector2Circular(3f, 3f);
                int dust = Dust.NewDust(target.Center, 0, 0, DustID.BubbleBurst_Blue, vel.X, vel.Y, 100, default, 1.5f);
                Main.dust[dust].noGravity = true;
            }

            // mini bubble
            int numProjectiles = 5;
            for (int i = 0; i < numProjectiles; i++)
            {
                Vector2 velocity = Main.rand.NextVector2CircularEdge(3f, 3f);
                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    target.Center,
                    velocity,
                    ProjectileID.WaterStream,
                    (int)(damageDone * 0.5f),
                    0f,
                    Projectile.owner
                );
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 origin = texture.Size() / 2f;
            float fade = 1f - Projectile.alpha / 255f;

            // glow 
            Color glowColor = new Color(100, 120, 200, 0) * fade * 0.6f;
            Main.EntitySpriteDraw(
                texture,
                Projectile.Center - Main.screenPosition,
                null,
                glowColor,
                Projectile.rotation,
                origin,
                new Vector2(28f / texture.Width, 56f / texture.Height) * Projectile.scale,
                SpriteEffects.None,
                0
            );

            // outer ring
            Color edgeColor = new Color(63, 66, 110, 0) * fade;
            Main.EntitySpriteDraw(
                texture,
                Projectile.Center - Main.screenPosition,
                null,
                edgeColor,
                Projectile.rotation,
                origin,
                new Vector2(22f / texture.Width, 44f / texture.Height) * Projectile.scale,
                SpriteEffects.None,
                0
            );

            // inner ring
            Color centerColor = new Color(120, 130, 200, 0) * fade;
            Main.EntitySpriteDraw(
                texture,
                Projectile.Center - Main.screenPosition,
                null,
                centerColor,
                Projectile.rotation,
                origin,
                new Vector2(20f / texture.Width, 40f / texture.Height) * Projectile.scale,
                SpriteEffects.None,
                0
            );

            return false;
        }
    }
}
