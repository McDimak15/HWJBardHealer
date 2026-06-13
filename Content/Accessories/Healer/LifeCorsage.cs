using Terraria;
using Terraria.ModLoader;
using ThoriumMod.Items;
using ThoriumMod.Items.Donate;
using ThoriumMod.Rarities;
using ThoriumMod.Utilities;
using ContinentOfJourney.Tiles;
using ContinentOfJourney.Items.Material;

namespace HWJBardHealer.Content.Accessories.Healer
{
    [AutoloadEquip(new EquipType[] { (EquipType)3 })]
    public class LifeCorsage : ThoriumItem
    {
        public override void SetDefaults()
        {
            this.isHealer = true;
            Item.width = 20;
            Item.height = 20;
            Item.value = Item.sellPrice(0, 0, 30, 0);
            Item.rare = DonatorRarity.Get(2);
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var thoriumPlayer = player.GetThoriumPlayer();

            player.lifeRegenTime += (float)((int)((float)thoriumPlayer.healBonus));
            player.lifeRegen += thoriumPlayer.healBonus;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<FragrantCorsage>(), 1);
            recipe.AddIngredient(ModContent.ItemType<EssenceofLife>(), 5);
            recipe.AddTile(ModContent.TileType<FountainofLife>());
            recipe.Register();
        }
    }
}
