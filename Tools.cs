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

        public static bool HasCheatSheet()
        {
            return (ModLoader.GetMod("CheatSheet") != null);
        }

        public static bool HasHerosMod()
        {
            return (ModLoader.GetMod("HEROsMod") != null);
        }

        public static Mod GetCheatSheet()
        {
            return (HasCheatSheet()) ? ModLoader.GetMod("CheatSheet") : null;
        }

        public static Mod GetHerosMod()
        {
            return (HasCheatSheet()) ? ModLoader.GetMod("HEROsMod") : null;
        }
    }
}
