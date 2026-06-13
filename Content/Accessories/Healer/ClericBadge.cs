using ContinentOfJourney.Items;
using HWJBardHealer.Content.Accessories.Bard;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using ThoriumMod;

namespace HWJBardHealer.Content.Accessories.Healer
{
    public class ClericBadge : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<BardBadge>();
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
            player.GetDamage<HealerDamage>() += 0.2f;
            player.GetCritChance<HealerDamage>() += 5f;
        }
    }

    public class ClericBadgeGlobalItem : GlobalItem
    {
        public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
        {
            if (item.type == ModContent.ItemType<WallofShadowTreasureBag>())
            {
                itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<ClericBadge>(), 5));
            }
        }
    }
}
