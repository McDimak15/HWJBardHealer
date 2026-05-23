using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using HWJBardHealer.Content.Weapons.Thrower;

namespace HWJBardHealer.Content.Projectiles.Thrower
{
    public class DiverTomahawkProj : ModProjectile
    {
        private int timer;

        public override void SetDefaults()
        {
            Projectile.width = 34;
            Projectile.height = 34;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Throwing;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            bool killFlag = false;
            if (Projectile.ai[0] == 0f)
            {
                if (timer >= 15)
                {
                    Projectile.velocity.X *= 0.98f;
                    Projectile.velocity.Y += 0.35f;
                }
                else timer++;
                float spinSpeed = Projectile.velocity.Length() * 0.03f;
                Projectile.rotation += spinSpeed;
            }
            else if (Projectile.ai[0] == 1f)
            {
                Projectile.ignoreWater = true;
                Projectile.tileCollide = false;
                Projectile.localAI[0]++;

                int npcIndex = (int)Projectile.ai[1];
                if (!Main.npc.IndexInRange(npcIndex))
                    killFlag = true;
                else
                {
                    NPC target = Main.npc[npcIndex];

                    if (target.active && !target.dontTakeDamage)
                    {
                        Projectile.Center = target.Center - Projectile.velocity * 2f;
                        Projectile.gfxOffY = target.gfxOffY;
                    }
                    else killFlag = true;
                }

                if (Projectile.localAI[0] >= 180f)
                    killFlag = true;
            }

            if (killFlag)
                Projectile.Kill();

            Lighting.AddLight(Projectile.Center, 0.05f, 0.12f, 0.25f);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Projectile.ai[0] != 0f)
                return;
            Projectile.ai[0] = 1f;
            Projectile.ai[1] = target.whoAmI;
            Projectile.velocity = (target.Center - Projectile.Center) * 0.75f;
            Projectile.friendly = false;
            Projectile.netUpdate = true;

            var data = target.GetGlobalNPC<DiverTomahawkGlobalNPC>();
            data.tomahawkStacks++;

            DoDarkHitFX();

            if (data.tomahawkStacks >= 4)
            {
                data.tomahawkStacks = 0;
                SpawnMeatballs(target);
            }
        }

        private void DoDarkHitFX()
        {
            SoundEngine.PlaySound(SoundID.NPCDeath14 with { Pitch = -0.45f });

            for (int i = 0; i < 12; i++)
            {
                int d = Dust.NewDust(
                    Projectile.position,
                    Projectile.width,
                    Projectile.height,
                    DustID.DungeonWater,
                    Main.rand.NextFloat(-2.5f, 2.5f),
                    Main.rand.NextFloat(-2.5f, 2.5f),
                    100,
                    new Color(10, 18, 32)
                );
                Main.dust[d].scale = Main.rand.NextFloat(1.2f, 1.9f);
                Main.dust[d].noGravity = true;
            }
        }

        private void SpawnMeatballs(NPC target)
        {
            SoundEngine.PlaySound(
                SoundID.DD2_BetsyFireballShot with { Pitch = -0.4f },
                target.Center
            );

            float[] angles = {
                0f,
                MathHelper.ToRadians(120f),
                MathHelper.ToRadians(240f)
            };

            float spawnSpeed = 4f;
            float spawnOffset = 20f;

            foreach (float a in angles)
            {
                Vector2 direction = a.ToRotationVector2();
                Vector2 startPos = target.Center + direction * spawnOffset;
                Vector2 initialVel = direction * spawnSpeed;

                int p = Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    startPos,
                    initialVel,
                    ModContent.ProjectileType<DiverMeatballProj>(),
                    Projectile.damage * 2,
                    2f,
                    Projectile.owner,
                    1f,
                    target.whoAmI 
                );
            }
        }
    }
}
