using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;
using ThoriumMod;

namespace HWJBardHealer.Content.Projectiles.Healer
{
    public class SolsticeHarvesterShot : ModProjectile
    {
        public override string Texture => "Terraria/Images/Extra_98";
        public override string GlowTexture => "ContinentOfJourney/Projectiles/SlimeGod_Master_12";
        private static readonly Color TrailColor = new Color(245, 109, 51);

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.aiStyle = 0;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 3; 
            Projectile.timeLeft = 180;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.alpha = 50;
            Projectile.Size = new Vector2(26f, 26f);
            Projectile.light = 0.6f;
            Projectile.DamageType = ThoriumDamageBase<HealerDamage>.Instance;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            if (Main.rand.NextBool(2))
            {
                Dust d = Dust.NewDustPerfect(
                    Projectile.Center,
                    DustID.GoldFlame,
                    -Projectile.velocity * 0.2f,
                    120,
                    Color.Goldenrod,
                    Main.rand.NextFloat(1.0f, 1.4f)
                );
                d.noGravity = true;
            }

            Lighting.AddLight(Projectile.Center, 0.8f, 0.7f, 0.3f);
            Projectile.rotation = Utils.ToRotation(Projectile.velocity);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.penetrate--;

            if (Projectile.penetrate <= 0)
            {
                Projectile.Kill();
            }
            else
            {
                Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
                SoundEngine.PlaySound(SoundID.Item10, Projectile.position);

                if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon)
                    Projectile.velocity.X = -oldVelocity.X;

                if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon)
                    Projectile.velocity.Y = -oldVelocity.Y;
            }

            return false;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);

            for (int i = 0; i < 10; i++)
            {
                Vector2 vel = Main.rand.NextVector2Circular(3f, 3f);
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.GoldFlame, vel, 150, Color.Orange, 1.4f);
                d.noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Daybreak, 240);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D projTex = ModContent.Request<Texture2D>(GlowTexture).Value;
            Texture2D trailTex = ModContent.Request<Texture2D>(Texture).Value;

            float fade = MathHelper.Min((float)Projectile.timeLeft, 15f) / 15f;

            for (int i = 1; i < Projectile.oldPos.Length; i++)
            {
                if (Projectile.oldPos[i] == Vector2.Zero) continue;

                float progress = (float)i / Projectile.oldPos.Length;
                float scale = Projectile.scale * MathHelper.Lerp(1.1f, 0.2f, progress) * fade;

                Color color = TrailColor * (1f - progress) * fade;
                color.A = 0;

                Vector2 drawPos = Projectile.oldPos[i] + Projectile.Size * 0.5f - Main.screenPosition;

                Main.EntitySpriteDraw(
                    trailTex,
                    drawPos,
                    null,
                    color,
                    Projectile.oldRot[i] - MathHelper.PiOver2,
                    trailTex.Size() / 2f,
                    scale,
                    SpriteEffects.None,
                    0
                );
            }

            Main.EntitySpriteDraw(
                projTex,
                Projectile.Center - Main.screenPosition,
                null,
                Color.White,
                Projectile.rotation,
                projTex.Size() / 2f,
                1f,
                SpriteEffects.None,
                0
            );

            return false;
        }
    }
}
