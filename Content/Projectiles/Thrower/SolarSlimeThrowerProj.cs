using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace HWJBardHealer.Content.Projectiles.Thrower
{
    public class SolarSlimeThrowerProj : ModProjectile
    {
        public override string Texture => "HWJBardHealer/Content/Projectiles/Thrower/SlimeWind";

        private float alpha;

        public override void SetDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 64;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 120;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Throwing;
        }

        private NPC FindTarget(float maxRange)
        {
            NPC best = null;

            float bestDistance = maxRange;
            for (int k = 0; k < Main.maxNPCs; k++)
            {
                NPC npc = Main.npc[k];
                if (npc.CanBeChasedBy(this))
                {
                    float dist = Vector2.Distance(Projectile.Center, npc.Center);
                    if (dist < bestDistance)
                    {
                        best = npc;
                        bestDistance = dist;
                    }
                }
            }
            return best;
        }

        public override void AI()
        {
            if (Projectile.timeLeft > 100)
                alpha = MathHelper.Lerp(0f, 1f, (120 - Projectile.timeLeft) / 20f);
            else if (Projectile.timeLeft < 15)
                alpha = MathHelper.Lerp(0f, 1f, Projectile.timeLeft / 15f);
            else
                alpha = 1f;

            NPC target = FindTarget(500f);
            Vector2 faceDirection;

            if (target != null)
            {
                faceDirection = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero);
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, faceDirection * 10f, 0.08f);
            }
            else
            {
                faceDirection = Projectile.velocity.SafeNormalize(Vector2.Zero);
            }
            float targetRot = faceDirection.ToRotation() + MathHelper.PiOver2;
            Projectile.rotation = MathHelper.Lerp(Projectile.rotation, targetRot, 0.15f);
            Lighting.AddLight(Projectile.Center, 1f * alpha, 0.6f * alpha, 0.2f * alpha);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 origin = tex.Size() / 2f;

            Main.EntitySpriteDraw(
                tex,
                Projectile.Center - Main.screenPosition,
                null,
                Color.White * alpha,
                Projectile.rotation,
                origin,
                0.9f,
                SpriteEffects.None,
                0
            );

            return false;
        }
    }
}