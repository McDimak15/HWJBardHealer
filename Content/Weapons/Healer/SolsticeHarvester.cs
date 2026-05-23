using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using ThoriumMod.Projectiles.Scythe;
using ThoriumMod.Items.HealerItems;
using ThoriumMod.Items.Terrarium;
using ContinentOfJourney.Items.Material;
using ContinentOfJourney.Tiles;
using HWJBardHealer.Content.Projectiles.Healer;

namespace HWJBardHealer.Content.Weapons.Healer
{
    public class SolsticeHarvester : ScytheItem
    {
        public override void SetStaticDefaults()
        {
            SetStaticDefaultsToScythe();
        }

        public override void SetDefaults()
        {
            SetDefaultsToScythe();

            Item.damage = 315;
            Item.width = 64;
            Item.height = 64;
            Item.value = Item.sellPrice(0, 3, 50, 0);
            Item.rare = ItemRarityID.Pink;
            Item.shoot = ModContent.ProjectileType<SolsticeHarvesterPro>();
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.useTime = 26;
            Item.useAnimation = 26;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.autoReuse = true;
            scytheSoulCharge = 2;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);

            for (int i = -1; i <= 1; i++)
            {
                Vector2 shootDir = Vector2.Normalize(Main.MouseWorld - player.Center).RotatedBy(MathHelper.ToRadians(i * 10f));
                Projectile.NewProjectile(
                    source,
                    player.Center,
                    shootDir * 8f,
                    ModContent.ProjectileType<SolsticeHarvesterShot>(),
                    damage,
                    knockback,
                    player.whoAmI
                );
            }

            return false;
        }

        public override void AddRecipes()
        {
            Mod CBH = ModLoader.TryGetMod("CalamityBardHealer", out Mod cbh) ? cbh : null;

            Recipe recipe = CreateRecipe();

            if (CBH != null)
            {
                ModItem syzygy = CBH.Find<ModItem>("Syzygy");
                recipe.AddIngredient(syzygy.Type, 1);
            }
            else
            {
                recipe.AddIngredient(ModContent.ItemType<TerrariumHolyScythe>(), 1);
            }

            recipe.AddIngredient(ModContent.ItemType<SolarFlareScoria>(), 12);
            recipe.AddTile(ModContent.TileType<FinalAnvil>());
            recipe.Register();

            if (CBH != null)
            {
                var milkyWayItem = CBH.Find<ModItem>("MilkyWay");
                foreach (var r in Main.recipe)
                {
                    if (r.createItem?.type == milkyWayItem.Type)
                    {
                        for (int i = 0; i < r.requiredItem.Count; i++)
                        {
                            var ingr = r.requiredItem[i];
                            if (ingr != null && ingr.type == CBH.Find<ModItem>("Syzygy").Type)
                            {
                                r.requiredItem[i] = new Item(ModContent.ItemType<SolsticeHarvester>(), ingr.stack);
                            }
                        }
                    }
                }
            }
        }
    }
}
