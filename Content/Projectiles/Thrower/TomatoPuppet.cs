using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;

namespace HWJBardHealer.Content.Projectiles.Thrower
{
    public class TomatoPuppet : ModProjectile
    {
        public override string Texture => "HWJBardHealer/Content/Projectiles/Thrower/TomatoPuppet";

        private const float OrbitRadius = 80f;
        private const float OrbitSpeed = 0.06f;
        private const int Lifetime = 600;
        private const int FadeDuration = 30;

        private float appearProgress;
        private int orbitIndex = -1;
        private int tomatoTimer = 0;
        private int activeTomatoId = -1;

        private Player ownerPlayer => Main.player[Projectile.owner];

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = Lifetime;
        }

        public void InitializeOrbit(Player player, int index)
        {
            orbitIndex = index;
            appearProgress = 0f;
            Projectile.timeLeft = Lifetime;
            Projectile.Center = player.Center + new Vector2(OrbitRadius, 0f);
        }

        public bool IsFullyActive() => appearProgress >= 0.99f && Projectile.timeLeft > 30;

        public override void AI()
        {
            Player player = ownerPlayer;
            if (!player.active || player.dead)
            {
                Projectile.Kill();
                return;
            }

            if (Projectile.timeLeft > FadeDuration)
                appearProgress = MathHelper.Clamp(appearProgress + 1f / FadeDuration, 0f, 1f);
            else
                appearProgress = MathHelper.Clamp(appearProgress - 1f / FadeDuration, 0f, 1f);

            int activeCount = 0;
            int myIndex = 0;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];
                if (proj.active && proj.owner == player.whoAmI && proj.type == Type)
                {
                    if (proj.whoAmI == Projectile.whoAmI)
                        myIndex = activeCount;
                    activeCount++;
                }
            }

            float angleStep = MathHelper.TwoPi / Math.Max(activeCount, 1);
            float angle = Main.GameUpdateCount * OrbitSpeed + myIndex * angleStep;
            Vector2 targetPos = player.Center + OrbitRadius *
                new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));

            Projectile.Center = Vector2.Lerp(Projectile.Center, targetPos, 0.2f);
            Projectile.rotation = 0f;

            tomatoTimer++;

            if (activeTomatoId != -1)
            {
                Projectile tomato = Main.projectile[activeTomatoId];
                if (!tomato.active || tomato.type != ModContent.ProjectileType<TomatoProj>())
                    activeTomatoId = -1; 
            }

            if (activeTomatoId == -1 && tomatoTimer >= 60 && appearProgress >= 0.99f)
            {
                tomatoTimer = 0;
                if (Main.myPlayer == Projectile.owner)
                {
                    Vector2 spawnPos = Projectile.Center + TomatoProj.SpawnOffset;
                    int projId = Projectile.NewProjectile(
                        Projectile.GetSource_FromThis(),
                        spawnPos,
                        Vector2.Zero,
                        ModContent.ProjectileType<TomatoProj>(),
                        Projectile.damage / 2,
                        0f,
                        Projectile.owner,
                        Projectile.whoAmI
                    );

                    activeTomatoId = projId;
                }
            }

            Lighting.AddLight(Projectile.Center, 1f * appearProgress, 0.3f * appearProgress, 0.3f * appearProgress);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player player = ownerPlayer;
            Texture2D puppetTex = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D stickTex = ModContent.Request<Texture2D>("HWJBardHealer/Content/Projectiles/Thrower/PuppetStick").Value;

            Vector2 playerPos = player.Center - Main.screenPosition;
            Vector2 puppetPos = Projectile.Center - Main.screenPosition;
            Vector2 stickVec = puppetPos - playerPos;
            float stickLength = stickVec.Length();
            float rotation = stickVec.ToRotation() - MathHelper.PiOver2;

            Vector2 origin = new Vector2(stickTex.Width * 0.5f, 0f);
            Vector2 scale = new Vector2(1f, stickTex.Height > 0 ? stickLength / stickTex.Height : 1f);

            Main.EntitySpriteDraw(stickTex, playerPos, null, Color.White * 0.9f * appearProgress, rotation, origin, scale, SpriteEffects.None, 0);
            float puppetScale = MathHelper.SmoothStep(0f, 1f, appearProgress);

            Main.EntitySpriteDraw(puppetTex, puppetPos, null, lightColor * appearProgress, 0f, puppetTex.Size() / 2f, puppetScale, SpriteEffects.None, 0);
            return false;
        }
    }
}
