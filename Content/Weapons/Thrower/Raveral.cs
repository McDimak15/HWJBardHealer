using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using HWJBardHealer.Content.Projectiles.Thrower;
using ContinentOfJourney.Items;

namespace HWJBardHealer.Content.Weapons.Thrower
{
    public class Raveral : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 303;
            Item.DamageType = DamageClass.Throwing;
            Item.width = 32;
            Item.height = 32;
            Item.useTime = 23;
            Item.scale = 1.3f;
            Item.useAnimation = 23;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.knockBack = 4f;
            Item.value = Item.buyPrice(0, 3, 50, 0);
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;

            Item.shoot = ModContent.ProjectileType<RaveralProj>();
            Item.shootSpeed = 12f;
        }
    }

    public class RaveralDrop : GlobalItem
    {
        public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
        {
            if (item.type == ModContent.ItemType<WallofShadowTreasureBag>())
            {
                itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<Raveral>(), 1));
            }
        }
    }
}
