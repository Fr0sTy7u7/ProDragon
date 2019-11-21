using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;
using EnsoulSharp.SDK.Utility;
using Core;

namespace Event
{
    public class EventManager 
    {
        public static void LoadEvents()
        {
            BaseUltCore.Initialize();
            Game.OnUpdate += args1 => BaseUltCore.Game_OnUpdate();
            Drawing.OnEndScene += args1 => BaseUltCore.Drawing_OnEndScene();
            Teleport.OnTeleport += BaseUltCore.Teleport_OnTeleport;
            var not = new Notification("BaseUlt", "xDreamms BaseUlt Loaded Supported Champions: \n Ezreal, Jinx, Ashe, Draven, Karthus,Ziggs, Lux, Xerath ");
            var not1 = new Notification("Base_Ult", "ProDragon added: \n Senna ");
            Notifications.Add(not1);
            DelayAction.Add(30000, () => { Notifications.Remove(not); });
            DelayAction.Add(30000, () => { Notifications.Remove(not1); });
        }

       
    }
}
