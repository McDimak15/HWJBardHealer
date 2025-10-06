using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace TestMod;

public class FollyProj : ModProjectile
{
    public override void SetDefaults()
    {
        Projectile.width = Projectile.height = 8;
        Projectile.extraUpdates = 1;
        Projectile.friendly = true;
        Projectile.DamageType = DamageClass.Throwing;
    }

    public override void AI()
    {
        if (Projectile.ai[1] > 40)
            Projectile.velocity.Y += 0.1f;
        Projectile.ai[1]++;

        Projectile.ai[0] = (Projectile.ai[0] + 1) % 6;
        if (Projectile.ai[0] == 0)
            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center - Projectile.velocity * 5, Projectile.velocity / 2f, Terraria.ID.ProjectileID.Leaf, Projectile.damage / 2, 3);

        Projectile.rotation = Projectile.velocity.ToRotation();
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        for (float i = -1.5f; i <= 1.5f; i++)
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity.RotatedByRandom(MathHelper.PiOver4 / 2).RotatedBy(MathHelper.PiOver4 * i), ModContent.ProjectileType<LifeEnergy>(), Projectile.damage, Projectile.knockBack);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        var texture = TextureAssets.Projectile[Type];

        var rot = Projectile.rotation + MathHelper.PiOver4;

        var offset = Projectile.rotation.ToRotationVector2() * -texture.Size() / 2;

        Main.spriteBatch.Draw(texture.Value, Projectile.Center - Main.screenPosition + offset, null, lightColor, rot, texture.Size() / 2, Projectile.scale, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0);

        return false;
    }
}