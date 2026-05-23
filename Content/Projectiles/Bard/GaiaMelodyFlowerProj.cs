using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using System;

namespace HWJBardHealer.Content.Projectiles.Bard
{
    public class GaiaMelodyFlowerProj : ModProjectile
    {
        public override string Texture => "ContinentOfJourney/Projectiles/Lifebringer_5";
        private float alpha;

        public override void SetDefaults()
        {
            Projectile.width = 36;
            Projectile.height = 36;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 300;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            if (Projectile.timeLeft > 285)
                alpha = MathHelper.Lerp(0f, 1f, (300 - Projectile.timeLeft) / 15f);
            else if (Projectile.timeLeft < 15)
                alpha = MathHelper.Lerp(0f, 1f, Projectile.timeLeft / 15f);
            else alpha = 1f;

            Projectile.rotation += 0.1f;
            Lighting.AddLight(Projectile.Center, 0.3f * alpha, 0.8f * alpha, 0.3f * alpha);

            Projectile.velocity *= 1.02f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Explode();
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Explode();
            return false;
        }

        private void Explode()
        {
            if (!Projectile.active) return;
            Projectile.active = false;

            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);

            for (int i = 0; i < 8; i++)
            {
                Vector2 vel = Vector2.UnitX.RotatedBy(MathHelper.TwoPi * i / 8f) * 3.5f;
                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    Projectile.Center,
                    vel,
                    ModContent.ProjectileType<GaiaMelodyPetalProj>(),
                    (int)(Projectile.damage * 0.8f),
                    1f,
                    Projectile.owner);
            }

            for (int i = 0; i < 10; i++)
            {
                int d = Dust.NewDust(Projectile.Center, 10, 10, DustID.Grass,
                    Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-3f, 3f), 100, default, 1.1f);
                Main.dust[d].noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 origin = tex.Size() / 2f;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null,
                Color.White * alpha, Projectile.rotation, origin, 1f, SpriteEffects.None, 0);
            return false;
        }
    }

    public class GaiaMelodyPetalProj : ModProjectile
    {
        public override string Texture => "ContinentOfJourney/Projectiles/Lifebringer_Master_12";
        private float alpha;

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.timeLeft = 70;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            if (Projectile.timeLeft > 60)
                alpha = MathHelper.Lerp(0f, 1f, (70 - Projectile.timeLeft) / 10f);
            else if (Projectile.timeLeft < 10)
                alpha = MathHelper.Lerp(0f, 1f, Projectile.timeLeft / 10f);
            else alpha = 1f;

            Projectile.rotation += 0.2f;
            Projectile.velocity *= 0.96f;
            Lighting.AddLight(Projectile.Center, 0.2f * alpha, 0.8f * alpha, 0.3f * alpha);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 origin = tex.Size() / 2f;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null,
                Color.White * alpha, Projectile.rotation, origin, 0.6f, SpriteEffects.None, 0);
            return false;
        }
    }
}
