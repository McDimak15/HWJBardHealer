using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using ThoriumMod;
using ThoriumMod.Items;
using ThoriumMod.Items.HealerItems;
using HWJBardHealer.Content.Projectiles.Healer;
using ContinentOfJourney.Tiles;

namespace HWJBardHealer.Content.Weapons.Healer
{
    public class Serenity : ThoriumItem
    {
        public int favoritePlayer = -1;

        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true; 
        }

        public override void SetDefaults()
        {
            Item.mana = 3;
            Item.DamageType = ThoriumDamageBase<HealerTool>.Instance;
            radiantLifeCost = 5;
            isHealer = true;

            Item.width = 34;
            Item.height = 34;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.value = Item.sellPrice(0, 2, 50, 0);
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SoundID.Item24;

            Item.shoot = ModContent.ProjectileType<SerenityBlessingProj>();
            Item.shootSpeed = 0f;

            Item.noUseGraphic = false;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<Cryotherapy>(), 1);
            recipe.AddIngredient(ModContent.ItemType<MoonlightStaff>(), 1);
            recipe.AddIngredient(ModContent.ItemType<NecroticStaff>(), 1);
            recipe.AddIngredient(ModContent.ItemType<RasWhisper>(), 1);
            recipe.AddTile(ModContent.TileType<HallowedAltar>());
            recipe.Register();
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            // right click
            if (player.altFunctionUse == 2)
            {
                float radius = 120f;
                float bestDist = float.MaxValue;
                Player chosen = null;

                for (int i = 0; i < Main.maxPlayers; i++)
                {
                    Player p = Main.player[i];
                    if (p.active && !p.dead && p.team == player.team && p.whoAmI != player.whoAmI)
                    {
                        float dist = Vector2.Distance(p.Center, Main.MouseWorld);
                        if (dist < radius && dist < bestDist)
                        {
                            bestDist = dist;
                            chosen = p;
                        }
                    }
                }

                if (chosen != null)
                {
                    favoritePlayer = chosen.whoAmI;
                    if (Main.myPlayer == player.whoAmI)
                        Main.NewText($"{chosen.name} is now your favorite ally!", new Color(255, 150, 220));

                    Vector2 start = player.Center;
                    Vector2 end = chosen.Center;
                    Vector2 diff = end - start;
                    int steps = (int)(diff.Length() / 6f);
                    for (int i = 0; i < steps; i++)
                    {
                        Vector2 pos = start + diff * (i / (float)steps);
                        Dust dust = Dust.NewDustPerfect(pos, DustID.PinkTorch, Vector2.Zero, 100, Color.HotPink, 1.4f);
                        dust.noGravity = true;
                    }

                    SoundEngine.PlaySound(SoundID.Item29 with { Volume = 0.7f, Pitch = 0.2f }, player.Center);
                }
                else
                {
                    if (Main.myPlayer == player.whoAmI)
                        Main.NewText("No ally found near your cursor.", Color.Gray);
                }

                return false;
            }

            if (favoritePlayer < 0 || favoritePlayer >= Main.maxPlayers)
            {
                if (Main.myPlayer == player.whoAmI)
                    Main.NewText("You must select a favorite ally first (Right-click).", Color.Gray);
                return false;
            }

            return base.CanUseItem(player);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source,
            Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // check
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];
                if (proj.active && proj.owner == player.whoAmI && proj.type == ModContent.ProjectileType<SerenityBlessingProj>())
                    return false;
            }

            // spawn
            Projectile.NewProjectile(
                player.GetSource_ItemUse(Item),
                player.Center,
                Vector2.Zero,
                ModContent.ProjectileType<SerenityBlessingProj>(),
                0,
                0f,
                player.whoAmI
            );

            SoundEngine.PlaySound(SoundID.Item44 with { Volume = 0.8f }, player.Center);
            return false;
        }

        public override void UpdateInventory(Player player)
        {
            if (player.dead && favoritePlayer != -1)
            {
                favoritePlayer = -1;
            }
        }
    }
}
