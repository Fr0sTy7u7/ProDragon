namespace EnsoulSharp.Ashe
{
    using System.Windows.Forms;

    using EnsoulSharp.SDK.MenuUI.Values;

    internal class MenuWrapper
    {
        public class Combat
        {
            public static readonly MenuBool Q = new MenuBool("q", "Use Q");
            public static readonly MenuBool QAA = new MenuBool("qaa", "^ Use Q reset AA", false);
            public static readonly MenuList QW = new MenuList("qw", "^ Use Q when", new[] { "Always", "After Attack", "Before Attack" });
            public static readonly MenuSlider QC = new MenuSlider("qc", "Use Q - Aoe check enermy around", 2,1,5);
            public static readonly MenuBool W = new MenuBool("w", "Use W");
            public static readonly MenuBool WAfterAA = new MenuBool("wa", "^ On AfterAttack");
            public static readonly MenuSlider WC = new MenuSlider("wc", "Use W - Aoe check enermy around", 2, 1, 5);
        }

        public class Harass
        {
            public static readonly MenuBool Q = new MenuBool("q", "Use Q");
            public static readonly MenuBool QM = new MenuBool("qm", "Use Q in minion", false);
            public static readonly MenuBool W = new MenuBool("w", "Use W", false);
            public static readonly MenuSlider Mana = new MenuSlider("minmana", "^ Min ManaPercent <= x%", 60,0,100);
        }

        public class Clear
        {
            public static readonly MenuList C = new MenuList("C", "Clear", new[] { "Legit", "Logic" });
        }

        public class JungleClear
        {
            public static readonly MenuBool Q = new MenuBool("q", "Use Q");
            public static readonly MenuBool QKS = new MenuBool("qks", "Use Q only monster can killsteal", false);
            public static readonly MenuBool W = new MenuBool("q", "Use W");
            public static readonly MenuSlider Mana = new MenuSlider("minmana", "^ Min ManaPercent <= x%", 60,0, 100);
        }

        public class KillAble
        {
            public static readonly MenuBool Q = new MenuBool("q", "Use Q");
            public static readonly MenuBool QM = new MenuBool("qm", "Use Q on minion", false);
            public static readonly MenuBool W = new MenuBool("w", "Use W");
            public static readonly MenuBool R = new MenuBool("r", "Use R");
        }

        public class Misc
        {
            public static readonly MenuBool WA = new MenuBool("wa", "Use W AntiGapcloser");
            public static readonly MenuBool WI = new MenuBool("wi", "Use W Interrupt Spell");
        }

        public class BU
        {
            public static readonly MenuBool BaseUlt = new MenuBool("BU", "BaseUlt turn on");
        }
        public class Draw
        {
            public static readonly MenuBool Q = new MenuBool("QD", "Deaw Q range");
            public static readonly MenuBool W = new MenuBool("w", "Draw W Range");
            public static readonly MenuBool OnlyReady = new MenuBool("od", "Only Spell Ready");
        }

        public class SemiR
        {
            public static readonly MenuKeyBind Key = new MenuKeyBind("semir", "Semi R Key", Keys.T, KeyBindType.Press);
        }
    }
}
