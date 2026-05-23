using System;
using Terraria;
using Terraria.ModLoader;
using ThoriumMod;
using ThoriumMod.Items;
using ThoriumMod.Items.BardItems;
using ThoriumMod.Utilities;
using ContinentOfJourney.Tiles;
using ContinentOfJourney.Items.Material;

namespace HWJBardHealer.Content.Accessories.Bard
{
    public class DarkPoetry : BardItem
    {
        public override void SetBardDefaults()
        {
            this.accDamage = "25% basic damage";
            base.Item.width = 20;
            base.Item.height = 20;
            base.Item.value = Item.sellPrice(0, 2, 0, 0);
            base.Item.rare = 4;
            base.Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            ThoriumPlayer thoriumPlayer = player.GetThoriumPlayer();
            thoriumPlayer.bardBuffDuration += 300;
            player.GetModPlayer<ThormwardPlayer>().accDarkPoetry = true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<FullScore>(), 1);
            recipe.AddIngredient(ModContent.ItemType<EssenceofDarkness>(), 8);
            recipe.AddTile(ModContent.TileType<FinalAnvil>());
            recipe.Register();
        }
    }
}
