using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ThoriumMod.Items.ThrownItems;
using ContinentOfJourney.Items.Accessories;
using ContinentOfJourney.Tiles;
using CalamityMod.Tiles.Furniture.CraftingStations;
using ContinentOfJourney;
using Microsoft.Xna.Framework;

namespace HWJBardHealer.Content.Accessories.Thrower
{
    public class TheBibleOfTheThrowerVol4 : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void ModifyTooltips(System.Collections.Generic.List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "BibleDamage", "20% increased thrower damage"));
            tooltips.Add(new TooltipLine(Mod, "BibleVelocity", "50% increased consumable thrower weapon velocity"));
            tooltips.Add(new TooltipLine(Mod, "BibleUseTime", "Decrease use time of consumable thrower weapons by 20%"));
            tooltips.Add(new TooltipLine(Mod, "BibleAcceleration", "When holding a thrower weapon, increase acceleration by 75%"));
            tooltips.Add(new TooltipLine(Mod, "Bible17", "17.5% of your thrower damage is duplicated"));

            TooltipLine exhaustion = new TooltipLine(Mod, "BibleExhaustion","Removes all Exhaustion when equipped");
            exhaustion.OverrideColor = new Color(95, 193, 4); 
            tooltips.Add(exhaustion);
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.value = Item.sellPrice(gold: 12);
            Item.rare = ItemRarityID.Yellow;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (ModLoader.TryGetMod("ThoriumMod", out Mod thorium))
            {
                player.GetModPlayer<ThoriumMod.ThoriumPlayer>().throwGuide3 = true;
            }

            player.GetDamage<ThrowingDamageClass>() += 0.2f;
 
            if (ModLoader.TryGetMod("ContinentOfJourney", out Mod coj))
            {
                TemplatePlayer cojPlayer = player.GetModPlayer<TemplatePlayer>();
                cojPlayer.BattersCap = true;
                cojPlayer.CatchersGlove = true;
                cojPlayer.TheBatter = true;
                cojPlayer.RunnersLegging = true;
            }
        }

        public override void AddRecipes()
        {
            if (ModLoader.HasMod("ThoriumMod") && ModLoader.HasMod("ContinentOfJourney"))
            {
                CreateRecipe()
                    .AddIngredient(ModContent.ItemType<ThrowingGuideVolume3>())
                    .AddIngredient(ModContent.ItemType<TheBatter>())
                    .AddIngredient(ModContent.ItemType<ContinentOfJourney.Items.Material.EssenceofBright>(), 5)
                    .AddTile(ModContent.TileType<FinalAnvil>())
                    .Register();
            }

        }
    }
}
