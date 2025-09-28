using Microsoft.Xna.Framework;
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
        public override BardInstrumentType InstrumentType => BardInstrumentType.Electronic;

        public override void SetStaticDefaults()
        {
            Empowerments.AddInfo<AquaticAbility>(2, 0);
        }

        public override void SetBardDefaults()
        {
            Item.damage = 54;
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
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Sapphire, 8);
            recipe.AddIngredient(ItemID.SoulofNight, 6);
            recipe.AddIngredient(ItemID.Wire, 20);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }
    }
}
    