using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using ThoriumMod;
using ThoriumMod.Projectiles.Bard;

namespace HWJBardHealer.Content.Projectiles.Bard
{
    public class DiurnalCycleStar : BardProjectile
    {
        public override string Texture => "HWJBardHealer/Content/Projectiles/Bard/Note";
        public override string GlowTexture => "Terraria/Images/Extra_98";
        public override BardInstrumentType InstrumentType => BardInstrumentType.Wind;

        private static Texture2D runtimeGlowTex;
        private static int runtimeGlowSize = 128;
        private static readonly Color BarColor = new Color(213, 40, 213);

        private Vector2 baseVelocity;
        private float waveOffset;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 25;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetBardDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 28;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 180;
            Projectile.extraUpdates = 1;
            Projectile.DamageType = ThoriumDamageBase<BardDamage>.Instance;
        }

        private void EnsureRuntimeGlow()
        {
            if (runtimeGlowTex != null) return;

            int size = runtimeGlowSize;
            Color[] colorData = new Color[size * size];

            Vector2 center = new Vector2(size / 2f, size / 2f);
            float maxDist = size / 2f;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    Vector2 pos = new Vector2(x + 0.5f, y + 0.5f);
                    float dist = Vector2.Distance(pos, center);
                    float t = MathHelper.Clamp(dist / maxDist, 0f, 1f);
                    float alpha = 1f - t;
                    alpha = alpha * alpha * (3f - 2f * alpha);
                    alpha = (float)Math.Pow(alpha, 0.9f);
                    Color c = new Color(255, 255, 255) * alpha;
                    colorData[y * size + x] = c;
                }
            }

            runtimeGlowTex = new Texture2D(Main.instance.GraphicsDevice, size, size);
            runtimeGlowTex.SetData(colorData);
        }

        public override void OnSpawn(IEntitySource source)
        {
            baseVelocity = Projectile.velocity.SafeNormalize(Vector2.UnitX) * 6f; 
            waveOffset = Main.rand.NextFloat(0f, MathHelper.TwoPi);
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0.7f, 0.6f, 0.9f);

            if (Main.rand.NextBool(4))
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Enchanted_Gold, 0f, 0f, 150, default, 1.3f);
                Main.dust[dust].velocity *= 0.3f;
                Main.dust[dust].noGravity = true;
            }

            float waveAmplitude = 16f; 
            float waveFrequency = 0.25f; 

            float sine = (float)Math.Sin(Main.GameUpdateCount * waveFrequency + waveOffset);
            Vector2 perpendicular = baseVelocity.RotatedBy(MathHelper.PiOver2).SafeNormalize(Vector2.Zero);
            Projectile.velocity = baseVelocity + perpendicular * sine * (waveAmplitude * 0.1f);

        }

        public new void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(SoundID.Item27, Projectile.position);
            for (int i = 0; i < 10; i++)
            {
                int dust = Dust.NewDust(Projectile.Center, 0, 0, DustID.GemAmethyst, 0f, 0f, 100, default, 1.3f);
                Main.dust[dust].velocity *= 2f;
                Main.dust[dust].noGravity = true;
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);
            for (int i = 0; i < 15; i++)
            {
                int dust = Dust.NewDust(Projectile.Center, 0, 0, DustID.Firework_Yellow, 0f, 0f, 120, default, 1.5f);
                Main.dust[dust].velocity *= 1.6f;
                Main.dust[dust].noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D glowTexture = ModContent.Request<Texture2D>(GlowTexture).Value;
            EnsureRuntimeGlow();

            float normalizedTime;
            if (Main.dayTime)
                normalizedTime = (float)(Main.time / Main.dayLength) * 0.5f + 0.25f; 
            else
                normalizedTime = (float)(Main.time / Main.nightLength) * 0.5f + 0.75f; 
            if (normalizedTime > 1f)
                normalizedTime -= 1f;

            float dayFactor = (float)(Math.Cos((normalizedTime - 0.5f) * MathHelper.TwoPi) * 0.5f + 0.5f);

            Color dayColor = new Color(252, 255, 174); 
            Color nightColor = new Color(213, 40, 213);
            Color dynamicColor = Color.Lerp(nightColor, dayColor, dayFactor);


            Main.EntitySpriteDraw(
                runtimeGlowTex,
                Projectile.Center - Main.screenPosition,
                null,
                dynamicColor * 0.35f,
                0f,
                runtimeGlowTex.Size() / 2f,
                0.5f,
                SpriteEffects.None,
                0
            );

            Vector2 center = Projectile.Center - Main.screenPosition;

            Main.EntitySpriteDraw(
                texture,
                center,
                null,
                dynamicColor,
                Projectile.rotation,
                Utils.Size(texture) / 2f,
                Projectile.scale,
                SpriteEffects.None
            );

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float fade = 1f - (i / (float)Projectile.oldPos.Length);
                fade *= 0.7f;

                Color trailColor = dynamicColor * fade;
                trailColor.A = 0;

                Vector2 drawPos = Projectile.oldPos[i] + new Vector2(Projectile.width / 2f, Projectile.height / 2f) - Main.screenPosition;
                if (i > 0)
                {
                    float rotation = Utils.ToRotation(Projectile.oldPos[i] - Projectile.oldPos[i - 1]) + MathHelper.PiOver2;
                    Main.EntitySpriteDraw(
                        glowTexture,
                        drawPos,
                        null,
                        trailColor,
                        rotation,
                        Utils.Size(glowTexture) / 2f,
                        new Vector2(0.8f, Projectile.scale),
                        SpriteEffects.None
                    );
                }
            }

            float pulse = 0.9f + (float)Math.Sin(Main.GameUpdateCount * 0.15f + Projectile.whoAmI) * 0.1f;
            Main.EntitySpriteDraw(
                glowTexture,
                center,
                null,
                dynamicColor * 0.6f * pulse,
                Projectile.rotation,
                Utils.Size(glowTexture) / 2f,
                Projectile.scale * 1.3f,
                SpriteEffects.None
            );

            return false;
        }

    }
}
