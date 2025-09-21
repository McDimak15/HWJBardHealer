using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ModLoader;
using ThoriumMod.Items.HealerItems;
using ContinentOfJourney.Items.Material;
using ContinentOfJourney.Tiles;

namespace TestMod;

public class ForbiddenGrowth : ScytheItem
{
    private int swingDirection;

    internal int spin;

    public override void SetStaticDefaults()
    {
        SetStaticDefaultsToScythe();
        ItemID.Sets.SkipsInitialUseSound[Item.type] = true;
    }

    public override void SetDefaults()
    {
        SetDefaultsToScythe();
        Item.damage = 77;
        scytheSoulCharge = 3;
        Item.width = 136;
        Item.height = 156;
        Item.rare = ItemRarityID.Cyan;
        Item.holdStyle = 6;
        Item.useStyle = 100;
        Item.noUseGraphic = false;
        Item.useTime = 24;
        Item.useAnimation = 24;
        Item.shoot = ModContent.ProjectileType<GrowthSwing>();
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        if (this.swingDirection != -1 && this.swingDirection != 1)
        {
            this.swingDirection = 1;
        }
        if (Main.myPlayer == player.whoAmI)
        {
            float attackTime = ((player.itemAnimationMax <= 0) ? Item.useAnimation : ((player.itemAnimationMax > player.itemTimeMax) ? player.itemTimeMax : player.itemAnimationMax));
            int z = Projectile.NewProjectile(source, position, Vector2.Normalize(Main.MouseWorld - player.MountedCenter), type, damage, knockback, player.whoAmI, attackTime, attackTime, player.GetAdjustedItemScale(Item));
            Main.projectile[z].direction = this.swingDirection;
            NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, z);
        }
        this.swingDirection = -this.swingDirection;

        this.spin = 1;

        return false;
    }

    public override void HoldStyle(Player player, Rectangle itemFrame)
    {
        player.itemLocation += new Vector2(-60f, 25f) * player.Directions;
    }

    public override void UseStyle(Player player, Rectangle itemFrame)
    {
        player.itemLocation = Vector2.Zero;
    }

    public override Color? GetAlpha(Color lightColor)
    {
        return Color.White;
    }

    public override bool MeleePrefix()
    {
        return true;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ModContent.ItemType<EssenceofLife>(), 2)
            .AddIngredient(ModContent.ItemType<LivingBar>(), 12)
            .AddTile(ModContent.TileType<FountainofLife>())
            .Register();
    }
}