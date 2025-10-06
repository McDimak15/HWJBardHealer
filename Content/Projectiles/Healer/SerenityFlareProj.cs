using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;
using Terraria.DataStructures;
using System;
using ThoriumMod.Utilities;
using HWJBardHealer.Core;

namespace HWJBardHealer.Content.Projectiles.Healer
{
    public class SerenityFlareProj : ModProjectile
    {
        public override string Texture => "Terraria/Images/Extra_98"; 
        public override string GlowTexture => "HWJBardHealer/Content/Projectiles/Thrower/Sparkle"; 
        private static readonly Color FlareColor = new Color(209, 77, 203);

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 3;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 240;
            Projectile.penetrate = 1;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 2; 
            Projectile.DamageType = ModLoader.GetMod("ThoriumMod").Find<DamageClass>("HealerDamage");
            Projectile.netImportant = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(SoundID.Item29 with { Volume = 0.8f, Pitch = 0.2f }, Projectile.Center);

            for (int i = 0; i < 20; i++)
            {
                Vector2 vel = Main.rand.NextVector2Circular(3f, 3f);
                int dust = Dust.NewDust(Projectile.Center, 0, 0, DustID.PinkTorch, vel.X, vel.Y, 150, default, 1.3f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 0.8f;
            }
            for (int i = 0; i < 12; i++)
            {
                Vector2 offset = Vector2.UnitX.RotatedBy(MathHelper.TwoPi * i / 12f) * 12f;
                Dust.NewDustPerfect(Projectile.Center + offset, DustID.PurpleTorch, offset.SafeNormalize(Vector2.Zero) * 2f, 100, Color.HotPink, 1.2f).noGravity = true;
            }

            Lighting.AddLight(Projectile.Center, 1.2f, 0.3f, 1.2f);
        }
        public override void AI()
        {
            int targetId = (int)Projectile.ai[0];
            int targetType = (int)Projectile.ai[1]; // 0 = player, 1 = NPC

            Projectile.rotation += 0.25f * Projectile.direction;

            float accelFactor = 6f + (240 - Projectile.timeLeft) * 0.08f;

            if (targetType == 0) 
            {
                if (targetId < 0 || targetId >= Main.maxPlayers) { Projectile.Kill(); return; }
                Player target = Main.player[targetId];
                if (!target.active) { Projectile.Kill(); return; }

                Vector2 desired = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * accelFactor;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, desired, 0.1f);

                if (Projectile.Hitbox.Intersects(target.Hitbox))
                {
                    if (Main.myPlayer == Projectile.owner)
                    {
                        int healAmount = (int)(Projectile.localAI[0] > 0 ? Projectile.localAI[0] : 5);

                        HealerHelper.HealPlayer(
                            Main.player[base.Projectile.owner],
                            target,
                            healAmount: healAmount, 
                            recoveryTime: 60,
                            healEffects: false
                        );

                        for (int i = 0; i < 25; i++)
                        {
                            Vector2 vel = Main.rand.NextVector2Circular(3f, 3f);
                            int d = Dust.NewDust(target.Center, 0, 0, DustID.PinkTorch, vel.X, vel.Y, 100, Color.HotPink, 1.5f);
                            Main.dust[d].noGravity = true;
                        }

                        for (int i = 0; i < 8; i++)
                        {
                            Vector2 ringVel = Vector2.UnitX.RotatedBy(MathHelper.TwoPi * i / 8f) * 6f;
                            Dust.NewDustPerfect(target.Center, DustID.PurpleTorch, ringVel, 120, Color.Magenta, 1.8f).noGravity = true;
                        }

                        SoundEngine.PlaySound(SoundID.Item4 with { Volume = 1f, Pitch = 0.1f }, target.Center);
                    }
                    Projectile.Kill();
                }
            }
            else 
            {
                if (targetId < 0 || targetId >= Main.maxNPCs) { Projectile.Kill(); return; }
                NPC target = Main.npc[targetId];
                if (!target.active) { Projectile.Kill(); return; }

                Vector2 desired = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * accelFactor;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, desired, 0.1f);

