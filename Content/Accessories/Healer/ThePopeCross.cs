using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;
using ThoriumMod;
using ThoriumMod.Items;
using ThoriumMod.Utilities;
using ThoriumMod.Items.HealerItems;
using ContinentOfJourney.Buffs;
using ContinentOfJourney.Items.Accessories;
using ContinentOfJourney.Items.Material;
using ContinentOfJourney.Tiles;

namespace HWJBardHealer.Content.Accessories.Healer
{
    public class ThePopeCross : ThoriumItem
    {
        public override void SetDefaults()
        {
            isHealer = true;
            Item.width = 24;
            Item.height = 28;
            Item.value = Item.sellPrice(0, 50, 0, 0);
            Item.rare = ItemRarityID.Yellow;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.buffImmune[ModContent.BuffType<DivineFireBuff>()] = true;
            player.GetAttackSpeed(ThoriumDamageBase<HealerTool>.Instance) += 0.1f;
            player.GetModPlayer<ThormwardPlayer>().accPopeCross.Set(Item);
            player.longInvince = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<UndyingCross>(), 1)
                .AddIngredient(ModContent.ItemType<SavingGrace>(), 1)
                .AddIngredient(ModContent.ItemType<EssenceofTime>(), 5)
                .AddTile(ModContent.TileType<FinalAnvil>())
                .Register();
        }
    }

    public class PopeCrossGlobalItem : GlobalItem
    {
        public override bool InstancePerEntity => true;

        public override void OnHitNPC(Item item, Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (item.DamageType == ThoriumDamageBase<HealerDamage>.Instance &&
                player.GetModPlayer<ThormwardPlayer>().accPopeCross.Active)
            {
                player.GetModPlayer<ThormwardPlayer>().IncrementPopeCrossProgress();
            }
        }
    }

    public class PopeCrossGlobalProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (projectile.owner < 0 || projectile.owner >= Main.maxPlayers)
                return;

            Player player = Main.player[projectile.owner];
            if (projectile.DamageType == ThoriumDamageBase<HealerDamage>.Instance &&
                player.GetModPlayer<ThormwardPlayer>().accPopeCross.Active)
            {
                player.GetModPlayer<ThormwardPlayer>().IncrementPopeCrossProgress();
            }
        }
    }
}
