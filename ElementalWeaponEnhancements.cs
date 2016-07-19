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
        * Version: 0.0.4
        * Please do not use the source code without permission in another mod
        * The source code is shared only, so that others can learn from it.
        * Todo: implement npc resistances
        * See the GH wiki or TCF forum post for more information and guidelines:
        * http://forums.terraria.org/index.php?threads/elemental-weapon-enhancements.46677/
        * https://github.com/gorateron/ElementalWeaponEnhancements/wiki
        */

        // Elemental Data
        public static List<Tuple<Mod, string, string, Color>> elementData = new List<Tuple<Mod, string, string, Color>>();
        public static List<float> elementModifiers = new List<float>();

        // Npc data
        public static List<Tuple<int, double>> elementNPCResistances = new List<Tuple<int, double>>();
        public static List<Tuple<int, double>> elementPlayerResistances = new List<Tuple<int, double>>();

        //public delegate void OnHitNPC(Item item, Player player, NPC target, int damage, float knockBack, bool crit);
        //public delegate void OnHitPVP(Item item, Player player, Player target, int damage, bool crit);

        // Item on hits
        internal static List<Action<Item, Player, NPC, int, float, bool>> elementOnHitNPC = new List<Action<Item, Player, NPC, int, float, bool>>();
        internal static List<Action<Item, Player, Player, int, bool>> elementOnHitPVP = new List<Action<Item, Player, Player, int, bool>>();

        // Item modify on hits
        internal static List<Func<Item, Player, NPC, int, float, bool, object[]>> elementModifyHitNPC = new List<Func<Item, Player, NPC, int, float, bool, object[]>>();
        internal static List<Func<Item, Player, Player, int, bool, object[]>> elementModifyHitPVP = new List<Func<Item, Player, Player, int, bool, object[]>>();

        // Projectile on hits
        internal static List<Action<Projectile, NPC, int, float, bool>> elementProjectileOnHitNPC = new List<Action<Projectile, NPC, int, float, bool>>();
        internal static List<Action<Projectile, Player, int, bool>> elementProjectileOnHitPVP = new List<Action<Projectile, Player, int, bool>>();

        // Projectile modify on hits
        internal static List<Func<Projectile, NPC, int, float, bool, object[]>> elementProjectileModifyHitNPC = new List<Func<Projectile, NPC, int, float, bool, object[]>>();
        internal static List<Func<Projectile, Player, int, bool, object[]>> elementProjectileModifyHitPVP = new List<Func<Projectile, Player, int, bool, object[]>>();

        // Add elemental data
        public void AddElementData(Mod mod, string name, Color useColor, Action<Item, Player, NPC, int, float, bool> _behaviour = null, Action<Item, Player, Player, int, bool> _behaviourPVP = null, Func<Item, Player, NPC, int, float, bool, object[]> _modifyHitNPC = null, Func<Item, Player, Player, int, bool, object[]> _modifyHitPVP = null, Action<Projectile, NPC, int, float, bool> _projectileOnHitNPC = null, Action<Projectile, Player, int, bool> _projectileOnHitPVP = null, Func<Projectile, NPC, int, float, bool, object[]> _projectileModifyHitNPC = null, Func<Projectile, Player, int, bool, object[]> _projectileModifyHitPVP = null)
        {
            elementData.Add(new Tuple<Mod, string, string, Color>(mod, name, mod.Name + "::" + name, useColor));
            elementModifiers.Add(1f);
            elementOnHitNPC.Add(_behaviour);
            elementOnHitPVP.Add(_behaviourPVP);
            elementModifyHitNPC.Add(_modifyHitNPC);
            elementModifyHitPVP.Add(_modifyHitPVP);
            elementProjectileOnHitNPC.Add(_projectileOnHitNPC);
            elementProjectileOnHitPVP.Add(_projectileOnHitPVP);
            elementProjectileModifyHitNPC.Add(_projectileModifyHitNPC);
            elementProjectileModifyHitPVP.Add(_projectileModifyHitPVP);
            ErrorLogger.Log("Added element: " + elementData[elementData.Count - 1].ToString());
        }

        // Unload element data
        public void UnloadElementData(string signature)
        {
            var elementList = elementData.FindIndex(x => x.Item3 == signature);
            if ((int)elementList != -1)
            {
                elementData.RemoveAt((int)elementList);
                elementModifiers.RemoveAt((int)elementList);
                elementOnHitNPC.RemoveAt((int)elementList);
                elementOnHitPVP.RemoveAt((int)elementList);
                elementModifyHitNPC.RemoveAt((int)elementList);
                elementModifyHitPVP.RemoveAt((int)elementList);
                elementProjectileOnHitNPC.RemoveAt((int)elementList);
                elementProjectileOnHitPVP.RemoveAt((int)elementList);
                elementProjectileModifyHitNPC.RemoveAt((int)elementList);
                elementProjectileModifyHitPVP.RemoveAt((int)elementList);
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
            if (elementOnHitNPC.Any())
                elementOnHitNPC.Clear();
            if (elementOnHitPVP.Any())
                elementOnHitPVP.Clear();
            if (elementModifyHitNPC.Any())
                elementModifyHitNPC.Clear();
            if (elementModifyHitPVP.Any())
                elementModifyHitPVP.Clear();
            if (elementProjectileOnHitNPC.Any())
                elementProjectileOnHitNPC.Clear();
            if (elementProjectileOnHitPVP.Any())
                elementProjectileOnHitPVP.Clear();
            if (elementProjectileModifyHitNPC.Any())
                elementProjectileModifyHitNPC.Clear();
            if (elementProjectileModifyHitPVP.Any())
                elementProjectileModifyHitPVP.Clear();

            // Create Vanilla elements
            AddElementData(this, "Earth", Color.Sienna);
            AddElementData(this, "Water", Color.DodgerBlue);
            AddElementData(this, "Air", Color.Cyan);
            AddElementData(this, "Fire", Color.Crimson);
        }

        public override void Unload()
        {
            // Unload Vanilla elements

            UnloadElementData(CreateIdentifier(this, "Earth"));
            UnloadElementData(CreateIdentifier(this, "Water"));
            UnloadElementData(CreateIdentifier(this, "Air"));
            UnloadElementData(CreateIdentifier(this, "Fire"));
        }

        public override void PostSetupContent()
        {
            //ErrorLogger.ClearLog();
            //this.Call("LogElements", null);

            // Log our data
            if (elementOnHitNPC.Any())
            {
                foreach (var item in elementOnHitNPC)
                {
                    if (item != null)
                        ErrorLogger.Log(item.ToString());
                }
            }

            if (elementOnHitPVP.Any())
            {
                foreach (var item in elementOnHitPVP)
                {
                    if (item != null)
                        ErrorLogger.Log(item.ToString());
                }
            }

            if (elementModifyHitNPC.Any())
            {
                foreach (var item in elementModifyHitNPC)
                {
                    if (item != null)
                        ErrorLogger.Log(item.ToString());
                }
            }

            if (elementModifyHitPVP.Any())
            {
                foreach (var item in elementModifyHitPVP)
                {
                    if (item != null)
                        ErrorLogger.Log(item.ToString());
                }
            }

            if (elementProjectileOnHitNPC.Any())
            {
                foreach (var item in elementProjectileOnHitNPC)
                {
                    if (item != null)
                        ErrorLogger.Log(item.ToString());
                }
            }

            if (elementProjectileOnHitPVP.Any())
            {
                foreach (var item in elementProjectileOnHitPVP)
                {
                    if (item != null)
                        ErrorLogger.Log(item.ToString());
                }
            }

            if (elementProjectileModifyHitNPC.Any())
            {
                foreach (var item in elementProjectileModifyHitNPC)
                {
                    if (item != null)
                        ErrorLogger.Log(item.ToString());
                }
            }

            if (elementProjectileModifyHitPVP.Any())
            {
                foreach (var item in elementProjectileModifyHitPVP)
                {
                    if (item != null)
                        ErrorLogger.Log(item.ToString());
                }
            }
        }

        // Handle other mod's requests
        public override object Call(params object[] args)
        {
            try
            {
                // Make sure our expected values are null if none, and we have the expected amount of arguments
                object[] newArgs = new object[12]; // change 12 on max number of args changes
                for (int i = 0; i < newArgs.Length; i++)
                {
                    newArgs[i] = null; 
                }
                for (int i = 0; i < args.Length; i++)
                {
                    newArgs[i] = args[i]; // " clone " args
                }

                string message = newArgs[0] as string;
                message = message.ToLower();

                if (message == "CreateElement".ToLower())
                {
                    // Create an element with specified data

                    Mod elementMod = newArgs[1] as Mod;
                    string elementName = newArgs[2] as string;
                    Color? elementColor = newArgs[3] as Color?;
                    Action<Item, Player, NPC, int, float, bool> npcBehaviour = newArgs[4] != null ? (Action<Item, Player, NPC, int, float, bool>)newArgs[4] : null;
                    Action<Item, Player, Player, int, bool> pvpBehaviour = newArgs[5] != null ? (Action<Item, Player, Player, int, bool>)newArgs[5] : null;
                    Func<Item, Player, NPC, int, float, bool, object[]> modifyHitNPC = newArgs[6] != null ? (Func<Item, Player, NPC, int, float, bool, object[]>)newArgs[6] : null;
                    Func<Item, Player, Player, int, bool, object[]> modifyHitPVP = newArgs[7]  != null ? (Func<Item, Player, Player, int, bool, object[]>)newArgs[7] : null;
                    Action<Projectile, NPC, int, float, bool> ProjectileOnHitNPC = newArgs[8] != null ? (Action<Projectile, NPC, int, float, bool>)newArgs[8] : null;
                    Action<Projectile, Player, int, bool> ProjectileOnHitPVP = newArgs[9] != null ? (Action<Projectile, Player, int, bool>)newArgs[9] : null;
                    Func<Projectile, NPC, int, float, bool, object[]> ProjectileModifyHitNPC = newArgs[10] != null ? (Func<Projectile, NPC, int, float, bool, object[]>)newArgs[10] : null;
                    Func<Projectile, Player, int, bool, object[]> ProjectileModifyHitPVP = newArgs[11] != null ? (Func<Projectile, Player, int, bool, object[]>)newArgs[11] : null;

                    if (elementMod == null || elementName.Length == 0 || elementName == null || !elementColor.HasValue)
                        return null;

                    // If there is no element with the same identifier, continue
                    var elementList = elementData.TakeWhile(x => x.Item3 == CreateIdentifier(elementMod, elementName)).ToList();
                    if (!elementList.Any())
                    {
                        AddElementData(elementMod, elementName, elementColor.Value, npcBehaviour, pvpBehaviour, modifyHitNPC, modifyHitPVP, ProjectileOnHitNPC, ProjectileOnHitPVP, ProjectileModifyHitNPC, ProjectileModifyHitPVP);
                    }
                }
                else if (message == "LogElements".ToLower())
                {
                    // Log a mod's elements, or all elements
                    Mod elementMod = newArgs[1] as Mod;
                    foreach (var item in elementData)
                    {
                        if (elementMod == null)
                            ErrorLogger.Log(item.ToString());
                        else if (item.Item1 == elementMod)
                            ErrorLogger.Log(item.ToString());
                    }
                }
                else if (message == "UnloadElement".ToLower())
                {
                    // Unload a mod's element
                    Mod elementMod = newArgs[1] as Mod;
                    string elementName = newArgs[2] as string;

                    if (elementMod == null || elementName.Length == 0 || elementName == null)
                        return null;

                    // If there is an element with the same signature, unload it
                    var elementList = elementData.FindIndex(x => x.Item3 == CreateIdentifier(elementMod, elementName));
                    if (elementList != -1)
                    {
                        UnloadElementData(CreateIdentifier(elementMod, elementName));
                    }
                }
                else if (message == "AlterElementModifier".ToLower())
                {
                    Mod elementMod = newArgs[1] as Mod;
                    Player elementPlayer = newArgs[2] as Player;
                    string elementName = newArgs[3] as string;
                    double? elementModifier = newArgs[4] as double?;

                    if (elementMod == null || elementPlayer == null || elementName.Length == 0 || elementName == null || !elementModifier.HasValue)
                        return null;

                    // If there is an element with the same signature, alter that modifier
                    var elementList = elementData.FindIndex(x => x.Item3 == CreateIdentifier(elementMod, elementName));
                    if ((int)elementList != -1)
                    {
                        AlterElementModifier(elementPlayer, CreateIdentifier(elementMod, elementName), elementModifier);
                    }
                }
                else if (message == "GetElementName".ToLower())
                {
                    // Get element name of index and return it

                    int? elementType = newArgs[1] as int?;
                    if (!elementType.HasValue || elementType.Value < 0)
                        return null;
                    var elementList = elementData.ElementAtOrDefault(elementType.Value);
                    if (elementList != new Tuple<Mod, string, string, Color>(null, "", "", default(Color)) || elementList != new Tuple<Mod, string, string, Color>(null, null, null, default(Color)))
                        return elementData[elementType.Value].Item2;
                }
                else if (message == "GetElementMod".ToLower())
                {
                    // Get element mod of index and return it

                    int? elementType = newArgs[1] as int?;
                    if (!elementType.HasValue || elementType.Value < 0)
                        return null;
                    var elementList = elementData.ElementAtOrDefault(elementType.Value);
                    if (elementList != new Tuple<Mod, string, string, Color>(null, "", "", default(Color)) && elementList != new Tuple<Mod, string, string, Color>(null, null, null, default(Color)))
                        return elementData[elementType.Value].Item1;
                }
                else if (message == "CountElements".ToLower())
                {
                    // Count elements of mod or all elements and return count

                    Mod elementMod = newArgs[1] as Mod;
                    if (elementMod == null)
                        return elementData.Count;

                    var elementList = elementData.TakeWhile(x => x.Item1 == elementMod).ToList();
                    return elementList.Count;

                }
                else if (message == "GetElementType".ToLower())
                {
                    // Get element type from mod with name and return it

                    Mod elementMod = newArgs[1] as Mod;
                    string elementName = newArgs[2] as string;
                    if (elementMod == null || elementName == null || elementName.Length == 0)
                        return null;

                    var elementList = elementData.FindIndex(x => x.Item1 == elementMod && x.Item2 == elementName && x.Item3 == (elementMod.Name + "::" + elementName));
                    if (elementList != -1)
                        return elementList as int?;
                }
                else if (message == "GetModElements".ToLower())
                {
                    // Get elements of mod and return their types in a List of ints

                    Mod elementMod = newArgs[1] as Mod;
                    if (elementMod == null)
                        return null;
                    var elementList = new List<Tuple<Mod, string, string, Color>>(elementData);
                    var returnList = new List<int>();
                    for (int i = 0; i < elementList.Count; i++)
                    {
                        if (elementList[i].Item1 == elementMod)
                            returnList.Add(i);
                    }
                    if (returnList.Count > 0)
                        return returnList;
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
