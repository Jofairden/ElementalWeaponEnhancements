using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using System.Reflection;
using Microsoft.Xna.Framework;
using System.Security.Cryptography;

namespace ElementalWeaponEnhancements
{
    public class ElementalFramework
    {
        internal static Mod mod
        {
            get { return ModLoader.GetMod("ElementalWeaponEnhancements"); }
        }

        internal class Data
        {
            internal const double rollChancePickup = 0.25;
            internal const double rollChanceReforge = 0.25;
            internal const double rollChanceCraft = 0.25;

            internal static List<Tuple<Mod, string, string, Color>> elementData = new List<Tuple<Mod, string, string, Color>>();
            internal static List<string> elementDisplayName = new List<string>();

            internal static List<Tuple<Behaviour.OnHitNPC, Behaviour.OnHitPVP, Behaviour.ProjectileOnHitNPC, Behaviour.ProjectileOnHitPVP>> elementOnHit = new List<Tuple<Behaviour.OnHitNPC, Behaviour.OnHitPVP, Behaviour.ProjectileOnHitNPC, Behaviour.ProjectileOnHitPVP>>();
            internal static List<Tuple<Behaviour.ModifyHitNPC, Behaviour.ModifyHitPVP, Behaviour.ProjectileModifyHitNPC, Behaviour.ProjectileModifyHitPVP>> elementModifyHit = new List<Tuple<Behaviour.ModifyHitNPC, Behaviour.ModifyHitPVP, Behaviour.ProjectileModifyHitNPC, Behaviour.ProjectileModifyHitPVP>>();

            internal static List<Info.CalculateDamage> elementCalculateDamage = new List<Info.CalculateDamage>();
            internal static List<Tuple<bool, double, double, double>> elementRollChance = new List<Tuple<bool, double, double, double>>(); // pickup reforge craft
            internal static List<int> elementWeight = new List<int>();
        }

        /// <summary>
        /// Contains behaviour delegates
        /// </summary>
        public class Behaviour
        {
            public delegate void OnHitNPC(Item item, Player player, NPC target, int damage, float knockBack, bool crit);
            public delegate void OnHitPVP(Item item, Player player, Player target, int damage, bool crit);
            public delegate Tuple<Item, Player, NPC, int, float, bool, bool> ModifyHitNPC(Item item, Player player, NPC target, int damage, float knockback, bool crit);
            public delegate Tuple<Item, Player, Player, int, bool, bool> ModifyHitPVP(Item item, Player player, Player target, int damage, bool crit);
            public delegate void ProjectileOnHitNPC(Projectile projectile, NPC target, int damage, float knockBack, bool crit);
            public delegate void ProjectileOnHitPVP(Projectile projectile, Player target, int damage, bool crit);
            public delegate Tuple<Projectile, NPC, int, float, bool, bool> ProjectileModifyHitNPC(Projectile projectile, NPC target, int damage, float knokback, bool crit);
            public delegate Tuple<Projectile, Player, int, bool, bool> ProjectileModifyHitPVP(Projectile projectile, Player target, int damage, bool crit);
        }

        /// <summary>
        /// Contains various misc delegates
        /// </summary>
        public class Info
        {
            public delegate int CalculateDamage(int damage);
        }

        public static int CreateElement(Mod mod, string name, Color color)
        {
            if (mod == null || name == null || name.Length == 0)
                return -1;

            if (Data.elementData.FindIndex(x => x.Item3 == CreateIdentifier(mod, name)) == -1)
            {
                Data.elementData.Add(new Tuple<Mod, string, string, Color>(mod, CleanString(name), CreateIdentifier(mod, name), color));
                Data.elementDisplayName.Add(null);
                Data.elementCalculateDamage.Add(null);
                Data.elementRollChance.Add(new Tuple<bool, double, double, double>(false, Data.rollChancePickup, Data.rollChanceReforge, Data.rollChanceCraft));
                Data.elementWeight.Add(100); // vanilla weight
                Data.elementOnHit.Add(new Tuple<Behaviour.OnHitNPC, Behaviour.OnHitPVP, Behaviour.ProjectileOnHitNPC, Behaviour.ProjectileOnHitPVP>(null, null, null, null));
                Data.elementModifyHit.Add(new Tuple<Behaviour.ModifyHitNPC, Behaviour.ModifyHitPVP, Behaviour.ProjectileModifyHitNPC, Behaviour.ProjectileModifyHitPVP>(null, null, null, null));
                return GetElement(mod, name);
            }
            else return -1;
        }

        public static void CreateDisplayName(int type, string displayName)
        {
            if (type < 0 || type + 1 > Data.elementData.Count || displayName == null || displayName.Length == 0)
                return;

            Data.elementDisplayName[type] = CleanString(displayName);
        }

        public static void ModifyDamage(int type, Info.CalculateDamage _delegate)
        {
            if (type < 0 || type + 1 > Data.elementData.Count || _delegate == null)
                return;

            Data.elementCalculateDamage[type] = _delegate;
        }

