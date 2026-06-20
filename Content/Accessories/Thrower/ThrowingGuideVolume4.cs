using ContinentOfJourney.Items.Material;
using ContinentOfJourney.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ThoriumMod.Items;
using ThoriumMod.Items.ThrownItems;
using ThoriumMod.Utilities;

namespace HWJBardHealer.Content.Accessories.Thrower
{
    public class ThrowingGuideVolume4 : ThoriumItem
    {
        public override void SetDefaults()
        {
            this.isThrower = true;
            Item.width = 28;
            Item.height = 28;
            Item.value = Item.sellPrice(gold: 5);
            Item.rare = ItemRarityID.Cyan;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Throwing) += 0.15f;
            player.ThrownVelocity += 0.15f;
            player.GetThoriumPlayer().accPiratesPurse = true;
            player.GetModPlayer<ThormwardPlayer>().throwGuide4 = true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<ThrowingGuideVolume3>());
            recipe.AddIngredient(ModContent.ItemType<PiratesPurse>());
            recipe.AddIngredient(ModContent.ItemType<CubistBar>(), 8);
            recipe.AddIngredient(ModContent.ItemType<EssenceofMatter>(), 4);
            recipe.AddTile(ModContent.TileType<FinalAnvil>());
            recipe.Register();
        }
    }
}
