using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using Terraria.Audio;
using Terraria.GameContent;
using ThoriumMod.Projectiles;
using HWJBardHealer.Core;

namespace HWJBardHealer.Content.Projectiles.Healer
{
    public class DipperWandProj : ThoriumProjectile
    {
        public override string Texture => "ContinentOfJourney/Projectiles/Pointer";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = ModLoader.GetMod("ThoriumMod").Find<DamageClass>("HealerDamage");
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = 0;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 180;
        }

        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(SoundID.Item72 with { Volume = 0.8f, PitchVariance = 0.2f }, Projectile.Center);

            for (int i = 0; i < 15; i++)
            {
                int dust = Dust.NewDust(Projectile.Center, 0, 0, DustID.Electric,
                    Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 2f), 150, default, 1.2f);
                Main.dust[dust].noGravity = true;
            }
        }

        public override void AI()
        {
            if (Projectile.velocity.Length() > 0.1f)
                Projectile.rotation = Projectile.velocity.ToRotation();

            // homing
            NPC closest = null;
            float closestDist = 300f;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && !npc.friendly && !npc.dontTakeDamage)
                {
                    float dist = Vector2.Distance(npc.Center, Projectile.Center);
                    if (dist < closestDist)
                    {
                        closestDist = dist;
                        closest = npc;
                    }
                }
            }

            if (closest != null)
            {
                Vector2 toTarget = closest.Center - Projectile.Center;
                toTarget.Normalize();
                toTarget *= 0.5f; // homing strength
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.velocity + toTarget, 0.08f);
            }

            Lighting.AddLight(Projectile.Center, 0.2f, 0.4f, 0.9f);

            if (Main.rand.NextBool(3))
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.BlueCrystalShard,
                    Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 100, default, 1f);
                Main.dust[dust].noGravity = true;
            }

            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                if (!player.active || player.dead) continue;

                if (i == Projectile.owner) continue;

                if (player.Hitbox.Intersects(Projectile.Hitbox))
                {
                    HealPlayer(player, 4);
                    Projectile.Kill();
                }
            }
        }

        private void HealPlayer(Player player, int healAmount)
        {
            HealerHelper.HealPlayer(
                Main.player[base.Projectile.owner],
                player,
                healAmount: healAmount,
                recoveryTime: 60,
                healEffects: false
            );

            if (Main.myPlayer == Projectile.owner)
            {
                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    player.Center,
                    Vector2.Zero,
                    ModContent.ProjectileType<DipperDroneProj>(),
                    0,
                    0f,
                    Projectile.owner
                );
            }

            for (int i = 0; i < 20; i++)
            {
                Vector2 vel = Main.rand.NextVector2Circular(2f, 2f);
                int dust = Dust.NewDust(player.Center, 0, 0, DustID.HealingPlus, vel.X, vel.Y, 100, default, 1.4f);
                Main.dust[dust].noGravity = true;
            }

            SoundEngine.PlaySound(SoundID.Item4 with { Volume = 0.8f, Pitch = 0.2f }, player.Center);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(SoundID.Item10, target.Center);

            for (int i = 0; i < 20; i++)
            {
                int dust = Dust.NewDust(target.Center, 0, 0, DustID.BlueCrystalShard,
                    Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-3f, 3f), 100, default, 1.5f);
                Main.dust[dust].noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 origin = texture.Size() / 2f;

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                if (Projectile.oldPos[i] == Vector2.Zero) continue;

                float progress = 1f - i / (float)Projectile.oldPos.Length;
                Vector2 drawPos = Projectile.oldPos[i] + Projectile.Size / 2f - Main.screenPosition;

                Color trailColor = Color.CornflowerBlue * progress * 0.6f;
                Main.EntitySpriteDraw(texture, drawPos, null, trailColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            }

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
    }
}
