using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThoriumMod.Projectiles.Scythe;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using CalamityMod;
using Terraria.ID;
using HWJBardHealer.Content.Weapons.Healer;
using CalamityMod.Buffs.DamageOverTime;

namespace HWJBardHealer.Content.Projectiles.Healer
{
    public class OtherworldlyScythePro : ScythePro
    {
        public override void SafeSetDefaults()
        {
            dustOffset = new Vector2(-35, 7f);
            dustCount = 0;
            dustType = 75;
            Projectile.Size = new Vector2(140f, 150f);
            Projectile.scale = 1.5f; 
        }

        public override void SafeOnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<CrushDepth>(), 180);

            Player owner = Main.player[Projectile.owner];
            owner.GetModPlayer<OtherworldlyPlayer>().OnScytheHit();

            base.SafeOnHitNPC(target, hit, damageDone);
        }

        public override void SafeModifyDamageHitbox(ref Rectangle hitbox)
        {
            int inflateX = (int)(30f);
            int inflateY = (int)(30f);
            hitbox.Inflate(-inflateX, -inflateY);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // Fade 
            lightColor *= MathHelper.Lerp(1f, 0f, Projectile.alpha / 255f);

            // Main scythe texture
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Main.EntitySpriteDraw(
                texture,
                Projectile.Center - Main.screenPosition,
                null,
                lightColor,
                Projectile.rotation + MathHelper.PiOver4 * Projectile.spriteDirection,
                texture.Size() / 2f,
                Projectile.scale,
                (Projectile.spriteDirection < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None),
                0f
            );

            return false;
        }
    }
}
