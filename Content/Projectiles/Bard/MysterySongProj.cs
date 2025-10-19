using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using ThoriumMod;
using ThoriumMod.Projectiles.Bard;

namespace HWJBardHealer.Content.Projectiles.Bard
{
    public class MysterySongProj : BardProjectile
    {
        public override string Texture => "HWJBardHealer/Content/Projectiles/Bard/MysterySongProj";
        public override BardInstrumentType InstrumentType => BardInstrumentType.String;

        private bool exploded = false;

        public override void SetBardDefaults()
        {
            Projectile.width = 76; 
            Projectile.height = 76;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 480;
            Projectile.extraUpdates = 1;
            Projectile.DamageType = ThoriumDamageBase<BardDamage>.Instance;
            Projectile.alpha = 255;
        }

        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                Projectile.frame = (Projectile.frame + 1) % 5;
            }

            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 20;
                if (Projectile.alpha < 0)
                    Projectile.alpha = 0;
            }


            if (Projectile.timeLeft > 200)
                Projectile.velocity *= 0.992f;
            else if (Projectile.timeLeft > 100)
                Projectile.velocity *= 0.985f;
            else
                Projectile.velocity *= 0.97f;

            Projectile.rotation += 0.05f;
            Projectile.scale = 1.9f + 0.1f * (float)Math.Sin(Main.GameUpdateCount * 0.1f);

            float pullRadius = 160f;
            float pullStrength = 0.8f;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && !npc.friendly && !npc.boss && npc.CanBeChasedBy(this))
                {
                    float dist = Vector2.Distance(npc.Center, Projectile.Center);
                    if (dist < pullRadius)
                    {
                        Vector2 dir = Projectile.Center - npc.Center;
                        float force = (1f - dist / pullRadius) * pullStrength;
                        npc.velocity += dir.SafeNormalize(Vector2.Zero) * force;
                    }
                }
            }

            for (int i = 0; i < Main.maxItems; i++)
            {
                Item item = Main.item[i];
                if (item.active && !item.beingGrabbed)
                {
                    float dist = Vector2.Distance(item.Center, Projectile.Center);
                    if (dist < pullRadius)
                    {
                        Vector2 dir = Projectile.Center - item.Center;
                        float force = (1f - dist / pullRadius) * (pullStrength * 0.8f);
                        item.velocity += dir.SafeNormalize(Vector2.Zero) * force;
                        if (dist < 16f)
                            item.position = Vector2.Lerp(item.position, Projectile.Center, 0.05f);
                    }
                }
            }

            if (Main.rand.NextBool(2))
            {
                float spawnDistanceOuter = Projectile.width * 1.8f; 
                Vector2 spawnPosOuter = Projectile.Center + Main.rand.NextVector2CircularEdge(spawnDistanceOuter, spawnDistanceOuter);
                Vector2 dirToCenterOuter = (Projectile.Center - spawnPosOuter).SafeNormalize(Vector2.Zero);
                Vector2 velOuter = dirToCenterOuter * Main.rand.NextFloat(2f, 5f);

                int dustOuter = Dust.NewDust(spawnPosOuter, 0, 0, DustID.Smoke, 0f, 0f, 200, default, 1.6f);
                Main.dust[dustOuter].velocity = velOuter;
                Main.dust[dustOuter].noGravity = true;
                Main.dust[dustOuter].color = Color.Lerp(Color.Black, new Color(10, 0, 10), 0.5f);
                Main.dust[dustOuter].fadeIn = 1.1f;
                Main.dust[dustOuter].customData = Projectile.Center;
            }

            for (int i = 0; i < Main.maxDustToDraw; i++)
            {
                Dust d = Main.dust[i];
                if (d.active && d.customData is Vector2 target)
                {
                    Vector2 toCenter = target - d.position;
                    float distance = toCenter.Length();

                    if (distance > 4f)
                    {
                        d.velocity = Vector2.Lerp(d.velocity, toCenter.SafeNormalize(Vector2.Zero) * 8f, 0.1f);
                    }
                    else
                    {
                        d.scale *= 0.95f;
                        if (d.scale < 0.3f)
                            d.active = false;
                    }
                }
            }

            Lighting.AddLight(Projectile.Center, 0.35f, 0.1f, 0.5f);

            if (Projectile.timeLeft <= 1 && !exploded)
            {
                Explode();
                exploded = true;
            }
        }

        private void Explode()
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);

            Vector2 pos = Projectile.Center;
            int radius = 220;
            int explosionDamage = (int)(Projectile.damage * 1.7f);

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && !npc.friendly && !npc.dontTakeDamage)
                {
                    float distance = Vector2.Distance(npc.Center, pos);
                    if (distance < radius)
                    {
                        int hitDamage = (int)(explosionDamage * (1f - distance / radius));
                        npc.StrikeNPC(new NPC.HitInfo()
                        {
                            Damage = hitDamage,
                            Knockback = 7f,
                            HitDirection = npc.Center.X < pos.X ? -1 : 1,
                            Crit = false
                        }, true, false);
                    }
                }
            }

            for (int i = 0; i < Main.maxItems; i++)
            {
                if (Main.item[i].active)
                {
                    Vector2 pushDir = (Main.item[i].Center - pos).SafeNormalize(Vector2.Zero);
                    Main.item[i].velocity += pushDir * 12f;
                }
            }

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && !npc.friendly)
                {
                    Vector2 pushDir = (npc.Center - pos).SafeNormalize(Vector2.Zero);
                    npc.velocity += pushDir * 9f;
                }
            }

            for (int i = 0; i < 80; i++)
            {
                Vector2 vel = Main.rand.NextVector2Circular(14f, 14f);
                int dust = Dust.NewDust(pos, 0, 0, DustID.Smoke, vel.X, vel.Y, 100, default, 1.8f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].color = Color.Black;
            }

            for (int i = 0; i < 50; i++)
            {
                Vector2 vel = Main.rand.NextVector2Circular(12f, 12f);
                int dust = Dust.NewDust(pos, 0, 0, DustID.Smoke, vel.X, vel.Y, 100, default, 1.5f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].color = Color.Black;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            int frameHeight = texture.Height / 5;
            Rectangle sourceRectangle = new Rectangle(0, frameHeight * Projectile.frame, texture.Width, frameHeight);

            Vector2 origin = new Vector2(texture.Width / 2f, frameHeight / 2f);
            Vector2 position = Projectile.Center - Main.screenPosition;
            float fade = 1f - Projectile.alpha / 255f;

            Color baseColor = new Color(240, 240, 240, 250) * fade;
            Color darkAura = new Color(10, 10, 10, 160) * fade * 0.8f;
            Color glowColor = new Color(250, 250, 250, 255) * fade * 0.8f;

            Main.EntitySpriteDraw(
                texture,
                position,
                sourceRectangle,
                darkAura,
                -Projectile.rotation * 0.4f,
                origin,
                Projectile.scale * 1.3f,
                SpriteEffects.None,
                0
            );

            Main.EntitySpriteDraw(
                texture,
                position,
                sourceRectangle,
                baseColor,
                Projectile.rotation,
                origin,
                Projectile.scale,
                SpriteEffects.None,
                0
            );

            Main.EntitySpriteDraw(
                texture,
                position,
                sourceRectangle,
                glowColor,
                Projectile.rotation,
                origin,
                Projectile.scale * 1.15f,
                SpriteEffects.None,
                0
            );

            return false;
        }

    }
}
