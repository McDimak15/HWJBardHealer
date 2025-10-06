using Terraria;
using Terraria.ModLoader;

namespace HWJBardHealer.Content.Buffs
{
    public class SpaceBarrierBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = false;
            Main.debuff[Type] = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.statDefense += 5;
            player.GetDamage(DamageClass.Generic) += 0.10f;
        }
    }
}
