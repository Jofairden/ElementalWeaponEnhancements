using Microsoft.Xna.Framework;
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
         * Version: 0.0.3
         * Please do not use the source code without permission
         * The source code is shared only, so that other can learn from it.
         * Todo: implement resistances
         * See the forum post to see how you can alter element modifiers, or add your own elements:
         *  http://forums.terraria.org/index.php?threads/elemental-weapon-enhancements.46677/
         */

        // Elemental Data
        public static List<Tuple<Mod, string, string, Color>> elementData = new List<Tuple<Mod, string, string, Color>>();
        public static List<float> elementModifiers = new List<float>();
        public static List<Tuple<int, double>> elementNPCResistances = new List<Tuple<int, double>>();
        public static List<Tuple<int, double>> elementPlayerResistances = new List<Tuple<int, double>>();

        // Add elemental data
        public void AddElementData(Mod mod, string name, Color useColor)
        {
            elementData.Add(new Tuple<Mod, string, string, Color>(mod, name, mod.Name + "::" + name, useColor));
            elementModifiers.Add(1f);
            ErrorLogger.Log("Added element: " + elementData[elementData.Count - 1].ToString());
        }

        // Unload element data
        public void UnloadElementData(string signature)
        {
            var elementList = elementData.FindIndex(x => x.Item3 == signature);
            if ((int)elementList != -1)
            {
                elementData.RemoveAt((int)elementList);
                ErrorLogger.Log("Unloaded element with signature " + signature);
            }
        }

        // Change element modifiers
        public void AlterElementModifier(Player player, string signature, double? modifier)
        {
            var elementList = elementData.FindIndex(x => x.Item3 == signature);
            if ((int)elementList != -1)
            {
                player.GetModPlayer<ElementalPlayer>(this).elementDamage[elementList] += (float)modifier.Value;
                //ErrorLogger.Log("Altered modifier for signature " + signature + " with " + modifier.ToString());
            }
        }

        public string CreateIdentifier(Mod mod, string name)
        {
            return mod.Name + "::" + name;
        }

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
            if (elementData.Any())
                elementData.Clear();
            if (elementModifiers.Any())
                elementModifiers.Clear();

            AddElementData(this, "Earth", Color.Sienna);
            AddElementData(this, "Water", Color.DodgerBlue);
            AddElementData(this, "Air", Color.Cyan);
            AddElementData(this, "Fire", Color.Crimson);
        }

        public override void Unload()
        {
            UnloadElementData(CreateIdentifier(this, "Earth"));
            UnloadElementData(CreateIdentifier(this, "Water"));
            UnloadElementData(CreateIdentifier(this, "Air"));
            UnloadElementData(CreateIdentifier(this, "Fire"));
        }

        public override void PostSetupContent()
        {
            //ErrorLogger.ClearLog();
            this.Call("LogElements", null);
        }

        // Handle other mod's requests
        public override object Call(params object[] args)
        {
            try
            {
                string message = args[0] as string;

                if (message == "CreateElement")
                {
                    Mod elementMod = args[1] as Mod;
                    string elementName = args[2] as string;
                    Color? elementColor = args[3] as Color?;

                    if (elementMod == null || elementName.Length == 0 || elementName == null || !elementColor.HasValue)
                        return null;

                    // If there is no element with the same identifier, continue
                    var elementList = elementData.TakeWhile(x => x.Item3 == CreateIdentifier(elementMod, elementName)).ToList();
                    if (!elementList.Any())
                    {
                        AddElementData(elementMod, elementName, elementColor.Value);
                    }
                }
                else if (message == "LogElements")
                {
                    // Log a mod's elements, or all elements
                    Mod elementMod = args[1] as Mod;
                    foreach (var item in elementData)
                    {
                        if (elementMod == null)
                            ErrorLogger.Log(item.ToString());
                        else if (item.Item1 == elementMod)
                            ErrorLogger.Log(item.ToString());
                    }
                }
                else if (message == "UnloadElement")
                {
                    // Unload a mod's element
                    Mod elementMod = args[1] as Mod;
                    string elementName = args[2] as string;

                    if (elementMod == null || elementName.Length == 0 || elementName == null)
                        return null;

                    // If there is an element with the same signature, unload it
                    var elementList = elementData.FindIndex(x => x.Item3 == CreateIdentifier(elementMod, elementName));
                    if (elementList != -1)
                    {
                        UnloadElementData(CreateIdentifier(elementMod, elementName));
                    }
                }
                else if (message == "AlterElementModifier")
                {
                    Mod elementMod = args[1] as Mod;
                    Player elementPlayer = args[2] as Player;
                    string elementName = args[3] as string;
                    double? elementModifier = args[4] as double?;

                    if (elementMod == null || elementPlayer == null || elementName.Length == 0 || elementName == null || !elementModifier.HasValue)
                        return null;

                    // If there is an element with the same signature, alter that modifier
                    var elementList = elementData.FindIndex(x => x.Item3 == CreateIdentifier(elementMod, elementName));
                    if ((int)elementList != -1)
                    {
                        AlterElementModifier(elementPlayer, CreateIdentifier(elementMod, elementName), elementModifier);
                    }
                }
                else if (message == "GetElementName")
                {
                    int? elementType = args[1] as int?;
                    if (!elementType.HasValue)
                        return null;
                    var elementList = elementData.ElementAtOrDefault(elementType.Value);
                    if (elementList == new Tuple<Mod, string, string, Color>(null, "", "", default(Color)) || elementList == new Tuple<Mod, string, string, Color>(null, null, null, default(Color)))
                    {
                        return null;
                    }
                    else
                    {
                        return elementData[elementType.Value].Item2;
                    }
                }
                else if (message == "GetElementMod")
                {
                    int? elementType = args[1] as int?;
                    if (!elementType.HasValue)
                        return null;
                    var elementList = elementData.ElementAtOrDefault(elementType.Value);
                    if (elementList == new Tuple<Mod, string, string, Color>(null, "", "", default(Color)) || elementList == new Tuple<Mod, string, string, Color>(null, null, null, default(Color)))
                    {
                        return null;
                    }
                    else
                    {
                        return elementData[elementType.Value].Item1;
                    }
                }
                else if (message == "CountElements")
                {
                    Mod elementMod = args[1] as Mod;
                    if (elementMod == null)
                        return elementData.Count;
                    else
                    {
                        var elementList = elementData.TakeWhile(x => x.Item1 == elementMod).ToList();
                        return elementList.Count;
                    }
                }

                return null;
            }
            catch (Exception e)
            {
                ErrorLogger.Log(e.ToString());
                return null;
            }
        }
    }
}
