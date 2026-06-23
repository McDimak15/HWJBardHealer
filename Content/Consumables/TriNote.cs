using ContinentOfJourney.Items.Material;
using ContinentOfJourney.Tiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ThoriumMod.Items.BardItems;
using ThoriumMod.Sounds;

namespace HWJBardHealer.Content.Consumables
{
    public class TriNote : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 64;
            Item.height = 46;
            Item.rare = ItemRarityID.Purple;
            Item.value = Item.sellPrice(gold: 1);
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.UseSound = ThoriumSounds.BasicSpell;
            Item.consumable = true;
        }
        public override bool? UseItem(Player player)
        {
            player.GetModPlayer<ConsumablePLayer>().usedTrinote = true;
            return true;
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            player.itemLocation.Y += 8f;
            player.itemLocation.X -= 32f * player.direction;
            player.itemRotation = MathHelper.ToRadians(15f) * player.direction;
        }

        public override bool CanUseItem(Player player)
        {
            return !player.GetModPlayer<ConsumablePLayer>().usedTrinote;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<InspirationFragment>());
            recipe.AddIngredient(ModContent.ItemType<InspirationShard>());
            recipe.AddIngredient(ModContent.ItemType<InspirationCrystalNew>());
            recipe.AddIngredient(ModContent.ItemType<TankOfThePastJungle>(),6);
            recipe.AddTile(ModContent.TileType<FinalAnvil>());
            recipe.Register();
        }
    }
}