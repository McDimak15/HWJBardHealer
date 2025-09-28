using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using HWJBardHealer.Content.Weapons.Healer;
using HWJBardHealer.Content.Buffs;

namespace HWJBardHealer.Content.Projectiles.Healer
{
    public class OtherworldlyBubble : ModProjectile
    {
        private bool initialized = false;

        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Projectile.width = 26;  
            Projectile.height = 26;  
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 300; 
            Projectile.ignoreWater = false;
            Projectile.alpha = 255; 
            Projectile.scale = 3.6f;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            // Wobble 
            if (!initialized)
            {
                initialized = true;
                Projectile.ai[0] = Main.rand.NextFloat(0f, MathHelper.TwoPi); 
                Projectile.ai[1] = Main.rand.NextFloat(0.02f, 0.05f); 

                // Ray
                Vector2 dir = Projectile.Center - player.Center;
                int steps = (int)(dir.Length() / 8f);
                for (int i = 0; i < steps; i++)
                {
                    Vector2 pos = player.Center + dir * (i / (float)steps);
                    Dust d = Dust.NewDustPerfect(pos, DustID.BlueCrystalShard, Vector2.Zero, 150, Color.CornflowerBlue, 1.2f);
                    d.noGravity = true;
                }
                Terraria.Audio.SoundEngine.PlaySound(SoundID.Item85, player.Center); 
            }

            // Smooth fade-in
            if (Projectile.alpha > 0)
                Projectile.alpha -= 15;
            if (Projectile.alpha < 0)
                Projectile.alpha = 0;

            if (Projectile.scale < 1f)
                Projectile.scale += 0.05f;
            if (Projectile.scale > 1f)
                Projectile.scale = 1f;

            // Random wobble movement
            float wobbleX = (float)System.Math.Sin(Main.GameUpdateCount * Projectile.ai[1] + Projectile.ai[0]) * 0.6f;
            float wobbleY = (float)System.Math.Cos(Main.GameUpdateCount * Projectile.ai[1] + Projectile.ai[0]) * 0.4f;
            Projectile.velocity = new Vector2(wobbleX, wobbleY - 0.2f); 

            // Glow
            Lighting.AddLight(Projectile.Center, 0.1f, 0.4f, 0.8f);

            // Collect
            if (player.Hitbox.Intersects(Projectile.Hitbox))
            {
                Collect(player);
            }
        }

        private void Collect(Player player)
        {
            // Heal
            player.statLife += 5;
            player.HealEffect(5, true);

            // Bubble count
            var modPlayer = player.GetModPlayer<OtherworldlyPlayer>();
            modPlayer.collectedBubbles++;
            if (modPlayer.collectedBubbles >= 3)
            {
                modPlayer.collectedBubbles = 0;
                player.AddBuff(ModContent.BuffType<BubbleShield>(), 60 * 15);
            }

            // FX
            for (int i = 0; i < 15; i++)
            {
                Dust.NewDust(Projectile.Center, 10, 10, DustID.Water,
                    Main.rand.NextFloat(-2f, 2f),
                    Main.rand.NextFloat(-2f, 2f),
                    150, default, 1.5f);
            }
            Terraria.Audio.SoundEngine.PlaySound(SoundID.Item4, Projectile.Center);

            Projectile.Kill();
        }

        public override void OnKill(int timeLeft)
        {
            // If expired naturally
            if (timeLeft <= 0)
            {
                for (int i = 0; i < 10; i++)
                {
                    Dust.NewDust(Projectile.Center, 10, 10, DustID.Water,
                        Main.rand.NextFloat(-1f, 1f),
                        Main.rand.NextFloat(-1f, 1f));
                }
                Terraria.Audio.SoundEngine.PlaySound(SoundID.Item85, Projectile.Center);
            }
        }
    }
}
