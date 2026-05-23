using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ContinentOfJourney.Items.Material;
using ThoriumMod.Items.ThrownItems;
using HWJBardHealer.Content.Projectiles.Thrower;

namespace HWJBardHealer.Content.Weapons.Thrower
{
    public class DiverTomahawk : ModItem
    {
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "DiverTomahawkTooltip",
                "“It seems to be made from a mix of natural and foreign materials”")
            {
                OverrideColor = new Color(57, 58, 112)
            });
        }
        public override void SetDefaults()
        {
            Item.damage = 60; 
            Item.DamageType = DamageClass.Throwing;
            Item.width = 30;
            Item.height = 30;
            Item.useTime = 26;
            Item.useAnimation = 26;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.knockBack = 8f;
            Item.value = Item.sellPrice(0, 0, 25, 0);
            Item.rare = ItemRarityID.Cyan;
            Item.UseSound = SoundID.Item1;
            Item.shoot = ModContent.ProjectileType<DiverTomahawkProj>();
            Item.shootSpeed = 13f;
            Item.maxStack = 1;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.Find<ModItem>("ThoriumMod", "RiftTearer"), 1);
            recipe.AddIngredient(ModContent.Find<ModItem>("ContinentOfJourney", "AbyssalChunk"), 8);
            recipe.AddIngredient(ModContent.Find<ModItem>("ContinentOfJourney", "FluorescentFibre"), 4);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }
    }

    public class DiverTomahawkGlobalNPC : GlobalNPC
    {
        public int tomahawkStacks;
        public override bool InstancePerEntity => true;

        public override void OnKill(NPC npc)
        {
            tomahawkStacks = 0;
        }
    }
}
