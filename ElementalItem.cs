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
            try
            {
                ElementalInfo info = item.GetModInfo<ElementalInfo>(mod);
                if (info.enhanced)
                {
                    string useName = (ElementalFramework.Data.elementDisplayName[info.elementalType] != null) ? ElementalFramework.Data.elementDisplayName[info.elementalType] : ElementalFramework.Data.elementData[info.elementalType].Item2;

                    TooltipLine tip = new TooltipLine(mod, "EWE:Tooltip", info.GetRealDamage(Main.player[Main.myPlayer]).ToString() + " " + useName + " Damage");
                    tip.overrideColor = ElementalFramework.Data.elementData[info.elementalType].Item4;
                    tooltips.Insert(2, tip);
                }
            }
            catch (Exception e)
            {
                ErrorLogger.Log(e.ToString());
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
            try
            {
                ElementalInfo info = item.GetModInfo<ElementalInfo>(mod);
                writer.Write(info.elementalType);
                writer.Write(info.elementalDamage);
                writer.Write(ElementalFramework.Data.elementData[info.elementalType].Item1.Name);
            }
            catch (Exception e)
            {
                ErrorLogger.Log(e.ToString());
            }
        }

        // Only runs if data was saved in the first place. Set our properties to our saved values.
        public override void LoadCustomData(Item item, BinaryReader reader)
        {
            try
            {
                ElementalInfo info = item.GetModInfo<ElementalInfo>(mod);
                int type = reader.ReadInt32();
                int damage = reader.ReadInt32();
                string modName = reader.ReadString();
                // If the mod that added the element is null, reset the element
                if (ModLoader.GetMod(modName) != null)
                {
                    info.SetProperties(true, type, damage, item);
                }
                else
                {
                    info.ResetProperties();
                    //RunElementSelection(item, 1.0, true);
                }
            }
            catch (Exception e)
            {
                ErrorLogger.Log(e.ToString());
            }
        }

        // ElementRunner, this code determines if you get an element or not.
        private void RunElementSelection(Item item, double chance = 1.0, bool ignoreCheck = false, bool noCheck = false)
        {
            ElementalInfo info = item.GetModInfo<ElementalInfo>(mod);
            info.justDropped = false;

            if (noCheck || (!info.enhanced && Tools.IsWeapon(item) && (ignoreCheck || Main.rand.NextDouble() <= chance)))
            {
                int element = info._CalculateElement(ElementalFramework.Data.elementData, Main.rand.Next(0, ElementalFramework.getTotalWeight()));
                info.SetProperties(true, element, info.CalculateNewDamage(ref item.damage), item);
            }
        }

        private void RunNewReforge(Item item, double chance = 1.0, bool ignoreCheck = false)
        {
            ElementalInfo info = item.GetModInfo<ElementalInfo>(mod);
            if (!info.enhanced)
            {
                RunElementSelection(item, chance, true, false);
            }
            else if (info.enhanced && Tools.IsWeapon(item) && (ignoreCheck || Main.rand.NextDouble() <= chance))
            {
                RunElementSelection(item, chance, true, true);
            }
        }

        // Only run our ElementRunner once, by checking the justDropped variable
        public override bool OnPickup(Item item, Player player)
        {
            ElementalInfo info = item.GetModInfo<ElementalInfo>(mod);
            if (info.justDropped)
                RunElementSelection(item, 0.25);
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
            RunNewReforge(item, 0.25);
            base.PostReforge(item);
        }

        // Add element damage to shot projectile(s)
        // See ElementalProjectile.cs
        // Info in ElementalInfo.cs
        public override bool Shoot(Item item, Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            // Now done in ElementalProjectile.cs
            //ElementalInfo info = item.GetModInfo<ElementalInfo>(mod);
            //if (info.enhanced)
            //{
            //    damage += info.GetRealDamage(player);
            //}
            return base.Shoot(item, player, ref position, ref speedX, ref speedY, ref type, ref damage, ref knockBack);
        }

        // Custom modify hit npc behaviour
        // Todo: resistances
        public override void ModifyHitNPC(Item item, Player player, NPC target, ref int damage, ref float knockBack, ref bool crit)
        {
            try
            {
                ElementalInfo info = item.GetModInfo<ElementalInfo>(mod);
                if (info.enhanced)
                {
                    bool addDamage = true;

                    if (ElementalFramework.Data.elementModifyHit[info.elementalType].Item1 != null)
                    {
                        var returnObjects = ElementalFramework.Data.elementModifyHit[info.elementalType].Item1.Invoke(item, player, target, damage, knockBack, crit);

                        if (((bool)returnObjects.Item7))
                            addDamage = false;

                        if (returnObjects != null)
                        {
                            item = (Item)returnObjects.Item1;
                            player = (Player)returnObjects.Item2;
                            target = (NPC)returnObjects.Item3;
                            damage = (int)returnObjects.Item4;
                            knockBack = (float)returnObjects.Item5;
                            crit = (bool)returnObjects.Item6;
                        }
                    }

                    if (addDamage)
                        damage += info.GetRealDamage(player);
                }
            }
            catch (Exception e)
            {
                ErrorLogger.Log(e.ToString());
            }
            base.ModifyHitNPC(item, player, target, ref damage, ref knockBack, ref crit);
        }

        // Custom modify hit pvp behaviour
        // Todo: resistances
        public override void ModifyHitPvp(Item item, Player player, Player target, ref int damage, ref bool crit)
        {
            try
            {
                ElementalInfo info = item.GetModInfo<ElementalInfo>(mod);
                if (info.enhanced)
                {
                    bool addDamage = true;

                    if (ElementalFramework.Data.elementModifyHit[info.elementalType].Item2 != null)
                    {
                        var returnObjects = ElementalFramework.Data.elementModifyHit[info.elementalType].Item2.Invoke(item, player, target, damage, crit);

                        if (!((bool)returnObjects.Item6))
                            addDamage = false;

                        if (returnObjects != null)
                        {
                            item = (Item)returnObjects.Item1;
                            player = (Player)returnObjects.Item2;
                            target = (Player)returnObjects.Item3;
                            damage = (int)returnObjects.Item4;
                            crit = (bool)returnObjects.Item5;
                        }
                    }

                    if (addDamage)
                        damage += info.GetRealDamage(player);
                }
            }
            catch (Exception e)
            {
                ErrorLogger.Log(e.ToString());
            }
            base.ModifyHitPvp(item, player, target, ref damage, ref crit);
        }

        // Custom on hit npc behaviour
        public override void OnHitNPC(Item item, Player player, NPC target, int damage, float knockBack, bool crit)
        {
            try
            {
                ElementalInfo info = item.GetModInfo<ElementalInfo>(mod);
                if (info.enhanced)
                {
                    if (ElementalFramework.Data.elementOnHit[info.elementalType].Item1 != null)
                        ElementalFramework.Data.elementOnHit[info.elementalType].Item1.Invoke(item, player, target, damage, knockBack, crit);
                }
            }
            catch (Exception e)
            {
                ErrorLogger.Log(e.ToString());
            }
            base.OnHitNPC(item, player, target, damage, knockBack, crit);
        }

        // Custom on hit pvp behaviour
        public override void OnHitPvp(Item item, Player player, Player target, int damage, bool crit)
        {
            try
            {
                ElementalInfo info = item.GetModInfo<ElementalInfo>(mod);
                if (info.enhanced)
                {
                    if (ElementalFramework.Data.elementOnHit[info.elementalType].Item2 != null)
                        ElementalFramework.Data.elementOnHit[info.elementalType].Item2.Invoke(item, player, target, damage, crit);
                }
            }
            catch (Exception e)
            {
                ErrorLogger.Log(e.ToString());
            }
            base.OnHitPvp(item, player, target, damage, crit);
        }
    }
}
