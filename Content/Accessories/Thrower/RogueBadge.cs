using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;
using System.Collections.Generic;
using Terraria.GameContent.ItemDropRules;
using CalamityMod;

namespace HWJBardHealer.Content.Accessories.Thrower
{
    public class RogueBadge : ModItem
    {
        public override void SetStaticDefaults()
        {
            if (!ModLoader.TryGetMod("CalamityMod", out _))
                return;

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            if (!ModLoader.TryGetMod("CalamityMod", out _))
                return;

            Item.width = 28;
            Item.height = 28;
            Item.value = Item.sellPrice(gold: 5);
            Item.rare = ItemRarityID.Yellow;
            Item.accessory = true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (!ModLoader.TryGetMod("CalamityMod", out _))
                return;

            tooltips.RemoveAll(t => t.Name.StartsWith("RogueBadge_"));
            tooltips.Add(new TooltipLine(Mod, "RogueBadge_1", "20% increased rogue damage"));
            tooltips.Add(new TooltipLine(Mod, "RogueBadge_2", "5% increased rogue critical strike chance"));
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (ModLoader.TryGetMod("CalamityMod", out Mod _))
            {
                player.GetDamage<ThrowingDamageClass>() += 0.20f;
                player.GetCritChance<ThrowingDamageClass>() += 5f;
            }
        }
    }
    public class RogueBadgeGlobalItem : GlobalItem
    {
        public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
        {
            if (ModLoader.TryGetMod("ContinentOfJourney", out Mod coJ) &&
                ModLoader.TryGetMod("CalamityMod", out Mod _))
            {
                if (item.type == coJ.Find<ModItem>("WallofShadowTreasureBag")?.Type)
                {
                    itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<RogueBadge>(), 5));
                }
            }
        }
    }
}
