using ContinentOfJourney.Items;
using HWJBardHealer.Content.Accessories.Healer;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace HWJBardHealer.Content.Accessories.Thrower
{
    public class RogueBadge : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<ClericBadge>();
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
            player.GetDamage(DamageClass.Throwing) += 0.2f;
            player.GetCritChance(DamageClass.Throwing) += 5f;
        }
    }
    public class RogueBadgeGlobalItem : GlobalItem
    {
        public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
        {
            if (item.type == ModContent.ItemType<WallofShadowTreasureBag>())
            {
                itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<RogueBadge>(), 5));
            }
        }
    }
}
