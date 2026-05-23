using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace HWJBardHealer.Content.Projectiles.Thrower
{
    public class SolarKunaiProj : ModProjectile
    {
        public override string Texture => "HWJBardHealer/Content/Weapons/Thrower/SolarKunai";
        public override string GlowTexture => "Terraria/Images/Extra_98";

        private int slowdownTimer;
        private bool exploded;
        private bool isMainKunai;
        private float fade = 1f;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Throwing;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 240;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
        }

        public override void OnSpawn(Terraria.DataStructures.IEntitySource source)
        {
            isMainKunai = Projectile.ai[0] == 1f;
        }

        public override void AI()
        {
            if (Projectile.velocity.LengthSquared() > 0.1f)
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            slowdownTimer++;

            if (slowdownTimer < 60)
            {
                Projectile.velocity *= 0.97f;
            }
            else if (!exploded)
            {
                Explode();
            }

            if (Projectile.timeLeft < 30)
                fade = Projectile.timeLeft / 30f;

            Lighting.AddLight(Projectile.Center, 1.1f * fade, 0.6f * fade, 0.2f * fade);
            Projectile.rotation = Utils.ToRotation(Projectile.velocity);
        }

        private void Explode()
        {
            if (exploded)
                return;

            exploded = true;

            SoundEngine.PlaySound(SoundID.Item14 with { Volume = 0.9f, Pitch = -0.3f }, Projectile.Center);

            if (isMainKunai)
            {
                for (int i = 0; i < 6; i++)
                {
                    Vector2 vel = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(3f, 5f);
                    int proj = Projectile.NewProjectile(
                        Projectile.GetSource_FromThis(),
                        Projectile.Center,
                        vel,
                        ModContent.ProjectileType<SolarSlimeThrowerProj>(),
                        (int)(Projectile.damage * 0.7f),
                        1f,
                        Projectile.owner
                    );
                    if (Main.projectile.IndexInRange(proj))
                        Main.projectile[proj].DamageType = Projectile.DamageType;
                }
            }

            for (int i = 0; i < 20; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.SolarFlare,
                    Vector2.One.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(2f, 4f),
                    150, Color.OrangeRed, 1.5f);
                d.noGravity = true;
            }

            Projectile.active = false;
        }

        public override void OnKill(int timeLeft)
        {
            if (!exploded)
                Explode();
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (!exploded)
                Explode();
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!exploded)
                Explode();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D glowTex = ModContent.Request<Texture2D>(GlowTexture).Value;
            Vector2 origin = mainTex.Size() / 2f;

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float progress = i / (float)Projectile.oldPos.Length;
                Color color = Color.Lerp(Color.OrangeRed, Color.Yellow, progress);
                color *= (1f - progress) * 0.7f * fade;
                color.A = 0;

                Main.EntitySpriteDraw(glowTex,
                    Projectile.oldPos[i] + Projectile.Size * 0.5f - Main.screenPosition,
                    new Rectangle(31, 0, 10, glowTex.Height),
                    color,
                    Projectile.oldRot[i] + MathHelper.PiOver2,
                    new Vector2(10f, glowTex.Height / 2f),
                    new Vector2(0.5f, 0.7f),
                    SpriteEffects.None,
                    0f);
            }

            Main.EntitySpriteDraw(mainTex, Projectile.Center - Main.screenPosition, null,
                Color.White * fade, Projectile.rotation + MathHelper.PiOver2, origin, 1f, SpriteEffects.None, 0);

            return false;
        }
    }
}
