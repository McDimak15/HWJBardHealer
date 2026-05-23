using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using ThoriumMod;
using ThoriumMod.Items;
using ThoriumMod.Items.HealerItems;
using Terraria.GameContent.ItemDropRules;
using ContinentOfJourney.Items;

namespace HWJBardHealer.Content.Weapons.Healer
{
    public class SolarStaff : ThoriumItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.mana = 10;
            Item.DamageType = ThoriumDamageBase<HealerTool>.Instance;
            radiantLifeCost = 6;
            isHealer = true;

            Item.width = 34;
            Item.height = 34;
            Item.useTime = 28;
            Item.useAnimation = 28;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3f;
            Item.value = Item.sellPrice(0, 1, 25, 0);
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SoundID.Item43;
            Item.autoReuse = true;

            Item.shoot = ModContent.ProjectileType<Projectiles.Healer.SolarStaffProj>();
            Item.shootSpeed = 10f;

            Item.noUseGraphic = false;
        }

        public class SolarStaffDrop : GlobalItem
        {
            public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
            {
                if (item.type == ModContent.ItemType<SlimeGodTreasureBag>())
                {
                    itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<SolarStaff>(), 1));
                }
            }
        }
    }
}
