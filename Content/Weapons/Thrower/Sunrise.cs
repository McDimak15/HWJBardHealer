using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using HWJBardHealer.Content.Projectiles.Thrower;
using ThoriumMod.Items.ThrownItems;
using ThoriumMod.Items.BossBuriedChampion;
using ContinentOfJourney.Items.ThrowerWeapons;
using ContinentOfJourney.Items;

namespace HWJBardHealer.Content.Weapons.Thrower
{
    public class Sunrise : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.damage = 46;
            Item.DamageType = DamageClass.Throwing;
            Item.width = 34;
            Item.height = 34;
            Item.useTime = 21;
            Item.useAnimation = 21;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 4f;
            Item.value = Item.buyPrice(gold: 5);
            Item.rare = ItemRarityID.Red;
            Item.UseSound = SoundID.Item1;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<SunriseProj>();
            Item.shootSpeed = 16f;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();

            recipe.AddIngredient<InkyToss>();
            recipe.AddIngredient<ChampionsGodHand>();
            recipe.AddIngredient<ToothOfCthulhu>();
            recipe.AddIngredient<Backstabber>();

            if (ModLoader.HasMod("CalamityMod"))
            {
                Mod calamity = ModLoader.GetMod("CalamityMod");
                if (calamity != null && calamity.TryFind("PurifiedGel", out ModItem purifiedGel))
                    recipe.AddIngredient(purifiedGel.Type, 5);
            }

            recipe.AddTile(TileID.DemonAltar);
            recipe.Register();
        }
    }
}
