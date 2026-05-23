using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ThoriumMod;
using ThoriumMod.Projectiles.Bard;
using ThoriumMod.Items;
using ThoriumMod.Empowerments;
using ContinentOfJourney.Items.Material;
using ThoriumMod.Items.ArcaneArmor;
using HWJBardHealer.Content.Projectiles.Bard;
using Terraria.Audio;

namespace HWJBardHealer.Content.Weapons.Bard
{
    public class BurdenedFate : BardItem
    {
        public override BardInstrumentType InstrumentType => BardInstrumentType.String;

        public override void SetStaticDefaults()
        {
            Empowerments.AddInfo<Defense>(2, 0);
            Empowerments.AddInfo<DamageReduction>(1, 0);
            Empowerments.AddInfo<InvincibilityFrames>(1, 0);
        }

        public override void SetBardDefaults()
        {
            Item.damage = 48;
            Item.width = 48;
            Item.height = 48;
            Item.knockBack = 2f;
            Item.value = Item.buyPrice(gold: 6);
            Item.rare = ItemRarityID.LightRed;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 20;
            Item.useTime = 20;
            Item.noMelee = true;
            Item.shoot = ModContent.ProjectileType<PuppetNoteProj>();
            Item.shootSpeed = 14f;
            Item.autoReuse = true;
            InspirationCost = 1;
            Item.UseSound = SoundID.Item9;
            Item.noUseGraphic = false;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-10f, -6f);
        }

        public override bool BardShoot(Player player, Terraria.DataStructures.EntitySource_ItemUse_WithAmmo source,
            Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int noteIndex;
            int roll = Main.rand.Next(100);
            if (roll < 6)
                noteIndex = 0;
            else if (roll < 40)
                noteIndex = 1;
            else if (roll < 75)
                noteIndex = 2;
            else
                noteIndex = 3;

            Vector2 spawnPos = player.MountedCenter + velocity.SafeNormalize(Vector2.UnitX) * 32f;

            int proj = Projectile.NewProjectile(
                source,
                spawnPos,
                velocity,
                type,
                damage,
                knockback,
                player.whoAmI,
                noteIndex
            );

            if (Main.projectile.IndexInRange(proj) && noteIndex == 0)
            {
                Main.projectile[proj].ai[0] = 0f;
                Main.projectile[proj].localAI[0] = 1f;
            }

            SoundEngine.PlaySound(SoundID.Item9 with { Volume = 0.7f, PitchVariance = 0.1f }, position);
            return false;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();

            recipe.AddIngredient(ModContent.ItemType<BewitchedCotton>(), 10);
            recipe.AddIngredient(ModContent.ItemType<YewWood>(), 8);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }
    }
}
