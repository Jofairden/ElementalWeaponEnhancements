using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace ElementalWeaponEnhancements
{
    public class ElementalWeaponEnhancements : Mod
    {
        /*
         * (c) Copyright Gorateron - Daniel Zondervan
         * Version: 0.0.1
         * Please do not use the source code without permission
         * The source code is shared only, so that other can learn from it.
         * Todo: Allow for custom element types, create secondary element types, create element resistances
         * 
         * See the forum post to see how you can alter element modifiers: 
         */

        public ElementalWeaponEnhancements()
        {
            Properties = new ModProperties()
            {
                Autoload = true,
                AutoloadGores = true,
                AutoloadSounds = true
            };
        }

        public void SetElementModifier(Player player, int? index, double? value)
        {
            if (index.HasValue && index != (int)ElementalInfo.ElementalType.Normal && value.HasValue)
            {
                player.GetModPlayer<ElementalPlayer>(this).customModifiers[index.Value] = true;
                player.GetModPlayer<ElementalPlayer>(this).customModifierValue[index.Value] = (float)value.Value;
            }
        }

        public override object Call(params object[] args)
        {
            string message = args[0] as string;
            if (message == "SetElementModifier")
            {
                SetElementModifier(args[1] as Player, args[2] as int?, args[3] as double?);
            }
            return null;
        }
    }
}
