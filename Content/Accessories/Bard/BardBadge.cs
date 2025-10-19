using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;
using System.Collections.Generic;
using Terraria.GameContent.ItemDropRules;
using ThoriumMod;


namespace HWJBardHealer.Content.Accessories.Bard
{
    public class BardBadge : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 28;
            Item.value = Item.sellPrice(gold: 5);
            Item.rare = ItemRarityID.Yellow;
            Item.accessory = true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {


            tooltips.RemoveAll(t => t.Name.StartsWith("BardBadge_"));
            tooltips.Add(new TooltipLine(Mod, "BardBadge_1", "20% increased symphonic damage"));
            tooltips.Add(new TooltipLine(Mod, "BardBadge_2", "5% increased symphonic critical strike chance"));
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            ThoriumPlayer thoriumPlayer = player.GetModPlayer<ThoriumPlayer>();
            player.GetDamage<BardDamage>() += 0.20f;
            player.GetCritChance<BardDamage>() += 0.05f;
        }
    }

    public class BardBadgeGlobalItem : GlobalItem
    {
        public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
        {
            if (ModLoader.TryGetMod("ContinentOfJourney", out Mod coJ) &&
                ModLoader.TryGetMod("ThoriumMod", out Mod _))
            {
                if (item.type == coJ.Find<ModItem>("WallofShadowTreasureBag")?.Type)
                {
                    itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<BardBadge>(), 5));
                }
            }
        }
    }
}
