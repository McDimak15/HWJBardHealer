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

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            ThoriumPlayer thoriumPlayer = player.GetModPlayer<ThoriumPlayer>();
            player.GetDamage<HealerDamage>() += 0.20f;
            player.GetCritChance<HealerDamage>() += 5f;
        }
    }

    public class ClericBadgeGlobalItem : GlobalItem
    {
        public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
        {
            if (ModLoader.TryGetMod("ContinentOfJourney", out Mod coJ))
            {
                if (item.type == coJ.Find<ModItem>("WallofShadowTreasureBag")?.Type)
                {
                    itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<ClericBadge>(), 5));
                }
            }
        }
    }
}
