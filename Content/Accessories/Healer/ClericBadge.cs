using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;
using System.Collections.Generic;
using Terraria.GameContent.ItemDropRules;
using ThoriumMod;
using ThoriumMod.Items.HealerItems;

namespace HWJBardHealer.Content.Accessories.Healer
{
    public class ClericBadge : ModItem
    {
        public override void SetStaticDefaults()
        {
            if (!ModLoader.TryGetMod("ThoriumMod", out _))
                return;

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            if (!ModLoader.TryGetMod("ThoriumMod", out _))
                return;

            Item.width = 28;
            Item.height = 28;
            Item.value = Item.sellPrice(gold: 5);
            Item.rare = ItemRarityID.Yellow;
            Item.accessory = true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (!ModLoader.TryGetMod("ThoriumMod", out _))
                return;

            tooltips.RemoveAll(t => t.Name.StartsWith("ClericBadge_"));
            tooltips.Add(new TooltipLine(Mod, "ClericBadge_1", "20% increased radiant damage"));
            tooltips.Add(new TooltipLine(Mod, "ClericBadge_2", "5% increased radiant critical strike chance"));
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (ModLoader.TryGetMod("ThoriumMod", out Mod _))
            {
                ThoriumPlayer thoriumPlayer = player.GetModPlayer<ThoriumPlayer>();
                player.GetDamage<HealerDamage>() += 0.20f;
                player.GetCritChance<HealerDamage>() += 0.05f;
            }
        }
    }

    public class ClericBadgeGlobalItem : GlobalItem
    {
        public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
        {
            if (ModLoader.TryGetMod("ContinentOfJourney", out Mod coJ) &&
                ModLoader.TryGetMod("ThoriumMod", out Mod _))
            {
                if (item.type == coJ.Find<ModItem>("WallofShadowTreasureBag")?.Type)
                {
                    itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<ClericBadge>(), 5));
                }
            }
        }
    }
}
