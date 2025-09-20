using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Buffs.DamageOverTime;
using HWJBardHealer.Content.Projectiles.Thrower;
using ContinentOfJourney.Items.Material;

namespace HWJBardHealer.Content.Projectiles.Thrower
{
    public class CubismKunaiProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Cubism Kunai");
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.DamageType = DamageClass.Throwing;
            Projectile.aiStyle = 2;
            AIType = ProjectileID.ThrowingKnife;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
            Projectile.extraUpdates = 1;
        }

        public override bool? CanCutTiles() => true;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;

            Main.EntitySpriteDraw(
                texture,
                drawPosition,
                null,
                lightColor,
                Projectile.rotation,
                new Vector2(texture.Width / 2f, texture.Height / 2f),
                Projectile.scale,
                SpriteEffects.None,
                0
             );
            return false;
        }

        public override bool PreKill(int timeLeft)
        {
            for (int i = 0; i < 12; i++)
            {
                int dust = Dust.NewDust(Projectile.position, 24, 24, 88);
                Main.dust[dust].velocity *= 5f;
                Main.dust[dust].noGravity = true;
            }
            return true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Vaporfied>(), 180);
            Terraria.Audio.SoundEngine.PlaySound(SoundID.Item62, Projectile.position);

            // Spawn cube particles
            for (int i = 0; i < 12; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height,
                    DustID.Electric, Main.rand.NextFloat(-3, 3), Main.rand.NextFloat(-3, 3), 150,
                    Color.White, Main.rand.NextFloat(1.2f, 1.8f));

                Main.dust[dust].noGravity = true;
                Main.dust[dust].rotation = MathHelper.ToRadians(Main.rand.Next(360));
                Main.dust[dust].scale = 1.1f;
            }

            // Spawn the big cube explosion
            Projectile.NewProjectile(
                Projectile.GetSource_FromThis(),
                Projectile.Center,
                Vector2.Zero,
                ModContent.ProjectileType<FissionThrow_2>(),
                Projectile.damage,
                Projectile.knockBack,
                Projectile.owner);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Terraria.Audio.SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
            return true;
        }

        public override void AI()
        {
            int dust = Dust.NewDust(Projectile.position - new Vector2(4, 4), 32, 32, 88);
            Main.dust[dust].noGravity = true;
        }
    }

}
