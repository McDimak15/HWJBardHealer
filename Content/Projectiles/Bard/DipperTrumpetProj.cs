using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;
using System;
using ThoriumMod.Projectiles.Bard;

namespace HWJBardHealer.Content.Projectiles.Bard
{
    public class DipperTrumpetProj : BardProjectile
    {
        private bool spawned;
        private Vector2 targetPos;
        private float wobbleTimer;
        private float horizontalDrift;

        public override string Texture => "Terraria/Images/Extra_98";
        public override string GlowTexture => "HWJBardHealer/Content/Projectiles/Thrower/Sparkle";
        private static readonly Color FlareColor = new Color(140, 210, 255);

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 3;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetBardDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 600;
            Projectile.light = 1.1f;
            Projectile.aiStyle = -1;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (!spawned)
            {
                targetPos = Main.MouseWorld;
                Projectile.Center = new Vector2(targetPos.X + Main.rand.NextFloat(-180f, 180f), Main.screenPosition.Y - 100f);
                Projectile.velocity = new Vector2(Main.rand.NextFloat(-3f, 3f), 5f); 
                spawned = true;
                horizontalDrift = Main.rand.NextFloat(-1.5f, 1.5f);

                SoundEngine.PlaySound(SoundID.Item9 with { Pitch = -0.3f, Volume = 0.7f }, player.Center);
            }

            wobbleTimer += 0.05f;

            Vector2 homingDirection = Vector2.UnitY; 
            NPC target = FindClosestNPC(400f);
            if (target != null)
            {
                Vector2 toTarget = target.Center - Projectile.Center;
                if (toTarget != Vector2.Zero)
                    toTarget.Normalize();

                homingDirection = Vector2.Lerp(Vector2.UnitY, toTarget, 0.25f);
            }

            Vector2 wobble = new Vector2((float)Math.Sin(wobbleTimer * 2f) * 1.8f + horizontalDrift, 0f);

            Projectile.velocity = Vector2.Lerp(Projectile.velocity, (homingDirection * 16f) + wobble, 0.1f);

            if (Projectile.position.Y > Main.screenPosition.Y + Main.screenHeight + 200f)
                Projectile.Kill();

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Lighting.AddLight(Projectile.Center, 0.3f, 0.5f, 1.2f);

            if (Main.rand.NextBool(2))
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.BlueTorch,
                    -Projectile.velocity * 0.15f, 150, FlareColor, 1.3f);
                d.noGravity = true;
            }

            Projectile.rotation = Utils.ToRotation(Projectile.velocity);
        }

        private NPC FindClosestNPC(float maxDetectDistance)
        {
            NPC closest = null;
            float sqrMax = maxDetectDistance * maxDetectDistance;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.active || npc.friendly || npc.dontTakeDamage) continue;
                float sqrDist = Vector2.DistanceSquared(npc.Center, Projectile.Center);
                if (sqrDist < sqrMax)
                {
                    sqrMax = sqrDist;
                    closest = npc;
                }
            }
            return closest;
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
