using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;
using ThoriumMod;
using ThoriumMod.Items.HealerItems;
using HWJBardHealer.Core;

namespace HWJBardHealer.Content.Projectiles.Healer
{
    public class SolarStaffProj : ModProjectile
    {
        public override string Texture => "Terraria/Images/Extra_98";
        private static readonly Color FlareColor = new Color(232, 120, 39);

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = 0;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.timeLeft = 120;
            Projectile.penetrate = 1;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = ThoriumDamageBase<HealerTool>.Instance;
            Projectile.alpha = 50;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 1.2f, 1.0f, 0.5f);

            //Projectile.rotation += 0.2f * Projectile.direction;

            if (Main.rand.NextBool(3))
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.GoldFlame, Projectile.velocity * -0.3f, 150, Color.Yellow, 1.2f);
                dust.noGravity = true;
            }

            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                if (!player.active || player.dead)
                    continue;

                if (Projectile.owner == i)
                    continue;

                if (Projectile.Hitbox.Intersects(player.Hitbox))
                {
                    HealerHelper.HealPlayer(
                        Main.player[Projectile.owner],
                        player,
                        healAmount: 10,
                        recoveryTime: 60,
                        healEffects: false
                    );

                    if (Main.myPlayer == Projectile.owner)
                    {
                        Player owner = Main.player[Projectile.owner];
                        int slimeType = ModContent.ProjectileType<SolarSlimeProj>();

                        if (owner.ownedProjectileCounts[slimeType] == 0)
                        {
                            Projectile.NewProjectile(
                                Projectile.GetSource_FromThis(),
                                owner.Center - new Vector2(0, 40),
                                Vector2.Zero,
                                slimeType,
                                0,
                                0f,
                                owner.whoAmI
                            );
                        }
                    }

                    Projectile.Kill();
                    return;
                }
            }

            Projectile.rotation = Utils.ToRotation(Projectile.velocity);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D trailTex = ModContent.Request<Texture2D>(Texture).Value;
            float fade = MathHelper.Min((float)Projectile.timeLeft, 15f) / 15f;
            float rotation;

            for (int i = 1; i < Projectile.oldPos.Length; i++)
            {
                if (Projectile.oldPos[i] == Vector2.Zero) continue;

                for (rotation = MathHelper.ToRadians((float)(base.Projectile.timeLeft + i * base.Projectile.oldPos.Length)) - base.Projectile.oldRot[i]; rotation > 3.1415927f; rotation -= 6.2831855f)
                {
                }
                while (rotation < -3.1415927f)
                {
                    rotation += 6.2831855f;
                }

                float progress = (float)i / Projectile.oldPos.Length;
                float scale = base.Projectile.scale * MathHelper.Lerp(1.1f, 0.2f, (float)i / (float)base.Projectile.oldPos.Length) * fade;

                Color color = FlareColor * (1f - progress) * fade;
                color.A = 0;

                Vector2 drawPos = Projectile.oldPos[i] + Projectile.Size * 0.5f - Main.screenPosition;
                float rotationDo = Projectile.oldRot[i] - 1.5707964f;

                Main.EntitySpriteDraw(trailTex, Projectile.oldPos[i] + Projectile.Size * 0.5f - Main.screenPosition, null, color, Projectile.oldRot[i] - 1.5707964f, Utils.Size(trailTex) * 0.5f, Projectile.scale * MathHelper.Lerp(1.1f, 0.2f, (float)i / (float)Projectile.oldPos.Length) * fade, 0, 0);
            }
            for (rotation = MathHelper.ToRadians((float)base.Projectile.timeLeft) - base.Projectile.rotation; rotation > 3.1415927f; rotation -= 6.2831855f)
            {
            }
            while (rotation < -3.1415927f)
            {
                rotation += 6.2831855f;
            }
            return false;
        }

    }
}
