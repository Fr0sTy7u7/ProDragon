using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnsoulSharp;
using Event;
using Menu;

namespace Core
{
    public class BaseUlt
    {
        public static AIHeroClient Player => ObjectManager.Player;

        public static void OnLoad()
        {
            MenuManager.LoadMenu();
            EventManager.LoadEvents();
        }
    }
}
