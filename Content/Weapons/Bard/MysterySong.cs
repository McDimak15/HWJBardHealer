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
using HWJBardHealer.Content.Projectiles.Bard;
using ContinentOfJourney.Items;

namespace HWJBardHealer.Content.Weapons.Bard
{
    public class MysterySong : BardItem
    {
        public override BardInstrumentType InstrumentType => BardInstrumentType.String;

        public override void SetStaticDefaults()
        {
            Empowerments.AddInfo<Defense>(3, 0);
            Empowerments.AddInfo<LifeRegeneration>(2, 0);
            Empowerments.AddInfo<DamageReduction>(2, 0);
        }

        public override void SetBardDefaults()
        {
            Item.damage = 100;
            InspirationCost = 3;
            Item.width = 48;
            Item.height = 48;
            Item.scale = 0.8f;

            Item.useTime = 26;
            Item.useAnimation = 26;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.knockBack = 2.5f;

            Item.value = Item.sellPrice(0, 2, 0, 0);
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = ThoriumSounds.SousNoise;

            Item.shoot = ModContent.ProjectileType<MysterySongProj>();
            Item.shootSpeed = 12f;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-30f, -10f);
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (velocity.LengthSquared() > 0.1f)
            {
                velocity.Normalize();
                position = player.MountedCenter + velocity * 40f;
            }
        }

        public class MysterySongDrop : GlobalItem
        {
            public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
            {
                if (item.type == ModContent.ItemType<WallofShadowTreasureBag>())
                {
                    itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<MysterySong>(), 1));
                }
            }
        }
    }
}
