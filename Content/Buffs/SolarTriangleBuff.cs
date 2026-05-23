using Terraria;
using Terraria.ModLoader;

namespace HWJBardHealer.Content.Buffs
{
    public class SolarTriangleBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = false;
            Main.buffNoTimeDisplay[Type] = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.lifeRegen += 6;
        }
    }
}
