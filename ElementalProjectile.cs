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
            ElementalProjectileInfo info = projectile.GetModInfo<ElementalProjectileInfo>(mod);
            if (info.enhanced)
            {
                if (ElementalWeaponEnhancements.elementProjectileOnHitNPC[info.elementalType] != null)
                    ElementalWeaponEnhancements.elementProjectileOnHitNPC[info.elementalType].Invoke(projectile, target, damage, knockback, crit);
            }
            base.OnHitNPC(projectile, target, damage, knockback, crit);
        }

        // Custom OnHitPVP
        public override void OnHitPvp(Projectile projectile, Player target, int damage, bool crit)
        {
            ElementalProjectileInfo info = projectile.GetModInfo<ElementalProjectileInfo>(mod);
            if (info.enhanced)
            {
                if (ElementalWeaponEnhancements.elementProjectileOnHitPVP[info.elementalType] != null)
                    ElementalWeaponEnhancements.elementProjectileOnHitPVP[info.elementalType].Invoke(projectile, target, damage, crit);
            }
            base.OnHitPvp(projectile, target, damage, crit);
        }

        // Custom ModifyHitNPC
        public override void ModifyHitNPC(Projectile projectile, NPC target, ref int damage, ref float knockback, ref bool crit)
        {
            ElementalProjectileInfo info = projectile.GetModInfo<ElementalProjectileInfo>(mod);
            if (info.enhanced)
            {
                object[] returnObjects = null;

                if (ElementalWeaponEnhancements.elementProjectileModifyHitNPC[info.elementalType] != null)
                    returnObjects = ElementalWeaponEnhancements.elementProjectileModifyHitNPC[info.elementalType].Invoke(projectile, target, damage, knockback, crit);

                if (returnObjects == null || returnObjects.Length == 0 || !((bool)returnObjects[5]))
                    damage += info.sourceItem.GetModInfo<ElementalInfo>(mod).GetRealDamage(Main.player[info.sourceItem.owner]);

                if (returnObjects != null)
                {
                    projectile = (Projectile)returnObjects[0];
                    target = (NPC)returnObjects[1];
                    damage = (int)returnObjects[2];
                    knockback = (float)returnObjects[3];
                    crit = (bool)returnObjects[4];
                }
            }
            base.ModifyHitNPC(projectile, target, ref damage, ref knockback, ref crit);
        }

        // Custom ModifyHitPVP
        public override void ModifyHitPvp(Projectile projectile, Player target, ref int damage, ref bool crit)
        {
            ElementalProjectileInfo info = projectile.GetModInfo<ElementalProjectileInfo>(mod);
            if (info.enhanced)
            {
                object[] returnObjects = null;

                if (ElementalWeaponEnhancements.elementProjectileModifyHitPVP[info.elementalType] != null)
                    returnObjects = ElementalWeaponEnhancements.elementProjectileModifyHitPVP[info.elementalType].Invoke(projectile, target, damage, crit);

                if (returnObjects == null || returnObjects.Length == 0 || !((bool)returnObjects[4]))
                    damage += info.sourceItem.GetModInfo<ElementalInfo>(mod).GetRealDamage(Main.player[info.sourceItem.owner]);

                if (returnObjects != null)
                {
                    projectile = (Projectile)returnObjects[0];
                    target = (Player)returnObjects[1];
                    damage = (int)returnObjects[2];
                    crit = (bool)returnObjects[3];
                }
            }
            base.ModifyHitPvp(projectile, target, ref damage, ref crit);
        }
    }
}
