using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.DataStructures;
using Terraria.GameContent; 
using System;
using System.IO;
using HWJBardHealer.Content.Weapons.Healer;
using ThoriumMod.Utilities; 


namespace HWJBardHealer.Content.Projectiles.Healer
{
    public class SerenityBlessingProj : ModProjectile
    {
        private int flareTimer;
        private int flareDelayTimer;

        private bool exploded;
        private bool wasMousePressedLastTick;

        private const int MaxIllumination = 210;
        private const float Radius = 160f;

        private bool explosionActive;
        private float explosionScale;
        private float explosionAlpha;

        private static Texture2D runtimeGlowTex;
        private static int runtimeGlowSize = 128;
        private static readonly Color BarColor = new Color(213, 40, 213);

        private const int BaseInterval = 60; 

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.timeLeft = 2;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (Projectile.localAI[0] == 0f)
            {
                Projectile.localAI[0] = 1f;
                Projectile.ai[0] = 100f; 
                SoundEngine.PlaySound(SoundID.Item44 with { Volume = 0.8f }, player.Center);
                Projectile.netUpdate = true;
            }

            Projectile.timeLeft = 2;
            Projectile.Center = player.Center + new Vector2(0, -50f);

            if (!player.active || player.dead || player.HeldItem.type != ModContent.ItemType<Weapons.Healer.Serenity>())
            {
                if (!exploded) Projectile.Kill();
                return;
            }

            if (explosionActive)
            {
                Projectile.timeLeft = Math.Max(Projectile.timeLeft, 2);
                explosionScale += 0.08f;
                explosionAlpha -= 0.05f;
                if (explosionAlpha <= 0f)
                    Projectile.Kill();
                NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, Projectile.whoAmI);
                return;
            }

            if (Main.myPlayer == Projectile.owner && Projectile.ai[0] > 0)
            {
                if (Main.GameUpdateCount % 3 == 0)
                {
                    Projectile.ai[0]--;
                    Projectile.netUpdate = true;
                }
            }


            bool mousePressed = Main.mouseLeft;
            if (mousePressed && !wasMousePressedLastTick)
            {
                Projectile.ai[0] = Math.Min(MaxIllumination, Projectile.ai[0] + 5f);
                SoundEngine.PlaySound(SoundID.Item4 with { Volume = 0.6f, Pitch = 0.2f }, Projectile.Center);

                for (int i = 0; i < 8; i++)
                {
                    int d = Dust.NewDust(Projectile.Center, 0, 0, DustID.PinkTorch,
                        Main.rand.NextFloat(-2, 2), Main.rand.NextFloat(-2, 2), 100, default, 1.2f);
                    Main.dust[d].noGravity = true;
                }

                Projectile.netUpdate = true;
            }
            wasMousePressedLastTick = mousePressed;

            if (Projectile.ai[0] <= 0 && !exploded)
            {
                Explosion(player);
                return;
            }

            flareTimer++;
            if (flareTimer >= BaseInterval)
            {
                flareTimer = 0;
                flareDelayTimer = 0;

                int flareCount = Math.Min(4, Math.Max(1, (int)(Projectile.ai[0] / 50f)));
                int healPerFlare = flareCount switch
                {
                    1 => 6,
                    2 => 3,
                    3 => 2,
                    4 => 2,
                    _ => 6
                };

                Projectile.localAI[1] = flareCount;
                Projectile.localAI[2] = healPerFlare;
            }

            if (Projectile.localAI[1] > 0)
            {
                flareDelayTimer++;
                if (flareDelayTimer >= 8) 
                {
                    flareDelayTimer = 0;
                    FireFlare(player, (int)Projectile.localAI[2]);
                    Projectile.localAI[1]--;
                }
            }

            float illumNorm = Projectile.ai[0] / MaxIllumination;
            Lighting.AddLight(Projectile.Center, new Vector3(1.2f, 0.3f, 1.2f) * illumNorm);
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

