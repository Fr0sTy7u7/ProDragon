using System;
using EnsoulSharp;
using EnsoulSharp.SDK;
using BlankAIO.Util;
using ActiveGapcloser = BlankAIO.Util.ActiveGapcloser;
using BlankAIO.Champions.Helpers;
using EnsoulSharp.SDK.Prediction;
using EnsoulSharp.SDK.MenuUI.Values;
using EnsoulSharp.SDK.MenuUI;

namespace BlankAIO.Champions
{
    public class Lulu
    {
        private static Spell Q, W, E, R;

        private static string ChampionName = "Lulu";
        private static AIHeroClient Player;
        private static Menu MainMenu;

        public static SpellSlot IgniteSlot;

        public static int nextJungleScan = 0;
        public static string[] jungleMobNames = new[] { "sru_blue", "sru_dragon", "sru_baron" };

        public static void OnLoad()
        {
            MainMenu = new Menu("BlankAIO Lulu", "BlankAIO Lulu");

            #region Combo Menu
            var comboMenu = new Menu("Combo", "Combo Settings");

            comboMenu.Add(new MenuBool("comboQ", "Use Q", true));

            MainMenu.Add(comboMenu);
            #endregion

            #region Farm Menu

            var farmMenu = new Menu("LaneClear", "Lane Clear Settings");

            farmMenu.Add(new MenuBool("useqfarm", "Use Q to Lane Clear", true));
            farmMenu.Add(new MenuBool("useefarm", "Use E to Lane Clear", true));

            MainMenu.Add(farmMenu);
            #endregion

            #region Jungle Farm Menu

            var jungleFarmMenu = new Menu("JungleFarm", "Jungle Clear Settings");

            jungleFarmMenu.Add(new MenuBool("useqjfarm", "Use Q to Jungle Clear"));
            jungleFarmMenu.Add(new MenuBool("useejfarm", "Use E to Jungle Clear"));

            MainMenu.Add(jungleFarmMenu);
            #endregion

            #region W Settings

            var wMenu = new Menu("wsettings", "W Settings");

            wMenu.Add(new MenuBool("winterrupt", "Interrupt Spells using W (Polymorph)"));
            wMenu.Add(new MenuBool("usewkite", "Use W to Kite (On Self)"));
            wMenu.Add(new MenuSlider("usewkited", "W Kite Distance", 300, 0, 500));

            MainMenu.Add(wMenu);
            #endregion

            #region E Settings

            var eMenu = new Menu("esettings", "E Settings");

            eMenu.Add(new MenuBool("autoeks", "Use E to KS", false));
            eMenu.Add(new MenuBool("autoeksjgl", "Use E to KS Blue Buff / Drake / Baron", false));

            MainMenu.Add(eMenu);

            #endregion

            #region R Settings

            var rMenu = new Menu("rsettings", "R Settings");

            rMenu.Add(new MenuBool("rinterrupt", "Automatically Interrupt dangerous spells using R"));
            rMenu.Add(new MenuBool("autorlow", "Auto R LowHP Allies"));
            rMenu.Add(new MenuSlider("autorhp", "%hp to Auto-Ult Allies", 15, 1, 50));

            MainMenu.Add(rMenu);
            #endregion

            #region Funneling

            var funnelMenu = new Menu("funnelmenu", "Funneling Menu (WIP, NOT WORKING)");

            funnelMenu.Add(new MenuBool("funnelonoff", "Funnel Targetted Allies"));
            funnelMenu.Add(new MenuBool("useontwult", "Use W and E on Twitch Ultimate"));
            funnelMenu.Add(new MenuBool("useonyiult", "Use W and E on Master Yi's Ultimate"));
            funnelMenu.Add(new MenuBool("warning", "WARNING ! THIS WILL OVERRIDE COMBO SETTINGS"));
            funnelMenu.Add(new MenuBool("useonkogcombo", "Use W and E on Kog'Maw upon orbwalking as Combo"));

            MainMenu.Add(funnelMenu);
            #endregion

            Player = ObjectManager.Player;
            if (Player.CharacterName != ChampionName)
                return;

            PixManager.DrawPix = true;

            Q = new Spell(SpellSlot.Q, 950);
            W = new Spell(SpellSlot.W, 650);
            E = new Spell(SpellSlot.E, 650);
            R = new Spell(SpellSlot.R, 900);

            Game.Print("Blank AIO, Lulu Loaded. This version is using the SDK orbwalker, please select 'SDK' with Orbwalker Selector.");

            Q.SetSkillshot(0.25f, 60f, 1450f, false, true, SkillshotType.Line);

            IgniteSlot = Player.GetSpellSlot("SummonerDot");

            EnsoulSharp.SDK.Events.Tick.OnTick += Game_OnUpdate;
        }

