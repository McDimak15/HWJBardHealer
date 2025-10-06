using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace TestMod;

public class LifeEnergy : ModProjectile
{
    public override void SetDefaults()
    {
        Projectile.width = Projectile.height = 30;
        Projectile.scale = 0.5f;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 20;
        Projectile.tileCollide = false;
        Projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
        Projectile.friendly = true;
    }

    public override void AI()
    {
        Projectile.scale += 0.06f;
        Projectile.Resize((int)(30 * Projectile.scale), (int)(30 * Projectile.scale));
        Projectile.velocity *= 0.92f;

        if (Projectile.timeLeft <= 10)
            Projectile.alpha += 10;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {

    }

    public override bool PreDraw(ref Color lightColor)
    {
        var texture = TextureAssets.Projectile[Type];

        var rot = Projectile.rotation;

        Main.spriteBatch.Draw(texture.Value, Projectile.Center - Main.screenPosition, null, lightColor with { A = (byte)(255 - Projectile.alpha) }, rot, texture.Size() / 2, Projectile.scale, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0);

        return false;
    }
}