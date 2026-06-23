using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using ThoriumMod;
using ThoriumMod.Utilities;

namespace HWJBardHealer.Content.Consumables
{
    public class ConsumablePLayer : ModPlayer
    {
        public bool usedTrinote = false;
        public bool usedUmbra = false;

        public override void Initialize()
        {
            usedTrinote = false;
            usedUmbra = false;
        }

        public override void UpdateEquips()
        {
            ThoriumPlayer thoriumPlayer = Player.GetThoriumPlayer();
            if (usedTrinote)
            {
                thoriumPlayer.inspirationRegenBonus += 0.1f;
            }
            if (usedUmbra)
            {
                thoriumPlayer.techPointsMax++;
            }
        }

        public override void SaveData(TagCompound tag)
        {
            if (usedTrinote)
                tag["UsedTriNote"] = true;

            if (usedTrinote)
                tag["UsedUmbra"] = true;
        }

        public override void LoadData(TagCompound tag)
        {
            usedTrinote = tag.ContainsKey("UsedTriNote") && tag.GetBool("UsedTriNote");
            usedUmbra = tag.ContainsKey("UsedUmbra") && tag.GetBool("UsedUmbra");
        }
    }
}