using System;
using System.Collections.Generic;
using System.Linq;
using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;
using EnsoulSharp.SDK.MenuUI.Values;
using EnsoulSharp.SDK.Prediction;
using EnsoulSharp.SDK.Utility;
using SharpDX;
using System.Drawing;

using Color = System.Drawing.Color;
namespace PoisonTwitch
{
    public class Twitch
    {

        /*
            Codded by CutePoison 
            Thx for help https://github.com/dauthleikr
            TO-DO

            1-I will add check to Attack Press Rune activate.
            2-Auto-Smite
            3-Lulu Funnel
            4-Skin Changer maybe?

        */

        private static Spell Q, W, E, R;
        private static Menu MainMenu;

        private static readonly float[] BaseDamage = { 20, 30, 40, 50, 60 };
        private static readonly float[] StackDamage = { 15, 20, 25, 30, 35 };
        private static readonly float[] MaxDamage = { 110, 150, 190, 230, 270 };

        public class ComboTwitch
        {
            public static readonly MenuBool W = new MenuBool("usew", "Use W");
            public static readonly MenuBool E = new MenuBool("usee", "Use E");
            public static readonly MenuBool R = new MenuBool("user", "Use R");
            public static readonly MenuSlider Rslider = new MenuSlider("rslider", "Use R for players count", 1, 1, 5);
            public static readonly MenuBool sW = new MenuBool("startw", "Start with W");
        }

        public class Draws
        {
            public static readonly MenuBool DQ = new MenuBool("DQ", "Draw Q Range");
            public static readonly MenuBool DW = new MenuBool("DW", "Draw W Range");
            public static readonly MenuBool DE = new MenuBool("DE", "Draw E Range");
            public static readonly MenuBool DR = new MenuBool("DR", "Draw R Range");
        }

        public class ClearSet
        {
            public static readonly MenuBool CW = new MenuBool("CW", "Use W to farm");
            public static readonly MenuBool CE = new MenuBool("CE", "Use E to farm");
            public static readonly MenuSlider WSlider = new MenuSlider("wslider", "Keep X% mana", 1, 1, 100);
            public static readonly MenuSlider useEfarm = new MenuSlider("usee2", "Use E to Minions", 1, 1, 5);
            public static readonly MenuBool useE = new MenuBool("usee", "Wait stacks for Dragon/Baron");
            public static readonly MenuSlider useW = new MenuSlider("usew", "Use W Minions to hit", 1, 1, 5);
        }

        public class KillSteal
        {
            public static readonly MenuBool kE = new MenuBool("kE", "Killsteal with E");
        }

        public class VenomSet
        {
            public static readonly MenuSlider venomSlider = new MenuSlider("venomslider", "Venom", 1, 1, 6);
        }

        public static void OnLoad()
        {
            
            //Spells
            Q = new Spell(SpellSlot.Q, 500f);

            W = new Spell(SpellSlot.W, 950f);
            W.SetSkillshot(0.25f, 80f, 1400f, false, SkillshotType.Circle);

            E = new Spell(SpellSlot.E, 1200f);
            R = new Spell(SpellSlot.R, 850f);

            Menus();

            Game.Print("Poison Twitch Loaded! by Cute Poison x)");

            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
        }

        private static void OnUpdate(EventArgs args)
        {
            if (ObjectManager.Player.IsDead || ObjectManager.Player.IsRecalling()) return;

            if (MenuGUI.IsChatOpen || MenuGUI.IsShopOpen) return;

            switch (Orbwalker.ActiveMode)
            {
                case OrbwalkerMode.Combo:
                    Combo();
                    break;
                case OrbwalkerMode.LaneClear:
                    WaveClear();
                    Jungle();
                    break;

            }
            KillStealE();
        }


        private static float GetQTime()
        {
            var buff = ObjectManager.Player.GetBuff("TwitchHideInShadows");
            if (buff == null && Q.State == SpellState.Ready) return Q.Level + 9f;
            if (buff == null) return 0;
            return buff.EndTime - Game.Time;
        }

        public static List<AIMinionClient> GetGenericJungleMinionsTargets()
        {
            return GetGenericJungleMinionsTargetsInRange(float.MaxValue);
        }

        public static List<AIMinionClient> GetGenericJungleMinionsTargetsInRange(float range)
        {
            return GameObjects.Jungle.Where(m => !GameObjects.JungleSmall.Contains(m) && m.IsValidTarget(range)).ToList();
        }

