using Terraria;
using Terraria.ModLoader;
using ThoriumMod.Items;
using ThoriumMod.Items.BardItems;
using ThoriumMod.Utilities;
using HWJBardHealer;
using ContinentOfJourney.Tiles;
using ContinentOfJourney.Items.Material;

namespace HWJBardHealer.Content.Accessories.Bard
{
    public class LifePetal : BardItem
    {
        public override void SetBardDefaults()
        {
            accDamage = "35 basic damage";
            Item.width = 28;
            Item.height = 28;
            Item.value = Item.sellPrice(0, 0, 30, 0);
            Item.rare = 2;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<ThormwardPlayer>().accLifePedal.Set(base.Item);
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<KickPetal>(), 1);
            recipe.AddIngredient(ModContent.ItemType<EssenceofLife>(), 5);
            recipe.AddTile(ModContent.TileType<FinalAnvil> ());
            recipe.Register();
        }
    }
}
