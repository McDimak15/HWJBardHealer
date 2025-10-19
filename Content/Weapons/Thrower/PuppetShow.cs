using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using HWJBardHealer.Content.Projectiles.Thrower;
using ContinentOfJourney.Items.Material;
using ThoriumMod.Items.ArcaneArmor;

namespace HWJBardHealer.Content.Weapons.Thrower
{
    public class PuppetShow : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.damage = 48;
            Item.DamageType = DamageClass.Throwing;
            Item.width = 32;
            Item.height = 32;
            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.knockBack = 5f;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.shootSpeed = 14f;
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.buyPrice(gold: 5);
            Item.UseSound = SoundID.Item1;
            Item.shoot = ModContent.ProjectileType<PuppetShowProj>();
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();

            recipe.AddIngredient(ModContent.ItemType<BewitchedCotton>(), 8);
            recipe.AddIngredient(ModContent.ItemType<YewWood>(), 8);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }
    }
}