        public static List<AIMinionClient> GetEnemyLaneMinionsTargets()
        {
            return GetEnemyLaneMinionsTargetsInRange(float.MaxValue);
        }

        public static List<AIMinionClient> GetEnemyLaneMinionsTargetsInRange(float range)
        {
            return GameObjects.EnemyMinions.Where(m => m.IsValidTarget(range)).ToList();
        }

        private static float GetActivateDamage(AIHeroClient target, int targetBuffCount = 0)
        {
            int Level = E.Level;
            if (targetBuffCount == 0) return 0;
            return (float)ObjectManager.Player.CalculateDamage(target, DamageType.Physical, Math.Min(MaxDamage[Level - 1] + ObjectManager.Player.TotalAttackDamage * 1.5f, targetBuffCount * (StackDamage[Level - 1] + ObjectManager.Player.TotalAttackDamage * 0.15f) + BaseDamage[Level - 1]));
        }

        private static float GetEPassiveAndActivateDamage(AIHeroClient target, int targetBuffCount = 0)
        {
            if (targetBuffCount == 0) return 0;
            return (float)GetDeadlyVenom(target) + GetActivateDamage(target, targetBuffCount);
        }

        public static float GetDeadlyVenom(AIHeroClient target)
        {
            var buff = target.GetBuff("twitchdeadlyvenom");
            if (buff == null) return 0f;
            return (float)(ObjectManager.Player.CalculateDamage(target, DamageType.True, ((int)(buff.EndTime - Game.Time) + 1) * GetVenomTicks() * buff.Count)) - ((int)(buff.EndTime - Game.Time)) * target.HPRegenRate;
        }

        public static int GetVenomTicks()
        {
            if (ObjectManager.Player.Level > 16) return 5;
            if (ObjectManager.Player.Level > 12) return 4;
            if (ObjectManager.Player.Level > 8) return 3;
            if (ObjectManager.Player.Level > 4) return 2;
            return 1;

        }

        private static void Jungle()
        {
            foreach (var jung in GetGenericJungleMinionsTargetsInRange(E.Range))
            {
                if (jung.IsValidTarget() && E.IsReady() && jung.GetBuffCount("twitchdeadlyvenom") >= 1 && jung.GetJungleType() == JungleType.Legendary && ClearSet.useE.Enabled)
                {
                    if (jung.Health <= E.GetDamage(jung))
                    {
                        E.Cast();
                    }
                }
            }

            foreach (var jung in GetGenericJungleMinionsTargetsInRange(E.Range))
            {
                if (jung.IsValidTarget() && E.IsReady() && VenomSet.venomSlider.Value <= jung.GetBuffCount("twitchdeadlyvenom"))
                {
                    E.Cast();
                    return;
                }
            }
            foreach (var jung in GetGenericJungleMinionsTargetsInRange(W.Range))
            {
                if (jung.IsValidTarget() && W.IsReady() && ClearSet.CW.Enabled && ObjectManager.Player.Mana >= W.Mana + E.Mana)
                {
                    var pred = W.GetPrediction(jung);
                    if (pred.Hitchance >= HitChance.Low)
                        W.Cast(pred.CastPosition);
                    return;
                }
            }

        }

        private static void WaveClear()
        {
            var minionsE = GetEnemyLaneMinionsTargetsInRange(E.Range);

                if (ClearSet.CE.Enabled && E.IsReady() && ObjectManager.Player.Mana >= E.Mana)
                {
                    if(minionsE.Count >= ClearSet.useEfarm.Value){
                        foreach (var minion in minionsE)
                        {

                            if (minion.HasBuff("twitchdeadlyvenom") && minion.Health <= E.GetDamage(minion))
                            {
                                E.Cast();
                                return;
                            }
                        }
                    }
                }


            var minionsW = GetEnemyLaneMinionsTargetsInRange(W.Range);
            foreach (var minion in minionsW)
            {
                if (ClearSet.CW.Enabled && minion.IsValidTarget() && W.IsReady() && ObjectManager.Player.Mana >= W.Mana + E.Mana && ObjectManager.Player.ManaPercent >= ClearSet.WSlider.Value)
                {
                    var loc = W.GetCircularFarmLocation(GetEnemyLaneMinionsTargetsInRange(W.Range));

                    if (loc.MinionsHit >= ClearSet.useW.Value && loc.Position.IsValid())
                    {
                        W.Cast(loc.Position);
                        return;
                    }
                }
            }

        }

