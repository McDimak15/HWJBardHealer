using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
//using ThoriumMod.Items.HealerItems;
using ThoriumMod.Items;
using ThoriumMod;
using HWJBardHealer.Content.Projectiles.Healer;

namespace HWJBardHealer.Content.Weapons.Healer
{
    public class Echolocation : ThoriumItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.mana = 30; 
            radiantLifeCost = 12;
            isHealer = true;

            Item.DamageType = ThoriumDamageBase<HealerTool>.Instance;
            Item.damage = 35;
            Item.width = 36;
            Item.height = 36;
            Item.useTime = 40;
            Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.value = Item.sellPrice(0, 2, 50, 0);
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SoundID.Item43; 
            Item.autoReuse = true;

            Item.shoot = ModContent.ProjectileType<EcholocationWhale>();
            Item.shootSpeed = 0f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile p = Main.projectile[i];
                    if (p.active && p.owner == player.whoAmI && p.type == type)
                    {
                        p.Kill();
                    }
                }

                Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, type, damage, knockback, player.whoAmI);
            }
            return false;
        }
    }
}