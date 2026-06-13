using ContinentOfJourney.Items;
using HWJBardHealer.Content.Accessories.Thrower;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using ThoriumMod;


namespace HWJBardHealer.Content.Accessories.Bard
{
    public class BardBadge : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<RogueBadge>();
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
            player.GetDamage<BardDamage>() += 0.2f;
            player.GetCritChance<BardDamage>() += 5f;
        }
    }

    public class BardBadgeGlobalItem : GlobalItem
    {
        public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
        {
            if (item.type == ModContent.ItemType<WallofShadowTreasureBag>())
            {
                itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<BardBadge>(), 5));
            }
        }
    }
}
