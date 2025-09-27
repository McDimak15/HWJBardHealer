using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using ThoriumMod.Projectiles.Scythe;

namespace HWJBardHealer.Content.Projectiles.Healer
{
    public class RodScythePro : ScythePro
    {
        public override void SafeSetDefaults()
        {
            Projectile.width = 104;
            Projectile.height = 203;
            Projectile.friendly = true;
            Projectile.hostile = false;

            dustOffset = new Vector2(-20f, 8f);
            dustCount = 4;
            dustType = DustID.GoldCoin;

        }
        public override void SafeOnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // Explosion dust
            for (int i = 0; i < 25; i++)
            {
                int dust = Dust.NewDust(target.position, target.width, target.height,
                    DustID.GoldCoin, Main.rand.NextFloat(-4, 4), Main.rand.NextFloat(-4, 4), 150, default, 1.5f);
                Main.dust[dust].noGravity = true;
            }

            Terraria.Audio.SoundEngine.PlaySound(SoundID.Item14, target.Center);

            // Small explosion
            int explosionDamage = (int)(damageDone * 0.9f);
            Projectile.NewProjectile(
                Projectile.GetSource_FromThis(),
                target.Center - new Vector2(0, 40),
                Vector2.Zero,
                ProjectileID.DD2ExplosiveTrapT3Explosion,
                explosionDamage,
                2f,
                Projectile.owner
            );
        }

        // Match hitbox
        public override void SafeModifyDamageHitbox(ref Rectangle hitbox)
        {
            int extraWidth = 40; 
            int extraHeight = 0;  
            hitbox.Inflate(extraWidth, extraHeight);
            //hitbox.Offset(0, 20);
        }
    }
}
