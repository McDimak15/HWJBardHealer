using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using ThoriumMod;
using ThoriumMod.Projectiles.Bard;

namespace HWJBardHealer.Content.Projectiles.Bard
{
    public class TriangleCircle : BardProjectile
    {
        public override string Texture => "HWJBardHealer/Content/Projectiles/Healer/FadeCircle";
        public override BardInstrumentType InstrumentType => BardInstrumentType.Percussion;

        private const float MaxRadius = 800f;
        private const float ExpansionRate = 20f;
        private const int Duration = 60;
        private float radius = 20f;
        private readonly HashSet<int> hitNPCs = new();

        public override void SetBardDefaults()
        {
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = Duration;
            Projectile.DamageType = ThoriumDamageBase<BardDamage>.Instance;
        }

        public override void AI()
        {
            radius += ExpansionRate;
            if (radius > MaxRadius)
                Projectile.Kill();

            Lighting.AddLight(Projectile.Center, 0.2f, 0.5f, 0.9f);

            foreach (NPC npc in Main.npc)
            {
                if (npc.active && !npc.friendly && npc.CanBeChasedBy(this) && !hitNPCs.Contains(npc.whoAmI))
                {
                    if (Vector2.Distance(npc.Center, Projectile.Center) <= radius + npc.width * 0.5f)
                    {
                        hitNPCs.Add(npc.whoAmI);
                        BardOnHitNPC(npc, default, 0);
                    }
                }
            }
        }

        public override void BardOnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.myPlayer == Projectile.owner)
            {
                int spiral = Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    target.Center,
                    Vector2.Zero,
                    ModContent.ProjectileType<TriangleRatio>(),
                    Projectile.damage / 2,
                    0f,
                    Projectile.owner
                );
                Main.projectile[spiral].ai[0] = 1f; 
            }

            SoundEngine.PlaySound(SoundID.Item29 with { Pitch = 0.4f, Volume = 1.1f }, target.Center);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive,
                SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null,
                Main.GameViewMatrix.TransformationMatrix);

            float progress = 1f - Projectile.timeLeft / (float)Duration;
            float scale = MathHelper.Lerp(0.1f, 6f, progress);
            Color color = new Color(130, 230, 255) * (1f - progress);

            Main.EntitySpriteDraw(
                tex,
                Projectile.Center - Main.screenPosition,
                null,
                color,
                0f,
                tex.Size() / 2f,
                scale,
                SpriteEffects.None,
                0
            );

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend,
                SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null,
                Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            int size = (int)(radius * 2f);
            hitbox = new Rectangle(
                (int)(Projectile.Center.X - size / 2f),
                (int)(Projectile.Center.Y - size / 2f),
                size,
                size
            );
        }
    }
}
