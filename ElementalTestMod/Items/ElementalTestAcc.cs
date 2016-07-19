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
            item.toolTip = "Increases all elemental damage by 15%\nand our own elements by another 15%";
            item.value = Item.buyPrice(5, 0, 0, 0);
            item.rare = 5;
            item.accessory = true;
        }

        // Increase all elemental damage by 15%, including custom elements
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            try
            {
                Mod eleMod = ModLoader.GetMod("ElementalWeaponEnhancements");
                if (eleMod != null)
                {
                    // This increases all elements damage by 15%, including custom elements (from other mods, and yours)
                    for (int i = 0; i < (int)eleMod.Call("CountElements", null); i++)
                    {
                        eleMod.Call("AlterElementModifier", eleMod.Call("GetElementMod", i) as Mod, player, eleMod.Call("GetElementName", i) as string, 0.15);
                    }

                    // Add another 15% to our elements
                    var myElements = eleMod.Call("GetModElements", mod) as List<int>;
                    //ErrorLogger.Log(myElements.ToString());
                    if (myElements != null && myElements.Any())
                    {
                        foreach (var item in myElements)
                        {
                            if (item != -1)
                                eleMod.Call("AlterElementModifier", mod, player, eleMod.Call("GetElementName", item) as string, 0.15);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ErrorLogger.Log(e.ToString());
            }
        }
    }
}
