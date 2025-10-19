using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;

namespace HWJBardHealer.Content.Projectiles.Thrower
{
    public class TomatoProj : ModProjectile
    {
        private const float Speed = 8f;
        private const float HomingStrength = 0.15f;
        private const float FadeInDuration = 18f;
        private const float WaitDuration = 30f;
        private const float GlowStartTime = FadeInDuration + WaitDuration;

        public static readonly Vector2 SpawnOffset = new Vector2(18f, -16f);

        private float appearTimer;
        private float alpha = 0f;
        private bool homingStarted = false;
        private float glowPulse = 0f;

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600; 
            Projectile.DamageType = DamageClass.Throwing;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs,
            List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }


        public override void AI()
        {
            appearTimer++;

            if (appearTimer <= FadeInDuration)
            {
                alpha = appearTimer / FadeInDuration;
                Projectile.velocity *= 0.9f;
            }
            else
                alpha = 1f;

            if (appearTimer > GlowStartTime && !homingStarted)
            {
                glowPulse = (float)Math.Sin(Main.GlobalTimeWrappedHourly * 6f) * 0.25f + 0.75f;
            }
            else
            {
                glowPulse = 0f;
            }

            int puppetIndex = (int)Projectile.ai[0];
            Projectile puppet = null;

            if (puppetIndex >= 0 && puppetIndex < Main.maxProjectiles)
            {
                Projectile p = Main.projectile[puppetIndex];
                if (p.active && p.type == ModContent.ProjectileType<TomatoPuppet>())
                    puppet = p;
            }

            NPC target = FindNearestEnemy(400f);

            if (!homingStarted)
            {
                if (target == null && puppet != null)
                {
                    Projectile.Center = puppet.Center + SpawnOffset;
                    Projectile.velocity = Vector2.Zero;
                }
                else if (target != null)
                {
                    homingStarted = true;
                    Vector2 desiredVel = Projectile.DirectionTo(target.Center) * Speed;
                    Projectile.velocity = desiredVel;
                }
                else if (puppet == null)
                { 
                    Projectile.velocity *= 0.95f;
                    Projectile.alpha += 2;
                }
            }
            else
            {
                if (target != null)
                {
                    Vector2 desiredVel = Projectile.DirectionTo(target.Center) * Speed;
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVel, HomingStrength);
                }
                else
                {
                    Projectile.velocity *= 0.98f;
                }
            }

            Lighting.AddLight(Projectile.Center, (1f * alpha + glowPulse * 0.6f), 0.3f * alpha, 0.3f * alpha);
        }

        private NPC FindNearestEnemy(float maxDist)
        {
            NPC closest = null;
            float minDist = maxDist;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.CanBeChasedBy(this))
                {
                    float dist = Vector2.Distance(Projectile.Center, npc.Center);
                    if (dist < minDist)
                    {
                        minDist = dist;
                        closest = npc;
                    }
                }
            }
            return closest;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D glowTex = tex;

            Vector2 pos = Projectile.Center - Main.screenPosition;
            Main.EntitySpriteDraw(tex, pos, null, lightColor * alpha, 0f, tex.Size() / 2f, 1f, SpriteEffects.None);

            if (glowPulse > 0f)
            {
                Main.EntitySpriteDraw(
                    glowTex,
                    pos,
                    null,
                    Color.White * glowPulse * 0.8f,
                    0f,
                    glowTex.Size() / 2f,
                    1.1f,
                    SpriteEffects.None
                );
            }

            return false;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath1, Projectile.position);
            for (int i = 0; i < 8; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Blood,
                    Main.rand.NextFloat(-2, 2), Main.rand.NextFloat(-2, 2), 150, default, 1.2f);
                Main.dust[dust].noGravity = true;
            }
        }
    }
}
