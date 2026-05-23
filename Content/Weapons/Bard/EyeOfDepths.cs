using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using ThoriumMod.Empowerments;
//using ThoriumMod.Items.BardItems;
using ThoriumMod.Projectiles.Bard;
using ThoriumMod.Sounds;
using ThoriumMod.Utilities;
using ThoriumMod.Items;
using ThoriumMod;
using HWJBardHealer.Content.Projectiles.Bard;

namespace HWJBardHealer.Content.Weapons.Bard
{
    public class EyeOfDepths : BardItem
    {
        public override BardInstrumentType InstrumentType => BardInstrumentType.Brass;

        public override void BardTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "EyeOfDepthsTooltip",
                "“You hear strange voices from the mouth of the tuba”")
            {
                OverrideColor = new Color(53, 54, 112)
            });
        }
        public override void SetStaticDefaults()
        {
            Empowerments.AddInfo<Damage>(1, 0);
            Empowerments.AddInfo<FlatDamage>(3, 0);
            Empowerments.AddInfo<CriticalStrikeChance>(3, 0);
            Empowerments.AddInfo<AquaticAbility>(2, 0);
        }

        public override void SetBardDefaults()
        {
            Item.damage = 80;
            InspirationCost = 2;
            Item.width = 48;
            Item.height = 48;
            Item.scale = 1.6f;

            Item.useTime = 22;
            Item.useAnimation = 22;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.knockBack = 3f;

            Item.value = Item.sellPrice(0, 1, 20, 0);
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = ThoriumSounds.Nocturne_Sound;

            Item.shoot = ModContent.ProjectileType<EyeOfDepthsProj>();
            Item.shootSpeed = 10f;

            Item.holdStyle = 0;
        }

        public override Vector2? HoldoutOffset()
        {
            return Vector2.Zero;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (velocity.LengthSquared() > 0.1f)
            {
                velocity.Normalize();
                position = player.MountedCenter + velocity * 30f; 
            }

            if (Main.myPlayer == player.whoAmI)
            {
                SoundEngine.PlaySound(ThoriumSounds.Nocturne_Sound, player.position);
            }
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.Find<ModItem>("ContinentOfJourney", "AbyssalChunk"), 4);
            recipe.AddIngredient(ModContent.Find<ModItem>("ContinentOfJourney", "DeepBar"), 4);
            recipe.AddIngredient(ModContent.Find<ModItem>("ContinentOfJourney", "FluorescentFibre"), 6);
            recipe.AddIngredient(ModContent.Find<ModItem>("ThoriumMod", "ScubaCurva"), 1);
            recipe.AddIngredient(ModContent.Find<ModItem>("ThoriumMod", "TwentyFourCaratTuba"), 1);
            recipe.AddIngredient(ModContent.Find<ModItem>("ThoriumMod", "SerpentsCry"), 1);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }
    }
}
    