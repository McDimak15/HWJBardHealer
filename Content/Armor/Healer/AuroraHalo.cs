using ContinentOfJourney.Items.Armor;
using ContinentOfJourney.Items.Material;
using ContinentOfJourney.Tiles;
using HWJBardHealer.Content.Projectiles.Healer;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using ThoriumMod;

namespace HWJBardHealer.Content.Armor.Healer
{
    [AutoloadEquip(EquipType.Head)]
    public class AuroraHalo : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            ArmorIDs.Head.Sets.DrawFullHair[Item.headSlot] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 24;
            Item.value = Item.sellPrice(0, 15, 0, 0);
            Item.rare = ItemRarityID.Red;
            Item.defense = 30;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(ThoriumDamageBase<HealerDamage>.Instance) += 0.45f;
            player.GetCritChance(ThoriumDamageBase<HealerDamage>.Instance) += 10f;
            player.GetDamage(DamageClass.Generic) -= 0.30f;
            player.statLifeMax2 += 40;
            player.lifeRegen += 5;
            player.manaRegen += 5;
            player.GetModPlayer<ThoriumPlayer>().healBonus += 10;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<AuroraRobe>() && legs.type == ModContent.ItemType<AuroraBoots>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = Language.GetTextValue("Mods.HWJBardHealer.ArmorSetBonuses.Healer_Aurora");
            Lighting.AddLight(player.Center, 0.8f, 0.8f, 0.8f);

            if (player.whoAmI == Main.myPlayer && player.ownedProjectileCounts[ModContent.ProjectileType<AuroraHaloSlime>()] <= 0)
            {
                Projectile.NewProjectile(player.GetSource_Accessory(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<AuroraHaloSlime>(), 0, 0f, player.whoAmI);
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<SunlightGel>(), 8)
                .AddIngredient(ModContent.ItemType<SolarFlareScoria>(), 6)
                .AddTile(ModContent.TileType<FinalAnvil>())
                .Register();
        }
    }
}