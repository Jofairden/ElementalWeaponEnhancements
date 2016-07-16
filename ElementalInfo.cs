using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;

namespace ElementalWeaponEnhancements
{
    class ElementalInfo : ItemInfo
    {
        public enum ElementalType
        {
            Normal,
            Earth,
            Water,
            Air,
            Fire
        }

        public Color?[] ElementalColor =
        {
            null,
            Color.Sienna,
            Color.DodgerBlue,
            Color.Cyan,
            Color.Crimson
        };

        public bool enhanced = false;
        public int damage = 0;
        private int _trueDamage = 0;
        public ElementalType type { get; private set; }
        public Item elementItem;

        public void SetPrimaryType(ElementalType setType)
        {
            type = setType;
        }

        public int GetDamage(Player player)
        {
            _trueDamage = (int)(Math.Ceiling(damage * player.GetModPlayer<ElementalPlayer>(mod).elementalDamage[(int)type]));
            return _trueDamage;
        }

        public void RollForItem(Item item, bool ignoreCheck = false)
        {
            if (ignoreCheck || Main.rand.Next(5) == 0)
            {
                CreateNewItem(item);
            }
        }

        public void RollPrimaryElement()
        {
            List<ElementalType> elements = new List<ElementalType>(Enum.GetValues(typeof(ElementalType)).Cast<ElementalType>().ToList());
            elements.Remove(ElementalType.Normal);
            elements.Remove(type);
            SetPrimaryType(elements[Main.rand.Next(elements.Count)]);
        }

        public void RollPrimaryDamage()
        {
            damage = Math.Max(1, Main.rand.Next((int)Math.Ceiling(elementItem.damage * 0.10f), (int)Math.Ceiling(elementItem.damage * .50f)));
        }

        private void CreateNewItem(Item item)
        {
            enhanced = true;
            SetPrimaryType((ElementalType)Main.rand.Next(1, Enum.GetNames(typeof(ElementalType)).Length));
            RollPrimaryDamage();
        }

        public void SetItem(ElementalType type, int damage)
        {
            enhanced = true;
            SetPrimaryType(type);
            this.damage = damage;
        }
    }
}
