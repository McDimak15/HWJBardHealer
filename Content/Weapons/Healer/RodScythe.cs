using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using ThoriumMod.Projectiles.Scythe;
using ThoriumMod.Items.HealerItems;
using HWJBardHealer.Content.Projectiles.Healer;

namespace HWJBardHealer.Content.Weapons.Healer
{
    public class RodScythe : ScytheItem
    {
        private int attackCounter = 0;

        public override void SetStaticDefaults()
        {
            SetStaticDefaultsToScythe();
        }

        public override void SetDefaults()
        {
            SetDefaultsToScythe();

            Item.damage = 60;
            scytheSoulCharge = 2;
            Item.width = 64;
            Item.height = 64;
            Item.value = Item.sellPrice(0, 2, 50, 0);
            Item.rare = ItemRarityID.LightRed;
            Item.shoot = ModContent.ProjectileType<RodScythePro>();
        }

        public override bool Shoot(Player player, Terraria.DataStructures.EntitySource_ItemUse_WithAmmo source,
    Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            attackCounter++;

            // Normal scythe projectile
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);

            // Every 3rd swing 
            if (attackCounter >= 3)
            {
                attackCounter = 0;

                Vector2 spawnPos = player.Center;

                for (int i = 0; i < 3; i++)
                {
                    Vector2 beamVelocity = velocity.RotatedBy(MathHelper.ToRadians(-10 + i * 10)) * 1.5f;
                    Projectile.NewProjectile(
                        source,
                        spawnPos,
                        beamVelocity,
                        ModContent.ProjectileType<RodScytheBeam>(),
                        damage / 2,
                        knockback,
                        player.whoAmI
                    );
                }

                Terraria.Audio.SoundEngine.PlaySound(SoundID.Item33, player.Center);
            }

            return false;
        }
    }
}