        static void Game_OnUpdate(EventArgs args)
        {
            switch (Orbwalker.ActiveMode)
            {
                case OrbwalkerMode.Combo:
                    Combo();
                    break;
            }

            if (MainMenu["E Settings"]["autoeks"].GetValue<MenuBool>().Enabled && E.IsReady())
            {
                ImABitch();
            }

            if (MainMenu["E Settings"]["autoeksjgl"].GetValue<MenuBool>().Enabled && E.IsReady())
            {
                JungleKS();
            }

            if (MainMenu["W Settings"]["usewkite"].GetValue<MenuBool>().Enabled && W.IsReady())
            {
                var d = MainMenu["W Settings"]["usewkited"].GetValue<MenuSlider>();
                if (Player.CountEnemyHeroesInRange(d.Value) >= 1)
                {
                    W.Cast(Player);
                }
            }

            if (MainMenu["R Settings"]["autorlow"].GetValue<MenuBool>().Enabled && R.IsReady())
            {
                foreach (var ally in GameObjects.AllyHeroes)
                {
                    if (ally.IsValidTarget(R.Range, false))
                    {
                        var e = MainMenu["R Settings"]["autorhp"].GetValue<MenuSlider>();
                        if (e >= 1 + 1 + 1 || ally.HealthPercent <= 15 && e >= 1)
                        {
                            R.Cast(ally);
                        }
                    }
                }

                var ec = Player.CountEnemyHeroesInRange(300);
                if (ec >= 1 + 1 + 1 || Player.HealthPercent <= 15 && ec >= 1)
                {
                    R.Cast(Player);
                }
            }
        }

        private static void JungleKS()
        {
            throw new NotImplementedException();
        }

        static void ShootQ(bool useE = true)
        {
            if (!Q.IsReady())
            {
                return;
            }

            AIBaseClient pixTarget = null;
            if (PixManager.Pix != null)
            {
                pixTarget = TargetSelector.GetTarget(Q.Range);
            }

            AIBaseClient luluTarget = TargetSelector.GetTarget(Q.Range);

            var pixTargetEffectiveHealth = pixTarget != null ? pixTarget.Health * (1 + pixTarget.SpellBlock / 100f) :
            float.MaxValue;
            var luluTargetEffectiveHealth = luluTarget != null ? luluTarget.Health * (1 + luluTarget.SpellBlock / 100f) :
            float.MaxValue;

            var target = pixTargetEffectiveHealth * 1.2f > luluTargetEffectiveHealth ? luluTarget : pixTarget;
            var flag = false;
            if (target != null)
            {
                Q.From = Player.Position;
                Q.RangeCheckFrom = Player.Position;
                if (!useE || !E.IsReady() || Q.From.Distance(target.Position) < Q.Range - 100)
                {
                    Q.Cast(target);
                }
                flag = true;
            }
        }

        static void Combo()
        {
            ShootQ();

            var eTarget = TargetSelector.GetTarget(E.Range, false);
            if (eTarget != null)
            {
                E.Cast(eTarget);
            }

            var comboDamage = GetComboDamage(eTarget);

            if (eTarget != null && Player.Distance(eTarget) < 600 && IgniteSlot != SpellSlot.Unknown &&
                Player.Spellbook.CanUseSpell(IgniteSlot) == SpellState.Ready)
            {
                if (comboDamage > eTarget.Health)
                {
                    Player.Spellbook.CastSpell(IgniteSlot, eTarget);
                }
            }
        }

        static void ImABitch()
        {

        }

        public static float GetComboDamage(AIHeroClient target)
        {
            var result = 0f;

            if (target == null)
            {
                return 0f;
            }

            if (Q.IsReady())
            {
                result += 2 * Q.GetDamage(target);
            }

            if (E.IsReady())
            {
                result += E.GetDamage(target);
            }

            result += 3 * (float)Player.GetAutoAttackDamage(target);

            return result;
        }
    }
}
