using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ThoriumMod; // for Bard damage base
using Terraria.Audio;

namespace HWJBardHealer.Content.Projectiles.Bard
{
    public class PuppetNoteProj : ModProjectile
    {
        public override string Texture => "HWJBardHealer/Content/Projectiles/Bard/PuppetNotes";
        private const string StringTexturePath = "HWJBardHealer/Content/Projectiles/Thrower/PuppetString";

        private float fade = 0f;
        private bool fadingOut = false;


        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            // The max lifetime is now just a safety limit, reduced to 180 ticks
            Projectile.timeLeft = 180;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = ThoriumMod.BardDamage.Instance;
            Projectile.extraUpdates = 0;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
        }

        public override void OnSpawn(Terraria.DataStructures.IEntitySource source)
        {
            int idx = (int)Projectile.ai[0];
            if (idx < 0 || idx > 3) idx = 3;
            Projectile.frame = idx;
            if (Projectile.localAI[0] == 1f)
            {
                Projectile.damage *= 2;
            }
            Projectile.ai[1] = 0f;

            // Fix from previous discussion: Nudge up on spawn if velocity is horizontal/downward to avoid ground clip.
            if (Projectile.velocity.Y >= 0f)
            {
                Projectile.position.Y -= 16f;
            }
        }

        public override void AI()
        {
            Projectile.ai[1] += 1f;

            // --- FADE IN ---
            if (!fadingOut)
            {
                fade += 0.05f;
                if (fade > 1f) fade = 1f;
            }

            // If the note is returning OR lifetime almost ended ? fade out
            if (Projectile.ai[1] > 10f && Projectile.timeLeft < 40)
            {
                fadingOut = true;
            }

            if (fadingOut)
            {
                fade -= 0.06f;
                if (fade <= 0f)
                    Projectile.Kill();
            }


            Player owner = Main.player[Projectile.owner];

            // Set rotation to a static value (0)
            Projectile.rotation = 0f;

            // The distance threshold for turning back (e.g., ~half a small screen width)
            const float maxOutwardDistance = 400f;

            // Calculate current distance from the player's center
            Vector2 toOwnerCenter = owner.Center - Projectile.Center;
            float dist = toOwnerCenter.Length();

            // Turnaround Logic: Check if it has reached the max distance OR if its initial speed was too low
            bool isReturning = Projectile.ai[1] > 10f && dist > maxOutwardDistance;

            if (!isReturning)
            {
                // Outward flight: Velocity is constant (set by the item's Shoot method)
                // If the projectile gets stuck (near zero velocity), force a return after a short delay
                if (Projectile.velocity.LengthSquared() < 0.1f && Projectile.ai[1] > 10f)
                {
                    isReturning = true;
                }
            }

            if (isReturning)
            {
                // Return phase

                // Recalculate target to the player's held position
                Vector2 targetPos = owner.MountedCenter + new Vector2(-10f, -6f);
                Vector2 toTarget = (targetPos - Projectile.Center);
                dist = toTarget.Length();

                if (dist > 2f)
                {
                    toTarget.Normalize();
                    // Max speed increased to 20f to ensure it returns faster
                    float speed = MathHelper.Lerp(10f, 20f, MathHelper.Clamp(dist / 400f, 0f, 1f));
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, toTarget * speed, 0.12f);
                }
                else
                {
                    SoundEngine.PlaySound(SoundID.Item4, owner.Center);
                    Projectile.Kill();
                }
            }

            // --- Lighting and Dust ---
            Lighting.AddLight(Projectile.Center, 0.5f, 0.3f, 0.7f);

            if (Main.rand.NextBool(3))
            {
                Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Enchanted_Gold);
                d.noGravity = true;
                d.velocity *= 0.2f;
                d.scale = Main.rand.NextFloat(0.6f, 1.1f);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player owner = Main.player[Projectile.owner];
            Texture2D stringTex = ModContent.Request<Texture2D>(StringTexturePath).Value;
            Texture2D noteTex = ModContent.Request<Texture2D>(Texture).Value;

            Vector2 start = owner.MountedCenter + new Vector2(-10f, -6f);
            Vector2 end = Projectile.Center;
            Vector2 diff = end - start;
            float length = diff.Length();

            // --- DRAW STRING  ---
            float rot = diff.ToRotation();
            float rotationForStick = rot - MathHelper.PiOver2;
            Vector2 origin = new Vector2(stringTex.Width * 0.5f, 0f);
            Vector2 scale = new Vector2(1f, stringTex.Height > 0 ? length / stringTex.Height : 1f);

            Main.EntitySpriteDraw(
                stringTex,
                start - Main.screenPosition,
                null,
                Color.White * fade,
                rotationForStick,
                origin,
                scale,
                SpriteEffects.None,
                0
            );

            // --- DRAW NOTE  ---
            int frames = Main.projFrames[Projectile.type];
            int frameHeight = noteTex.Height / frames;
            Rectangle src = new Rectangle(0, Projectile.frame * frameHeight, noteTex.Width, frameHeight);

            Vector2 drawOrigin = new Vector2(noteTex.Width / 2f, frameHeight / 2f);
            Vector2 drawPos = Projectile.Center - Main.screenPosition;

            Main.EntitySpriteDraw(
                noteTex,
                drawPos,
                src,
                lightColor * fade,
                Projectile.rotation,
                drawOrigin,
                Projectile.scale,
                SpriteEffects.None,
                0
            );

            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return base.Colliding(projHitbox, targetHitbox);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.ai[1] = Projectile.timeLeft;

            fadingOut = true;
            fade = 1f;

            for (int i = 0; i < 20; i++)
            {
                Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Enchanted_Gold);
                d.velocity = Main.rand.NextVector2Circular(6f, 6f);
                d.scale = Main.rand.NextFloat(1.1f, 1.5f);
                d.noGravity = true;
            }

            SoundEngine.PlaySound(SoundID.Item14, target.Center);
        }

    }
}