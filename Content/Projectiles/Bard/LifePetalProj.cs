using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ThoriumMod.Dusts;
using ThoriumMod.Buffs;
using ThoriumMod.Utilities;
using ThoriumMod.Projectiles.Bard;
using HWJBardHealer; 
using ContinentOfJourney.Buffs; 

namespace HWJBardHealer.Content.Projectiles.Bard
{
    public class LifePetalProj : BardProjectile
    {
        public int Index => (int)Projectile.ai[0];

        private float rot;
        private float pulse1;
        private float pulse2;

        public override void SetBardDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.netImportant = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 1600;
            Projectile.localNPCHitCooldown = 5;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return (Projectile.ai[1] == 0f)
                ? new Color?(Color.White * 0.2f)
                : new Color?(Color.White * 0.75f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 origin = new Vector2(Projectile.width * 0.5f, Projectile.height * 0.55f);
            Main.EntitySpriteDraw(
                ModContent.Request<Texture2D>(Texture, ReLogic.Content.AssetRequestMode.ImmediateLoad).Value,
                Projectile.Center - Main.screenPosition,
                null,
                Color.White * ((Projectile.ai[1] == 0f) ? 0.1f : 0.35f),
                Projectile.rotation,
                origin,
                1.1f + pulse1,
                SpriteEffects.None,
                0f
            );
            return true;
        }

        public override void BardModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            Player player = Main.player[Projectile.owner];
            modifiers.HitDirectionOverride = (target.Center.X < player.Center.X) ? -1 : 1;
        }

        public override void BardOnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.ai[1] = 0f;
            SpawnDust(Color.LightGreen);
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            var modPlayer = player.GetModPlayer<ThormwardPlayer>();

            if (!modPlayer.accLifePedal.Active)
                return;

            Projectile.timeLeft = 2;
            rot += 0.035f;

            Projectile.Center = player.Center + Utils.RotatedBy(
                new Vector2(0f, 60f),
                rot + Index * (MathHelper.TwoPi / player.ownedProjectileCounts[Projectile.type])
            );

            Projectile.gfxOffY = player.gfxOffY;
            Projectile.rotation -= 0.035f;

            if (pulse2 == 0f)
            {
                pulse1 += 0.04f;
                if (pulse1 >= 0.4f) pulse2 = 1f;
            }
            else
            {
                pulse1 -= 0.04f;
                if (pulse1 <= 0f) pulse2 = 0f;
            }

            if (Projectile.ai[1] == 0f)
            {
                Projectile.friendly = false;
                return;
            }

            Projectile.friendly = true;

            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player ally = Main.player[i];
                if (ally.active && ally.whoAmI != player.whoAmI && ally.DistanceSQ(Projectile.Center) < 900f)
                {
                    if (Main.myPlayer == ally.whoAmI)
                    {
                        ally.AddBuff(BuffID.Panic, 180);
                        ally.AddBuff(ModContent.BuffType<ReanimationBuff>(), 180);
                    }

                    SpawnDust(Color.Pink);
                    Projectile.ai[1] = 0f;
                    return;
                }
            }

            for (int n = 0; n < Main.maxNPCs; n++)
            {
                NPC npc = Main.npc[n];
                if (npc.active && !npc.friendly && !npc.dontTakeDamage && npc.DistanceSQ(Projectile.Center) < 900f)
                {
                    npc.AddBuff(BuffID.Venom, 300);

                    SpawnDust(Color.LimeGreen);
                    Projectile.ai[1] = 0f;
                    return;
                }
            }
        }

        private void SpawnDust(Color color)
        {
            for (int i = 0; i < 20; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height,
                    ModContent.DustType<PollenDust>(),
                    Utils.NextFloat(Main.rand, -3f, 3f),
                    Utils.NextFloat(Main.rand, -3f, 3f),
                    125, color, 1.25f);
                Main.dust[dust].noGravity = true;
            }
        }
    }
}
