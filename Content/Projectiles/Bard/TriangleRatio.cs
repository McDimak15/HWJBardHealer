using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;
using ThoriumMod;
using ThoriumMod.Projectiles.Bard;

namespace HWJBardHealer.Content.Projectiles.Bard
{
    public class TriangleRatio : BardProjectile
    {
        public override string Texture => "Terraria/Images/Extra_98";

        private const float PHI = 1.61803398875f;
        private static readonly float b = (float)(Math.Log(PHI) / (Math.PI / 2));
        private const int MaxSegments = 1200;

        private float angleProgress = 0f;
        private float fade = 1f;
        private const float InitialTheta = 4.5f; 

        public override BardInstrumentType InstrumentType => BardInstrumentType.Percussion;

        public override void SetBardDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 40;
            Projectile.DamageType = ThoriumDamageBase<BardDamage>.Instance;
        }

        public override void AI()
        {
            angleProgress += 0.4f;

            if (Projectile.timeLeft < 20)
                fade -= 0.05f;

            Lighting.AddLight(Projectile.Center, 0.2f, 0.6f, 1.0f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive,
                SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null,
                Main.GameViewMatrix.TransformationMatrix);

            Vector2 center = Projectile.Center - Main.screenPosition;
            Color color = new Color(120, 220, 255) * fade;

            Vector2 prev = center;
            float theta = InitialTheta;
            float step = 0.06f;

            for (int i = 0; i < MaxSegments && theta < angleProgress + InitialTheta; i++)
            {
                float r = 2f * (float)Math.Exp(b * theta);
                Vector2 next = center + r * new Vector2((float)Math.Cos(theta), (float)Math.Sin(theta));

                float rotation = (next - prev).ToRotation() + MathHelper.PiOver2;
                float length = (next - prev).Length();

                float stretch = (length / tex.Width) * 1.3f;

                Main.EntitySpriteDraw(
                    tex,
                    (prev + next) / 2f,
                    null,
                    color,
                    rotation,
                    tex.Size() / 2f,
                    new Vector2(stretch, 0.4f),
                    SpriteEffects.None,
                    0
                );

                prev = next;
                theta += step;
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend,
                SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null,
                Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
    }
}
