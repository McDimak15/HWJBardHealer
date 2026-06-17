using Terraria;
using Terraria.ModLoader;
using ThoriumMod;
using ThoriumMod.Items;
using ThoriumMod.Items.BardItems;
using ThoriumMod.Utilities;
using ContinentOfJourney.Tiles;
using ContinentOfJourney.Items.Material;
using ThoriumMod.Rarities;
using ThoriumMod.Items.Terrarium;

namespace HWJBardHealer.Content.Accessories.Bard
{
    public class HarmonyHeadgear : BardItem
    {
        public override void SetBardDefaults()
        {   
            base.Item.width = 20;
            base.Item.height = 20;
            base.Item.value = Item.sellPrice(0, 20, 0, 0);
            base.Item.rare = ModContent.RarityType<TerrariumRarity>();
            base.Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            ThoriumPlayer thoriumPlayer = player.GetThoriumPlayer();
            player.GetThoriumPlayer().bardResourceMax2 += 5;
            player.GetModPlayer<ThormwardPlayer>().accHarmonyHeadgear = true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<BandKit>());
            recipe.AddIngredient(ModContent.ItemType<Headset>());
            recipe.AddIngredient(ModContent.ItemType<TerrariumCore>(), 4);
            recipe.AddIngredient(ModContent.ItemType<TankOfThePastHallow>(), 5);
            recipe.AddTile(ModContent.TileType<FinalAnvil>());
            recipe.Register();
        }
    }
}
