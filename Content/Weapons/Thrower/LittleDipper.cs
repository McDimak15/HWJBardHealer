using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using HWJBardHealer.Content.Projectiles.Thrower;
using ContinentOfJourney.Items;

namespace HWJBardHealer.Content.Weapons.Thrower
{
    public class LittleDipper : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 50;
            Item.DamageType = DamageClass.Throwing;
            Item.width = 32;
            Item.height = 32;
            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 4;
            Item.value = Item.buyPrice(0, 5, 0, 0);
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.noUseGraphic = true;

            Item.shoot = ModContent.ProjectileType<LittleDipperProj>();
            Item.shootSpeed = 12f;
        }
    }

    public class LittleDipperDrop : GlobalItem
    {
        public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
        {
            if (item.type == ModContent.ItemType<BigDipperTreasureBag>())
            {
                itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<LittleDipper>(), 1));
            }
        }
    }
}
