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
        public float[] elementalDamage =
        {
            1f, // no element
            1f,
            1f,
            1f,
            1f
        };

        public float[] customModifierValue =
        {
            0f,
            0f,
            0f,
            0f,
            0f
        };

        public bool[] customModifiers =
        {
            false,
            false,
            false,
            false,
            false
        };

        public override void ResetEffects()
        {
            for (int i = 0; i < elementalDamage.Length; i++)
            {
                elementalDamage[i] = 1f;
            }
            for (int i = 0; i < customModifierValue.Length; i++)
            {
                customModifierValue[i] = 0f;
            }
            for (int i = 0; i < customModifiers.Length; i++)
            {
                customModifiers[i] = false;
            }
        }

        public override void UpdateDead()
        {
            for (int i = 0; i < elementalDamage.Length; i++)
            {
                elementalDamage[i] = 1f;
            }
            for (int i = 0; i < customModifierValue.Length; i++)
            {
                customModifierValue[i] = 0f;
            }
            for (int i = 0; i < customModifiers.Length; i++)
            {
                customModifiers[i] = false;
            }
        }

        public override void PostUpdateEquips()
        {
            for (int i = 0; i < customModifiers.Length; i++)
            {
                if (customModifiers[i])
                {
                    elementalDamage[i] += customModifierValue[i];
                }
            }
        }
    }
}
