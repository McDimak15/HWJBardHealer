using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using ThoriumMod.Projectiles.Scythe;
using static Microsoft.Xna.Framework.MathHelper;

namespace TestMod;

public class GrowthSwing : ScythePro
{
    public override string Texture => "TestMod/ForbiddenGrowth";//$"Terraria/Images/Projectile_{ProjectileID.Excalibur}";


    private bool shouldSpin;

    public override void SafeSetStaticDefaults()
    {
        //Main.projFrames[Projectile.type] = 8;
    }


    public override void SafeSetDefaults()
    {
        Projectile.width = 116;
        Projectile.height = 122;
        Projectile.idStaticNPCHitCooldown = 4;
        Projectile.alpha = 255;
        Projectile.manualDirectionChange = true;
    }

    public override bool PreAI()
    {
        if (++Projectile.frameCounter > 4)
        {
            if (++Projectile.frame >= Main.projFrames[Projectile.type])
            {
                Projectile.frame = 0;
            }
            Projectile.frameCounter = 0;
        }

        float swingTime = 0f;
        float swingDelta = 0f;
        Player player = Main.player[Projectile.owner];
        Projectile.scale = 1f * player.GetAdjustedItemScale(player.HeldItem);
        if (Projectile.ai[1] <= 0f || player.dead)
        {
            Projectile.Kill();
            return false;
        }
        Projectile.timeLeft = (int)Projectile.ai[1];
        player.itemTime = (int)Projectile.ai[1];
        player.itemAnimation = (int)Projectile.ai[1];
        player.heldProj = Projectile.whoAmI;
        player.compositeFrontArm.enabled = true;
        if (Projectile.velocity.X != 0f)
        {
            player.ChangeDir((Projectile.velocity.X > 0f) ? 1 : (-1));
        }
        float swing = 0f;
        float armSwing = 0f;
        if (Projectile.direction == -1)
        {
            if (Projectile.ai[1] / Projectile.ai[0] > 0.75f)
            {
                if (player.whoAmI == Main.myPlayer)
                {
                    if (player.HeldItem.ModItem is ForbiddenGrowth po)
                    {
                        shouldSpin = po.spin >= 3;
                    }
                    Projectile.velocity = Vector2.Normalize(Main.MouseWorld - player.MountedCenter);
                    NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, Projectile.whoAmI);
                }
                swingTime = (Projectile.ai[1] / Projectile.ai[0] - 0.75f) * 4f;
                swing = Projectile.velocity.ToRotation() + MathHelper.ToRadians(MathHelper.SmoothStep(135f, 80f, swingTime) * (float)player.direction);
                armSwing = Projectile.velocity.ToRotation() + MathHelper.Lerp((float)Math.PI / 2f, (float)Math.PI / 4f, swingTime) * (float)player.direction;
            }
            else if (shouldSpin)
            {
                swingTime = Projectile.ai[1] / (Projectile.ai[0] * 0.75f);
                swingDelta = (Projectile.ai[1] + 1f) / (Projectile.ai[0] * 0.75f);
                swingTime = MathHelper.SmoothStep(0f, 1f, swingTime);
                swingDelta = MathHelper.SmoothStep(0f, 1f, swingDelta);
                swingDelta = Math.Abs(swingTime + (float)Math.PI - (swingDelta + (float)Math.PI));
                swing = MathHelper.Lerp(MathHelper.ToRadians(100f) * (float)(-player.direction), MathHelper.ToRadians(495f) * (float)player.direction, swingTime) + Projectile.velocity.ToRotation();
                armSwing = MathHelper.Lerp((float)Math.PI / 2f * (float)(-player.direction), 7.853982f * (float)player.direction, swingTime) + Projectile.velocity.ToRotation();
            }
            else
            {
                swingTime = Projectile.ai[1] / (Projectile.ai[0] * 0.75f);
                swingDelta = (Projectile.ai[1] + 1f) / (Projectile.ai[0] * 0.75f);
                for (int e = 0; e < 4; e++)
                {
                    swingTime = MathHelper.SmoothStep(0f, 1f, swingTime);
                    swingDelta = MathHelper.SmoothStep(0f, 1f, swingDelta);
                }
                swingDelta = Math.Abs(swingTime + (float)Math.PI - (swingDelta + (float)Math.PI));
                swing = MathHelper.Lerp(MathHelper.ToRadians(100f) * (float)(-player.direction), MathHelper.ToRadians(135f) * (float)player.direction, swingTime) + Projectile.velocity.ToRotation();
                armSwing = MathHelper.Lerp((float)Math.PI / 2f * (float)(-player.direction), (float)Math.PI / 2f * (float)player.direction, swingTime) + Projectile.velocity.ToRotation();
            }
            Projectile.spriteDirection = -player.direction;
        }
        else
        {
            if (Projectile.ai[1] / Projectile.ai[0] > 0.75f)
            {
                if (player.whoAmI == Main.myPlayer)
                {
                    if (player.HeldItem.ModItem is ForbiddenGrowth po2)
                    {
                        shouldSpin = po2.spin >= 3;
                    }
                    Projectile.velocity = Vector2.Normalize(Main.MouseWorld - player.MountedCenter);
                    NetMessage.SendData(27, -1, -1, null, Projectile.whoAmI);
                }
                swingTime = (Projectile.ai[1] / Projectile.ai[0] - 0.75f) * 4f;
                swing = Projectile.velocity.ToRotation() - MathHelper.ToRadians(MathHelper.SmoothStep(135f, 80f, swingTime) * (float)player.direction);
                armSwing = Projectile.velocity.ToRotation() - MathHelper.Lerp((float)Math.PI / 2f, (float)Math.PI / 4f, swingTime) * (float)player.direction;
            }
            else if (shouldSpin)
            {
                swingTime = Projectile.ai[1] / (Projectile.ai[0] * 0.75f);
                swingDelta = (Projectile.ai[1] + 1f) / (Projectile.ai[0] * 0.75f);
                swingTime = MathHelper.SmoothStep(0f, 1f, swingTime);
                swingDelta = MathHelper.SmoothStep(0f, 1f, swingDelta);
                swingDelta = Math.Abs(swingTime + (float)Math.PI - (swingDelta + (float)Math.PI));
                swing = MathHelper.Lerp(MathHelper.ToRadians(100f) * (float)player.direction, MathHelper.ToRadians(495f) * (float)(-player.direction), swingTime) + Projectile.velocity.ToRotation();
                armSwing = MathHelper.Lerp((float)Math.PI / 2f * (float)player.direction, 7.853982f * (float)(-player.direction), swingTime) + Projectile.velocity.ToRotation();
            }
            else
            {
                swingTime = Projectile.ai[1] / (Projectile.ai[0] * 0.75f);
                swingDelta = (Projectile.ai[1] + 1f) / (Projectile.ai[0] * 0.75f);
                for (int i = 0; i < 4; i++)
                {
                    swingTime = MathHelper.SmoothStep(0f, 1f, swingTime);
                    swingDelta = MathHelper.SmoothStep(0f, 1f, swingDelta);
                }
                swingDelta = Math.Abs(swingTime + (float)Math.PI - (swingDelta + (float)Math.PI));
                swing = MathHelper.Lerp(MathHelper.ToRadians(100f) * (float)player.direction, MathHelper.ToRadians(135f) * (float)(-player.direction), swingTime) + Projectile.velocity.ToRotation();
                armSwing = MathHelper.Lerp((float)Math.PI / 2f * (float)player.direction, (float)Math.PI / 2f * (float)(-player.direction), swingTime) + Projectile.velocity.ToRotation();
            }
            Projectile.spriteDirection = player.direction;
        }
        if (Projectile.localAI[2] == 0f && Projectile.ai[1] <= Projectile.ai[0] * 0.5f)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                if (!shouldSpin)
                {
                    Vector2 offset = Vector2.Normalize(Main.MouseWorld - player.MountedCenter);
                    for (int i = 0; i < 4; i++)
                        Projectile.NewProjectile(player.GetSource_ItemUse(player.HeldItem), Main.MouseWorld + offset * new Vector2(Main.screenWidth, Main.screenHeight).Length() / 2, -offset * 5, ProjectileID.Leaf, Projectile.damage / 3, Projectile.knockBack / 2, player.whoAmI);

                    //Vector2 shootDir = Vector2.Normalize(Main.MouseWorld - player.MountedCenter).RotatedBy((float)Math.PI / 4f * (float)Projectile.direction + (float)Main.rand.Next(-10, 11) * ((float)Math.PI / 4f) * 0.02f);
                    //int p = Projectile.NewProjectile(player.GetSource_ItemUse(player.HeldItem), Main.MouseWorld - shootDir * 480f, shootDir * 16f, ModContent.ProjectileType<CherubimSlash>(), Projectile.damage, Projectile.knockBack, player.whoAmI);
                    //NetMessage.SendData(27, -1, -1, null, p);
                    //p = Projectile.NewProjectile(player.GetSource_ItemUse(player.HeldItem), Main.MouseWorld - shootDir * 480f, shootDir * 32f, ModContent.ProjectileType<CherubimRift>(), Projectile.damage, Projectile.knockBack, player.whoAmI);
                    //NetMessage.SendData(27, -1, -1, null, p);
                }
                else
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        if (j != 0)
                        {
                            //NetMessage.SendData(27, -1, -1, null, p2);
                            //p2 = Projectile.NewProjectile(player.GetSource_ItemUse(player.HeldItem), Main.MouseWorld - shootDir2 * 480f, shootDir2 * 32f, ModContent.ProjectileType<CherubimRift>(), Projectile.damage, Projectile.knockBack, player.whoAmI);
                            //NetMessage.SendData(27, -1, -1, null, p2);
                        }
                    }
                }
            }
            SoundEngine.PlaySound(in SoundID.DD2_MonkStaffSwing, player.position);
            Projectile.localAI[2] += 1f;
        }
        if (Projectile.alpha > 0)
        {
            Projectile.alpha -= 17;
        }
        player.compositeFrontArm.rotation = armSwing - (float)Math.PI / 2f - (player.gravDir - 1f) * ((float)Math.PI / 2f);
        Projectile.Center = player.GetFrontHandPosition(player.compositeFrontArm.stretch, player.compositeFrontArm.rotation);
        Projectile.rotation = swing;
        if (swingDelta > 0f)
        {
            for (int k = 0; k < (int)MathHelper.ToDegrees(swingDelta); k++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(20f, 20f) * Projectile.scale + new Vector2(84f, 40f * (float)(-Projectile.direction)).RotatedBy(Projectile.rotation - (MathHelper.ToRadians(k + 1) * 2f + ((float)player.direction - 1f) * 0.5f * ((float)Math.PI / 4f)) * (float)Projectile.direction) * Projectile.scale, DustID.GreenMoss, Vector2.Zero, 0, Color.White, Projectile.scale);
                dust.velocity -= Vector2.UnitY.RotatedBy(Projectile.rotation - MathHelper.ToRadians(k) * (float)Projectile.direction) * Main.rand.Next(60, 91) * ((k % 2 == 0) ? (-0.05f) : 0.05f);
                dust.noGravity = true;
                dust.scale = Main.rand.NextFloat(0.6f, 0.9f) * Projectile.scale;
            }
        }
        if (Projectile.ai[1] > 0f)
        {
            Projectile.ai[1] -= 1f;
        }
        return false;
    }

    public override void SafeOnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        Projectile.NewProjectile(Projectile.GetSource_OnHit(target), Main.player[Projectile.owner].Center, Main.rand.NextVector2Circular(6, 6), ModContent.ProjectileType<ForbiddenFruit>(), (int)(damageDone * 0.5f), 1, ai0: -1);

        Color dustColor = Color.Green;
        Color dustColor2 = Color.Green;
        for (int i = 0; i < 10; i++)
        {
            float rot = MathHelper.ToRadians(MathHelper.ToRadians(36f) * (float)i);
            Vector2 spinningpoint = Vector2.Normalize(target.Center - Projectile.Center).RotatedBy((float)Math.PI / 4f * (float)Projectile.spriteDirection) * Projectile.velocity.Length();
            Vector2 offset = spinningpoint.RotatedBy(rot * Main.rand.NextFloat(3.1f, 9.1f)) * new Vector2(Main.rand.NextFloat(1.5f, 5.5f));
            Vector2 velOffset = spinningpoint.RotatedBy(rot * Main.rand.NextFloat(3.1f, 9.1f)) * new Vector2(Main.rand.NextFloat(1.5f, 5.5f));
            GeneralParticleHandler.SpawnParticle((Particle)new MediumMistParticle(target.Center + offset, velOffset * Main.rand.NextFloat(1.5f, 3f), dustColor, dustColor2, Main.rand.NextFloat(0.9f, 1.2f), 160f, Main.rand.NextFloat(0.03f, -0.03f)));
            Dust dust = Dust.NewDustPerfect(target.Center + offset, DustID.GreenMoss, new Vector2(velOffset.X, velOffset.Y), 0, Color.Lerp(dustColor, dustColor2, (float)Main.rand.Next(11) * 0.1f), 0.6f);
            dust.noGravity = true;
            dust.velocity = velOffset;
            dust.scale = Main.rand.NextFloat(0.6f, 0.9f);
        }

        for (int i = 0; i < 3; i++)
        {
            Color impactColor = Color.Green;
            float impactParticleScale = Main.rand.NextFloat(1f, 1.75f);
            SparkleParticle impactParticle = new(target.Center + Main.rand.NextVector2Circular(target.width * 0.75f, target.height * 0.75f), Vector2.Zero, impactColor, Color.Green, impactParticleScale, 8, 0, 2.5f);
            GeneralParticleHandler.SpawnParticle(impactParticle);
        }

        for (int i = 0; i < 5; i++)
        {
            Vector2 sparkVelocity = (target.Center - Main.player[Projectile.owner].MountedCenter).SafeNormalize(Vector2.UnitX) * 20f;
            Vector2 sparkVelocity2 = sparkVelocity.RotatedByRandom(0.35f) * Main.rand.NextFloat(0.5f, 1.8f);
            int sparkLifetime2 = Main.rand.Next(23, 35);
            float sparkScale2 = Main.rand.NextFloat(0.95f, 1.8f);
            Color sparkColor2 = Main.rand.NextBool() ? Color.Green : Color.ForestGreen;
            if (Main.rand.NextBool())
            {
                AltSparkParticle spark = new AltSparkParticle(target.Center + Main.rand.NextVector2Circular(target.width * 0.5f, target.height * 0.5f) + sparkVelocity * 1.2f, sparkVelocity2, false, (int)(sparkLifetime2), sparkScale2, sparkColor2);
                GeneralParticleHandler.SpawnParticle(spark);
            }
            else
            {
                LineParticle spark = new LineParticle(target.Center + Main.rand.NextVector2Circular(target.width * 0.5f, target.height * 0.5f) + sparkVelocity * 1.2f, sparkVelocity2, false, (int)(sparkLifetime2), sparkScale2, Main.rand.NextBool() ? Color.Green : Color.ForestGreen);
                GeneralParticleHandler.SpawnParticle(spark);
            }
        }

        for (int i = 0; i < 25; i++)
        {
            int dustID = DustID.GreenMoss;
            Dust dust2 = Dust.NewDustPerfect(target.Center + Main.rand.NextVector2Circular(target.width * 0.5f, target.height * 0.5f), dustID, ((target.Center - Main.player[Projectile.owner].MountedCenter).SafeNormalize(Vector2.UnitX) * 20f).RotatedByRandom(0.55f) * Main.rand.NextFloat(0.3f, 1.1f));
            dust2.scale = Main.rand.NextFloat(0.9f, 2.4f);
            dust2.noGravity = true;
        }
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        float point = 0f;
        return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.rotation.ToRotationVector2() * Projectile.height * Projectile.scale, (float)Projectile.width * Projectile.scale * 0.5f, ref point);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        lightColor = Color.White;
        Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture);
        Main.EntitySpriteDraw(
            texture,
            Projectile.Center - Main.screenPosition,
            new Rectangle(
                0,
                texture.Height / Main.projFrames[Projectile.type] * Projectile.frame,
                texture.Width,
                texture.Height / Main.projFrames[Projectile.type]
            ),
            lightColor,
            Projectile.rotation + (float)Math.PI / 2f - MathHelper.ToRadians((float)Projectile.spriteDirection * 2f),
            new Vector2(texture.Width / 2 - Projectile.spriteDirection * 5, texture.Height / Main.projFrames[Projectile.type] - 45),
            Projectile.scale * 1.33f,
            (Projectile.spriteDirection < 0) ? SpriteEffects.FlipHorizontally : SpriteEffects.None
        );

        if (Projectile.ai[1] / Projectile.ai[0] < 0.75f)
        {
            Player player = Main.player[Projectile.owner];
            float swingTime = Projectile.ai[1] / (Projectile.ai[0] * 0.75f);
            if (shouldSpin)
            {
                swingTime = MathHelper.SmoothStep(0f, 1f, swingTime);
            }
            else
            {
                for (int e = 0; e < 4; e++)
                {
                    swingTime = MathHelper.SmoothStep(0f, 1f, swingTime);
                }
            }
            swingTime = Vector2.UnitX.RotatedBy(swingTime * (float)Math.PI).Y - 0.3f;
            if (swingTime < 0f)
            {
                swingTime = 0f;
            }
            float rotation;
            for (rotation = Projectile.rotation + Main.GlobalTimeWrappedHourly * ((float)Math.PI / 4f); rotation > (float)Math.PI; rotation -= (float)Math.PI * 2f)
            {
            }
            for (; rotation < -(float)Math.PI; rotation += (float)Math.PI * 2f)
            {
            }
            SpriteEffects spriteEffects = ((Projectile.spriteDirection > 0) ? SpriteEffects.FlipVertically : SpriteEffects.None);
            spriteEffects |= SpriteEffects.FlipHorizontally;
            lightColor = Color.Green;
            lightColor.A = 0;
            texture = (Texture2D)ModContent.Request<Texture2D>("CalamityMod/Particles/TrientCircularSmear");
            Main.EntitySpriteDraw(texture, player.MountedCenter - new Vector2(4f, 2f) * player.Directions - Main.screenPosition, null, lightColor * swingTime, Projectile.rotation, texture.Size() / 2f, Projectile.scale * 1.7f, spriteEffects);
            texture = (Texture2D)ModContent.Request<Texture2D>("CalamityMod/Particles/SlashSmear");
            Main.EntitySpriteDraw(texture, player.MountedCenter - new Vector2(4f, 2f) * player.Directions - Main.screenPosition, null, lightColor * swingTime * 0.7f, Projectile.rotation, texture.Size() / 2f, Projectile.scale * 1.1f, spriteEffects);
        }
        return false;
    }

    public override void SendExtraAI(BinaryWriter writer)
    {
        writer.Write(shouldSpin);
        writer.Write(Projectile.direction);
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        shouldSpin = reader.ReadBoolean();
        Projectile.direction = reader.ReadInt32();
    }

    public override bool? CanDamage()
    {
        if (!(Projectile.ai[1] < Projectile.ai[0] * 0.6f) || !(Projectile.ai[1] > Projectile.ai[0] * 0.1f))
        {
            return false;
        }
        return null;
    }
}