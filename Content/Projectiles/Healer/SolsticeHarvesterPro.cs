using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.DataStructures;
using ThoriumMod.Projectiles.Scythe;
using ContinentOfJourney.Buffs;

namespace HWJBardHealer.Content.Projectiles.Healer
{
    public class SolsticeHarvesterPro : ScythePro
    {
        public override void SafeSetDefaults()
        {
            Projectile.width = 141;
            Projectile.height = 145;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.scale = 1.3f;

            dustOffset = new Vector2(-20f, 8f);
            dustCount = 5;
            dustType = DustID.GoldFlame;
        }

        public override void SafeAI()
        {
            Player player = Main.player[Projectile.owner];

            float actualSpeed = rotationSpeed * 0.35f; 
            Projectile.rotation += Projectile.spriteDirection * actualSpeed;
            Projectile.spriteDirection = player.direction;
            player.heldProj = Projectile.whoAmI;
            Projectile.Center = player.Center;
            Projectile.gfxOffY = player.gfxOffY;
        }


        public override void SafeOnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Vector2 center = target.Center;

            Terraria.Audio.SoundEngine.PlaySound(SoundID.Item74 with
            {
                Volume = 0.9f,
                Pitch = -0.35f
            }, center);

            int sliceCount = 3;
            for (int i = 0; i < sliceCount; i++)
            {
                float randomRot = (target.Center - Main.player[Projectile.owner].Center).ToRotation()
                                + Main.rand.NextFloat(-0.4f, 0.4f);
                BigSliceVisual.New(center, randomRot, 40 + Main.rand.Next(10), 5f);
            }

            if (!target.HasBuff(ModContent.BuffType<AfterburnBuff>()))
                target.AddBuff(ModContent.BuffType<AfterburnBuff>(), 180);

            Lighting.AddLight(center, 1.8f, 1.3f, 0.6f);
        }

        public override void ModifyDust(Dust dust, Vector2 position, int scytheIndex)
        {
            if (scytheIndex == scytheCount - 1)
            {
                dust.scale *= 1.7f;
                dust.color = Color.Lerp(Color.OrangeRed, Color.Gold, Main.rand.NextFloat());
            }
        }

        public override void SafeModifyDamageHitbox(ref Rectangle hitbox)
        {
            hitbox.Inflate(40, 0);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch sb = Main.spriteBatch;
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 origin = texture.Size() / 2f;

            sb.Draw(
                texture,
                Projectile.Center - Main.screenPosition,
                null,
                Color.White,
                Projectile.rotation,
                origin,
                Projectile.scale,
                Projectile.spriteDirection == -1 ? SpriteEffects.FlipVertically : SpriteEffects.None,
                0f
            );

            Texture2D glowTex = TextureAssets.Extra[98].Value;
            sb.Draw(
                glowTex,
                Projectile.Center - Main.screenPosition,
                null,
                Color.Gold * 0.3f,
                0f,
                glowTex.Size() / 2f,
                new Vector2(Projectile.scale * 1.1f, Projectile.scale * 0.6f),
                SpriteEffects.None,
                0f
            );

            return false;
        }
    }

    public class BigSliceVisual : ModProjectile
    {
        public override string Texture => "Terraria/Images/Extra_98";

        public override void SetDefaults()
        {
            Projectile.width = 300;
            Projectile.height = 120;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 40; 
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.alpha = 255;
        }

        public static void New(Vector2 position, float rotation, int time = 40, float scale = 5f)
        {
            int p = Projectile.NewProjectile(
                new EntitySource_Misc("SolsticeBigSlice"),
                position,
                Vector2.Zero,
                ModContent.ProjectileType<BigSliceVisual>(),
                0, 0f,
                Main.myPlayer
            );
            if (Main.projectile.IndexInRange(p))
            {
                var pr = Main.projectile[p];
                pr.rotation = rotation;
                pr.timeLeft = time;
                pr.scale = scale;
            }
        }

        public override void AI()
        {
            if (Projectile.timeLeft > 25)
                Projectile.alpha -= 20;
            else
                Projectile.alpha += 15;

            Projectile.rotation += 0.01f * Math.Sign(Main.rand.NextFloat(-1f, 1f));

            if (Projectile.alpha < 0) Projectile.alpha = 0;
            if (Projectile.alpha > 255) Projectile.Kill();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Extra[98].Value;
            Vector2 origin = tex.Size() / 2f;
            float fade = 1f - Projectile.alpha / 255f;

            Color col = Color.Lerp(Color.OrangeRed, Color.Gold, 0.5f) * fade;
            col.A = 0;

            Main.EntitySpriteDraw(
                tex,
                Projectile.Center - Main.screenPosition,
                null,
                col,
                Projectile.rotation,
                origin,
                new Vector2(Projectile.scale, 0.45f),
                SpriteEffects.None,
                0f
            );

            Main.EntitySpriteDraw(
                tex,
                Projectile.Center - Main.screenPosition,
                null,
                Color.White * (fade * 0.4f),
                Projectile.rotation,
                origin,
                new Vector2(Projectile.scale * 0.7f, 0.3f),
                SpriteEffects.None,
                0f
            );

            return false;
        }
    }
}
