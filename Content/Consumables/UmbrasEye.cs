using ContinentOfJourney.Items.Material;
using ContinentOfJourney.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ThoriumMod.Items.ThrownItems;
using ThoriumMod.Sounds;
// Use offset Currently WIP
namespace HWJBardHealer.Content.Consumables
{
    public class UmbrasEye : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 22;
            Item.rare = ItemRarityID.Red;
            Item.value = Item.sellPrice(gold: 6);
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.UseSound = ThoriumSounds.BasicSpell;
            Item.consumable = true;
        }
        public override bool? UseItem(Player player)
        {
            player.GetModPlayer<ConsumablePLayer>().usedUmbra = true;
            return true;
        }

        public override bool CanUseItem(Player player)
        {
            return !player.GetModPlayer<ConsumablePLayer>().usedUmbra;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<EssenceofDarkness>(), 6);
            recipe.AddIngredient(ModContent.ItemType<DarkMatter>(), 4);
            recipe.AddIngredient(ModContent.ItemType<TankOfThePastJungle>(),6);
            recipe.AddTile(ModContent.TileType<FinalAnvil>());
            recipe.Register();
        }
    }
}