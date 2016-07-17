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
        // If an item is enhanced, create the custom tooltip and insert it below the vanilla damage.
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            ElementalInfo info = item.GetModInfo<ElementalInfo>(mod);
            if (info.enhanced)
            {
                TooltipLine tip = new TooltipLine(mod, "EWE:Tooltip", info.GetRealDamage(Main.player[Main.myPlayer]).ToString() + " " + ElementalWeaponEnhancements.elementData[info.elementalType].Item2.ToString() + " Damage");
                tip.overrideColor = ElementalWeaponEnhancements.elementData[info.elementalType].Item4;
                tooltips.Insert(2, tip);
            }
        }

        // If an item is enhanced, it needs custom saving.
        public override bool NeedsCustomSaving(Item item)
        {
            ElementalInfo info = item.GetModInfo<ElementalInfo>(mod);
            return (info.enhanced) ? true : base.NeedsCustomSaving(item);
        }

        // Save the current type, and damage value.
        public override void SaveCustomData(Item item, BinaryWriter writer)
        {
            ElementalInfo info = item.GetModInfo<ElementalInfo>(mod);
            writer.Write(info.elementalType);
            writer.Write(info.elementalDamage);
            writer.Write(ElementalWeaponEnhancements.elementData[info.elementalType].Item1.Name);
        }

        // Only runs if data was saved in the first place. Set our properties to our saved values.
        public override void LoadCustomData(Item item, BinaryReader reader)
        {
            ElementalInfo info = item.GetModInfo<ElementalInfo>(mod);
            int type = reader.ReadInt32();
            int damage = reader.ReadInt32();
            string modName = reader.ReadString();
            if (ModLoader.GetMod(modName) != null)
            {
                info.SetProperties(true, type, damage, item);
            }
            else
            {
                info.ResetProperties();
                RunElementSelection(item, 1.0, true);
            }
        }

        // ElementRunner, this code determines if you get an element or not.
        private void RunElementSelection(Item item, double chance = 1.0, bool ignoreCheck = false)
        {
            ElementalInfo info = item.GetModInfo<ElementalInfo>(mod);
            info.justDropped = false;
            if (!info.enhanced && Tools.IsWeapon(item) && (ignoreCheck || Main.rand.NextDouble() <= chance))
            {
                info.SetProperties(true, Main.rand.Next(0, ElementalWeaponEnhancements.elementData.Count), info.CalculateDamage(ref item.damage), item);
            }
        }

        // Only run our ElementRunner once, by checking the justDropped variable
        public override bool OnPickup(Item item, Player player)
        {
            ElementalInfo info = item.GetModInfo<ElementalInfo>(mod);
            if (info.justDropped)
                RunElementSelection(item);
            return base.OnPickup(item, player);
        }

        // Have a chance to get an element when the item is crafted
        public override void OnCraft(Item item, Recipe recipe)
        {
            RunElementSelection(item, 0.25);
            base.OnCraft(item, recipe);
        }

        // Have a chance to get an element when the item is reforged
        public override void PostReforge(Item item)
        {
            RunElementSelection(item, 0.25);
            base.PostReforge(item);
        }

        // Add element damage to shot projectile(s)
        public override bool Shoot(Item item, Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            ElementalInfo info = item.GetModInfo<ElementalInfo>(mod);
            if (info.enhanced)
            {
                damage += info.GetRealDamage(player);
            }
            return base.Shoot(item, player, ref position, ref speedX, ref speedY, ref type, ref damage, ref knockBack);
        }

        // Add element damage to hits
        // Todo: resistances
        public override void ModifyHitNPC(Item item, Player player, NPC target, ref int damage, ref float knockBack, ref bool crit)
        {
            ElementalInfo info = item.GetModInfo<ElementalInfo>(mod);
            if (info.enhanced)
            {
                damage += info.GetRealDamage(player);
            }
            base.ModifyHitNPC(item, player, target, ref damage, ref knockBack, ref crit);
        }

        // Add element damage to hits in PvP
        // Todo: resistances
        public override void ModifyHitPvp(Item item, Player player, Player target, ref int damage, ref bool crit)
        {
            ElementalInfo info = item.GetModInfo<ElementalInfo>(mod);
            if (info.enhanced)
            {
                damage += info.GetRealDamage(player);
            }
            base.ModifyHitPvp(item, player, target, ref damage, ref crit);
        }
    }
}
