using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using ThoriumMod.Items.HealerItems;
using ThoriumMod.Tiles;
using ThoriumMod.Items;
using ThoriumMod;
using ContinentOfJourney.Items;

namespace HWJBardHealer.Content.Weapons.Healer
{
    public class DipperWand : ThoriumItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.mana = 8;
            Item.DamageType = ThoriumDamageBase<HealerTool>.Instance;
            Item.damage = 20;
            radiantLifeCost = 5;
            isHealer = true;

            Item.width = 32;
            Item.height = 32;
            Item.useTime = 24;
            Item.useAnimation = 24;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3f;
            Item.value = Item.sellPrice(0, 1, 0, 0);
            Item.rare = ItemRarityID.Orange;
            Item.UseSound = SoundID.Item24;
            Item.autoReuse = true;

            Item.shoot = ModContent.ProjectileType<Projectiles.Healer.DipperWandProj>();
            Item.shootSpeed = 8f;

            Item.noUseGraphic = false;
        }

        public class DipperWandDrop : GlobalItem
        {
            public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
            {
                if (item.type == ModContent.ItemType<BigDipperTreasureBag>())
                {
                    itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<DipperWand>(), 1));
                }
            }
        }
    }
}
