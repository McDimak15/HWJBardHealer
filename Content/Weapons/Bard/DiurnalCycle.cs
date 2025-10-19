using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ThoriumMod;
using ThoriumMod.Items;
using ThoriumMod.Items.BardItems;
using ThoriumMod.Empowerments;
using HWJBardHealer.Content.Projectiles.Bard;
using Microsoft.Xna.Framework;
using Terraria.Audio;

namespace HWJBardHealer.Content.Weapons.Bard
{
    public class DiurnalCycle : BardItem
    {
        public override BardInstrumentType InstrumentType => BardInstrumentType.Wind;

        public override void SetStaticDefaults()
        {
            Empowerments.AddInfo<AttackSpeed>(2, 0);
            Empowerments.AddInfo<ResourceGrabRange>(1, 0);

            Mod CBH = ModLoader.TryGetMod("CalamityBardHealer", out var calamityBardHealer) ? calamityBardHealer : null;
            if (CBH != null)
            {
                Empowerments.AddInfo<ResourceRegen>(2, 0);
                Empowerments.AddInfo<ResourceMaximum>(1, 0);
            }
            else
            {
                Empowerments.AddInfo<ResourceConsumptionChance>(2, 0);
            }
        }

        public override void SetBardDefaults()
        {
            Item.damage = 62;
            Item.width = 48;
            Item.height = 48;
            Item.knockBack = 1.2f;
            Item.value = Item.buyPrice(gold: 6);
            Item.rare = ItemRarityID.Pink;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 16;
            Item.useTime = 16;
            Item.noMelee = true;
            Item.shoot = ModContent.ProjectileType<DiurnalCycleStar>(); 
            Item.shootSpeed = 20f;
            Item.UseSound = SoundID.Item91;
            Item.autoReuse = true;
            InspirationCost = 1;
        }


        public override bool BardShoot(Player player,Terraria.DataStructures.EntitySource_ItemUse_WithAmmo source,Vector2 position,Vector2 velocity,int type,int damage,float knockback)
        {
            float spread = 8f; 
            float rot = MathHelper.ToRadians(Main.rand.NextFloat(-spread, spread));
            Vector2 perturbedVelocity = velocity.RotatedBy(rot) * Main.rand.NextFloat(0.6f, 0.7f);

            Vector2 spawnPos = position + Vector2.Normalize(velocity) * 70f;

            Projectile.NewProjectile(
                    source,
                    spawnPos,
                    perturbedVelocity,
                    type,
                    damage,
                    knockback,
                    player.whoAmI
                );


            SoundEngine.PlaySound(SoundID.Item9 with { Volume = 0.7f, PitchVariance = 0.1f }, position);

            return false;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-13f, -10f);
        }

        public override void AddRecipes()
        {
            Mod CBH = ModLoader.TryGetMod("CalamityBardHealer", out var calamityBardHealer) ? calamityBardHealer : null;
            Mod calamity = ModLoader.TryGetMod("CalamityMod", out var calamityMod) ? calamityMod : null;

            Recipe recipe = CreateRecipe();

            recipe.AddIngredient<HoneyRecorder>();
            recipe.AddIngredient<BoneTrumpet>();

            if (CBH != null)
            {
                    recipe.AddIngredient(CBH.Find<ModItem>("Windward").Type);
                    recipe.AddIngredient(CBH.Find<ModItem>("FilthyFlute").Type);

            }
            else
            {
                recipe.AddIngredient<Panflute>();
                recipe.AddIngredient<RivetingTadpole>();
            }
            if (calamity != null)
               recipe.AddIngredient(calamity.Find<ModItem>("PurifiedGel").Type, 5);

            recipe.AddTile(TileID.DemonAltar);
            recipe.Register();
            
        }
    }
}
