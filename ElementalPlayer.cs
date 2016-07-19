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
    class ElementalPlayer : ModPlayer
    {
        public List<float> elementDamage;

        // Copy element modifiers
        public override void Initialize()
        {
            elementDamage = new List<float>(ElementalWeaponEnhancements.elementModifiers);
        }
        
        // Reset modifiers to default
        public override void ResetEffects()
        {
            if (elementDamage.Any())
            {
                for (int i = 0; i < elementDamage.Count; i++)
                {
                    elementDamage[i] = 1f;
                }
            }
        }

    }
}
