using System;
using System.Linq;
using Color = System.Drawing.Color;
using SharpDX;
using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.Events;
using EnsoulSharp.SDK.MenuUI;
using EnsoulSharp.SDK.MenuUI.Values;
using EnsoulSharp.SDK.Prediction;
using EnsoulSharp.SDK.Utility;

namespace Swain
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            GameEvent.OnGameLoad += OnGameLoad;
        }
        private static void OnGameLoad()
        {
            if (ObjectManager.Player.CharacterName != "Swain")
                return;

            Swain.OnLoad();
        }
    }
    internal class MenuSettings
    {
        public class Combo
        {
            public static readonly MenuBool c = new MenuBool("c", "Combo Active");
            public static readonly MenuBool cQ = new MenuBool("cQ", "Use Q");
            public static readonly MenuBool cW = new MenuBool("cW", "Use W");
            public static readonly MenuBool cE = new MenuBool("cE", "Use E");
            public static readonly MenuBool cR = new MenuBool("cR", "Use R");
        }
        public class Harass
        {
            public static readonly MenuBool h = new MenuBool("h", "Harass Active");
            public static readonly MenuBool hQ = new MenuBool("hQ", "Use Q");
            public static readonly MenuBool hW = new MenuBool("hW", "Use W");
            public static readonly MenuBool hE = new MenuBool("hE", "Use E");
        }
        public class LaneClear
        {

            public static readonly MenuBool l = new MenuBool("l", "LaneClear Active");
            public static readonly MenuBool lQ = new MenuBool("useQ", "Use Q");
            public static readonly MenuBool lW = new MenuBool("useW", "Use W");
        }
        public class JungleClear
        {
            public static readonly MenuBool j = new MenuBool("j", "JungleClear Active");
            public static readonly MenuBool jQ = new MenuBool("useQ", "Use Q");
            public static readonly MenuBool jW = new MenuBool("useW", "Use W");
        }
        public class LastHit
        {
            public static readonly MenuBool l = new MenuBool("l", "LastHit Active");
            public static readonly MenuBool lQ = new MenuBool("useQ", "Use Q");
            public static readonly MenuBool lW = new MenuBool("useW", "Use W");
        }
        public class Drawing
        {
            public static readonly MenuBool d = new MenuBool("d", "Drawing Active");
            public static readonly MenuBool dR = new MenuBool("dr", "Drawing R gapcloser");

            public static readonly MenuBool dQ = new MenuBool("dQ", "Draw Q");
            public static readonly MenuBool dW = new MenuBool("dW", "Draw W");
            public static readonly MenuBool dE = new MenuBool("dE", "Draw E");

        }
        public class clear
        {
            public static MenuList qStackMode = new MenuList("qStackMode", "Select Mode:", new[] { "LastHit 1 Minion", "LastHit >= 2 Minions" }, 2);
        }
    }
    internal class Swain
    {
        private static SpellSlot Flash;
        private static SpellSlot Ignite;
        private static Spell Q, W, E, R;
        private static AIHeroClient me = ObjectManager.Player;
        private static Menu mn;

        public static void OnLoad()
        {
            Q = new Spell(SpellSlot.Q, 600f);
            Q.SetSkillshot(0f, 200, 99999, false, SkillshotType.Cone);

            W = new Spell(SpellSlot.W, 3500f);
            W.SetSkillshot(1f, 250, 0, false, SkillshotType.Circle);

            E = new Spell(SpellSlot.E, 850f);
            E.SetSkillshot(0.5f, 100, 1200, false, SkillshotType.Line, HitChance.High);

            R = new Spell(SpellSlot.R, 650f);

            Ignite = me.GetSpellSlot("summonerdot");
            Flash = me.GetSpellSlot("SummonerFlash");



            mn = new Menu(me.CharacterName, "Swain by ProDragon", true);

            #region Menu Init

            var comboMenu = new Menu("comboMenu", "Combo")
            {
                MenuSettings.Combo.c
                //MenuSettings.Combo.cQ,
                //MenuSettings.Combo.cW,
                //MenuSettings.Combo.cE,
                //MenuSettings.Combo.cR,
            };
            mn.Add(comboMenu);

            var harassMenu = new Menu("harassMenu", "Harass")
            {
                MenuSettings.Harass.h
                //MenuSettings.Harass.hQ,
                //MenuSettings.Harass.hW,
                //MenuSettings.Harass.hE
            };
            mn.Add(harassMenu);

            var laneClearMenu = new Menu("laneClearMenu", "Lane Clear")
            {
                MenuSettings.LaneClear.l
                //MenuSettings.LaneClear.lQ,
                //MenuSettings.LaneClear.lW
            };
            mn.Add(laneClearMenu);

            var jungleClearMenu = new Menu("jungleClearMenu", "Jungle Clear")
            {
                MenuSettings.JungleClear.j
                //MenuSettings.JungleClear.jQ,
                //MenuSettings.JungleClear.jW
            };
            mn.Add(jungleClearMenu);

            var lastHitMenu = new Menu("lastHitMenu", "Last Hit")
            {
                MenuSettings.LastHit.l
                //MenuSettings.LastHit.lQ,
                //MenuSettings.LastHit.lW

            };
            mn.Add(lastHitMenu);

            var drawingMenu = new Menu("drawingMenu", "Drawing")
            {
                MenuSettings.Drawing.d,
                MenuSettings.Drawing.dR
                //MenuSettings.Drawing.dQ,
                //MenuSettings.Drawing.dW,
                //MenuSettings.Drawing.dE,
            };
            mn.Add(drawingMenu);
            mn.Add(MenuSettings.clear.qStackMode);

            mn.Attach();

            #endregion

            Tick.OnTick += OnUpdate;
            Drawing.OnDraw += OnDraw;
        }
        private static void OnUpdate(EventArgs args)
        {
            if (me.IsDead || me.IsRecalling())
                return;
            if (MenuGUI.IsChatOpen || MenuGUI.IsShopOpen)
                return;

            switch (Orbwalker.ActiveMode)
            {
                case OrbwalkerMode.Combo:
                    Combo();
                    break;
                case OrbwalkerMode.Harass:
                    break;
                case OrbwalkerMode.LaneClear:
                    clear();
                    break;
                case OrbwalkerMode.LastHit:
                    break;
            }
        }


        public static void OnDraw(EventArgs args)
        {
            if (MenuSettings.Drawing.d.Enabled)
            {

                if (me.IsDead)
                {
                    return;
                }

                if (Q.IsReady())
                {
                    Render.Circle.DrawCircle(me.Position, Q.Range, Color.Red, 50);
                }
                if (E.IsReady())
                {
                    Render.Circle.DrawCircle(me.Position, E.Range, Color.Yellow, 1);
                }
                if (R.IsReady())
                {
                    Render.Circle.DrawCircle(me.Position, E.Range, Color.Red, 5);
                }
            }
        }

        /*public static void useQcombo()
        {
            var target = TargetSelector.GetTarget(Q.Range, true);
            {
                if (target.IsValidTarget(Q.Range))
                {
                    Q.Cast(target.Position);
                }
            }
        }

        public static void useWcombo()
        {
            var t = TargetSelector.GetTarget(W.Range - 50, true);
            if (t.IsValidTarget(W.Range - 50))
            {
                W.Cast(t.Position);
            }                
        }

        public static void useEcombo()
        {
            var t = TargetSelector.GetTarget(E.Range - 100, true);
            if (t.IsValidTarget(E.Range))
            {
                E.Cast(t);
            }
        }*/

        public static void Combo()
        {
            var tq = TargetSelector.GetTarget(Q.Range);
            if (tq.Health > me.GetSpellDamage(tq, SpellSlot.Q))
            {
                if (tq.IsValidTarget(Q.Range - 100))
                {
                    Q.Cast(tq.Position);
                }
            }
            if (tq.Health <= me.GetSpellDamage(tq, SpellSlot.Q))
            {
                if (tq.IsValidTarget(Q.Range))
                {
                    Q.Cast(tq.Position);
                }
            }
            var tw = TargetSelector.GetTarget(W.Range);
            if (tw.Health <= me.GetSpellDamage(tw, SpellSlot.W))
            {
                if (tq.IsValidTarget(W.Range))
                {
                    W.Cast(tw.PreviousPosition);
                }
            }
            if (tw.Health > me.GetSpellDamage(tw, SpellSlot.W))
            {
                if (tw.DistanceToPlayer() > me.GetRealAutoAttackRange())
                {
                    W.Cast(tw.PreviousPosition);
                }
            }
            var te = TargetSelector.GetTarget(E.Range);
            if (te.Health > me.GetSpellDamage(te, SpellSlot.E))
            {
                if (te.IsValidTarget(E.Range - 100))
                {
                    E.Cast(te.PreviousPosition);
                }
            }
            if (te.Health <= me.GetSpellDamage(te, SpellSlot.E))
            {
                if (te.IsValidTarget(E.Range))
                {
                    E.Cast(te.PreviousPosition);
                }
            }
            var tr = TargetSelector.GetTarget(R.Range);
            if (tr.IsValidTarget(R.Range))
            {
                if (me.HealthPercent < 30)
                {
                    if (tr.HealthPercent > 30)
                    {
                        R.Cast();
                    }
                }
                if (me.HealthPercent < 20)
                {
                    R.Cast();
                }
                if (me.CountEnemyHeroesInRange(500) > 3)
                {
                    R.Cast();
                }
                if(me.GetSpellDamage(tr, SpellSlot.R) + me.GetSpellDamage(tq, SpellSlot.Q) > tr.Health)
                {
                    R.Cast();
                }
            }
        }

        public static void killsteal()
        {
            var tq = TargetSelector.GetTarget(Q.Range);
            var tw = TargetSelector.GetTarget(W.Range);
            var te = TargetSelector.GetTarget(E.Range);
            var tr = TargetSelector.GetTarget(R.Range);

            if (tq.Health <= me.GetSpellDamage(tq, SpellSlot.Q))
            {
                if (tq.IsValidTarget(Q.Range))
                {
                    if (Q.IsReady())
                    {
                        Q.Cast(tq.Position);
                    }
                }
            }
            if (tw.Health <= me.GetSpellDamage(tw, SpellSlot.W))
            {
                if (tw.IsValidTarget(W.Range))
                {
                    if (W.IsReady())
                    {
                        W.Cast(tw.PreviousPosition);
                    }
                }
            }
            if (te.Health <= me.GetSpellDamage(te, SpellSlot.E))
            {
                if (te.IsValidTarget(E.Range))
                {
                    if (E.IsReady())
                    {
                        E.Cast(te.PreviousPosition);
                    }
                }
            }
            if (tr.Health <= me.GetSpellDamage(tr, SpellSlot.R))
            {
                if(tr.IsValidTarget(R.Range))
                {
                    if (R.IsReady())
                    {
                        if (me.HasBuff("SwainR"))
                        {
                            R.Cast();
                        }
                    }
                }
            }

            //killsteall with flash
            if (tq.Health <= me.GetSpellDamage(tq, SpellSlot.Q))
            {
                if (tq.IsValidTarget(Q.Range + 400))
                {
                    if (tq.DistanceToPlayer() > Q.Range)
                    {
                        if (Q.IsReady())
                        {
                            if (Flash.IsReady())
                            {
                                me.Spellbook.CastSpell(Flash, tq.Position);
                                DelayAction.Add(100, () =>
                                {
                                    Q.Cast(tq.Position);
                                });
                            }
                        }
                    }
                }
            }
            if (tr.Health <= me.GetSpellDamage(tr, SpellSlot.R))
            {
                if (tq.IsValidTarget(R.Range + 400))
                {
                    if (tr.DistanceToPlayer() > R.Range)
                    {
                        if (R.IsReady())
                        {
                            if (me.HasBuff("SwainR"))
                            {
                                if (Flash.IsReady())
                                {
                                    R.Cast();
                                    DelayAction.Add(100, () =>
                                    {
                                        me.Spellbook.CastSpell(Flash, tr.Position);
                                    });
                                }
                            }
                        }
                    }
                }
            }
        }
        public static void clear()
        {
            var allMinions = GameObjects.EnemyMinions.Where(x => x.IsMinion() && !x.IsDead).OrderBy(x => x.Distance(me.Position));

            if (allMinions.Count() == 0)
                return;

            if (Q.IsReady())
            {
                if (me.ManaPercent < 40)
                    return;

                foreach (var min in allMinions.Where(x => x.IsValidTarget(Q.Range - 15) && x.Health < Q.GetDamage(x)))
                {
                    var getPrediction = Q.GetPrediction(min, true);
                    var getCollisions = getPrediction.CollisionObjects.ToList();

                    switch (MenuSettings.clear.qStackMode.SelectedValue)
                    {
                        case "LastHit 1 Minion":
                            if (getCollisions.Any() && getCollisions.Count() <= 1)
                            {
                                Q.Cast(getPrediction.CastPosition);
                            }
                            else
                            {
                                Q.Cast(getPrediction.CastPosition);
                            }
                            break;
                        case "LastHit >= 2 Minions":
                            if (getCollisions.Any() && (getCollisions.Count() == 1 && getCollisions.FirstOrDefault().Health < Q.GetDamage(getCollisions.FirstOrDefault()) - 10))
                            {
                                Q.Cast(getPrediction.CastPosition);
                            }
                            else if (getCollisions.Count() >= 2 && getCollisions[0].Health < Q.GetDamage(getCollisions[0]) / 3 && getCollisions[1].Health < Q.GetDamage(getCollisions[1]) / 3)
                            {
                                Q.Cast(getPrediction.CastPosition);
                            }
                            break;
                    }
                }

            }
        }
    }
}
