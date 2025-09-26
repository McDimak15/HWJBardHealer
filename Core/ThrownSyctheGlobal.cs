using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace WHummusMultiModBalancing.Content.ItemChanges.ThrowableScythes
{
    [ExtendsFromMod("ThoriumMod")]
    public class ThrowableScythes : GlobalItem
    {
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            List<ModItem> throwableScythes = new();

            if (ModLoader.TryGetMod("HWJBardHealer", out Mod HWJbardhealer))
            {
                if (entity.type == HWJbardhealer.Find<ModItem>("OtherworldlyScythe").Type) return true;
                if (entity.type == HWJbardhealer.Find<ModItem>("RodScythe").Type) return true;
            }

            foreach (var item in throwableScythes)
            {
                if (item != null && entity.type == item.Type)
                    return true;
            }
            return false;
        }

        public override bool InstancePerEntity => true;

        public float ThrowDistance = 180f; //base throw distance

        private void SetCustomThrowDistance(Item item)
        {
            if (ModLoader.TryGetMod("HWJBardHealer", out Mod HWJbardhealer))
            {
                if (item.type == HWJbardhealer.Find<ModItem>("OtherworldlyScythe").Type) ThrowDistance = 85f;
                if (item.type == HWJbardhealer.Find<ModItem>("RodScythe").Type) ThrowDistance = 125f;
            }
        }

        public override bool AltFunctionUse(Item item, Player player)
        {
            return true;
        }

        public override void SetDefaults(Item entity)
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[entity.type] = true;
        }

        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            SetCustomThrowDistance(item);

            if (Main.myPlayer == player.whoAmI)
            {
                int projIndex = -1;

                if (player.altFunctionUse == 2)
                {
                    Vector2 throwVel = Vector2.Normalize(Main.MouseWorld - player.MountedCenter) * -ThrowDistance;

                    projIndex = Projectile.NewProjectile(
                        source,
                        position,
                        throwVel,
                        type,
                        damage - damage / 10,
                        knockback,
                        player.whoAmI,
                        (Main.rand.Next(2, 5) + 1) * 0.1f, // ai[0]
                        player.itemTime // ai[1]
                    );

                    if (projIndex >= 0)
                        NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, projIndex);

                    return false;
                }
            }
            return base.Shoot(item, player, source, position, velocity, type, damage, knockback);
        }

        public override float UseTimeMultiplier(Item item, Player player)
        {
            return player.altFunctionUse == 2 ? 2f : 1f;
        }

        public override float UseAnimationMultiplier(Item item, Player player)
        {
            return player.altFunctionUse == 2 ? 2f : 1f;
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            bool isThrowableScythe = false;

            if (ModLoader.TryGetMod("HWJBardHealer", out Mod HWJbardhealer))
            {
                if (item.type == HWJbardhealer.Find<ModItem>("OtherworldlyScythe").Type) isThrowableScythe = true;
                if (item.type == HWJbardhealer.Find<ModItem>("RodScythe").Type) isThrowableScythe = true;
            }

            if (isThrowableScythe)
            {
                tooltips.Add(new TooltipLine(Mod, "ScytheThrow",
                    Terraria.Localization.Language.GetTextValue("Mods.HWJBardHealer.ItemTooltips.ScytheThrow")));
            }
        }
    }
}
