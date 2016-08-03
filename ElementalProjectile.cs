using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace ElementalWeaponEnhancements
{
    class ElementalProjectile : GlobalProjectile
    {
        // Hacky way of determining if a projectile was shot by an enhanced item
        // Very hacky, but there's no better way sadly.
        public override bool PreAI(Projectile projectile)
        {
            try
            {
                ElementalProjectileInfo info = projectile.GetModInfo<ElementalProjectileInfo>(mod);
                if (info.num++ == 0 && !projectile.hostile && projectile.friendly)
                {
                    Player player = Main.player[projectile.owner];
                    Item item = player.inventory[player.selectedItem];
                    ElementalInfo itemInfo = item.GetModInfo<ElementalInfo>(mod);
                    
                    if (itemInfo.enhanced)
                    {
                        info.enhanced = true;
                        info.elementalProjectile = projectile;
                        info.elementalType = itemInfo.elementalType;
                        info.sourceItem = item;
                    }
                }
            }
            catch (Exception e)
            {
                ErrorLogger.Log(e.ToString());
            }
            return base.PreAI(projectile);
        }

        // Custom OnHitNPC
        public override void OnHitNPC(Projectile projectile, NPC target, int damage, float knockback, bool crit)
        {
            try
            {
                ElementalProjectileInfo info = projectile.GetModInfo<ElementalProjectileInfo>(mod);
                if (info.enhanced)
                {
                    if (ElementalFramework.Data.elementOnHit[info.elementalType].Item3 != null)
                        ElementalFramework.Data.elementOnHit[info.elementalType].Item3.Invoke(projectile, target, damage, knockback, crit);
                }
            }
            catch (Exception e)
            {
                ErrorLogger.Log(e.ToString());
            }
            base.OnHitNPC(projectile, target, damage, knockback, crit);
        }

        // Custom OnHitPVP
        public override void OnHitPvp(Projectile projectile, Player target, int damage, bool crit)
        {
            try
            {
                ElementalProjectileInfo info = projectile.GetModInfo<ElementalProjectileInfo>(mod);
                if (info.enhanced)
                {
                    if (ElementalFramework.Data.elementOnHit[info.elementalType].Item4 != null)
                        ElementalFramework.Data.elementOnHit[info.elementalType].Item4.Invoke(projectile, target, damage, crit);
                }
            }
            catch (Exception e)
            {
                ErrorLogger.Log(e.ToString());
            }
            base.OnHitPvp(projectile, target, damage, crit);
        }

        // Custom ModifyHitNPC
        public override void ModifyHitNPC(Projectile projectile, NPC target, ref int damage, ref float knockback, ref bool crit)
        {
            try
            {
                ElementalProjectileInfo info = projectile.GetModInfo<ElementalProjectileInfo>(mod);
                if (info.enhanced)
                {
                    bool addDamage = true;

                    if (ElementalFramework.Data.elementModifyHit[info.elementalType].Item3 != null)
                    {
                        var returnObjects = ElementalFramework.Data.elementModifyHit[info.elementalType].Item3.Invoke(projectile, target, damage, knockback, crit);

                        if ((!(bool)returnObjects.Item6))
                            addDamage = false;

                        if (returnObjects != null)
                        {
                            projectile = (Projectile)returnObjects.Item1;
                            target = (NPC)returnObjects.Item2;
                            damage = (int)returnObjects.Item3;
                            knockback = (float)returnObjects.Item4;
                            crit = (bool)returnObjects.Item5;
                        }
                    }

                    if (addDamage)
                    {
                        damage += info.sourceItem.GetModInfo<ElementalInfo>(mod).GetRealDamage(Main.player[info.sourceItem.owner]);
                    }
                }
            }
            catch (Exception e)
            {
                ErrorLogger.Log(e.ToString());
            }
            base.ModifyHitNPC(projectile, target, ref damage, ref knockback, ref crit);
        }

        // Custom ModifyHitPVP
        public override void ModifyHitPvp(Projectile projectile, Player target, ref int damage, ref bool crit)
        {
            try
            {
                ElementalProjectileInfo info = projectile.GetModInfo<ElementalProjectileInfo>(mod);
                if (info.enhanced)
                {
                    bool addDamage = true;

                    if (ElementalFramework.Data.elementModifyHit[info.elementalType].Item4 != null)
                    {
                        var returnObjects = ElementalFramework.Data.elementModifyHit[info.elementalType].Item4.Invoke(projectile, target, damage, crit);

                        if ((!(bool)returnObjects.Item5))
                            addDamage = false;

                        if (returnObjects != null)
                        {
                            projectile = (Projectile)returnObjects.Item1;
                            target = (Player)returnObjects.Item2;
                            damage = (int)returnObjects.Item3;
                            crit = (bool)returnObjects.Item5;
                        }

                        if (addDamage)
                        {
                            damage += info.sourceItem.GetModInfo<ElementalInfo>(mod).GetRealDamage(Main.player[info.sourceItem.owner]);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ErrorLogger.Log(e.ToString());
            }
            base.ModifyHitPvp(projectile, target, ref damage, ref crit);
        }
    }
}
