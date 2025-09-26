using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using ThoriumMod.Projectiles.Scythe;

namespace HWJBardHealer.Core
{
    [ExtendsFromMod("ThoriumMod")]
    public class HWJThrownScytheProjectile : GlobalProjectile
    {
        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation)
        {
            List<ModItem> throwableScythes = new();

            if (ModLoader.TryGetMod("HWJBardHealer", out Mod HWJbardhealer))
            {
                if (entity.type == HWJbardhealer.Find<ModProjectile>("OtherworldlyScythePro").Type) return true;
                if (entity.type == HWJbardhealer.Find<ModProjectile>("RodScythePro").Type) return true;
            }

            foreach (var item in throwableScythes)
            {
                if (item != null && entity.type == item.Type)
                    return true;
            }
            return false;
        }

        public override bool InstancePerEntity => true;

        public float rotationSpeed; // Base rotation speed
        public int scytheCount;
        public int dustCount;
        public int dustType;
        public Vector2 dustOffset = Vector2.Zero;

        public Vector2 DustCenterBase;
        public Vector2 DustCenter => DustCenterBase + dustOffset;

        public override bool PreAI(Projectile projectile)
        {
            // Grab values from Thorium scythes if applicable
            if (projectile.ModProjectile is ScythePro scythePro)
            {
                rotationSpeed = scythePro.rotationSpeed;
                scytheCount = scythePro.scytheCount;
                dustCount = scythePro.dustCount;
                dustType = scythePro.dustType;
                dustOffset = scythePro.dustOffset;
                DustCenterBase = new Vector2(projectile.width / 2f, projectile.height / 2f);
            }

            if (projectile.ai[0] != 0f) // Thrown mode
            {
                // --- Ensure scaling from ProjectileSizeScaling is respected ---

                Player player = Main.player[projectile.owner];
                player.heldProj = projectile.whoAmI;

                // Face the correct way based on aim
                player.ChangeDir(projectile.velocity.X < 0f ? 1 : -1);
                projectile.spriteDirection = player.direction;

                // Spin faster while thrown
                projectile.rotation += rotationSpeed * projectile.spriteDirection * 2.5f;

                // Keep projectile alive for full ai[1] duration
                if (projectile.ai[1] - projectile.ai[2] > projectile.timeLeft)
                    projectile.timeLeft++;

                float attackTime = (++projectile.ai[2]) / projectile.ai[1];
                float v = projectile.velocity.Length();

                // Orbit multipliers (width = horizontal, height = vertical)
                float widthScale = GetCustomWidth(projectile);
                float heightScale = GetCustomHeight(projectile);

                // Base orbit circle
                Vector2 orbit = Vector2.UnitX.RotatedBy(attackTime * MathHelper.TwoPi);

                // Apply scales: velocity length × custom × projectile.scale
                orbit *= new Vector2(
                    v * widthScale,
                    v * heightScale * Math.Max(projectile.ai[0], 0.001f) * projectile.spriteDirection
                );

                orbit = orbit.RotatedBy(projectile.velocity.ToRotation());
                projectile.Center = player.MountedCenter + orbit - projectile.velocity;

                // Play sound when rotation loops
                if (projectile.rotation > Math.PI)
                {
                    SoundEngine.PlaySound(SoundID.Item1, projectile.Center);
                    projectile.rotation -= MathHelper.TwoPi;
                }
                else if (projectile.rotation < -Math.PI)
                {
                    SoundEngine.PlaySound(SoundID.Item1, projectile.Center);
                    projectile.rotation += MathHelper.TwoPi;
                }

                SpawnDust(projectile);

                return false; // Skip normal scythe AI
            }

            return true; // Normal swing AI for left-click
        }

        // -------- Custom orbit multipliers --------
        private float GetCustomWidth(Projectile projectile)
        {
            // Thorium defaults
            if (ModLoader.TryGetMod("ThoriumMod", out Mod thorium))
            {
                //if (projectile.type == thorium.Find<ModProjectile>("TitanScythePro").Type) return 1.2f;
                //if (projectile.type == thorium.Find<ModProjectile>("BatScythePro").Type) return 1.1f;
            }

            // Ragnarok defaults
            if (ModLoader.TryGetMod("RagnarokMod", out Mod ragnarok))
            {
                //if (projectile.type == ragnarok.Find<ModProjectile>("ProfanedScythePro").Type) return 1.4f;
            }

            return 1.15f; // fallback
        }

        private float GetCustomHeight(Projectile projectile)
        {
            if (ModLoader.TryGetMod("ThoriumMod", out Mod thorium))
            {
                //if (projectile.type == thorium.Find<ModProjectile>("TitanScythePro").Type) return 2.5f;
                //if (projectile.type == thorium.Find<ModProjectile>("BatScythePro").Type) return 1.75f;
            }

            if (ModLoader.TryGetMod("RagnarokMod", out Mod ragnarok))
            {
                //if (projectile.type == ragnarok.Find<ModProjectile>("ProfanedScythePro").Type) return 3f;
            }

            return 1.5f; // fallback
        }

        // -------- Dust spawning (unchanged except for using new DustCenterBase) --------
        private void SpawnDust(Projectile projectile)
        {
            int num = dustCount;
            int num2 = scytheCount;
            int num3 = dustType;
            Vector2 dustCenter = DustCenter;
            if (num2 <= 0 || num <= 0 || num3 <= -1)
                return;

            for (int i = 0; i < num2; i++)
            {
                float num4 = (float)i * ((float)Math.PI * 2f / (float)num2);
                float rotation = projectile.rotation;
                Vector2 val = dustCenter;
                if (projectile.spriteDirection < 0)
                    val.X = 0f - val.X;

                val = Utils.RotatedBy(val, (double)(rotation + num4), default);
                Vector2 val2 = projectile.Center + new Vector2(0f, projectile.gfxOffY) + val;
                for (int j = 0; j < num; j++)
                {
                    Dust val3 = Dust.NewDustPerfect(val2, num3, (Vector2?)Vector2.Zero, 0, default(Color), 1f);
                    val3.noGravity = true;
                    val3.noLight = true;
                    ModifyDust(projectile, val3, val2, i);
                }
            }
        }

        public void ModifyDust(Projectile projectile, Dust dust, Vector2 position, int scytheIndex)
        {
            if (projectile.ModProjectile is ScythePro scythePro)
                scythePro.ModifyDust(dust, position, scytheIndex);
        }
    }
}
