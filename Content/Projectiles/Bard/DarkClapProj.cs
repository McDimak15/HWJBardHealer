using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;
using ThoriumMod.Projectiles.Bard;
using ThoriumMod.Sounds;

namespace HWJBardHealer.Content.Projectiles.Bard
{
    public class DarkClapProj : BardProjectile
    {
        public override void SetBardDefaults()
        {
            Projectile.width = 80;
            Projectile.height = 80;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 20;
        }

        public override void BardOnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.immune[Projectile.owner] = 5;
        }

        public override void AI()
        {
            if (Projectile.timeLeft < 10 && Projectile.ai[0] <= 0f)
            {
                SoundEngine.PlaySound(ThoriumSounds.Clap_Sound, Projectile.position);

                int num = 50;
                for (int i = 0; i < num; i++)
                {
                    Vector2 vector = Utils.RotatedBy(
                        -Utils.RotatedBy(Vector2.UnitY, i * MathHelper.TwoPi / num),
                        Projectile.velocity.ToRotation()
                    );

                    Dust dust = Dust.NewDustPerfect(
                        Projectile.Center + vector * 2f,
                        DustID.Smoke,
                        vector * 5f,
                        0,
                        Color.Gray,
                        1.35f
                    );
                    dust.noGravity = true;
                    dust.alpha = 160;
                }

                Projectile.ai[0] = 1f;
                Projectile.friendly = true;
            }
        }
    }

    public class DarkPoetryGlobalProj : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public override void OnKill(Projectile projectile, int timeLeft)
        {
            if (projectile.owner < 0 || projectile.owner >= Main.maxPlayers)
                return;

            Player player = Main.player[projectile.owner];
            ThormwardPlayer modPlayer = player.GetModPlayer<ThormwardPlayer>();

            if (modPlayer.accDarkPoetry == false)
                return;

            if (projectile.ModProjectile is not BardProjectile)
                return;

            if (projectile.localAI[1] != 1f && projectile.numHits < 1)
                return;

            if (Main.myPlayer != player.whoAmI)
                return;

            int damage = (int)(projectile.originalDamage * 0.25f);

            Projectile.NewProjectile(
                player.GetSource_FromThis(),
                projectile.Center,
                Vector2.Zero,
                ModContent.ProjectileType<DarkClapProj>(),
                damage,
                0f,
                player.whoAmI
            );
        }
    }
}
