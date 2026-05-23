using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using ThoriumMod.Projectiles;
using ContinentOfJourney.Buffs;

namespace HWJBardHealer.Content.Projectiles.Thrower
{
    public class CubismKunaiProj : ThoriumProjectile
    {
        private int timer;

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Throwing;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            bool killFlag = false;

            if (Projectile.ai[0] == 0f)
            {
                if (timer >= 15)
                {
                    Projectile.velocity.X *= 0.98f;
                    Projectile.velocity.Y += 0.35f;
                }
                else timer++;

                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            }
            else if (Projectile.ai[0] == 1f)
            {
                Projectile.ignoreWater = true;
                Projectile.tileCollide = false;
                Projectile.localAI[0]++;

                int npcIndex = (int)Projectile.ai[1];
                if (npcIndex < 0 || npcIndex >= 200)
                {
                    killFlag = true;
                }
                else
                {
                    NPC target = Main.npc[npcIndex];
                    if (target.active && !target.dontTakeDamage)
                    {
                        Projectile.Center = target.Center - Projectile.velocity * 2f;
                        Projectile.gfxOffY = target.gfxOffY;
                    }
                    else killFlag = true;
                }

                if (Projectile.localAI[0] >= 180f)
                    killFlag = true;
            }

            if (killFlag)
                Projectile.Kill();

            Lighting.AddLight(Projectile.Center, 0.3f, 0.6f, 1.2f);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Projectile.ai[0] != 0f)
                return; 

            Projectile.ai[0] = 1f;
            Projectile.ai[1] = target.whoAmI;
            Projectile.velocity = (target.Center - Projectile.Center) * 0.75f;
            Projectile.friendly = false;
            Projectile.netUpdate = true;
            target.AddBuff(ModContent.BuffType<DoomedBuff>(), 300);

            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];
                if (proj.active && proj.owner == Projectile.owner && proj.type == Projectile.type && proj.whoAmI != Projectile.whoAmI)
                {
                    if (proj.ai[0] == 1f && proj.ai[1] == Projectile.ai[1])
                        proj.Kill();
                }
            }

            if (Main.myPlayer == Projectile.owner)
            {
                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    Projectile.Center,
                    Vector2.Zero,
                    ModContent.ProjectileType<CubismKunaiSlash>(),
                    (int)(Projectile.damage * 0.9f),
                    3f,
                    Projectile.owner,
                    Projectile.rotation
                );
            }

            SoundEngine.PlaySound(SoundID.Item71 with { Pitch = 0.25f, Volume = 0.9f }, Projectile.Center);
            for (int i = 0; i < 6; i++)
            {
                int d = Dust.NewDust(Projectile.Center, 0, 0, DustID.Electric,
                    Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 2f));
                Main.dust[d].noGravity = true;
            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 8; i++)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Electric,
                    Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 2f));
                Main.dust[d].noGravity = true;
            }
            SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);
        }
    }

    public class CubismKunaiSlash : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_684";

        private float scale = 0.3f;
        private float alpha = 1f;

        public override void SetDefaults()
        {
            Projectile.width = 80;
            Projectile.height = 80;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 20;
            Projectile.DamageType = DamageClass.Throwing;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.ai[0];
            scale += 0.15f;
            alpha -= 0.06f;
            if (alpha <= 0f) Projectile.Kill();

            Lighting.AddLight(Projectile.Center, 0.4f, 0.8f, 1.4f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Color color = new Color(180, 230, 255, 255) * alpha;
            Vector2 origin = tex.Size() / 2f;
            Vector2 stretch = new Vector2(scale, scale * 0.2f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive,
                SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null,
                Main.GameViewMatrix.TransformationMatrix);

            Main.EntitySpriteDraw(
                tex,
                Projectile.Center - Main.screenPosition,
                null,
                color,
                Projectile.rotation,
                origin,
                stretch,
                SpriteEffects.None,
                0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend,
                SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null,
                Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
    }
}
