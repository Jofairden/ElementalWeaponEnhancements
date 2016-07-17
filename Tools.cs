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
    class Tools
    {
        public static bool IsWeapon(Item item)
        {
            return (item.damage > 0 && (item.melee || item.ranged || item.magic || item.thrown || item.summon));
        }
    }
}
