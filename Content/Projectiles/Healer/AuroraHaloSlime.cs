using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;
using Terraria.GameContent;
using HWJBardHealer.Core;
using ThoriumMod;
using ContinentOfJourney.Items.Armor;
using HWJBardHealer.Content.Armor.Healer;

namespace HWJBardHealer.Content.Projectiles.Healer
{
    public class AuroraHaloSlime : ModProjectile
    {
        private static Texture2D runtimeGlowTex;
        private static readonly int runtimeGlowSize = 128;
        private int cycleTimer = 0;
        private int currentSlimeFrame = 8;
        private const int MaxSlimeFrames = 10;
        private int targetAllyIndex = -1;

        public override string Texture => "Terraria/Images/Projectile_0";
        public override string GlowTexture => "Terraria/Images/Extra_98";

        public override void SetStaticDefaults()
        {
            Main.projPet[Projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 36;
            Projectile.height = 22;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.netImportant = true;
            Projectile.timeLeft = 2;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];

            Item headItem = owner.armor[0];
            bool hasSet = headItem.type == ModContent.ItemType<AuroraHalo>() &&
                          headItem.ModItem.IsArmorSet(owner.armor[0], owner.armor[1], owner.armor[2]);

            if (!owner.active || owner.dead || !hasSet)
            {
                Projectile.Kill();
                return;
            }
            Projectile.timeLeft = 2;
            cycleTimer++;

            Vector2 idlePos = owner.Center + new Vector2(owner.direction * -40, -60);

            // Seeking
            if (cycleTimer <= 150)
            {
                if (cycleTimer == 1 && Main.myPlayer == Projectile.owner)
                {
                    FindNearestAlly(owner);
                    Projectile.netUpdate = true;
                }

                Vector2 destination = (targetAllyIndex != -1 && Main.player[targetAllyIndex].active)
                    ? Main.player[targetAllyIndex].Center
                    : idlePos;

                MoveTowards(destination, 18f, 15f);

                if (cycleTimer == 149 && Main.myPlayer == Projectile.owner && targetAllyIndex != -1)
                {
                    Player target = Main.player[targetAllyIndex];
                    if (Projectile.Distance(target.Center) < 120f)
                    {
                        HealerHelper.HealPlayer(owner, target, 1);
                        for (int i = 0; i < 15; i++)
                            Dust.NewDust(target.position, target.width, target.height, DustID.GemDiamond);
                    }
                }
            }
            // Returning
            else if (cycleTimer <= 300)
            {
                MoveTowards(idlePos, 20f, 12f);
            }
            // Waiting
            else
            {
                if (cycleTimer == 301)
                {
                    Projectile.velocity *= 0.1f;
                    ChangeSlimeForm();
                }

                idlePos.Y += (float)Math.Sin(Main.GlobalTimeWrappedHourly * 2.5f) * 8f;
                Projectile.Center = Vector2.Lerp(Projectile.Center, idlePos, 0.12f);
                Projectile.velocity *= 0.8f;

                if (cycleTimer >= 450) cycleTimer = 0;
            }

            // Tilt
            float speedTilt = Projectile.velocity.X * 0.07f;
            float idleSway = (cycleTimer > 300) ? (float)Math.Sin(Main.GlobalTimeWrappedHourly * 2f) * 0.12f : 0f;
            Projectile.rotation = MathHelper.Lerp(Projectile.rotation, speedTilt + idleSway, 0.1f);
        }

        private void MoveTowards(Vector2 target, float maxSpeed, float inertia)
        {
            Vector2 desiredVelocity = target - Projectile.Center;
            float distance = desiredVelocity.Length();

            if (distance < 10f)
            {
                Projectile.velocity *= 0.8f;
                return;
            }

            desiredVelocity.Normalize();

            float speed = (distance < 120f) ? MathHelper.Lerp(3f, maxSpeed, distance / 120f) : maxSpeed;

            desiredVelocity *= speed;
            Projectile.velocity = (Projectile.velocity * (inertia - 1f) + desiredVelocity) / inertia;
        }

        private void FindNearestAlly(Player owner)
        {
            float nearestDist = 800f; 
            targetAllyIndex = -1;

            for (int i = 0; i < 255; i++)
            {
                Player ally = Main.player[i];
                if (ally.active && !ally.dead && ally.whoAmI != owner.whoAmI && ally.team == owner.team && owner.team != 0)
                {
                    float dist = Vector2.Distance(owner.Center, ally.Center);
                    if (dist < nearestDist)
                    {
                        nearestDist = dist;
                        targetAllyIndex = i;
                    }
                }
            }
            if (targetAllyIndex == -1) targetAllyIndex = owner.whoAmI;
        }

        private void ChangeSlimeForm()
        {
            int nextFrame;
            do { nextFrame = Main.rand.Next(0, MaxSlimeFrames); } while (nextFrame == currentSlimeFrame);
            currentSlimeFrame = nextFrame;

            SoundEngine.PlaySound(SoundID.Item27 with { Pitch = 0.5f, Volume = 0.7f }, Projectile.Center);
            for (int i = 0; i < 15; i++)
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Cloud, 0, 0, 100, Color.White, 0.8f).noGravity = true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch sb = Main.spriteBatch;
            Texture2D slimeTex = ModContent.Request<Texture2D>("HWJBardHealer/Content/Projectiles/Bard/SmallSlimes").Value;
            EnsureRuntimeGlow();

            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Color glowColor = GetSlimeColor(slimeTex);
            glowColor.A = 0;

            sb.Draw(runtimeGlowTex, drawPos, null, glowColor * 0.5f, 0f, new Vector2(runtimeGlowSize / 2f), 0.7f, SpriteEffects.None, 0f);
            Rectangle frameRect = new(0, currentSlimeFrame * 22, 36, 22);
            sb.Draw(slimeTex, drawPos, frameRect, Color.White, Projectile.rotation, new Vector2(18, 11), 1f, SpriteEffects.None, 0f);

            return false;
        }

        private Color GetSlimeColor(Texture2D tex)
        {
            try
            {
                Color[] pixels = new Color[1];
                tex.GetData(0, new Rectangle(18, (currentSlimeFrame * 22) + 11, 1, 1), pixels, 0, 1);
                return pixels[0];
            }
            catch { return Color.Cyan; }
        }

        private void EnsureRuntimeGlow()
        {
            if (runtimeGlowTex != null) return;
            int size = runtimeGlowSize;
            Color[] colorData = new Color[size * size];
            Vector2 center = new(size / 2f);
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dist = Vector2.Distance(new Vector2(x, y), center);
                    float alpha = (float)Math.Pow(MathHelper.Clamp(1f - (dist / (size / 2f)), 0f, 1f), 3);
                    colorData[y * size + x] = Color.White * alpha;
                }
            }
            runtimeGlowTex = new Texture2D(Main.instance.GraphicsDevice, size, size);
            runtimeGlowTex.SetData(colorData);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(cycleTimer);
            writer.Write(targetAllyIndex);
            writer.Write(currentSlimeFrame);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            cycleTimer = reader.ReadInt32();
            targetAllyIndex = reader.ReadInt32();
            currentSlimeFrame = reader.ReadInt32();
        }
    }
}