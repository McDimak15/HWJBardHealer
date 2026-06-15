using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using ThoriumMod;
using ThoriumMod.Utilities;
using ThoriumMod.Buffs.Healer;
using ThoriumMod.Projectiles.Healer;
using ThoriumMod.Items;
using System;

namespace HWJBardHealer.Core
{
    public static class HealerHelper
    {
        public static bool HealPlayerLocal(Player healer, Player target, int healAmount = 1, int recoveryTime = 0, bool healEffects = true, Action<Player> extraEffects = null, Func<Player, bool> canHealTarget = null)
        {
            if (canHealTarget != null && !canHealTarget(target))
                return false;

            if (recoveryTime > 0)
            {
                target.GetThoriumPlayer().SetLifeRecoveryEffect(LifeRecoveryEffectType.Generic, (short)recoveryTime, request: true);
                target.AddBuff(ModContent.BuffType<QuickRecovery>(), recoveryTime, true, false);
            }

            if (healEffects)
                OnHealEffects(healer, target);

            extraEffects?.Invoke(target);
            healAmount += healer.GetThoriumPlayer().healBonus;

            target.statLife += healAmount;
            if (target.statLife > target.statLifeMax2)
                target.statLife = target.statLifeMax2;

            target.HealEffect(healAmount, true);
            target.GetThoriumPlayer().mostRecentHeal = healAmount;
            target.GetThoriumPlayer().mostRecentHealer = healer.whoAmI;

            NetMessage.SendData(16, -1, -1, null, target.whoAmI);
            healer.ApplyInteractionNearbyNPCs();

            return true;
        }
        // I removed BalanceConfig for now because I cant compile mod
		public static bool HealPlayer(Player healer, Player target, int healAmount = 1, int recoveryTime = 0, bool healEffects = true, Action<Player> extraEffects = null, Func<Player, bool> canHealTarget = null) {
			if(canHealTarget != null && !canHealTarget(target)) return false;
			int type = ModContent.ProjectileType<HealNoEffects>();
			if(healEffects) type = ModContent.ProjectileType<Heal>();
			if(healer.whoAmI == Main.myPlayer) {
				int p = Projectile.NewProjectile(healer.GetSource_OnHit(target), target.Center, Vector2.Zero, type, 0, 0f, healer.whoAmI, /*ModContent.GetInstance<BalanceConfig>().healing **/ healAmount, target.whoAmI);
				NetMessage.SendData(27, -1, -1, null, p);
			}
			if(recoveryTime > 0) {
				target.GetThoriumPlayer().SetLifeRecoveryEffect(LifeRecoveryEffectType.Generic, (short)recoveryTime, true);
				target.AddBuff(ModContent.BuffType<QuickRecovery>(), recoveryTime, true, false);
			}
			if(extraEffects != null) extraEffects(target);
			return true;
		}

        public static void OnHealEffects(Player healer, Player target)
        {
            ThoriumPlayer thoriumHealer = healer.GetThoriumPlayer();
            ThoriumPlayer thoriumTarget = target.GetThoriumPlayer();
            bool selfHeal = healer.whoAmI == target.whoAmI;

            // Accessories / Set effects
            if (thoriumHealer.accForgottenCrossNecklace)
                target.AddBuff(ModContent.BuffType<ForgottenCrossNecklaceBuff>(), 900);
            if (thoriumHealer.setBlooming)
                target.AddBuff(ModContent.BuffType<BloomingSetBuff>(), 600);

            // PvP / special effects
            if (!selfHeal)
            {
                if (thoriumHealer.honeyHeart && target.statLife <= healer.statLife)
                    target.AddPVPBuff(48, 300);

                if (thoriumHealer.innerFlame.Active && thoriumHealer.LowestPlayer != healer.whoAmI && healer.whoAmI == Main.myPlayer)
                {
                    int p = Projectile.NewProjectile(healer.GetSource_Accessory(thoriumHealer.innerFlame.Item, null),
                        healer.Center.X, healer.Center.Y - 50f, 0f, 0f,
                        ModContent.ProjectileType<InnerFlamePro>(), 0, 0f, healer.whoAmI);
                    NetMessage.SendData(27, -1, -1, null, p);
                }

            }
        }
    }
}
