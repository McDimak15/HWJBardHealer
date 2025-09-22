using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics; 

using System;                           
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Buffs.DamageOverTime;
using HWJBardHealer.Content.Projectiles.Thrower;
using ContinentOfJourney.Items.Material;

namespace HWJBardHealer.Content.Weapons.Thrower
{
    public class CubismKunai : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.damage = 90;
            Item.DamageType = DamageClass.Throwing;
            Item.width = 24;
            Item.height = 24;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 3f;
            Item.value = Item.buyPrice(0, 0, 15, 0);
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = SoundID.Item1;
            Item.shoot = ModContent.ProjectileType<CubismKunaiProj>();
            Item.shootSpeed = 12f;
            Item.consumable = true;
            Item.maxStack = 9999;
        }

        public override void AddRecipes()
        {
            CreateRecipe(50)
                .AddIngredient(ModContent.ItemType<CubistBar>(), 1)
                .AddIngredient(ModContent.ItemType<EssenceofMatter>(), 1)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
