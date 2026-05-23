using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using ThoriumMod;
using ThoriumMod.Projectiles.Bard;
using ThoriumMod.Items;
using ThoriumMod.Sounds;
using HWJBardHealer.Content.Projectiles.Bard;
using ThoriumMod.Empowerments;
using ContinentOfJourney.Items;
using ContinentOfJourney.Tiles;

namespace HWJBardHealer.Content.Weapons.Bard
{
    public class Sunshine : BardItem
    {
        public override BardInstrumentType InstrumentType => BardInstrumentType.String;

        public override void SetStaticDefaults()
        {
            Empowerments.AddInfo<Defense>(4, 0);
            Empowerments.AddInfo<DamageReduction>(4, 0);
        }

        public override void SetBardDefaults()
        {
            Item.damage = 270;
            InspirationCost = 3;
            Item.width = 48;
            Item.height = 48;
            Item.scale = 1.4f;

            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.knockBack = 2.5f;

            Item.value = Item.sellPrice(0, 2, 0, 0);
            Item.rare = ItemRarityID.Yellow;
            Item.UseSound = ThoriumSounds.Cello_Sound;

            Item.shoot = ModContent.ProjectileType<SunshineSlimeProj>();
            Item.shootSpeed = 8f;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-10f, -5f);

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (velocity.LengthSquared() > 0.01f)
            {
                velocity.Normalize();
                position = player.MountedCenter + velocity * 40f;
            }
        }

        public class SunshineDrop : GlobalItem
        {
            public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
            {
                if (item.type == ModContent.ItemType<SlimeGodTreasureBag>())
                {
                    itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<Sunshine>(), 1));
                }
            }
        }
    }
}
