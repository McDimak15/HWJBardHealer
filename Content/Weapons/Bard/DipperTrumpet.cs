using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using ThoriumMod;
using ThoriumMod.Items;
using ThoriumMod.Empowerments;
using ThoriumMod.Sounds;
using HWJBardHealer.Content.Projectiles.Bard;
using ContinentOfJourney.Items;

namespace HWJBardHealer.Content.Weapons.Bard
{
    public class DipperTrumpet : BardItem
    {
        public override BardInstrumentType InstrumentType => BardInstrumentType.Brass;

        public override void SetStaticDefaults()
        {
            Empowerments.AddInfo<CriticalStrikeChance>(2, 0);
            Empowerments.AddInfo<AttackSpeed>(1, 0);
        }

        public override void SetBardDefaults()
        {
            Item.damage = 50;
            InspirationCost = 3;
            Item.width = 48;
            Item.height = 48;
            Item.scale = 1f;

            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.knockBack = 4f;

            Item.value = Item.sellPrice(0, 1, 80, 0);
            Item.rare = ItemRarityID.Yellow;
            Item.UseSound = ThoriumSounds.Trumpet_Sound;

            Item.shoot = ModContent.ProjectileType<DipperTrumpetProj>();
            Item.shootSpeed = 0f;

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

        public class DipperTrumpetDrop : GlobalItem
        {
            public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
            {
                if (item.type == ModContent.ItemType<BigDipperTreasureBag>())
                {
                    itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<DipperTrumpet>(), 1));
                }
            }
        }
    }
}
