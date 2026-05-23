using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ThoriumMod.Projectiles;
using HWJBardHealer.Core;

namespace HWJBardHealer.Content.Projectiles.Healer
{
    public abstract class WhaleEchoBase : ThoriumProjectile
    {
        protected List<Vector2> wavePoints = new List<Vector2>();
        protected float expansionLength = 0f;
        protected Color waveColor = new Color(130, 144, 144); 

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1; 
            Projectile.timeLeft = 120;
            Projectile.alpha = 100;
        }

        public override bool ShouldUpdatePosition() => true;

        public override void AI()
        {
            Projectile.ai[1]++;
            expansionLength += 0.8f; 
            Projectile.alpha += 2;

            UpdateWavePoints();

            float speed = GetSpeed();
            int targetIdx = (int)Projectile.ai[0];
            Entity target = GetTarget(targetIdx);

            if (target != null && target.active)
            {
                Vector2 dest = (target is NPC npc) ? npc.Center : ((Player)target).Center;
                Vector2 desiredVelocity = Vector2.Normalize(dest - Projectile.Center) * speed;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 0.06f);
            }

            if (Projectile.alpha >= 255) Projectile.Kill();
        }

        private void UpdateWavePoints()
        {
            wavePoints.Clear();
            float baseRotation = Projectile.velocity.ToRotation();

            for (float i = -0.8f; i <= 0.8f; i += 0.2f)
            {
                float waveValue = (float)Math.Sin(i * 5 + Projectile.ai[1] * 0.2f) * 6f;

                Vector2 pos = Projectile.Center + (baseRotation + i).ToRotationVector2() * (expansionLength + waveValue);
                wavePoints.Add(pos);
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            foreach (var point in wavePoints)
            {
                Rectangle hitRect = new Rectangle((int)point.X - 8, (int)point.Y - 8, 16, 16);
                if (hitRect.Intersects(targetHitbox)) return true;
            }
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Terraria.GameContent.TextureAssets.Extra[98].Value;
            Vector2 origin = texture.Size() / 2f;
            float scale = Projectile.scale * (1f - Projectile.alpha / 255f);

            foreach (var point in wavePoints)
            {
                Main.EntitySpriteDraw(texture, point - Main.screenPosition, null, waveColor * (1f - Projectile.alpha / 255f), Projectile.velocity.ToRotation() + MathHelper.PiOver2, origin, scale * 0.4f, SpriteEffects.None, 0);
            }
            return false;
        }

        protected abstract float GetSpeed();
        protected abstract Entity GetTarget(int index);
    }

    public class WhaleEchoHarm : WhaleEchoBase
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.Typhoon;
        protected override float GetSpeed() => 9f;
        protected override Entity GetTarget(int index) => (index >= 0 && index < 200) ? Main.npc[index] : null;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.localNPCImmunity[target.whoAmI] = 20;
            target.immune[Projectile.owner] = 0;
        }
    }

    public class WhaleEchoHeal : WhaleEchoBase
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.Typhoon;
        protected override float GetSpeed() => 11f;
        protected override Entity GetTarget(int index) => (index >= 0 && index < 255) ? Main.player[index] : null;

        public override void AI()
        {
            base.AI();
            int targetIdx = (int)Projectile.ai[0];
            if (targetIdx >= 0 && targetIdx < Main.maxPlayers)
            {
                Player target = Main.player[targetIdx];
                if (target.active && !target.dead && Colliding(Projectile.Hitbox, target.Hitbox) == true)
                {
                    if (Main.myPlayer == Projectile.owner)
                    {
                        int amt = (target.whoAmI == Projectile.owner) ? 6 : 12;
                        HealerHelper.HealPlayer(Main.player[Projectile.owner], target, amt, 60, false);
                    }
                    Projectile.Kill();
                }
            }
        }
    }
}