        // Flares
        private void FireFlare(Player player, int healAmount)
        {
            Serenity serenity = player.HeldItem.ModItem as Serenity;
            Player target = null;

            if (serenity != null && serenity.favoritePlayer >= 0)
            {
                Player fav = Main.player[serenity.favoritePlayer];
                if (fav.active && !fav.dead && fav.team == player.team)
                    target = fav;
            }

            if (target == null)
            {
                foreach (Player p in Main.player)
                {
                    if (!p.active || p.dead || p.whoAmI == player.whoAmI) continue;
                    if (p.team != player.team) continue;
                    target = p;
                    break;
                }
            }

            if (target == null) return;

            Vector2 velocity = (target.Center - Projectile.Center).SafeNormalize(Vector2.UnitY) * 6f;
            int pid = Projectile.NewProjectile(
                Projectile.GetSource_FromThis(),
                Projectile.Center,
                velocity,
                ModContent.ProjectileType<SerenityFlareProj>(),
                0,
                0f,
                Projectile.owner,
                target.whoAmI,
                0f
            );

            if (pid >= 0 && pid < Main.maxProjectiles)
            {
                Main.projectile[pid].localAI[0] = healAmount;
                NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, Projectile.whoAmI);
                Main.projectile[pid].netUpdate = true;
            }
        
        }

        private void Explosion(Player player)
        {
            if (exploded) return;
            exploded = true;

            int dmg = 800;
            SoundEngine.PlaySound(SoundID.Item14, player.Center);

            for (int i = 0; i < 40; i++)
                Dust.NewDust(player.position, player.width, player.height, DustID.Smoke, Main.rand.NextFloat(-3, 3), Main.rand.NextFloat(-3, 3));

            SoundEngine.PlaySound(SoundID.Item14 with { Pitch = -0.2f }, player.Center);

            for (int i = 0; i < 30; i++)
            {
                Vector2 vel = Main.rand.NextVector2Circular(4f, 4f);
                int dust = Dust.NewDust(Projectile.Center, 0, 0, DustID.PinkTorch, vel.X, vel.Y, 150, default, 1.5f);
                Main.dust[dust].noGravity = true;
            }

            explosionActive = true;
            explosionScale = 0f;
            explosionAlpha = 1f;

                foreach (NPC npc in Main.npc)
                {
                    if (npc.active && !npc.friendly && !npc.dontTakeDamage &&
                        Vector2.Distance(npc.Center, player.Center) < 600f)
                    {
                        int direction = npc.Center.X > player.Center.X ? 1 : -1;
                        NPC.HitInfo hitInfo = new NPC.HitInfo
                        {
                            Damage = dmg,
                            Knockback = 0f,
                            HitDirection = direction,
                            Crit = false
                        };
                        npc.StrikeNPC(hitInfo);
                        NetMessage.SendStrikeNPC(npc, hitInfo);
                    }
                }

                int bigDamage = 150;
                player.Hurt(
                    PlayerDeathReason.ByCustomReason(NetworkText.FromLiteral($"{player.name} was consumed by failing Serenity.")),
                    bigDamage,
                    0
                );
            NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, Projectile.whoAmI);

            if (Main.myPlayer == player.whoAmI)
            {
                for (int i = 0; i < 5; i++)
                {
                    Vector2 pulseVel = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi) * 12f;
                    Dust.NewDustPerfect(player.Center, DustID.PinkTorch, pulseVel, 150, Color.Magenta, 3f).noGravity = true;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>("HWJBardHealer/Content/Projectiles/Healer/SerenityBlessingProj").Value;

            EnsureRuntimeGlow();

            float glowAmount = (float)Projectile.ai[0] / MaxIllumination;
            float pulse = 1f + (float)Math.Sin(Main.GlobalTimeWrappedHourly * 6f) * 0.05f;
            float glowScale = (0.8f + 0.6f * glowAmount) * pulse;

            float minGlow = 0.25f;
            float strength = minGlow + 0.75f * glowAmount;
            Color glowTint = new Color(255, 100, 255) * strength;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive,
                SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null,
                Main.GameViewMatrix.TransformationMatrix);

            Main.EntitySpriteDraw(
                runtimeGlowTex,
                Projectile.Center - Main.screenPosition,
                null,
                glowTint,
                0f,
                runtimeGlowTex.Size() / 2f,
                glowScale * (runtimeGlowSize / 64f),
                SpriteEffects.None,
                0
            );

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend,
                SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null,
                Main.GameViewMatrix.TransformationMatrix);

            float scale = (1f + (Projectile.ai[0] / (float)MaxIllumination) * 0.2f) * 1.2f;
            Color idolColor = Color.White;

            Main.EntitySpriteDraw(
                tex,
                Projectile.Center - Main.screenPosition,
                null,
                idolColor,
                0f,
                tex.Size() / 2f,
                scale,
                SpriteEffects.None,
                0
            );

            // Bar
            Vector2 barWorldPos = Projectile.Center + new Vector2(0f, 90f); 
            Vector2 barScreenPos = barWorldPos - Main.screenPosition;
            float progress = Projectile.ai[0] / (float)MaxIllumination;
            int barWidth = 90;
            int barHeight = 10;

            Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value,
                new Rectangle((int)barScreenPos.X - barWidth / 2, (int)barScreenPos.Y, barWidth, barHeight),
                Color.Black * 0.6f);

            Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value,
                new Rectangle((int)barScreenPos.X - barWidth / 2, (int)barScreenPos.Y, (int)(barWidth * progress), barHeight),
                BarColor);

            Texture2D frameTex = ModContent.Request<Texture2D>("HWJBardHealer/Content/Projectiles/Healer/SerenityFrame").Value;

            Vector2 frameOrigin = frameTex.Size() / 2f;
            Vector2 framePos = barWorldPos - Main.screenPosition;

            float frameScale = 0.6f;

            Main.EntitySpriteDraw(
                frameTex,
                framePos,
                null,
                Color.White,
                0f,
                frameOrigin,
                frameScale,
                SpriteEffects.None,
                0
            );

            if (explosionActive)
            {
                Texture2D circleTex = ModContent.Request<Texture2D>("HWJBardHealer/Content/Projectiles/Healer/FadeCircle").Value;
                float baseScale = Radius / (circleTex.Width / 2f);
                float drawScale = baseScale * explosionScale * 2.5f;
                Color explosionColor = new Color(255, 80, 255, 200) * explosionAlpha;

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive,
                    SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null,
                    Main.GameViewMatrix.TransformationMatrix);

                Main.EntitySpriteDraw(
                    circleTex,
                    Projectile.Center - Main.screenPosition,
                    null,
                    explosionColor,
                    0f,
                    circleTex.Size() / 2f,
                    drawScale,
                    SpriteEffects.None,
                    0
                );

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend,
                    SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null,
                    Main.GameViewMatrix.TransformationMatrix);

                NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, Projectile.whoAmI);
                return false;
            }

            return false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(explosionActive);
            writer.Write(explosionScale);
            writer.Write(explosionAlpha);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            explosionActive = reader.ReadBoolean();
            explosionScale = reader.ReadSingle();
            explosionAlpha = reader.ReadSingle();
            ModContent.GetInstance<HWJBardHealer>().Logger.Info($"[Sunrise] explosionAlpha = {explosionAlpha}");
        }
    }
}
