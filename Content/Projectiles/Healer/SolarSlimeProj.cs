using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;
using Terraria.GameContent;
using HWJBardHealer.Content.Buffs;

namespace HWJBardHealer.Content.Projectiles.Healer
{
    public class SolarSlimeProj : ModProjectile
    {
        private float fadeOut = 1f;
        private static Texture2D runtimeGlowTex;
        private static readonly int runtimeGlowSize = 128;

        private int markTimer;
        private float triangleFade = 0f;
        private int triangleCooldown = 0;
        private readonly List<Vector2> marks = new();

        private int storedItemType = -1; 

        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetDefaults()
        {
            Projectile.width = 36;
            Projectile.height = 22;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = int.MaxValue; 
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (!owner.active || owner.dead)
            {
                Projectile.Kill();
                return;
            }

            if (storedItemType == -1)
                storedItemType = owner.HeldItem?.type ?? -1;
            else if (owner.HeldItem?.type != storedItemType)
            {
                SoundEngine.PlaySound(SoundID.Item27 with { Volume = 0.8f, Pitch = -0.3f }, Projectile.Center);
                Projectile.Kill();
                return;
            }

            Vector2 offset = new(owner.direction * -40, -60);
            Projectile.Center = Vector2.Lerp(
                Projectile.Center,
                owner.Center + offset + new Vector2(0, (float)Math.Sin(Main.GlobalTimeWrappedHourly * 2f) * 6f),
                0.1f
            );
            Projectile.rotation += 0.03f;

            if (triangleCooldown > 0)
                triangleCooldown--;

            markTimer++;
            if (markTimer >= 180 && marks.Count < 3 && triangleCooldown <= 0)
            {
                markTimer = 0;

                Vector2 markPos = Projectile.Center + new Vector2(0, 10f);
                marks.Add(markPos);

                SoundEngine.PlaySound(SoundID.Item29 with { Volume = 0.6f }, markPos);
                for (int d = 0; d < 15; d++)
                {
                    Dust dust = Dust.NewDustPerfect(markPos, DustID.GoldFlame,
                        Main.rand.NextVector2Circular(2f, 2f), 150, Color.Yellow, 1.3f);
                    dust.noGravity = true;
                }

                if (marks.Count == 3)
                {
                    Projectile.localAI[0] = 1500;
                    triangleFade = 0f;
                }
            }

            if (marks.Count == 3)
            {
                if (Projectile.localAI[0] > 0)
                    Projectile.localAI[0]--;

                if (Projectile.localAI[0] > 1440)
                    triangleFade = MathHelper.Lerp(triangleFade, 1f, 0.1f);
                else if (Projectile.localAI[0] < 60)
                    triangleFade = MathHelper.Lerp(triangleFade, 0f, 0.1f);
                else
                    triangleFade = 1f;

                Vector2 a = marks[0];
                Vector2 b = marks[1];
                Vector2 c = marks[2];

                foreach (Player p in Main.player)
                {
                    if (p.active && !p.dead && IsPointInTriangle(p.Center, a, b, c))
                        p.AddBuff(ModContent.BuffType<SolarTriangleBuff>(), 2);
                }

                if (Main.rand.NextBool(3))
                {
                    Vector2 pos = a + Main.rand.NextFloat() * (b - a) + Main.rand.NextFloat() * (c - a);
                    if (IsPointInTriangle(pos, a, b, c))
                    {
                        Dust dust = Dust.NewDustPerfect(pos, DustID.GoldFlame,
                            Main.rand.NextVector2Circular(1f, 1f), 150, Color.Goldenrod, 1.1f);
                        dust.noGravity = true;
                    }
                }

                Lighting.AddLight((a + b + c) / 3f, 1.5f, 1.2f, 0.6f);

                if (Projectile.localAI[0] == 60)
                {
                    SoundEngine.PlaySound(SoundID.Item27 with
                    {
                        Volume = 0.8f,
                        Pitch = -0.3f
                    }, Projectile.Center);

                    for (int i = 0; i < 20; i++)
                    {
                        Vector2 flashPos = a + Main.rand.NextFloat() * (b - a) + Main.rand.NextFloat() * (c - a);
                        Dust.NewDustPerfect(flashPos, DustID.GoldFlame,
                            Main.rand.NextVector2Circular(3f, 3f), 150, Color.Goldenrod, 1.4f).noGravity = true;
                    }
                }

                if (Projectile.localAI[0] <= 0)
                {
                    marks.Clear();
                    triangleFade = 0f;
                    triangleCooldown = 600; 
                }
            }
        }

