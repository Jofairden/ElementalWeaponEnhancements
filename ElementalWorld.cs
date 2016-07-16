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
    class ElementalWorld : ModWorld
    {
        int num = 0;

        public override void PostUpdate()
        {
            if (num++ == 0)
            {
                if (Main.netMode != 1)
                {
                    Main.NewText("Elemental Weapon Enhancements Version: 0.0.1 by Gorateron");
                }
                else
                {
                    NetMessage.SendData(MessageID.ChatText, -1, -1, "Elemental Weapon Enhancements Version: 0.0.1 by Gorateron");
                }
            }
        }
    }
}
