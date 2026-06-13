using Terraria;
using Terraria.ModLoader;

namespace HWJBardHealer.Content.Buffs
{
    public class BubbleShield : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = false;
            Main.debuff[Type] = false;
        }
    }
}