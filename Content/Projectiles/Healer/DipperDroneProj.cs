using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.DataStructures;
using System;
using HWJBardHealer.Content.Buffs;

namespace HWJBardHealer.Content.Projectiles.Healer
{
    public class DipperDroneProj : ModProjectile
    {
        private float appearProgress;
        private float floatTimer;

        private float circleScale;
        private float circleAlpha;
        private bool circleActive;
        private int circleCooldown;

        private bool explosionActive;
        private float explosionScale;
        private float explosionAlpha;

        private readonly bool[] playerInAura = new bool[Main.maxPlayers];

        private const float Radius = 160f;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4; 
        }

        public override void SetDefaults()
        {
            Projectile.width = 62;
            Projectile.height = 184;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 60 * 15;
        }

        public override void OnSpawn(IEntitySource source)
        {
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile other = Main.projectile[i];
                if (other.active && other.type == Projectile.type && other.whoAmI != Projectile.whoAmI)
                {
                    Projectile.Kill();
                    return;
                }
            }
        }

        public override void AI()
        {
            Projectile.velocity = Vector2.Zero;

            floatTimer += 0.05f;
            Projectile.position.Y += (float)Math.Sin(floatTimer) * 0.3f;

            if (appearProgress < 1f)
                appearProgress += 0.05f;

            // Explosion
            if (Projectile.timeLeft == 1 && !explosionActive)
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    Projectile.NewProjectile(
                        Projectile.GetSource_FromThis(),
                        Projectile.Center,
                        Vector2.Zero,
                        ProjectileID.DD2ExplosiveTrapT1Explosion,
                        Projectile.originalDamage,
                        6,
                        Projectile.owner
                    );
                }

                explosionActive = true;
                explosionScale = 0f;
                explosionAlpha = 1f;
                Projectile.timeLeft = 30;

                for (int i = 0; i < 30; i++)
                {
                    Vector2 vel = Main.rand.NextVector2Circular(4f, 4f);
                    int dust = Dust.NewDust(Projectile.Center, 0, 0, DustID.Electric, vel.X, vel.Y, 150, default, 1.5f);
                    Main.dust[dust].noGravity = true;
                }

                SoundEngine.PlaySound(SoundID.Item14 with { Pitch = -0.2f }, Projectile.Center);
            }

            if (explosionActive)
            {
                explosionScale += 0.08f;
                explosionAlpha -= 0.05f;
                if (explosionAlpha <= 0f)
                {
                    Projectile.Kill();
                }
                return; 
            }

            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                if (!player.active || player.dead) continue;

                bool inside = Vector2.Distance(player.Center, Projectile.Center) < Radius;

                if (inside)
                {
                    player.AddBuff(ModContent.BuffType<SpaceBarrierBuff>(), 500);

                    if (!playerInAura[i])
                    {
                        playerInAura[i] = true;
                        OnEnterAura(player);
                    }
                }
                else if (playerInAura[i])
                {
                    playerInAura[i] = false;
                    OnLeaveAura(player);
                }
            }

            if (circleActive)
            {
                circleScale += 0.02f;
                circleAlpha = 1f - circleScale;

                if (circleScale >= 1f)
                {
                    circleActive = false;
                    circleCooldown = Main.rand.Next(30, 60);
                }
            }
            else if (circleCooldown > 0)
            {
                circleCooldown--;
                if (circleCooldown <= 0)
                {
                    circleActive = true;
                    circleScale = 0f;
                    circleAlpha = 1f;
                }
            }
            else
            {
                circleActive = true;
                circleScale = 0f;
                circleAlpha = 1f;
            }

            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 12)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame >= Main.projFrames[Projectile.type])
                    Projectile.frame = 0;
            }
        }


        private void OnEnterAura(Player player)
        {
            for (int i = 0; i < 15; i++)
            {
                Vector2 vel = Main.rand.NextVector2Circular(2f, 2f);
                int dust = Dust.NewDust(player.Center, 0, 0, DustID.Electric, vel.X, vel.Y, 150, default, 1.3f);
                Main.dust[dust].noGravity = true;
            }

            SoundEngine.PlaySound(SoundID.Item29 with { Volume = 0.6f, Pitch = 0.2f }, player.Center);
        }

        private void OnLeaveAura(Player player)
        {
            for (int i = 0; i < 25; i++)
            {
                Vector2 vel = Main.rand.NextVector2Circular(3f, 3f);
                int dust = Dust.NewDust(player.Center, 0, 0, DustID.Electric, vel.X, vel.Y, 150, default, 1.5f);
                Main.dust[dust].noGravity = true;
            }

            for (int i = 0; i < 15; i++)
            {
                Vector2 vel = Main.rand.NextVector2Circular(2f, 2f);
                int dust = Dust.NewDust(player.Center, 0, 0, DustID.BlueTorch, vel.X, vel.Y, 180, default, 1.2f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 1.5f;
            }

            int ringDustCount = 36;
            float ringRadius = 48f;
            for (int i = 0; i < ringDustCount; i++)
            {
                float angle = MathHelper.TwoPi * i / ringDustCount;
                Vector2 pos = player.Center + angle.ToRotationVector2() * ringRadius;
                int dust = Dust.NewDust(pos, 0, 0, DustID.Smoke, 0f, 0f, 200, default, 1.4f);
                Main.dust[dust].velocity = angle.ToRotationVector2() * 2f;
                Main.dust[dust].noGravity = true;
            }

            SoundEngine.PlaySound(SoundID.Item27 with { Volume = 0.8f, Pitch = -0.4f }, player.Center);
        }

        public override void OnKill(int timeLeft)
        {
            if (timeLeft <= 0)
            {
                int explosionDamage = Projectile.originalDamage;
                int explosionKnockback = 6;
                if (Main.myPlayer == Projectile.owner)
                {
                    Projectile.NewProjectile(
                        Projectile.GetSource_FromThis(),
                        Projectile.Center,
                        Vector2.Zero,
                        ProjectileID.DD2ExplosiveTrapT1Explosion,
                        explosionDamage,
                        explosionKnockback,
                        Projectile.owner
                    );
                }

                explosionActive = true;
                explosionScale = 0f;
                explosionAlpha = 1f;

                for (int i = 0; i < 30; i++)
                {
                    Vector2 vel = Main.rand.NextVector2Circular(4f, 4f);
                    int dust = Dust.NewDust(Projectile.Center, 0, 0, DustID.Electric, vel.X, vel.Y, 150, default, 1.5f);
                    Main.dust[dust].noGravity = true;
                }

                SoundEngine.PlaySound(SoundID.Item14 with { Pitch = -0.2f }, Projectile.Center);
            }
        }



        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D circleTex = ModContent.Request<Texture2D>("HWJBardHealer/Content/Projectiles/Healer/FadeCircle").Value;

            if (explosionActive)
            {
                float baseScale = Radius / (circleTex.Width / 2f);
                float drawScale = baseScale * explosionScale * 2.5f; 
                Color explosionColor = new Color(220, 255, 255, 200) * explosionAlpha;

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

                return false; 
            }

            Texture2D droneTex = ModContent.Request<Texture2D>("HWJBardHealer/Content/Projectiles/Healer/DipperDroneProj").Value;
            int frameHeight = droneTex.Height / Main.projFrames[Projectile.type];
            Rectangle frame = new Rectangle(0, Projectile.frame * frameHeight, droneTex.Width, frameHeight);
            Vector2 origin = new(frame.Width / 2f, frameHeight / 2f);

            Main.EntitySpriteDraw(
                droneTex,
                Projectile.Center - Main.screenPosition,
                frame,
                Color.White,
                0f,
                origin,
                appearProgress,
                SpriteEffects.None,
                0
            );

            float baseScaleAura = (Radius / (circleTex.Width / 2f)) * appearProgress;
            Color auraColor = new Color(180, 240, 255, 230) * appearProgress;

            Lighting.AddLight(Projectile.Center, 0.3f * appearProgress, 0.6f * appearProgress, 1.2f * appearProgress);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive,
                SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null,
                Main.GameViewMatrix.TransformationMatrix);

            Main.EntitySpriteDraw(
                circleTex,
                Projectile.Center - Main.screenPosition,
                null,
                auraColor,
                0f,
                circleTex.Size() / 2f,
                baseScaleAura,
                SpriteEffects.None,
                0
            );

            if (circleActive)
            {
                float innerScale = baseScaleAura * circleScale;
                Color pulseColor = new Color(200, 255, 255, 240) * circleAlpha * appearProgress;

                Main.EntitySpriteDraw(
                    circleTex,
                    Projectile.Center - Main.screenPosition,
                    null,
                    pulseColor,
                    0f,
                    circleTex.Size() / 2f,
                    innerScale,
                    SpriteEffects.None,
                    0
                );
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend,
                SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null,
                Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

    }
}
