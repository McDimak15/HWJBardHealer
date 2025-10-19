using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;

namespace HWJBardHealer.Content.Projectiles.Thrower
{
    public class PuppetShowProj : ModProjectile
    {
        private int timer;

        public override string Texture => "HWJBardHealer/Content/Weapons/Thrower/PuppetShow";

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.penetrate = 3;
            Projectile.DamageType = DamageClass.Throwing;
            Projectile.timeLeft = 300;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            Projectile.ai[0] = 1f;
            Projectile.ai[1] = target.whoAmI;
            Projectile.velocity = (target.Center - Projectile.Center) * 0.75f;
            Projectile.netUpdate = true;
            Projectile.friendly = false;

            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];
                if (proj.active && proj.owner == Projectile.owner && proj.type == Projectile.type && proj.whoAmI != Projectile.whoAmI)
                {
                    if (proj.ai[1] == Projectile.ai[1])
                        proj.Kill();
                }
            }

            Player player = Main.player[Projectile.owner];
            SoundEngine.PlaySound(SoundID.Item27, player.Center);

            int puppetCount = 0;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];
                if (proj.active && proj.owner == player.whoAmI && proj.type == ModContent.ProjectileType<TomatoPuppet>())
                {
                    if (proj.ModProjectile is TomatoPuppet puppet && puppet.IsFullyActive())
                        puppetCount++;
                }
            }


            if (puppetCount < 3)
            {
                Vector2 spawnOffset = new Vector2(40f, 0f).RotatedByRandom(MathHelper.TwoPi);
                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    player.Center + spawnOffset,
                    Vector2.Zero,
                    ModContent.ProjectileType<TomatoPuppet>(),
                    Projectile.damage / 2,
                    0f,
                    player.whoAmI
                );
            }

        }


        public override void AI()
        {
            bool kill = false;

            if (Projectile.ai[0] == 0f)
            {
                if (timer >= 15)
                {
                    Projectile.velocity.X *= 0.98f;
                    Projectile.velocity.Y += 0.35f;
                }
                else
                {
                    timer++;
                }

                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(90f);
            }
            else if (Projectile.ai[0] == 1f)
            {
                int npcIndex = (int)Projectile.ai[1];
                Projectile.ignoreWater = true;
                Projectile.tileCollide = false;
                Projectile.localAI[0] += 1f;

                if (Projectile.localAI[0] >= 180f)
                {
                    kill = true;
                }

                if (npcIndex < 0 || npcIndex >= Main.maxNPCs)
                {
                    kill = true;
                }
                else if (Main.npc[npcIndex].active && !Main.npc[npcIndex].dontTakeDamage)
                {
                    Projectile.Center = Main.npc[npcIndex].Center - Projectile.velocity * 2f;
                    Projectile.gfxOffY = Main.npc[npcIndex].gfxOffY;
                }
                else
                {
                    kill = true;
                }
            }
            else
            {
                Projectile.localAI[0] += 1f;
                if (Projectile.localAI[0] >= 60f)
                {
                    kill = true;
                }
            }

            if (kill)
                Projectile.Kill();
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return true;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
            {
                int dust = Dust.NewDust(
                    Projectile.position,
                    Projectile.width,
                    Projectile.height,
                    DustID.Torch,
                    Main.rand.NextFloat(-3, 3),
                    Main.rand.NextFloat(-3, 3),
                    150,
                    default,
                    1.25f
                );
                Main.dust[dust].noGravity = true;
            }

            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
        }
    }
}
