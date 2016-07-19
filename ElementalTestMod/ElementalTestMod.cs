using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ElementalTestMod
{
    public class ElementalTestMod : Mod
    {
        public ElementalTestMod()
        {
            Properties = new ModProperties()
            {
                Autoload = true,
                AutoloadGores = true,
                AutoloadSounds = true
            };
        }

        // It is very important the hooks has all the same parameters as the vanilla hooks! (aka, don't change any of the parameters in your custom behaviour hooks)
        private static void PoisonOnHitNPC(Item item, Player player, NPC target, int damage, float knockBack, bool crit)
        {
            target.AddBuff(BuffID.Ichor, 900); // Add the ichor debuff to the target when an item with poison damage hits them
        }

        private static void PoisonProjectileOnHitNPC(Projectile projectile, NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Ichor, 900); // Projectiles shot should also apply ichor
        }

        private static object[] PoisonModifyHitNPC(Item item, Player player, NPC target, int damage, float knockBack, bool crit)
        {
            // Here you can run any code that will happen in ModifyHitNPC
            // You must return an array that hols all objects present in the parameters in the same order
            // The last object in the array must be a bool, if the bool is true then by default the element damage is added to the item's damage
            // If the bool is false the element's damage will not be added (for example, this would allow for your own custom damage calculation here)
            damage += 25; // To keep things simple, I just want to add 25 extra damage to the hit
            crit = true; // Also, I always want poison hits to crit
            return new object[] { item, player, target, damage, knockBack, crit, true };
        }

        private static object[] PoisonProjectileModifyHitNPC(Projectile projectile, NPC target, int damage, float knockback, bool crit)
        {
            // projectiles should also deal 25 more damage and crit
            damage += 25;
            crit = true;
            return new object[] { projectile, target, damage, knockback, crit, true };
        }
        
        private static void VikingOnHitPVP(Item item, Player player, Player target, int damage, bool crit)
        {
            // Viking damage deals 25 less damage in PvP, but it adds bleeding
            target.AddBuff(BuffID.Bleeding, 300);
        }

        private static void VikingProjectileOnHitPVP(Projectile projectile, Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Bleeding, 300); // projectiles should also apply bleeding
        }

        private static object[] VikingModifyHitPVP(Item item, Player player, Player target, int damage, bool crit)
        {
            // Viking damage deals 25 less damage
            damage -= 25;
            return new object[] { item, player, target, damage, crit, true };
        }

        private static object[] VikingProjectileModifyHitPVP(Projectile projectile, Player target, int damage, bool crit)
        {
            damage -= 25; // projectiles should also deal 25 less damage
            return new object[] { projectile, target, damage, crit, true };
        }

        // Add our custom elements
        public override void Load()
        {
            try
            {
                Mod eleMod = ModLoader.GetMod("ElementalWeaponEnhancements");
                if (eleMod != null)
                {
                    // You can NOT change the action/func format! (if you do, things will break)
                    var _poisonOnHitNPC = new Action<Item, Player, NPC, int, float, bool>(PoisonOnHitNPC);
                    var _poisonModifyHitNPC = new Func<Item, Player, NPC, int, float, bool, object[]>(PoisonModifyHitNPC);
                    var _poisonProjectileOnHitNPC = new Action<Projectile, NPC, int, float, bool>(PoisonProjectileOnHitNPC);
                    var _poisonProjectileModifyHitNPC = new Func<Projectile, NPC, int, float, bool, object[]>(PoisonProjectileModifyHitNPC);

                    eleMod.Call("CreateElement", this, "Poison", Color.ForestGreen, _poisonOnHitNPC, null, _poisonModifyHitNPC, null, _poisonProjectileOnHitNPC, null, _poisonProjectileModifyHitNPC);
                    // Here's the parameters you must pass: (all in order)
                    // Type of call (the first string, so "CreateElement")
                    // What mod the call comes from
                    // Name of your custom element
                    // The color for the tooltip to use

                    // Possible calls:
                    // CreateElement, LogElements, UnloadElement, AlterElementModifier, GetElementName, GetElementMod, CountElements, GetElementType, GetModElements
                    // See the GH wiki on usage of these calls
                    // https://github.com/gorateron/ElementalWeaponEnhancements/wiki

                    // It is not required to pass custom behaviour, so you could end it at the Color.
                    // But if you want custom behaviour, such as made above, you need to pass them as shown here.
                    // The order is as follows:
                    // OnHitNPC
                    // OnHitPVP
                    // ModifyHitNPC
                    // ModifyHitPVP
                    // ProjectileOnHitNPC
                    // ProjectileOnHitPVP
                    // ProjectileModifyHitNPC
                    // ProjectileModifyHitPVP
                    // If you don't want a certain custom behaviour you should pass null

                    // Viking damage, deals 25 less damage in PvP but applies a bleed
                    var _vikingOnHitPVP = new Action<Item, Player, Player, int, bool>(VikingOnHitPVP);
                    var _vikingModifyHitPVP = new Func<Item, Player, Player, int, bool, object[]>(VikingModifyHitPVP);
                    var _vikingProjectileOnHitPVP = new Action<Projectile, Player, int, bool>(VikingProjectileOnHitPVP);
                    var _vikingProjectileModifyHitPVP = new Func<Projectile, Player, int, bool, object[]>(VikingProjectileModifyHitPVP);

                    eleMod.Call("CreateElement", this, "Viking", Color.BlanchedAlmond, null, _vikingOnHitPVP, null, _vikingModifyHitPVP, null, _vikingProjectileOnHitPVP, null, _vikingProjectileModifyHitPVP);
                }
            }
            catch (Exception e)
            {
                ErrorLogger.Log(e.ToString());
            }
        }

        public override void PostSetupContent()
        {
            //Mod eleMod = ModLoader.GetMod("ElementalWeaponEnhancements");
            //if (eleMod != null)
            //{
            //    eleMod.Call("LogElements", null); // log all elements
            //    eleMod.Call("LogElements", this); // log only our custom elements
            //}
        }

        // Unload our custom elements
        public override void Unload()
        {
            Mod eleMod = ModLoader.GetMod("ElementalWeaponEnhancements");
            if (eleMod != null)
            {
                eleMod.Call("UnloadElement", this, "Poison");
                eleMod.Call("UnloadElement", this, "Viking");
            }
        }
    }
}