        private static void Combo()
        {


            if ((ComboTwitch.sW.Enabled || ComboTwitch.W.Enabled) && W.IsReady() && ObjectManager.Player.Mana >= W.Mana + E.Mana)
            {
                var target = TargetSelector.GetTarget(W.Range);
                if (target != null && target.IsValidTarget(W.Range))
                {
                    var pred = W.GetPrediction(target);
                    if (pred.Hitchance >= HitChance.Low)
                        W.Cast(pred.CastPosition);
                }
            }

            if (ComboTwitch.R.Enabled && R.IsReady() && ObjectManager.Player.Mana >= R.Mana)
            {
                var target = GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(R.Range) && !x.IsInvulnerable);
                if(target.Count() >= ComboTwitch.Rslider.Value)
                R.Cast();
            }

            if (ComboTwitch.E.Enabled && E.IsReady() && ObjectManager.Player.Mana >= E.Mana)
            {
                var target = TargetSelector.GetTarget(E.Range);
                if (target != null && target.IsValidTarget(E.Range) && !target.IsInvulnerable && KillableE(target))
                {
                    E.Cast();
                }
            }

        }

        private static void KillStealE()
        {
            if (KillSteal.kE.Enabled)
            {
                var targets = GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(E.Range) && !x.IsInvulnerable);
                foreach (var target in targets)
                {
                    if (target.IsValidTarget(E.Range) && KillableE(target) && E.IsReady() && ObjectManager.Player.Mana >= E.Mana)
                        E.Cast();
                }
            }
        }

        private static bool KillableE(AIHeroClient target)
        {
            var targetBuffCount = target.GetBuffCount("twitchdeadlyvenom");
            if (targetBuffCount == 0) return false;

            return GetEPassiveAndActivateDamage(target, targetBuffCount) >= target.Health;
        }

        private static void OnDraw(EventArgs args)
        {
            if (ObjectManager.Player.IsDead)
                return;

            if (Draws.DQ.Enabled)
            {
                var sTime = GetQTime();
                if (sTime > 0)
                    Render.Circle.DrawCircle(GameObjects.Player.Position, sTime * ObjectManager.Player.MoveSpeed, Color.Black);

            }
             if (Draws.DW.Enabled && W.IsReady())
                    Render.Circle.DrawCircle(GameObjects.Player.Position, W.Range, Color.White);
             if (Draws.DE.Enabled && E.IsReady())
                    Render.Circle.DrawCircle(GameObjects.Player.Position, E.Range, Color.Red);
             if (Draws.DR.Enabled && R.IsReady())
                    Render.Circle.DrawCircle(GameObjects.Player.Position, R.Range, Color.Blue);
        }

        public static void Menus()
        {

            //main menu
            MainMenu = new Menu("poisontwitch", "Poison Twitch", true);

            //combo
            var comboMenu = new Menu("combo", "Combo");
            comboMenu.Add(ComboTwitch.W);
            comboMenu.Add(ComboTwitch.E);
            comboMenu.Add(ComboTwitch.R);
            comboMenu.Add(ComboTwitch.sW);
            comboMenu.Add(ComboTwitch.Rslider);

            //draw settings
            var drawMenu = new Menu("draws", "Draws");
            drawMenu.Add(Draws.DQ);
            drawMenu.Add(Draws.DW);
            drawMenu.Add(Draws.DE);
            drawMenu.Add(Draws.DR);

            //killsteal
            var killStealMenu = new Menu("killsteal", "KillSteal");
            killStealMenu.Add(KillSteal.kE);

            //venom settings
            var venomMenu = new Menu("venom", "Venom Settings");
            venomMenu.Add(VenomSet.venomSlider);

            //clear settings
            var clearMenu = new Menu("clear", "Clear Settings");
            clearMenu.Add(ClearSet.CW);
            clearMenu.Add(ClearSet.WSlider);
            clearMenu.Add(ClearSet.CE);
            clearMenu.Add(ClearSet.useEfarm);
            clearMenu.Add(ClearSet.useE);
            clearMenu.Add(ClearSet.useW);

            //into the main menu
            MainMenu.Add(comboMenu);
            MainMenu.Add(drawMenu);
            MainMenu.Add(killStealMenu);
            MainMenu.Add(venomMenu);
            MainMenu.Add(clearMenu);
            MainMenu.Attach();

        }
    }
}
