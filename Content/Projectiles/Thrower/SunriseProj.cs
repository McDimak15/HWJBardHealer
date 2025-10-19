using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using ThoriumMod.Buffs;

namespace HWJBardHealer.Content.Projectiles.Thrower
{
    public class SunriseProj : ModProjectile
    {
        private int timer;

        private bool explosionActive;
        private float explosionScale;
        private float explosionAlpha;

        private const float CircleSizeMultiplier = 0.12f;
        private const float ExplosionDamageRadius = 80f;
        private const int ExplosionDamage = 40;

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 600;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;
            Projectile.DamageType = DamageClass.Throwing;
        }

        public override void AI()
        {
            if (explosionActive)
            {
                explosionScale += 0.08f;
                explosionAlpha -= 0.05f;
                if (explosionAlpha <= 0f)
                    Projectile.Kill();
                return;
            }

            bool killFlag = false;

            if (Projectile.ai[0] == 0f)
            {
                if (timer >= 15)
                {
                    float drag = 0.98f;
                    float gravity = 0.35f;
                    Projectile.velocity.X *= drag;
                    Projectile.velocity.Y += gravity;
                }
                else
                {
                    timer++;
                }

                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(90f);
            }
            else if (Projectile.ai[0] == 1f)
            {
                int targetIndex = (int)Projectile.ai[1];
                Projectile.ignoreWater = true;
                Projectile.tileCollide = false;
                Projectile.localAI[0] += 1f;

                if (Projectile.localAI[0] >= 180f)
                    killFlag = true;

                if (targetIndex < 0 || targetIndex >= Main.maxNPCs)
                {
                    killFlag = true;
                }
                else
                {
                    NPC target = Main.npc[targetIndex];
                    if (target.active && !target.dontTakeDamage)
                    {
                        Projectile.Center = target.Center;
                        Projectile.gfxOffY = target.gfxOffY;
                        Projectile.velocity = Vector2.Zero;

                        target.AddBuff(ModContent.BuffType<Singed>(), 5);

                    }
                    else
                    {
                        killFlag = true;
                    }
                }
            }
            else
            {
                Projectile.localAI[0] += 1f;
                if (Projectile.localAI[0] >= 60f)
                    killFlag = true;
            }

            if (killFlag)
                Projectile.Kill();

            Lighting.AddLight(Projectile.Center, 1.4f, 0.6f, 0.2f);

            if (Main.rand.NextBool(2))
            {
                for (int i = 0; i < 2; i++)
                {
                    int dustType = Main.rand.Next(new int[]
                    {
                        DustID.Torch,
                        DustID.InfernoFork,
                        DustID.Torch
                    });
                    int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustType, 0f, 0f, 100, default, 1.5f);
                    Main.dust[dust].velocity *= 0.4f;
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].scale *= Main.rand.NextFloat(1f, 1.4f);
                }
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            Projectile.ai[0] = 1f;
            Projectile.ai[1] = target.whoAmI;
            Projectile.velocity = Vector2.Zero;
            Projectile.netUpdate = true;
            Projectile.friendly = false;

            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile other = Main.projectile[i];
                if (other.active && other.owner == Projectile.owner && other.type == Projectile.type && other.whoAmI != Projectile.whoAmI)
                {
                    if (other.ai[0] == 1f && other.ai[1] == Projectile.ai[1])
                        other.Kill();
                }
            }

            target.AddBuff(ModContent.BuffType<Singed>(), 240);

            StartExplosionEffect();
        }

        private void StartExplosionEffect()
        {
            explosionActive = true;
            explosionScale = 0f;
            explosionAlpha = 1f;

            SoundEngine.PlaySound(SoundID.Item14 with { Pitch = -0.3f }, Projectile.Center);

            for (int i = 0; i < 30; i++)
            {
                Vector2 vel = Main.rand.NextVector2Circular(4f, 4f);
                int dustType = Main.rand.Next(new int[]
                {
                    DustID.Torch,
                    DustID.InfernoFork,
                    DustID.Torch
                });
                int dust = Dust.NewDust(Projectile.Center, 0, 0, dustType, vel.X, vel.Y, 150, default, 1.6f);
                Main.dust[dust].noGravity = true;
            }

            if (Main.myPlayer == Projectile.owner)
            {
                int damage = (int)(Projectile.damage * 0.8f);
                float radius = ExplosionDamageRadius;

                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.active && !npc.friendly && !npc.dontTakeDamage)
                    {
                        float distance = Vector2.Distance(npc.Center, Projectile.Center);
                        if (distance < radius)
                        {
                            npc.StrikeNPC(new NPC.HitInfo
                            {
                                Damage = damage,
                                Knockback = 3f,
                                HitDirection = npc.Center.X < Projectile.Center.X ? -1 : 1,
                                Crit = Main.rand.NextBool(5)
                            });

                            npc.AddBuff(ModContent.BuffType<Singed>(), 180);
                        }
                    }
                }
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10 with { Volume = 0.7f, Pitch = 0.4f }, Projectile.Center);
            for (int i = 0; i < 10; i++)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch,
                    Main.rand.NextFloat(-3, 3), Main.rand.NextFloat(-3, 3), 150, default, 1.3f);
                Main.dust[d].noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            if (explosionActive)
            {
                Texture2D circleTex = ModContent.Request<Texture2D>("HWJBardHealer/Content/Projectiles/Healer/FadeCircle").Value;
                float drawScale = explosionScale * 1.5f * CircleSizeMultiplier;
                Color explosionColor = new Color(255, 130, 50, 220) * explosionAlpha;

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
            }

            Main.EntitySpriteDraw(
                texture,
                Projectile.Center - Main.screenPosition,
                null,
                Color.White,
                Projectile.rotation,
                texture.Size() / 2f,
                Projectile.scale,
                SpriteEffects.None,
                0
            );

            return false;
        }
    }
}
