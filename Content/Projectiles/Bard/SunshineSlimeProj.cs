using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ThoriumMod;

namespace HWJBardHealer.Content.Projectiles.Bard
{
    public class SunshineSlimeProj : ModProjectile
    {
        public override string Texture => "ContinentOfJourney/Projectiles/SlimeGod_Master_15"; 

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 2; 
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 60;
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 800;
        }

        public override void SetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 28;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 5;
            Projectile.timeLeft = 355;
            Projectile.tileCollide = false;
            Projectile.DamageType = ThoriumDamageBase<BardDamage>.Instance;
            Projectile.ignoreWater = false;
            Projectile.netUpdate = true;

            Projectile.extraUpdates = 0;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 7;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public override void PostDraw(Color lightColor)
        {
            float overall_alpha = Terraria.Utils.Clamp(Projectile.localAI[0] / 30, 0f, 1f);
            if (Projectile.timeLeft < 60) overall_alpha = Terraria.Utils.Clamp((float)(Projectile.timeLeft - 20) / 40, 0f, 1f);
            Vector2 unit1 = Projectile.Center - Projectile.position;

            List<CustomVertexInfo> bars = new();
            for (int i = 1; i < Projectile.oldPos.Length; ++i)
            {
                if (Projectile.oldPos[i] == Vector2.Zero) break;
                int width = (int)(Terraria.Utils.Clamp((Projectile.position - Projectile.oldPos[1]).Length() / 12f, 0, 1) * 120);
                var normalDir = Projectile.oldPos[i - 1] - Projectile.oldPos[i];
                normalDir = Vector2.Normalize(new Vector2(-normalDir.Y, normalDir.X));
                var factor = i <= 4 ? (1 - (float)Math.Pow(i * 0.25f, 2)) : ((float)(i - 4) / (Projectile.oldPos.Length - 4));
                var factor2 = (float)i / Projectile.oldPos.Length;

                var color = Color.Lerp(new Color(255, 255, 255, 255), new Color(0, 0, 0, 0), factor);

                var w = MathHelper.Lerp(1f, 0.05f, factor);
                Vector2 unit2 = Vector2.Normalize(Projectile.velocity);
                bars.Add(new CustomVertexInfo(Projectile.oldPos[i] + normalDir * -width + unit1 + 4 * unit2, color, new Vector3((float)Math.Sqrt(factor2), 1, w)));
                bars.Add(new CustomVertexInfo(Projectile.oldPos[i] + normalDir * width + unit1 + 4 * unit2, color, new Vector3((float)Math.Sqrt(factor2), 0, w)));
            }
            List<CustomVertexInfo> triangleList = new();
            if (bars.Count > 2)
            {
                triangleList.Add(bars[0]);
                var vertex = new CustomVertexInfo((bars[0].Position + bars[1].Position) * 0.5f + Vector2.Normalize(Projectile.velocity) * 30, new Color(0, 0, 0, 0), new Vector3(0, 0.025f, 0.075f));
                triangleList.Add(bars[1]);
                triangleList.Add(vertex);
                for (int i = 0; i < bars.Count - 2; i += 2)
                {
                    triangleList.Add(bars[i]);
                    triangleList.Add(bars[i + 2]);
                    triangleList.Add(bars[i + 1]);

                    triangleList.Add(bars[i + 1]);
                    triangleList.Add(bars[i + 2]);
                    triangleList.Add(bars[i + 3]);
                }
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone);
                RasterizerState originalState = Main.graphics.GraphicsDevice.RasterizerState;
                var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
                var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0)) * Main.GameViewMatrix.TransformationMatrix;
                ContinentOfJourney.ContinentOfJourney.Effect_R.Parameters["uTransform"].SetValue(model * projection);
                ContinentOfJourney.ContinentOfJourney.Effect_R.Parameters["uTime"].SetValue(-(float)Projectile.ai[1] * 0.03f);
                Main.graphics.GraphicsDevice.Textures[0] = ModContent.Request<Texture2D>("ContinentOfJourney/Projectiles/DrawSample/heatmap").Value;
                Main.graphics.GraphicsDevice.Textures[1] = ModContent.Request<Texture2D>("ContinentOfJourney/Projectiles/DrawSample/Extra_196").Value;
                Main.graphics.GraphicsDevice.Textures[2] = ModContent.Request<Texture2D>("ContinentOfJourney/Projectiles/DrawSample/Extra_196").Value;
                Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
                Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointWrap;
                Main.graphics.GraphicsDevice.SamplerStates[2] = SamplerState.PointWrap;
                ContinentOfJourney.ContinentOfJourney.Effect_R.CurrentTechnique.Passes[0].Apply();
                Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, triangleList.ToArray(), 0, triangleList.Count / 3);
                Main.graphics.GraphicsDevice.RasterizerState = originalState;
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            Main.EntitySpriteDraw(ModContent.Request<Texture2D>("ContinentOfJourney/Images/SunlightDisciple_2").Value,
                    Projectile.oldPos[1] + unit1 - Main.screenPosition, new Rectangle(0, 0, 80, 80), Color.Orange * 0.66f * overall_alpha, 0, new Vector2(40, 40), 1.5f * Projectile.scale, SpriteEffects.None, 0);

            Main.EntitySpriteDraw(ModContent.Request<Texture2D>("ContinentOfJourney/Images/SunlightDisciple_2").Value,
                    Projectile.oldPos[1] + unit1 - Main.screenPosition, new Rectangle(0, 0, 80, 80), Color.White * overall_alpha, 0, new Vector2(40, 40), 1f * Projectile.scale, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawOrigin = new Vector2(texture.Width / 2, texture.Height / Main.projFrames[Projectile.type] / 2);
            Main.EntitySpriteDraw
                (texture, Projectile.oldPos[1] + unit1 - Main.screenPosition, new Rectangle(0, Projectile.frame * texture.Height / Main.projFrames[Projectile.type], texture.Width, texture.Height / Main.projFrames[Projectile.type]), Color.White * overall_alpha, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);

        }
        private struct CustomVertexInfo : IVertexType
        {
            private static VertexDeclaration _vertexDeclaration = new VertexDeclaration(new VertexElement[3]
            {
                new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0),
                new VertexElement(8, VertexElementFormat.Color, VertexElementUsage.Color, 0),
                new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.TextureCoordinate, 0)
            });
            public Vector2 Position;
            public Color Color;
            public Vector3 TexCoord;

            public CustomVertexInfo(Vector2 position, Color color, Vector3 texCoord)
            {
                this.Position = position;
                this.Color = color;
                this.TexCoord = texCoord;
            }
            public VertexDeclaration VertexDeclaration
            {
                get
                {
                    return _vertexDeclaration;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            lightColor = Color.White;
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Projectile.timeLeft < 60) return false;
            for (int i = 1; i < Projectile.oldPos.Length - 10; ++i)
            {
                if (new Rectangle((int)Projectile.oldPos[i].X, (int)Projectile.oldPos[i].Y, 28, 28).Intersects(targetHitbox)) return true;
            }
            return false;
        }

        public override void AI()
        {
            Projectile.frame = 0;
            Projectile.localAI[0] += 1f;
            Projectile.ai[1] += 1f;

            float homingDelay = 30f;
            if (Projectile.ai[1] < homingDelay)
            {
                Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(0.6f));
            }
            else
            {
                NPC target = null;
                float bestDist = 800f;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.CanBeChasedBy(this))
                    {
                        float d = Vector2.Distance(Projectile.Center, npc.Center);
                        if (d < bestDist)
                        {
                            bestDist = d;
                            target = npc;
                        }
                    }
                }
                if (target != null)
                {
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, Vector2.Normalize(target.Center - Projectile.Center) * 18f, 0.06f);
                }
            }

            if (Projectile.velocity.Length() < 4f)
                Projectile.velocity = Vector2.Normalize(Projectile.velocity) * 4f;

            Lighting.AddLight(Projectile.Center, 0.4f, 0.8f, 0.3f);
        }
    }
}
