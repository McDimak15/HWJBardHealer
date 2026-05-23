using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using System;

namespace HWJBardHealer.Content.Projectiles.Bard
{
    public class GaiaMelodySnakeProj : ModProjectile
    {
        public override string GlowTexture => "Terraria/Images/Extra_98";


        private const float HomingRange = 500f;
        private const float HomingStrength = 0.15f;
        private float alpha;
        private Vector2 initialDirection;
        private float angleOffset;
        private float swayCounter;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[base.Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[base.Projectile.type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 108;
            Projectile.friendly = true;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 200;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 1;
            Main.projFrames[Projectile.type] = 2;
        }

        public override void OnSpawn(IEntitySource source)
        {
            initialDirection = Projectile.velocity.SafeNormalize(Vector2.UnitX);

            float baseAngle = initialDirection.ToRotation();
            angleOffset = Main.rand.NextBool() ? MathHelper.ToRadians(25f) : MathHelper.ToRadians(-25f);
            Projectile.velocity = initialDirection.RotatedBy(angleOffset) * Projectile.velocity.Length();
        }

        public override void AI()
        {
            if (Projectile.timeLeft > 180)
                alpha = MathHelper.Lerp(0f, 1f, (200 - Projectile.timeLeft) / 20f);
            else if (Projectile.timeLeft < 15)
                alpha = MathHelper.Lerp(0f, 1f, Projectile.timeLeft / 15f);
            else alpha = 1f;

            Projectile.frameCounter++;
            if (Projectile.frameCounter > 6)
            {
                Projectile.frameCounter = 0;
                Projectile.frame = (Projectile.frame + 1) % 2;
            }

            swayCounter += 0.02f;
            Projectile.velocity = Projectile.velocity.RotatedBy(Math.Sin(swayCounter) * 0.02f);

            NPC target = FindForwardTarget(HomingRange);
            if (target != null)
            {
                Vector2 toTarget = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero);
                Vector2 desiredVelocity = toTarget * 12f;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, HomingStrength);
            }

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            Projectile.rotation = Utils.ToRotation(Projectile.velocity);

            Lighting.AddLight(Projectile.Center, 0.4f * alpha, 0.9f * alpha, 0.4f * alpha);
        }

        private NPC FindForwardTarget(float maxRange)
        {
            NPC best = null;
            float bestDist = maxRange;
            Vector2 forward = initialDirection;

            for (int k = 0; k < Main.maxNPCs; k++)
            {
                NPC npc = Main.npc[k];
                if (npc.CanBeChasedBy(this))
                {
                    Vector2 dirToNPC = npc.Center - Projectile.Center;
                    float dist = dirToNPC.Length();
                    if (dist < bestDist)
                    {
                        dirToNPC.Normalize();
                        if (Vector2.Dot(forward, dirToNPC) > 0.3f)
                        {
                            best = npc;
                            bestDist = dist;
                        }
                    }
                }
            }
            return best;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 origin = new(tex.Width / 2, tex.Height / 4);
            Rectangle frame = new(0, Projectile.frame * tex.Height / 2, tex.Width, tex.Height / 2);

            Texture2D glowTex = ModContent.Request<Texture2D>(GlowTexture).Value;
            float fade = MathHelper.Lerp(1f, 0f, Projectile.alpha / 255f) *
                         (MathHelper.Min(15f, (float)Projectile.timeLeft) / 15f);

            if (Projectile.velocity != Vector2.Zero)
            {
                for (int i = 0; i < Projectile.oldPos.Length; i++)
                {
                    lightColor = Color.Lerp(Color.LightGreen, Color.Green, i / (float)Projectile.oldPos.Length);
                    lightColor.A = 0;
                    lightColor *= fade;

                    Main.EntitySpriteDraw(
                        glowTex,
                        Projectile.oldPos[i] + Projectile.Size * 0.5f - Main.screenPosition,
                        new Rectangle?(new Rectangle(31, 0, 10, glowTex.Height)),
                        lightColor,
                        Projectile.oldRot[i] + MathHelper.PiOver2,
                        new Vector2(10f, glowTex.Height * 0.5f),
                        new Vector2(0.4f, 0.6f) * Projectile.scale,
                        SpriteEffects.None,
                        0f
                    );
                }
            }

            for (int i = 0; i < 5; i++)
            {
                Vector2 trailPos = Projectile.Center - Projectile.velocity * i * 0.4f;
                float trailAlpha = alpha * (1f - i * 0.18f);
                Main.EntitySpriteDraw(tex, trailPos - Main.screenPosition, frame,
                    Color.Lime * trailAlpha, Projectile.rotation, origin, 1f, SpriteEffects.None, 0);
            }

            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, frame,
                Color.White * alpha, Projectile.rotation, origin, 1f, SpriteEffects.None, 0);

            return false;
        }

    }
}
