using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics; 
using System;                           
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using HWJBardHealer.Content.Projectiles.Thrower;
using ContinentOfJourney.Items.Material;
using ContinentOfJourney.Tiles;

namespace HWJBardHealer.Content.Weapons.Thrower
{
    public class CubismKunai : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.damage = 637;
            Item.DamageType = DamageClass.Throwing;
            Item.width = 24;
            Item.height = 24;
            Item.useTime = 24;
            Item.useAnimation = 24;
            Item.useStyle = 1;
            Item.noMelee = true;
            Item.knockBack = 7f;
            Item.value = Item.sellPrice(0, 0, 15, 0);
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = SoundID.Item1;
            Item.shoot = ModContent.ProjectileType<CubismKunaiProj>();
            Item.shootSpeed = 12f;
            Item.maxStack = 9999;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<CubistBar>(), 6)
                .AddIngredient(ModContent.ItemType<EssenceofMatter>(), 6)
                .AddTile(ModContent.TileType<FountainofMatter>())
                .Register();
        }

    }
}
