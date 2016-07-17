using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace ElementalTestMod.Items
{
    class ElementalTestAcc : ModItem
    {
        public override void SetDefaults()
        {
            item.name = "Test Acc";
            item.width = 28;
            item.height = 28;
            item.toolTip = "Increases all elemental damage by 15%\nand our custom damage by another 15%";
            item.value = Item.buyPrice(5, 0, 0, 0);
            item.rare = 5;
            item.accessory = true;
        }

        // Increase all elemental damage by 15%, including custom elements
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            Mod eweMod = ModLoader.GetMod("ElementalWeaponEnhancements");
            if (eweMod != null)
            {
                // This increases all elements damage by 15%, including custom elements (from other mods, and yours)
                for (int i = 0; i < (int)eweMod.Call("CountElements", null); i++)
                {
                    eweMod.Call("AlterElementModifier", eweMod.Call("GetElementMod", i) as Mod, player, eweMod.Call("GetElementName", i) as string, 0.15);
                }

                // This code adds another 15% to our own custom elements
                for (int i = 0; i < 3; i++)
                {
                    string name;
                    switch (i)
                    {
                        case 1:
                            name = "Void";
                            break;
                        case 2:
                            name = "Dank";
                            break;
                        case 0:
                        default:
                            name = "Aether";
                            break;
                    }
                    eweMod.Call("AlterElementModifier", mod, player, name, 0.15);
                }
            }
        }
    }
}
