using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace ElementalWeaponEnhancements.Items
{
    class ChronicShifter : ModItem
    {
        public override void SetDefaults()
        {
            item.name = "Chronic Shifter";
            item.toolTip = "This ancient medallion glows with energy";
            item.value = Item.buyPrice(0, 5);
            item.width = 50;
            item.height = 50;
            item.rare = 6;
            item.maxStack = 1;
        }

        public override bool CanRightClick()
        {
            return true;
        }

        public override void RightClick(Player player)
        {
            item.stack = 2;

            ElementalInfo info = player.inventory[0].GetModInfo<ElementalInfo>(mod);

            if (player.inventory[0].type != 0 && info.enhanced)
            {
                info.RollPrimaryDamage();
            }
        }
    }
}
