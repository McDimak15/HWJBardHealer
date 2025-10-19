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
            // DisplayName.SetDefault("Deluxe Dew Collector");
            // Tooltip.SetDefault("Combines the properties of Dew Collector and Fungus Deluxe");
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
            tooltips.Add(new TooltipLine(Mod, "Dew1", "Healing an ally grants them 2 life recovery and generates dew"));
            tooltips.Add(new TooltipLine(Mod, "Dew2", "Picking up dew increases your own life recovery by 4"));
            tooltips.Add(new TooltipLine(Mod, "Dew3", "Healing spells will heal an additional 6 life"));
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
