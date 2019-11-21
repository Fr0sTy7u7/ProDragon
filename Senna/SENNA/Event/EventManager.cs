using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;
using EnsoulSharp.SDK.Utility;
using EnsoulSharp.Ashe;

namespace EnsoulSharp.Ashe
{
    public class EventManager 
    {
        public static void LoadEvents()
        {
            BaseUltCore.Initialize();
            Game.OnUpdate += args1 => BaseUltCore.Game_OnUpdate();
            Drawing.OnEndScene += args1 => BaseUltCore.Drawing_OnEndScene();
            Teleport.OnTeleport += BaseUltCore.Teleport_OnTeleport;
            var not = new Notification("BaseUlt", "Supported Champions: \n Ezreal, Jinx, Ashe, Draven, Karthus,Ziggs, Lux, Xerath,Senna ");
            var not1 = new Notification("xDreamms BaseUlt1", "ProDragon added: \n Senna ");
            Notifications.Add(not);
            DelayAction.Add(30000, () => { Notifications.Remove(not); });
        }

       
    }
}
