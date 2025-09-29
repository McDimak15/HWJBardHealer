using Terraria;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;
using System;

namespace HWJBardHealer.Content.Projectiles.Healer
{
    public class RodScytheBeam : ModProjectile
    {
        private bool didSpawnEffect = false;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 120;
            Projectile.penetrate = 1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.alpha = 0;
            Projectile.scale = 0.8f;
            Projectile.width = (int)(Projectile.width * 0.8f);
            Projectile.height = (int)(Projectile.height * 0.8f);
        }

        public override void AI()
        {
            // Find closest enemy
            NPC closest = null;
            float closestDist = 800f;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && !npc.friendly && !npc.dontTakeDamage)
                {
                    float dist = Vector2.Distance(npc.Center, Projectile.Center);
                    if (dist < closestDist)
                    {
                        closestDist = dist;
                        closest = npc;
                    }
                }
            }

            // Kill instantly if no enemy found
            if (closest == null)
            {
                Projectile.Kill();
                return;
            }

            // Spawn effect 
            if (!didSpawnEffect)
            {
                didSpawnEffect = true;

                 SoundEngine.PlaySound(SoundID.Item72 with { Volume = 0.8f, PitchVariance = 0.3f }, Projectile.Center);

                // Burst of dust
                for (int i = 0; i < 25; i++)
                {
                    Vector2 velocity = Main.rand.NextVector2Circular(3f, 3f);
                    int dust = Dust.NewDust(Projectile.Center, 0, 0, DustID.GoldFlame, velocity.X, velocity.Y, 100, default, 1.3f);
                    Main.dust[dust].noGravity = true;
                }

                // Sparkles
                for (int i = 0; i < 10; i++)
                {
                    Vector2 velocity = Main.rand.NextVector2Circular(2f, 2f);
                    int dust = Dust.NewDust(Projectile.Center, 0, 0, DustID.FireworkFountain_Yellow, velocity.X, velocity.Y, 150, default, 1.6f);
                    Main.dust[dust].noGravity = true;
                }
            }

            // Homing
            float homingSpeed = 20f;
            Vector2 direction = closest.Center - Projectile.Center;
            direction.Normalize();
            direction *= homingSpeed;

            Projectile.velocity = (Projectile.velocity * 20f + direction) / 21f;

            // Spin + light
            Projectile.rotation += 0.5f;
            Lighting.AddLight(Projectile.Center, 0.9f, 0.8f, 0.2f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // Explosion dust
            for (int i = 0; i < 40; i++)
            {
                Vector2 speed = Main.rand.NextVector2Circular(6f, 6f);
                int dust = Dust.NewDust(target.Center, 0, 0, DustID.GoldFlame, speed.X, speed.Y, 150, default, Main.rand.NextFloat(1.2f, 2.2f));
                Main.dust[dust].noGravity = true;
            }

            for (int i = 0; i < 15; i++)
            {
                int dust = Dust.NewDust(target.position, target.width, target.height, DustID.FireworkFountain_Yellow,
                    Main.rand.NextFloat(-3f, 3f),
                    Main.rand.NextFloat(-3f, 3f),
                    120,
                    default,
                    1.5f
                );
                Main.dust[dust].noGravity = true;
            }

            SoundEngine.PlaySound(SoundID.Item14 with { Volume = 1.1f, PitchVariance = 0.2f }, target.Center);

            int explosionDamage = (int)(damageDone * 0.75f);
            Projectile.NewProjectile(
                Projectile.GetSource_FromThis(),
                target.Center - new Vector2(0, 40),
                Vector2.Zero,
                ProjectileID.DD2ExplosiveTrapT3Explosion,
                explosionDamage,
                3f,
                Projectile.owner
            );
        }

        // trail 
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 origin = new(texture.Width / 2f, texture.Height / 2f);

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                if (Projectile.oldPos[i] == Vector2.Zero) continue;

                float progress = 1f - i / (float)Projectile.oldPos.Length;
                Vector2 drawPos = Projectile.oldPos[i] + Projectile.Size / 2f - Main.screenPosition;

                Color trailColor;
                if (progress > 0.66f)
                    trailColor = Color.Lerp(Color.White, Color.Gray, (progress - 0.66f) / 0.34f);
                else if (progress > 0.33f)
                    trailColor = Color.Lerp(Color.Gray, Color.White, (progress - 0.33f) / 0.33f);
                else
                    trailColor = Color.Lerp(Color.White, Color.Gray, progress / 0.33f);

                trailColor *= progress * 0.8f;
                Main.EntitySpriteDraw(texture, drawPos, null, trailColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            }

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (target.friendly) return false;
            return base.CanHitNPC(target);
        }

        public override bool CanHitPlayer(Player target) => false;
    }
}