                Projectile.friendly = true;
                Projectile.hostile = false;
            }

            Projectile.rotation = Utils.ToRotation(Projectile.velocity);
        }



        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D trailTex = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D glowTex = ModContent.Request<Texture2D>(GlowTexture).Value; 
            float fade = MathHelper.Min((float)Projectile.timeLeft, 15f) / 15f;
            float rotation;

            // Trail
            for (int i = 1; i < Projectile.oldPos.Length; i++)
            {
                if (Projectile.oldPos[i] == Vector2.Zero) continue;

                for (rotation = MathHelper.ToRadians((float)(base.Projectile.timeLeft + i * base.Projectile.oldPos.Length)) - base.Projectile.oldRot[i]; rotation > 3.1415927f; rotation -= 6.2831855f)
                {
                }
                while (rotation < -3.1415927f)
                {
                    rotation += 6.2831855f;
                }

                float progress = (float)i / Projectile.oldPos.Length;
                float scale = base.Projectile.scale * MathHelper.Lerp(1.1f, 0.2f, (float)i / (float)base.Projectile.oldPos.Length) * fade;

                Color color = FlareColor * (1f - progress) * fade;
                color.A = 0;

                Vector2 drawPos = Projectile.oldPos[i] + Projectile.Size * 0.5f - Main.screenPosition;
                float rotationDo = Projectile.oldRot[i] - 1.5707964f;

                Main.EntitySpriteDraw(trailTex, Projectile.oldPos[i] + Projectile.Size * 0.5f - Main.screenPosition, null, color, Projectile.oldRot[i] - 1.5707964f, Utils.Size(trailTex) * 0.5f, Projectile.scale * MathHelper.Lerp(1.1f, 0.2f, (float)i / (float)Projectile.oldPos.Length) * fade, 0, 0);
            }
            for (rotation = MathHelper.ToRadians((float)base.Projectile.timeLeft) - base.Projectile.rotation; rotation > 3.1415927f; rotation -= 6.2831855f)
            {
            }
            while (rotation < -3.1415927f)
            {
                rotation += 6.2831855f;
            }
            Color glowColor = FlareColor * 0.8f;
            glowColor.A = 0;
            Vector2 origin = trailTex.Size() / 2f;

            Main.EntitySpriteDraw(trailTex, Projectile.Center - Main.screenPosition, null, glowColor * fade, Main.GlobalTimeWrappedHourly * 6.2831855f, Utils.Size(trailTex) / 2f, Projectile.scale * 0.25f * fade, 0, 0);
            Main.EntitySpriteDraw(trailTex, Projectile.Center - Main.screenPosition, null, glowColor * fade, Main.GlobalTimeWrappedHourly * 3.1415927f, Utils.Size(trailTex) / 2f, Projectile.scale * 0.5f * fade, 0, 0);
            Main.EntitySpriteDraw(trailTex, Projectile.Center - Main.screenPosition, null, glowColor * fade, Main.GlobalTimeWrappedHourly * 1.5707964f, Utils.Size(trailTex) / 2f, Projectile.scale * 0.75f * fade, 0, 0);

            Main.EntitySpriteDraw(trailTex, Projectile.Center - Main.screenPosition, null, new Color(180, 120, 180, 0) * fade, Main.GlobalTimeWrappedHourly * 6.2831855f, Utils.Size(trailTex) / 2f, Projectile.scale * 0.125f * fade, 0, 0);
            Main.EntitySpriteDraw(trailTex, Projectile.Center - Main.screenPosition, null, new Color(180, 120, 180, 0) * fade, Main.GlobalTimeWrappedHourly * 3.1415927f, Utils.Size(trailTex) / 2f, Projectile.scale * 0.375f * fade, 0, 0);
            Main.EntitySpriteDraw(trailTex, Projectile.Center - Main.screenPosition, null, new Color(180, 120, 180, 0) * fade, Main.GlobalTimeWrappedHourly * 1.5707964f, Utils.Size(trailTex) / 2f, Projectile.scale * 0.625f * fade, 0, 0);

            return false;
        }
    }
}
