using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace HWJBardHealer.Content.Projectiles.Thrower
{
    public class DiverMeatballProj : ModProjectile
    {
        public override string Texture => "ContinentOfJourney/NPCs/Boss_Diver/Diver_Minion";

        public override void SetDefaults()
        {
            Projectile.width = 36;
            Projectile.height = 36;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Throwing;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 999;

            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 999;
        }

        public override void AI()
        {
            float fadeDuration = 15f;

            int npcIndex = (int)Projectile.ai[1];

            if (Projectile.ai[0] == 1f)
            {
                Projectile.friendly = false;
                Projectile.localAI[0]++;
                Projectile.Opacity = MathHelper.Clamp(Projectile.localAI[0] / fadeDuration, 0f, 1f);
                Projectile.velocity *= 0.98f;

                if (Projectile.localAI[0] >= fadeDuration)
                {
                    Projectile.ai[0] = 2f;
                    Projectile.friendly = true;
                    Projectile.timeLeft = 50;
                    Projectile.velocity = Vector2.Zero;

                    if (Main.npc.IndexInRange(npcIndex) && Main.npc[npcIndex].active)
                    {
                        Vector2 offset = Projectile.Center - Main.npc[npcIndex].Center;
                        Projectile.localAI[1] = offset.ToRotation();
                    }
                }
            }
            else if (Projectile.ai[0] == 2f)
            {
                bool validTarget = Main.npc.IndexInRange(npcIndex) && Main.npc[npcIndex].active;

                if (!validTarget)
                {
                    Projectile.Kill();
                    return;
                }

                NPC target = Main.npc[npcIndex];
                float orbitRadius = 40f;
                float angularSpeed = MathHelper.ToRadians(2.5f);
                Projectile.localAI[1] += angularSpeed;
                Vector2 offset = Projectile.localAI[1].ToRotationVector2() * orbitRadius;
                Projectile.Center = target.Center + offset;
                Projectile.Opacity = 1f;
                Projectile.velocity = Vector2.Zero;
            }

            Projectile.rotation += 0.12f;

            if (Main.rand.NextBool(5))
            {
                int d = Dust.NewDust(
                    Projectile.position,
                    Projectile.width,
                    Projectile.height,
                    DustID.Wraith,
                    0, 0, 120,
                    new Color(18, 28, 45)
                );
                Main.dust[d].noGravity = true;
                Main.dust[d].scale = 1.3f;
            }
            Lighting.AddLight(Projectile.Center, 0.08f * Projectile.Opacity, 0.14f * Projectile.Opacity, 0.28f * Projectile.Opacity);
        }


        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode with { Pitch = -0.3f }, Projectile.Center);

            for (int i = 0; i < 35; i++)
            {
                int d = Dust.NewDust(
                    Projectile.position,
                    Projectile.width,
                    Projectile.height,
                    DustID.DungeonWater,
                    Main.rand.NextFloat(-5f, 5f),
                    Main.rand.NextFloat(-5f, 5f),
                    150,
                    new Color(12, 20, 36)
                );
                Main.dust[d].noGravity = true;
                Main.dust[d].scale = Main.rand.NextFloat(1.5f, 2.4f);
            }

            int explosionRadius = 96;

            Rectangle explodeArea = new Rectangle(
                (int)Projectile.Center.X - explosionRadius / 2,
                (int)Projectile.Center.Y - explosionRadius / 2,
                explosionRadius,
                explosionRadius
            );

            NPC.HitInfo hitInfo = new NPC.HitInfo()
            {
                SourceDamage = Projectile.damage,
                Knockback = 6f,
                HitDirection = Projectile.direction,
            };

            for (int i = 0; i < Main.npc.Length; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && !npc.friendly && npc.Hitbox.Intersects(explodeArea))
                {
                    npc.StrikeNPC(hitInfo);
                }
            }
        }
    }
}
