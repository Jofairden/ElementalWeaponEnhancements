using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using System.IO;
using Microsoft.Xna.Framework;

namespace ElementalWeaponEnhancements
{
    class ElementalItem : GlobalItem
    {
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            ElementalInfo info = item.GetModInfo<ElementalInfo>(mod);
            if (info.enhanced)
            {
                if (info.type != ElementalInfo.ElementalType.Normal)
                {
                    TooltipLine tip = new TooltipLine(mod, "EWE:Tooltip", info.GetDamage(Main.player[item.owner]).ToString() + " " + Enum.GetName(typeof(ElementalInfo.ElementalType), info.type) + " Damage");
                    if (info.ElementalColor[(int)info.type].HasValue)
                        tip.overrideColor = info.ElementalColor[(int)info.type].Value;
                    tooltips.Insert(2, tip);
                }
            }
        }

        public override bool NeedsCustomSaving(Item item)
        {
            return item.GetModInfo<ElementalInfo>(mod).enhanced ? true : base.NeedsCustomSaving(item);
        }

        public override void SaveCustomData(Item item, BinaryWriter writer)
        {
            ElementalInfo info = item.GetModInfo<ElementalInfo>(mod);
            if (info.enhanced && info.type != ElementalInfo.ElementalType.Normal)
            {
                writer.Write((int)info.type);
                writer.Write(info.damage);
            }
        }

        public override void LoadCustomData(Item item, BinaryReader reader)
        {
            ElementalInfo info = item.GetModInfo<ElementalInfo>(mod);
            info.elementItem = item;
            info.SetItem((ElementalInfo.ElementalType)reader.ReadInt32(), reader.ReadInt32());
        }

        public override bool OnPickup(Item item, Player player)
        {
            ElementalInfo info = item.GetModInfo<ElementalInfo>(mod);
            if (!info.enhanced && Tools.IsWeapon(item))
            {
                info.elementItem = item;
                info.RollForItem(item);
            }
            return base.OnPickup(item, player);
        }

        public override bool Shoot(Item item, Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            ElementalInfo info = item.GetModInfo<ElementalInfo>(mod);
            if (info.enhanced)
            {
                damage += info.GetDamage(player);
            }
            return base.Shoot(item, player, ref position, ref speedX, ref speedY, ref type, ref damage, ref knockBack);
        }

        public override void ModifyHitNPC(Item item, Player player, NPC target, ref int damage, ref float knockBack, ref bool crit)
        {
            base.ModifyHitNPC(item, player, target, ref damage, ref knockBack, ref crit);
            ElementalInfo info = item.GetModInfo<ElementalInfo>(mod);
            if (info.enhanced)
            {
                int hasDamage = info.GetDamage(player);
                damage += Math.Min(hasDamage, Main.rand.Next((int)Math.Ceiling(hasDamage * 0.35f), hasDamage));
            }
        }
    }
}
