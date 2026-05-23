using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using HWJBardHealer.Content.Projectiles.Thrower;
using ContinentOfJourney.Items;

namespace HWJBardHealer.Content.Weapons.Thrower
{
    public class SolarKunai : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 102;
            Item.DamageType = DamageClass.Throwing;
            Item.width = 32;
            Item.height = 32;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 2f;
            Item.value = Item.sellPrice(0, 3, 50, 0);
            Item.rare = ItemRarityID.Yellow;
            Item.UseSound = SoundID.Item1;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<SolarKunaiProj>();
            Item.shootSpeed = 13f;
            Item.autoReuse = true;
        }

        public override bool Shoot(Player player, Terraria.DataStructures.EntitySource_ItemUse_WithAmmo source,
            Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = 0; i < 3; i++)
            {
                float angleOffset = 0;
                float offsetY = 0;
                bool isMiddle = (i == 1);

                if (i == 0) { angleOffset = MathHelper.ToRadians(-8f); offsetY = -8f; } 
                if (i == 2) { angleOffset = MathHelper.ToRadians(8f); offsetY = 8f; }

                Vector2 perturbed = velocity.RotatedBy(angleOffset);
                Vector2 spawnPos = position + new Vector2(0, offsetY);

                Projectile.NewProjectile(
                    source,
                    spawnPos,
                    perturbed,
                    type,
                    damage,
                    knockback,
                    player.whoAmI,
                    isMiddle ? 1f : 0f
                );
            }

            return false;
        }

        public class SolarKunaiDrop : GlobalItem
        {
            public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
            {
                if (item.type == ModContent.ItemType<SlimeGodTreasureBag>())
                {
                    itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<SolarKunai>(), 1));
                }
            }
        }
    }
}
