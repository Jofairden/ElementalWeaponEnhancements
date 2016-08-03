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
    // undocumented
    class ElementalProjectileInfo : ProjectileInfo
    {
        public int num { get; set; }
        public bool enhanced { get; set; }
        public int elementalType { get; set; }
        public Projectile elementalProjectile { get; set; }
        public Item sourceItem { get; set; }
    }

    class ElementalInfo : ItemInfo
    {
        // Elemental Variables
        public bool enhanced { get; private set; }
        public bool justDropped = true;
        public int elementalType { get; private set; }
        public int elementalDamage { get; private set; }
        public Item elementalItem { get; private set; }

        // Set elemental properties
        internal void SetProperties(bool enabled, int type, int damage, Item item)
        {
            enhanced = enabled;
            elementalType = type;
            elementalDamage = damage;
            elementalItem = item;
            justDropped = false;
        }

        // Reset properties
        internal void ResetProperties()
        {
            enhanced = false;
            elementalType = 0;
            elementalDamage = 0;
            elementalItem = null;
        }

        // Get a real damage value, calculated with a player's elemental modifier
        public int GetRealDamage(Player player)
        {
            return (int)Math.Ceiling(((double)elementalDamage  * player.GetModPlayer<ElementalPlayer>(mod).elementDamage[elementalType]));
        }

        // Calculate a damage value
        internal int CalculateDamage(ref int refDamage)
        {
            return (int)(Main.rand.Next((int)Math.Ceiling(refDamage * 0.10f), (int)Math.Ceiling(refDamage * 0.50f)));
        }

        // Get a new damage value
        internal int CalculateNewDamage(ref int refDamage)
        {
            int _damage = (refDamage == -1) ? elementalItem.damage : refDamage;
            return (ElementalFramework.Data.elementCalculateDamage[elementalType] != null) ? ElementalFramework.Data.elementCalculateDamage[elementalType].Invoke(_damage) : CalculateDamage(ref _damage);
        }

        // Calculate a new element. The element can not be the element it was prior to changing it.
        public void CalculateNewElement()
        {
            var elementList = ElementalFramework.Data.elementData.ToList();
            elementList.Remove(ElementalFramework.Data.elementData[elementalType]);
            if (elementList.Any())
            {
                int _weight = Main.rand.Next(0, ElementalFramework.getTotalWeight());
                int _selected = _CalculateElement(elementList, _weight);
                /*int random = Main.rand.Next(0, (int)elementList.Count);
                //The zero-based index of the first occurrence of item within the entire List<T>, if found; otherwise, –1.
                int newElement = ElementalFramework.Data.elementData.FindIndex(x => x.Item3 == elementList[random].Item3);
                if (newElement != -1)
                {
                    elementalType = newElement;
                }*/

                if (_selected != -1)
                {
                    elementalType = _selected;
                }
            }
        }

        // Get element by weight
        internal int _CalculateElement(List<Tuple<Mod, string, string, Color>> _data, int _weight)
        {
            int _selected = -1;
            foreach (var item in _data)
            {
                int _thisWeight = ElementalFramework.Data.elementWeight[ElementalFramework.GetElement(item.Item1, item.Item2)];

                if (_weight < _thisWeight)
                {
                    _selected = ElementalFramework.Data.elementData.FindIndex(x => x.Item3 == item.Item3);
                    break;
                }

                _weight = _weight - _thisWeight;
            }

            return _selected;
        }
    }
}