        private static bool IsPointInTriangle(Vector2 p, Vector2 a, Vector2 b, Vector2 c)
        {
            float area = 0.5f * (-b.Y * c.X + a.Y * (-b.X + c.X) + a.X * (b.Y - c.Y) + b.X * c.Y);
            float s = 1f / (2f * area) *
                (a.Y * c.X - a.X * c.Y + (c.Y - a.Y) * p.X + (a.X - c.X) * p.Y);
            float t = 1f / (2f * area) *
                (a.X * b.Y - a.Y * b.X + (a.Y - b.Y) * p.X + (b.X - a.X) * p.Y);
            return s >= 0 && t >= 0 && (s + t) <= 1;
        }

        private void EnsureRuntimeGlow()
        {
            if (runtimeGlowTex != null)
                return;

            int size = runtimeGlowSize;
            Color[] colorData = new Color[size * size];
            Vector2 center = new(size / 2f);
            float maxDist = size / 2f;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dist = Vector2.Distance(new Vector2(x, y), center);
                    float t = MathHelper.Clamp(dist / maxDist, 0f, 1f);
                    float alpha = 1f - t;
                    alpha = alpha * alpha * (3f - 2f * alpha);
                    colorData[y * size + x] = new Color(255, 230, 100) * alpha;
                }
            }

            runtimeGlowTex = new Texture2D(Main.instance.GraphicsDevice, size, size);
            runtimeGlowTex.SetData(colorData);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch sb = Main.spriteBatch;
            Texture2D slimeTex = ModContent.Request<Texture2D>("HWJBardHealer/Content/Projectiles/Bard/SmallSlimes").Value;
            Texture2D backTex = ModContent.Request<Texture2D>("HWJBardHealer/Content/Projectiles/Healer/SolarSlimeBack").Value;
            Texture2D markTex = ModContent.Request<Texture2D>("HWJBardHealer/Content/Projectiles/Healer/SlimeMarkProj").Value;
            Texture2D linkTex = TextureAssets.Extra[98].Value;
            EnsureRuntimeGlow();

            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            sb.Draw(backTex, drawPos, null, Color.Orange * 0.6f * fadeOut, -Projectile.rotation,
                backTex.Size() / 2, 1f, SpriteEffects.None, 0f);
            sb.Draw(runtimeGlowTex, drawPos, null, Color.White * 0.8f * fadeOut, 0f,
                new Vector2(runtimeGlowSize / 2f), 0.6f, SpriteEffects.None, 0f);

            Rectangle frameRect = new(0, 8 * 22, 36, 22);
            sb.Draw(slimeTex, drawPos, frameRect, Color.White * fadeOut, Projectile.rotation,
                new Vector2(18, 11), 1f, SpriteEffects.None, 0f);

            if (triangleFade > 0.05f && marks.Count == 3)
            {
                Color beamColor = Color.Lerp(Color.OrangeRed, Color.Gold,
                    (float)Math.Sin(Main.GlobalTimeWrappedHourly * 2f) * 0.5f + 0.5f);
                beamColor *= triangleFade * fadeOut;

                for (int i = 0; i < 3; i++)
                    DrawHorizontalBeam(sb, linkTex, marks[i] - Main.screenPosition,
                        marks[(i + 1) % 3] - Main.screenPosition, beamColor);
            }

            foreach (var mark in marks)
            {
                Vector2 pos = mark - Main.screenPosition;
                float pulse = (float)(Math.Sin(Main.GlobalTimeWrappedHourly * 3f + mark.X) * 0.2f + 0.8f);
                sb.Draw(markTex, pos, null, Color.White * 0.9f * fadeOut,
                    0f, markTex.Size() / 2f, 1f, SpriteEffects.None, 0f);
            }

            return false;
        }

        private void DrawHorizontalBeam(SpriteBatch sb, Texture2D tex, Vector2 start, Vector2 end, Color color)
        {
            Vector2 diff = end - start;
            float rotation = diff.ToRotation();
            float length = diff.Length();
            float stepLength = tex.Width * 0.25f;
            int segments = Math.Max(1, (int)(length / stepLength));
            float adjustedRotation = rotation + MathHelper.PiOver2;

            for (int i = 0; i <= segments; i++)
            {
                float t = i / (float)segments;
                Vector2 pos = Vector2.Lerp(start, end, t);
                float pulse = (float)(Math.Sin(Main.GlobalTimeWrappedHourly * 4f + t * 6f) * 0.25f + 0.75f);
                Color drawColor = color * pulse;

                sb.Draw(tex, pos, null, drawColor, adjustedRotation, tex.Size() / 2f,
                    new Vector2(0.25f, 0.8f), SpriteEffects.None, 0f);
                sb.Draw(tex, pos, null, Color.White * 0.3f * pulse * triangleFade,
                    adjustedRotation, tex.Size() / 2f, new Vector2(0.3f, 1.2f), SpriteEffects.None, 0f);
            }
        }
    }
}
