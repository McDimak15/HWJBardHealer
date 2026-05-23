using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using ThoriumMod;
using ThoriumMod.Dusts;
using ThoriumMod.Projectiles;
using ThoriumMod.Projectiles.Healer;
using ThoriumMod.Utilities;

namespace HWJBardHealer.Content.Projectiles.Healer
{
    public class BifurcationProj : ThoriumProjectile
    {
        private bool hasHit;

        public override void SetDefaults()
        {
            Projectile.DamageType = ThoriumDamageBase<HealerDamage>.Instance;
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ownerHitCheck = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.hide = true;
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            hitbox.Inflate(Projectile.width / 2, Projectile.height / 2);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            Player player = Main.player[Projectile.owner];
            modifiers.HitDirectionOverride = (target.Center.X < player.Center.X) ? -1 : 1;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.direction = player.direction;
            player.heldProj = Projectile.whoAmI;

            if (player.dead || player.frozen || player.itemAnimation <= 1)
            {
                Projectile.Kill();
                if (player.itemAnimation <= 1)
                    player.reuseDelay = 2;
                return;
            }

            for (int i = 0; i < 3; i++)
            {
                int dust = Dust.NewDust(Projectile.Center, Projectile.width / 2, Projectile.height / 2, DustID.MoonBoulder, 0f, 0f, 150, Color.White, 1f);
                Main.dust[dust].noGravity = true;
            }

            float animProgress = (float)player.itemAnimation / player.itemAnimationMax;
            float t = 1f - animProgress;
            float velocityRot = Projectile.velocity.ToRotation();
            float velocityLen = Projectile.velocity.Length();
            float flailSwingRadius = Projectile.ai[1];
            Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter, false, true);

            Vector2 offset = Vector2.UnitX.RotatedBy(Math.PI + t * MathHelper.TwoPi) * new Vector2(velocityLen, flailSwingRadius);
            Projectile.Center = playerCenter + offset.RotatedBy(velocityRot) + Vector2.UnitX.RotatedBy(velocityRot) * (velocityLen + 15f);
            Projectile.rotation = (Projectile.Center - playerCenter).ToRotation();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];
            if (!hasHit && target.CanBeChasedBy())
            {
                Terraria.Audio.SoundEngine.PlaySound(SoundID.Item29, target.Center);

                int orbCount = 2;
                int orbType = ModContent.ProjectileType<HealingOrbTeal>();
                if (player.ownedProjectileCounts[orbType] >= 8)
                {
                    orbCount = 0;
                }
                IEntitySource source = Projectile.GetSource_OnHit(target);
                for (int i = 0; i < orbCount; i++)
                {
                    Vector2 velocity = Main.rand.NextVector2Circular(4f, 4f);
                    Projectile.NewProjectile(source, target.Center.X, target.Center.Y, (float)Main.rand.Next(-5, 5), (float)Main.rand.Next(-5, 5), orbType, 0, 0f, base.Projectile.owner, 0f, 0f, 0f);
                }

                for (int j = 0; j < 12; j++)
                {
                    int d = Dust.NewDust(target.position, target.width, target.height, DustID.GoldFlame, Main.rand.NextFloat(-3, 3), Main.rand.NextFloat(-3, 3), 150, Color.Goldenrod, 1.2f);
                    Main.dust[d].noGravity = true;
                }

                if (Main.myPlayer == Projectile.owner)
                {
                    Projectile.NewProjectile(
                        Projectile.GetSource_OnHit(target),
                        target.Center,
                        Vector2.Zero,
                        ModContent.ProjectileType<PenroseTriangleProj>(),
                        (int)(damageDone * 0.1f),
                        0f,
                        Projectile.owner,
                        target.whoAmI 
                    );
                }

                hasHit = true;
            }
        }

        public override bool ShouldUpdatePosition() => false;

        public override bool PreDraw(ref Color lightColor)
        {
            ProjectileExtras.DrawChain(Projectile.whoAmI, Main.player[Projectile.owner].MountedCenter, Texture + "_Chain", null);

            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 origin = texture.Size() / 2f;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, origin, 1f, SpriteEffects.None, 0f);
            return false;
        }
    }
}
