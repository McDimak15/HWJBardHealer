using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.GameContent.ItemDropRules;
using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using ThoriumMod;
using ThoriumMod.Items;
using ThoriumMod.Items.HealerItems;
using HWJBardHealer.Content.Projectiles.Healer;
using HWJBardHealer.Content.Buffs;
using ContinentOfJourney.Items;


namespace HWJBardHealer.Content.Weapons.Healer
{
    public class OtherworldlyScythe : ScytheItem
    {
        public override void SetStaticDefaults()
        {
            SetStaticDefaultsToScythe();
        }

        public override void SetDefaults()
        {
            SetDefaultsToScythe();

            Item.damage = 125;
            scytheSoulCharge = 3;
            Item.width = 180;
            Item.height = 180;
            Item.value = CalamityGlobalItem.RarityHotPinkBuyPrice;
            Item.rare = ModContent.RarityType<HotPink>();
            Item.shoot = ModContent.ProjectileType<OtherworldlyScythePro>();
            Item.autoReuse = true;
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source,
            Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, player.Center, velocity, type, damage, knockback, player.whoAmI);
            return false;
        }
    }

    // bubble collection
    public class OtherworldlyPlayer : ModPlayer
    {
        private int hitCounter = 0;
        public int collectedBubbles = 0;

        public void OnScytheHit()
        {
            hitCounter++;

            if (hitCounter >= 8)
            {
                hitCounter = 0;
                CollectBubble();
            }
        }

        private void CollectBubble()
        {
            // Dont spawn if bubbles are already active
            int activeBubbles = 0;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].active && Main.projectile[i].type == ModContent.ProjectileType<OtherworldlyBubble>() && Main.projectile[i].owner == Player.whoAmI)
                    activeBubbles++;
            }
            if (activeBubbles > 0) return;

            // Spawn 3 bubbles in random directions
            for (int i = 0; i < 3; i++)
            {
                float angle = Main.rand.NextFloat(MathHelper.TwoPi);
                float distance = Main.rand.Next(120, 160); // radius
                Vector2 spawnPos = Player.Center + angle.ToRotationVector2() * distance;

                Projectile.NewProjectile(
                    Player.GetSource_FromThis(),
                    spawnPos,
                    Vector2.Zero,
                    ModContent.ProjectileType<OtherworldlyBubble>(),
                    0,
                    0,
                    Player.whoAmI
                );
            }

            // FX
            for (int i = 0; i < 15; i++)
            {
                Dust.NewDust(Player.Center, 20, 20, DustID.Water,
                    Main.rand.NextFloat(-2f, 2f),
                    Main.rand.NextFloat(-2f, 2f),
                    150, default, 1.3f);
            }
            Terraria.Audio.SoundEngine.PlaySound(SoundID.Item85, Player.Center);
        }




        // Damage reduction
        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            if (Player.HasBuff(ModContent.BuffType<BubbleShield>()))
            {
                modifiers.FinalDamage *= 0.9f;
                Player.ClearBuff(ModContent.BuffType<BubbleShield>());

                // FX
                for (int i = 0; i < 20; i++)
                {
                    Dust.NewDust(Player.Center, 10, 10, DustID.Water,
                        Main.rand.NextFloat(-2f, 2f),
                        Main.rand.NextFloat(-2f, 2f));
                }
                Terraria.Audio.SoundEngine.PlaySound(SoundID.Item54, Player.Center);
            }
        }
    }

    public class OtherworldlyScytheDrop : GlobalItem
    {
        public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
        {
            if (item.type == ModContent.ItemType<DiverTreasureBag>())
            {
                itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<OtherworldlyScythe>(), 1));
            }
        }
    }
}
