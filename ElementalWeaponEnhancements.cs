using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ElementalWeaponEnhancements
{

    public class ElementalWeaponEnhancements : Mod
    {
        /*
        * (c) Copyright Gorateron - Daniel Zondervan
        * Version: 0.0.5
        * Please do not use the source code without permission in another mod
        * The source code is shared only, so that others can learn from it.
        * See the GH wiki or TCF forum post for more information and guidelines:
        * http://forums.terraria.org/index.php?threads/elemental-weapon-enhancements.46677/
        * https://github.com/gorateron/ElementalWeaponEnhancements/wiki
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

        public override void Load()
        {
            ElementalFramework.ClearData();
            // Create Vanilla elements
            //AddElementData(this, "Earth", Color.Sienna);
            //AddElementData(this, "Water", Color.DodgerBlue);
            //AddElementData(this, "Air", Color.Cyan);
            //AddElementData(this, "Fire", Color.Crimson);
        }

        public override void Unload()
        {
            // Unload Vanilla elements

            //UnloadElementData(CreateIdentifier(this, "Earth"));
            //UnloadElementData(CreateIdentifier(this, "Water"));
            //UnloadElementData(CreateIdentifier(this, "Air"));
            //UnloadElementData(CreateIdentifier(this, "Fire"));
        }

        public override void PostSetupContent()
        {
            foreach (var item in ElementalFramework.Data.elementData)
            {
                if (item != null)
                    ErrorLogger.Log(item.ToString());
            }

            foreach (var item in ElementalFramework.Data.elementOnHit)
            {
                if (item != null)
                    ErrorLogger.Log(item.ToString());
            }

            foreach (var item in ElementalFramework.Data.elementModifyHit)
            {
                if (item != null)
                    ErrorLogger.Log(item.ToString());
            }

        }
    }
}
