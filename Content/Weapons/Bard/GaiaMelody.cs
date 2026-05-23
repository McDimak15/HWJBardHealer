using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using ThoriumMod;
using ThoriumMod.Empowerments;
using ThoriumMod.Projectiles.Bard;
using ThoriumMod.Sounds;
using ThoriumMod.Utilities;
using ThoriumMod.Items;
using ThoriumMod.Items.BardItems;
using HWJBardHealer.Content.Projectiles.Bard;
using ContinentOfJourney.Items.Material;
using ContinentOfJourney.Tiles;

namespace HWJBardHealer.Content.Weapons.Bard
{
    public class GaiaMelody : BardItem
    {
        public override BardInstrumentType InstrumentType => BardInstrumentType.Wind;

        public override void SetStaticDefaults()
        {
            Empowerments.AddInfo<ResourceRegen>(4, 0);
            Empowerments.AddInfo<ResourceGrabRange>(4, 0);
            Empowerments.AddInfo<ResourceMaximum>(2, 0);
        }

        public override void SetBardDefaults()
        {
            Item.damage = 315;
            InspirationCost = 3;
            Item.width = 48;
            Item.height = 48;
            Item.scale = 0.9f;

            Item.useTime = 28;
            Item.useAnimation = 28;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.knockBack = 3f;

            Item.value = Item.sellPrice(0, 3, 0, 0);
            Item.rare = ItemRarityID.Lime;
            Item.UseSound = ThoriumSounds.Panflute_Sound;

            Item.shoot = ModContent.ProjectileType<GaiaMelodyFlowerProj>();
            Item.shootSpeed = 12f;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2?(new Vector2(0f, 0f));
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity,
            ref int type, ref int damage, ref float knockback)
        {
            if (velocity.LengthSquared() > 0.1f)
            {
                velocity.Normalize();
                position = player.MountedCenter + velocity * 40f;
            }
        }

        public override bool BardShoot(Player player, Terraria.DataStructures.EntitySource_ItemUse_WithAmmo source,
            Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer != player.whoAmI)
                return false;

            Projectile.NewProjectile(source, position, velocity,
                ModContent.ProjectileType<GaiaMelodyFlowerProj>(),
                damage, knockback, player.whoAmI);

            Vector2 dir = velocity.SafeNormalize(Vector2.UnitX);
            Vector2 perpendicular = dir.RotatedBy(MathHelper.PiOver2);
            Vector2 upperPos = position + dir * 20f + perpendicular * 20f;
            Vector2 lowerPos = position + dir * 20f - perpendicular * 20f;

            Projectile.NewProjectile(source, upperPos, velocity * 0.9f,
                ModContent.ProjectileType<GaiaMelodySnakeProj>(),
                (int)(damage * 2.8f), knockback, player.whoAmI);

            Projectile.NewProjectile(source, lowerPos, velocity * 0.9f,
                ModContent.ProjectileType<GaiaMelodySnakeProj>(),
                (int)(damage * 2.8f), knockback, player.whoAmI);

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<Pungi>(), 1)
                .AddIngredient(ModContent.ItemType<EssenceofLife>(), 8)
                .AddIngredient(ModContent.ItemType<LivingBar>(), 6)
                .AddTile(ModContent.TileType<FountainofLife>())
                .Register();
        }
    }
}
