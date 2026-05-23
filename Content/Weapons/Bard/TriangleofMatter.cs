using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;
using ThoriumMod;
using ThoriumMod.Sounds;
using ThoriumMod.Empowerments;
using ThoriumMod.Items;
using ThoriumMod.Items.BardItems;
using ContinentOfJourney.Items.Material;
using ContinentOfJourney.Tiles;
using HWJBardHealer.Content.Projectiles.Bard;

namespace HWJBardHealer.Content.Weapons.Bard
{
    public class TriangleofMatter : BardItem
    {
        public override BardInstrumentType InstrumentType => BardInstrumentType.Percussion;

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            Empowerments.AddInfo<FlightTime>(4, 0);
            Empowerments.AddInfo<MovementSpeed>(4, 0);
            Empowerments.AddInfo<JumpHeight>(3, 0);
        }

        public override void SetBardDefaults()
        {
            Item.width = 44;
            Item.height = 44;
            Item.useStyle = 14;
            Item.holdStyle = 6;
            Item.useTime = 60;
            Item.useAnimation = 60;
            Item.damage = 320;
            Item.knockBack = 8f;
            Item.autoReuse = true;
            Item.value = Item.sellPrice(0, 10, 0, 0);
            Item.rare = 8;
            Item.noMelee = true;
            Item.DamageType = ThoriumDamageBase<BardDamage>.Instance;
            Item.shoot = ModContent.ProjectileType<TriangleCircle>();
            Item.shootSpeed = 1f;
            Item.UseSound = ThoriumSounds.Xylophone_Sound;
            InspirationCost = 2;
        }

        public override Color? GetAlpha(Color lightColor) => Color.White;

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<TheTriangle>(), 1);
            recipe.AddIngredient(ModContent.ItemType<CubistBar>(), 6);
            recipe.AddIngredient(ModContent.ItemType<EssenceofMatter>(), 4);
            recipe.AddTile(ModContent.TileType<FountainofMatter>());
            recipe.Register();
        }
    }
}
