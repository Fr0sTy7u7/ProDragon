using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI.Values;
using EnsoulSharp.Ashe;

namespace EnsoulSharp.Ashe
{
    public class MenuManager : BaseUlt
    {
        public static EnsoulSharp.SDK.MenuUI.Menu BaseUltMenu { get; set; }

        public static void LoadMenu()
        {

            //Menu
            BaseUltMenu = new EnsoulSharp.SDK.MenuUI.Menu("xDreammsBaseUlt", "xDreamms BaseUlt", true);

            var settings = new EnsoulSharp.SDK.MenuUI.Menu("Setttings", "Settings");
            settings.AddMenuBool("baseult", "BaseUlt");
            settings.AddMenuBool("showrecalls", "Show Recalls");
            settings.AddMenuBool("showallies", "Show Allies");
            settings.AddMenuBool("showenemies", "Show Enemies");
            settings.AddMenuBool("checkcollision", "Check Collision");
            settings.AddMenuSlider("timeLimit", "FOW Time Limit (SEC)", 0, 0, 120);
            settings.AddMenuKeybind("nobaseult", "No BaseUlt while", Keys.Space, KeyBindType.Press);
            settings.AddMenuSeperator("UseBaseUlt","Use BaseUlt On: ");
            foreach (var aiHeroClient in GameObjects.EnemyHeroes)
            {
                settings.AddMenuBool("target" + aiHeroClient.CharacterName, aiHeroClient.CharacterName);
            }
            BaseUltMenu.Add(settings);

            //var barMenu = new EnsoulSharp.SDK.MenuUI.Menu("BarSettings", "Bar Settings");
            //barMenu.Add(new MenuSlider("BarSettingsX", "X Offsets", 3000,0,3000)).GetValue<MenuSlider>().ValueChanged += OnValueChanged;
            //barMenu.Add(new MenuSlider("BarSettingsY", "Y Offsets", 3000, 0, 3000)).GetValue<MenuSlider>().ValueChanged+= OnValueChanged2;
            //BaseUltMenu.Add(barMenu);
            //BaseUltMenu.Attach();
        }

        private static void OnValueChanged2(object sender, EventArgs eventArgs)
        {
            BaseUltCore.Y = (int)TacticalMap.Y - MenuManager.BarSettingsY;
        }

        private static void OnValueChanged(object sender, EventArgs eventArgs)
        {
            BaseUltCore.X = (int) TacticalMap.X - MenuManager.BarSettingsX;
        }

        public static int BarSettingsX { get { return BaseUltMenu.GetMenuSlider("BarSettings", "BarSettingsX"); } }
        public static int BarSettingsY { get { return BaseUltMenu.GetMenuSlider("BarSettings", "BarSettingsY"); } }

        public static bool baseult { get { return BaseUltMenu.GetMenuBool("Setttings", "baseult"); } }
        public static bool showrecalls { get { return BaseUltMenu.GetMenuBool("Setttings", "showrecalls"); } }
        public static bool showallies { get { return BaseUltMenu.GetMenuBool("Setttings", "showallies"); } }
        public static bool showenemies { get { return BaseUltMenu.GetMenuBool("Setttings", "showenemies"); } }
        public static bool checkcollision { get { return BaseUltMenu.GetMenuBool("Setttings", "checkcollision"); } }
        public static int timeLimit { get { return BaseUltMenu.GetMenuSlider("Setttings", "timeLimit"); } }
        public static bool nobaseult { get { return BaseUltMenu.GetMenuKeybind("Setttings", "nobaseult"); } }
    }
}
