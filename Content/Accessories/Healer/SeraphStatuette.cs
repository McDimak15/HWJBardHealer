using ContinentOfJourney.Items.Material;
using ContinentOfJourney.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ThoriumMod;
using ThoriumMod.Items;
using ThoriumMod.Items.HealerItems;
using ThoriumMod.Utilities;

namespace HWJBardHealer.Content.Accessories.Healer
{
    public class SeraphStatuette : ThoriumItem
    {
        public override void SetDefaults()
        {
            isHealer = true;
            Item.width = 112;
            Item.height = 58;
            Item.value = Item.sellPrice(0, 50, 0, 0);
            Item.rare = ItemRarityID.Yellow;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.statLifeMax2 += 40;
            player.statManaMax2 += 20;
            ThoriumPlayer thoriumPlayer = player.GetThoriumPlayer();
            thoriumPlayer.darkAura = true;
            thoriumPlayer.healBonus += 3;
            thoriumPlayer.ascensionStatuette = true;
            thoriumPlayer.rebirthStatuette = true;
            player.GetDamage(ThoriumDamageBase<HealerDamage>.Instance) += 0.2f;
            player.GetAttackSpeed(ThoriumDamageBase<HealerDamage>.Instance) += 0.1f;
            player.GetAttackSpeed(ThoriumDamageBase<HealerTool>.Instance) += 0.15f;
            player.GetCritChance(ThoriumDamageBase<HealerDamage>.Instance) += 12f;
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D texture = ModContent.Request<Texture2D>("HWJBardHealer/Content/Accessories/Healer/SeraphStatuette").Value;
            spriteBatch.Draw(texture, position, frame, drawColor, 0f, texture.Size() / 2f, 0.5f, 0, 0f);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<ArchangelHeart>(), 1)
                .AddIngredient(ModContent.ItemType<ArchDemonCurse>(), 1)
                .AddIngredient(ModContent.ItemType<NirvanaStatuette>(), 1)
                .AddIngredient(ModContent.ItemType<DivineShard>(), 10)
                .AddIngredient(ModContent.ItemType<PurifiedShards>(), 10)
                .AddTile(ModContent.TileType<FinalAnvil>())
                .Register();
        }
    }
}
