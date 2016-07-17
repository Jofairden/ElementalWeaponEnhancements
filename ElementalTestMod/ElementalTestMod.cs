using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace ElementalTestMod
{
    public class ElementalTestMod : Mod
    {
        public ElementalTestMod()
        {
            Properties = new ModProperties()
            {
                Autoload = true,
                AutoloadGores = true,
                AutoloadSounds = true
            };
        }

        // Add our custom elements
        public override void Load()
        {
            Mod eweMod = ModLoader.GetMod("ElementalWeaponEnhancements");
            if (eweMod != null)
            {
                eweMod.Call("CreateElement", this, "Aether", Color.Aquamarine);
                eweMod.Call("CreateElement", this, "Void", Color.DarkGray);
                eweMod.Call("CreateElement", this, "Dank", Color.Green);
            }
        }
        
        // Unload our custom elements
        public override void Unload()
        {
            Mod eweMod = ModLoader.GetMod("ElementalWeaponEnhancements");
            if (eweMod != null)
            {
                eweMod.Call("UnloadElement", this, "Aether");
                eweMod.Call("UnloadElement", this, "Void");
                eweMod.Call("UnloadElement", this, "Dank");
            }
        }
    }
}
