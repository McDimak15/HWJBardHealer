using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace HWJBardHealer.Content.Projectiles.Thrower
{
    public class FissionThrow_2 : ModProjectile
    {
        // Use the exact same texture as the original melee version
        public override string Texture => "ContinentOfJourney/Projectiles/Meelee/Fission_2";

        public override void SetDefaults()
        {
            Projectile.width = 200;
            Projectile.height = 200;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 31;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Throwing;
            Projectile.ignoreWater = true;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 2;
        }

        public override bool? CanCutTiles() => true;

        public override bool PreDraw(ref Color lightColor)
        {
            int frame = 31 - Projectile.timeLeft;
            int Y = (int)(frame / 6);
            int X = frame % 6;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive,
                SamplerState.AnisotropicClamp, DepthStencilState.None,
                RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            Main.EntitySpriteDraw(ModContent.Request<Texture2D>(Texture).Value,
                Projectile.Center - Main.screenPosition,
                new Rectangle(X * 200, Y * 200, 200, 200),
                Color.White,
                0,
                new Vector2(100, 100),
                Projectile.scale,
                SpriteEffects.None,
                0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend,
                SamplerState.AnisotropicClamp, DepthStencilState.None,
                RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

        public override void AI()
        {
            Projectile.scale += 0.1f;
        }
    }
}
