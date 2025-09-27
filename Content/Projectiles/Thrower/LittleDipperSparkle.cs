using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Audio;

namespace HWJBardHealer.Content.Projectiles.Thrower
{
    public class LittleDipperSparkle : ModProjectile
    {
        public override string Texture => "HWJBardHealer/Content/Projectiles/Thrower/Sparkle";

        private const float DelayTicks = 30f;
        private float Timer => Projectile.localAI[0];

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
            Projectile.DamageType = DamageClass.Throwing;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.alpha = 255;
        }

        public override void AI()
        {
            Projectile.localAI[0]++;

            // fade in first 20 ticks, fade out last 15 ticks
            if (Projectile.localAI[0] < 20f)
                Projectile.alpha = (int)MathHelper.Lerp(255, 0, Projectile.localAI[0] / 20f);
            else if (Projectile.timeLeft < 15)
                Projectile.alpha = (int)MathHelper.Lerp(0, 255, (15 - Projectile.timeLeft) / 15f);
            else
                Projectile.alpha = 0;

            int targetId = (int)Projectile.ai[0];
            if (targetId < 0 || targetId >= Main.maxNPCs) { Projectile.Kill(); return; }
            NPC target = Main.npc[targetId];
            if (!target.active || target.friendly) { Projectile.Kill(); return; }

            if (Projectile.localAI[0] < DelayTicks)
            {
                // orbit around enemy
                float orbitSpeed = 0.05f;
                float radius = Math.Max(target.width, target.height) * 0.8f + 40f;
                float angle = (Projectile.ai[1] * MathHelper.TwoPi / 6f) + Projectile.localAI[0] * orbitSpeed;
                Projectile.Center = target.Center + radius * new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                Projectile.velocity = Vector2.Zero;
            }
            else if (Projectile.velocity == Vector2.Zero)
            {
                // dash 
                Vector2 dir = (target.Center - Projectile.Center).SafeNormalize(Vector2.UnitY);
                Projectile.velocity = dir * 6f;
            }

            // acceleration
            if (Projectile.velocity != Vector2.Zero)
                Projectile.velocity *= 1.02f;

            // star rotation
            Projectile.rotation += 0.2f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 origin = tex.Size() / 2f;
            Color drawColor = new Color(255, 255, 200) * (1f - Projectile.alpha / 255f);

            Main.EntitySpriteDraw(
                tex,
                Projectile.Center - Main.screenPosition,
                null,
                drawColor,
                Projectile.rotation, 
                origin,
                1f,
                SpriteEffects.None,
                0
            );

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // Puff 
            for (int i = 0; i < 8; i++)
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GoldCoin,
                    Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 2f));

            SoundEngine.PlaySound(SoundID.Item27 with { Volume = 0.8f, PitchVariance = 0.3f }, Projectile.Center);
            Projectile.Kill();
        }
    }
}
