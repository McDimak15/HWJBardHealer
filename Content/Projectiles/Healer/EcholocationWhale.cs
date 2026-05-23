using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using ThoriumMod.Projectiles;

namespace HWJBardHealer.Content.Projectiles.Healer
{
    public class EcholocationWhale : ThoriumProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 32;
            Projectile.DamageType = ModLoader.GetMod("ThoriumMod").Find<DamageClass>("HealerDamage");
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 1800;
            Projectile.netUpdate = true;
        }

        public override void AI()
        {
            Projectile.velocity.Y = (float)Math.Sin(Main.GlobalTimeWrappedHourly * 3f) * 0.4f;

            if (Main.rand.NextBool(5))
            {
                Color mainColor = new Color(130, 144, 144);
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.WhiteTorch, 0f, 0f, 100, mainColor, 1.2f);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity *= 0.5f;
            }

            Projectile.ai[0]++;

            if (Projectile.ai[0] >= 45)
            {
                Projectile.ai[0] = 0;

                if (Main.myPlayer == Projectile.owner)
                {
                    bool shotFired = TryShootPlayer() || TryShootEnemy();

                    if (shotFired)
                    {
                        SoundEngine.PlaySound(SoundID.Item85 with { Pitch = -0.2f, Volume = 0.6f }, Projectile.Center); 
                        Projectile.netUpdate = true;
                    }
                }
            }
        }

        private bool TryShootPlayer()
        {
            Player targetPlayer = null;
            float closestDist = 600f;

            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                if (player.active && !player.dead && player.statLife < player.statLifeMax2)
                {
                    float dist = Vector2.Distance(player.Center, Projectile.Center);
                    if (dist < closestDist)
                    {
                        closestDist = dist;
                        targetPlayer = player;
                    }
                }
            }

            if (targetPlayer != null)
            {
                Vector2 vel = Vector2.Normalize(targetPlayer.Center - Projectile.Center) * 4f;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, vel, ModContent.ProjectileType<WhaleEchoHeal>(), 0, 0, Projectile.owner, targetPlayer.whoAmI);
                return true;
            }
            return false;
        }

        private bool TryShootEnemy()
        {
            NPC targetNPC = null;
            float closestDist = 600f;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.CanBeChasedBy())
                {
                    float dist = Vector2.Distance(npc.Center, Projectile.Center);
                    if (dist < closestDist && Collision.CanHitLine(Projectile.Center, 1, 1, npc.Center, 1, 1))
                    {
                        closestDist = dist;
                        targetNPC = npc;
                    }
                }
            }

            if (targetNPC != null)
            {
                Vector2 vel = Vector2.Normalize(targetNPC.Center - Projectile.Center) * 4f;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, vel, ModContent.ProjectileType<WhaleEchoHarm>(), Projectile.damage, Projectile.knockBack, Projectile.owner, targetNPC.whoAmI);
                return true;
            }
            return false;
        }
    }
}