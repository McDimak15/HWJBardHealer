using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using ThoriumMod;

namespace HWJBardHealer.Content.Projectiles.Healer
{
    public class PenroseTriangleProj : ModProjectile
    {
        public override string Texture => "ContinentOfJourney/NPCs/Boss_TheMaterealizer/TheMaterealizer_Minion";

        private NPC target;
        private int hitTimer;

        private const float ScaleLerpSpeed = 0.08f;
        private float smoothedScale = 1f;
        private float pulseScale = 1f;

        public override void SetDefaults()
        {
            Projectile.width = 92;
            Projectile.height = 92;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 120;
            Projectile.alpha = 0;
            Projectile.DamageType = ThoriumDamageBase<HealerDamage>.Instance;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.hide = true;
        }

        public override void DrawBehind(
            int index,
            List<int> behindNPCsAndTiles,
            List<int> behindNPCs,
            List<int> behindProjectiles,
            List<int> overPlayers,
            List<int> overWiresUI)
        {
            behindNPCs.Add(index);
        }

        public override void AI()
        {
            int targetIndex = (int)Projectile.ai[0];
            if (targetIndex < 0 || targetIndex >= Main.maxNPCs)
            {
                Projectile.Kill();
                return;
            }

            target = Main.npc[targetIndex];
            if (!target.active || target.life <= 0)
            {
                Projectile.Kill();
                return;
            }

            Projectile.Center = target.Center;
            Projectile.rotation += 0.08f * ((targetIndex % 2 == 0) ? 1f : -1f);

            pulseScale = 1f + 0.05f * (float)System.Math.Sin(Main.GlobalTimeWrappedHourly * 3f);

            if (Projectile.timeLeft > 90)
                Projectile.alpha = Utils.Clamp(Projectile.alpha - 10, 50, 180);
            else if (Projectile.timeLeft < 30)
                Projectile.alpha = Utils.Clamp(Projectile.alpha + 8, 100, 255);

            hitTimer++;
            if (hitTimer >= 10)
            {
                hitTimer = 0;
                if (Main.myPlayer == Projectile.owner)
                {
                    var src = Projectile.GetSource_FromThis();
                    int p = Projectile.NewProjectile(
                        src,
                        target.Center,
                        Vector2.Zero,
                        ModContent.ProjectileType<PenroseTriangleHit>(),
                        Projectile.damage,
                        0f,
                        Projectile.owner
                    );
                    if (Main.projectile.IndexInRange(p))
                        Main.projectile[p].DamageType = Projectile.DamageType;
                }
            }

            var tex = ModContent.Request<Texture2D>(Texture).Value;
            int texFrameWidth = 92;
            float targetSize = (target.width + target.height) / 2f;
            float desiredScale = MathHelper.Clamp(targetSize / (float)texFrameWidth, 0.6f, 3.0f);

            smoothedScale = MathHelper.Lerp(smoothedScale, desiredScale * 1.5f, ScaleLerpSpeed);

            smoothedScale = Math.Max(smoothedScale, 0.6f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;

            Rectangle frame = new Rectangle(0, 94 * 3, 92, 92); 

            Vector2 origin = frame.Size() / 2f;

            float finalScale = smoothedScale * pulseScale;

            float opacity = 0.8f * (1f - Projectile.alpha / 255f);
            Color color = Color.White * opacity;

            Main.EntitySpriteDraw(
                tex,
                Projectile.Center - Main.screenPosition,
                frame,
                color,
                Projectile.rotation,
                origin,
                finalScale,
                SpriteEffects.None,
                0
            );

            return false;
        }
    }

    public class PenroseTriangleHit : ModProjectile
    {
        public override string Texture => "Terraria/Images/Extra_98";

        public override void SetDefaults()
        {
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 2;
            Projectile.penetrate = 1;
            Projectile.alpha = 255;
            Projectile.DamageType = ThoriumDamageBase<HealerDamage>.Instance;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override bool PreDraw(ref Color lightColor) => false;
    }
}