        public static void SetWeight(int type, int weight = 100)
        {
            if (type < 0 || type + 1 > Data.elementData.Count)
                return;

            if (weight < 1)
                weight = 1;

            if (weight > 200)
                weight = 200;

            Data.elementWeight[type] = weight;
        }

        public static void CreateOnHitBehaviour(int type, Behaviour.OnHitNPC _onHitNPC, Behaviour.OnHitPVP _onHitPVP = null, Behaviour.ProjectileOnHitNPC _projectileOnHitNPC = null, Behaviour.ProjectileOnHitPVP _projectileOnHitPVP = null)
        {
            if (type < 0 || type + 1 > Data.elementData.Count)
                return;

            Data.elementOnHit[type] = new Tuple<Behaviour.OnHitNPC, Behaviour.OnHitPVP, Behaviour.ProjectileOnHitNPC, Behaviour.ProjectileOnHitPVP>(_onHitNPC, _onHitPVP, _projectileOnHitNPC, _projectileOnHitPVP);
        }

        public static void CreateModifyHitBehaviour(int type, Behaviour.ModifyHitNPC _modifyHitNPC, Behaviour.ModifyHitPVP _modifyHitPVP = null, Behaviour.ProjectileModifyHitNPC _projectileModifyHitNPC = null, Behaviour.ProjectileModifyHitPVP _projectileModifyHitPVP = null)
        {
            if (type < 0 || type + 1 > Data.elementData.Count)
                return;

            Data.elementModifyHit[type] = new Tuple<Behaviour.ModifyHitNPC, Behaviour.ModifyHitPVP, Behaviour.ProjectileModifyHitNPC, Behaviour.ProjectileModifyHitPVP>(_modifyHitNPC, _modifyHitPVP, _projectileModifyHitNPC, _projectileModifyHitPVP);
        }

        public static List<int> GetElements(Mod mod = null)
        {
            if (!Data.elementData.Any())
                return null;

            List<int> returnList = new List<int>();
            List<Tuple<Mod, string, string, Color>> elementList = new List<Tuple<Mod, string, string, Color>>(Data.elementData);

            if (mod != null)
            {
                elementList = Data.elementData.TakeWhile(x => x.Item1 == mod).ToList();
            }

            if (elementList.Any())
            {
                foreach (var item in elementList)
                {
                    returnList.Add(GetElement(item.Item1, item.Item2));
                }

                return returnList;
            }

            return null;
        }

        public static int GetElement(Mod mod, string name)
        {
            if (mod == null || name == null || name.Length == 0)
                return -1;

            return Data.elementData.FindIndex(x => x.Item1 == mod && x.Item2 == name && x.Item3 == CreateIdentifier(mod, name));
        }

        public static string GetElementName(int type)
        {
            if (type < 0 || type + 1 > Data.elementData.Count)
                return null;

            return Data.elementData[type].Item2;
        }

        public static string GetDisplayName(int type)
        {
            if (type < 0 || type + 1 > Data.elementData.Count)
                return null;

            return Data.elementDisplayName[type];
        }

        public static Mod GetMod(int type)
        {
            if (type < 0 || type + 1 > Data.elementData.Count)
                return null;

            return Data.elementData[type].Item1;
        }

        public static void ModifyElementModifier(int type, Player player, float modifier)
        {
            if (type < 0 || type + 1 > Data.elementData.Count || type + 1 > player.GetModPlayer<ElementalPlayer>(mod).elementDamage.Count)
                return;

            player.GetModPlayer<ElementalPlayer>(mod).elementDamage[type] += (float)modifier;
        }

        // Internal

        internal static void SetRollChance(int type, double pickup = Data.rollChanceCraft, double reforge = Data.rollChanceReforge, double craft = Data.rollChanceCraft)
        {
            if (type < 0 || type + 1 > Data.elementDisplayName.Count || pickup < 0 || reforge < 0 || craft < 0)
                return;

            Data.elementRollChance[type] = new Tuple<bool, double, double, double>(true, pickup, reforge, craft);
        }

        internal static string CreateIdentifier(Mod mod, string name)
        {
            string input = mod.Name + "::" + name;
            using (MD5 encrypt = MD5.Create())
            {
                byte[] hash = encrypt.ComputeHash(Encoding.Default.GetBytes(input));
                Guid result = new Guid(hash);
                return result.ToString();
            }
        }

        internal static void ClearData()
        {
            Data.elementData.Clear();
            Data.elementOnHit.Clear();
            Data.elementModifyHit.Clear();
            Data.elementDisplayName.Clear();
            Data.elementCalculateDamage.Clear();
            Data.elementRollChance.Clear();
            Data.elementWeight.Clear();
        }

        internal static string CleanString(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        internal static int getTotalWeight()
        {
            int _w = 0;
            if (Data.elementWeight.Any())
            {
                foreach (var item in Data.elementWeight)
                {
                    _w += item;
                }
            }
            return _w;
        }
    }
}
