using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace ElementalWeaponEnhancements
{
    class ElementalNPC : GlobalNPC
    {
        public override void SetupShop(int type, Chest shop, ref int nextSlot)
        {
            if (type == NPCID.Merchant && NPC.downedBoss2)
            {
                shop.item[nextSlot].SetDefaults(mod.ItemType("ElementShifter"));
                nextSlot++;
                shop.item[nextSlot].SetDefaults(mod.ItemType("ChronicShifter"));
                nextSlot++;
            }
        }
    }
}
