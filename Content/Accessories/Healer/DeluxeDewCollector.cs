using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using ThoriumMod.Items.HealerItems;
using ThoriumMod.Utilities;
using ContinentOfJourney.Items.Accessories;
using ContinentOfJourney.Items.Material;
using ContinentOfJourney.Tiles;

namespace HWJBardHealer.Content.Accessories.Healer
{
    public class DeluxeDewCollector : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 30;
            Item.accessory = true;
            Item.value = Item.buyPrice(gold: 10);
            Item.rare = ItemRarityID.Lime;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            ModContent.GetInstance<DewCollector>().UpdateAccessory(player, hideVisual);
            player.GetThoriumPlayer().healBonus += 6;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<DewCollector>(), 1) 
                .AddIngredient(ModContent.ItemType<FungusDeluxe>(), 1) 
                .AddIngredient(ModContent.ItemType<EssenceofLife>(), 1)
                .AddTile(ModContent.TileType<FinalAnvil>())
                .Register();
        }
    }
}
