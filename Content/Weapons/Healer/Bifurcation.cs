using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using ThoriumMod;
using ThoriumMod.Items;
using ThoriumMod.Items.HealerItems;
using ThoriumMod.Projectiles.Healer;
using ContinentOfJourney.Items.Material;
using ContinentOfJourney.Tiles;
using HWJBardHealer.Content.Projectiles.Healer;

namespace HWJBardHealer.Content.Weapons.Healer
{
    public class Bifurcation : ThoriumItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DamageType = ThoriumDamageBase<HealerDamage>.Instance;
            Item.damage = 321;
            isHealer = true;
            healDisplay = true;
            healType = HealType.AllyAndPlayer;
            healBonusMax = 6;

            Item.width = 40;
            Item.height = 40;
            Item.useTime = 24;
            Item.useAnimation = 24;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.knockBack = 6f;
            Item.value = Item.sellPrice(0, 4, 50, 0);
            Item.rare = ItemRarityID.Yellow;
            Item.UseSound = new SoundStyle?(SoundID.DD2_MonkStaffSwing);
            Item.autoReuse = true;

            Item.shoot = ModContent.ProjectileType<BifurcationProj>();
            Item.shootSpeed = 220f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float swingOffset = 30.625f * player.direction;
            Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, type, damage, knockback, player.whoAmI, 0f, swingOffset);
            return false;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<TheEffuser>(), 1);
            recipe.AddIngredient(ModContent.ItemType<CubistBar>(), 8);
            recipe.AddIngredient(ModContent.ItemType<EssenceofMatter>(), 4);
            recipe.AddTile(ModContent.TileType<FountainofMatter>());
            recipe.Register();
        }
    }
}
