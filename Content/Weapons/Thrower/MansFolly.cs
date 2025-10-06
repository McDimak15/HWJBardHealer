using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ThoriumMod;

namespace TestMod;

public class MansFolly : ModItem
{
    public override void SetStaticDefaults()
    {
        Terraria.Item.staff[Type] = true;
    }

    public override void SetDefaults()
    {
        Item.height = Item.width = 76;
        Item.DamageType = DamageClass.Throwing;

        Item.useStyle = ItemUseStyleID.Swing;
        Item.noMelee = true;
        Item.noUseGraphic = true;
        Item.damage = 100;
        Item.knockBack = 3;
        Item.useTime = 50;
        Item.useAnimation = 50;
        Item.value = Item.buyPrice(0, 0, 15, 0);
        Item.rare = ItemRarityID.Pink;

        Item.shoot = ModContent.ProjectileType<FollyProj>();
        Item.shootSpeed = 8;
        Item.UseSound = SoundID.Item18;
    }
}