using System;

using System.Linq;
using EnsoulSharp;
using Extensions = SupportAIO.Common.Extensions;
using Color = System.Drawing.Color;
using Menu = EnsoulSharp.SDK.MenuUI.Menu;
using Spell = EnsoulSharp.SDK.Spell;
using Champion = SupportAIO.Common.Champion;
using System.Windows.Forms;
using EnsoulSharp.SDK.MenuUI.Values;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.Prediction;
using EnsoulSharp.SDK.Utility;

namespace SupportAIO.Champions
{
    class Sona : Champion
    {
        private int language;

        internal Sona()
        {
            this.SetSpells();
            this.SetMenu();
            this.SetEvents();
        }

        protected override void Combo()
        {
            bool useQ = RootMenu["combo"]["useq"];
            bool useW = RootMenu["combo"]["usew"];
            bool useE = RootMenu["combo"]["usee"];
            bool useR = RootMenu["combo"]["user"];

            var target = Extensions.GetBestEnemyHeroTargetInRange(R.Range);

            if (!target.IsValidTarget())
            {

                return;
            }

            if (target.IsValidTarget(Q.Range) && useQ)
            {                   
                Q.CastOnUnit(target);
                DelayAction.Add(100, () =>
                {
                    E.Cast();
                });
            }

            if(ObjectManager.Player.HasBuffOfType(BuffType.Slow))
            {
                E.Cast();
            }

            if (ObjectManager.Player.MaxHealth - ObjectManager.Player.Health > 25 * ObjectManager.Player.Level )
            {
                W.Cast();
            }

            var bestTarget = Extensions.GetBestKillableHero(Q, DamageType.Magical, false);
            if (bestTarget != null &&
                Player.GetSpellDamage(bestTarget, SpellSlot.R) >= bestTarget.Health && bestTarget.IsValidTarget(R.Range) && R.IsReady())
            {
                R.Cast(bestTarget);
            }

            if (bestTarget != null &&
                Player.GetSpellDamage(bestTarget, SpellSlot.R) + Player.GetSpellDamage(bestTarget, SpellSlot.Q)  >= bestTarget.Health && bestTarget.IsValidTarget(Q.Range) && R.IsReady())
            {
                R.Cast(bestTarget);
                DelayAction.Add(100, () =>
                {
                    Q.Cast();
                });
            }
        }

        protected override void Drawings()
        {
            if (RootMenu["drawings"]["drawr"])
            {
                Render.Circle.DrawCircle(Player.Position, R.Range, Color.Crimson);
            }
            if (RootMenu["drawings"]["drawq"])
            {
                Render.Circle.DrawCircle(Player.Position, Q.Range, Color.Yellow);
            }
        }

        protected override void Killsteal()
        {

        }


        protected override void Harass()
        {
            bool useQ = RootMenu["harass"]["useq"];
            bool useW = RootMenu["harass"]["usew"];
            bool useE = RootMenu["harass"]["usee"];



            var target = Extensions.GetBestEnemyHeroTargetInRange(R.Range);

            if (!target.IsValidTarget())
            {

                return;
            }

            if (target.IsValidTarget(Q.Range) && useQ)
            {
                Q.Cast(target);
            }
        }
        
        public static void heal()
        {
            if (ObjectManager.Player.MaxHealth - ObjectManager.Player.Health > 25 * ObjectManager.Player.Level)
            {
                W.Cast();
            }
        }


         
        protected override void SetSpells()
        {

            Q = new Spell(SpellSlot.Q, 850);
            W = new Spell(SpellSlot.W, 1000);
            E = new Spell(SpellSlot.E, 350);
            R = new Spell(SpellSlot.R, 1000);

            Q.SetTargetted(0.5f, 1200);
            R.SetSkillshot(0.5f, 125, 3000f, false, SkillshotType.Line);

        }

        protected override void SemiR()
        {
            if (RootMenu["combo"]["semir"].GetValue<MenuKeyBind>().Active)
            {
                var target = Extensions.GetBestEnemyHeroTargetInRange(R.Range);

                if (!target.IsValidTarget())
                {

                    return;
                }


                if (target != null &&
                    target.IsValidTarget(R.Range))
                {
                    var pred = R.GetPrediction(target);
                    if (pred.Hitchance >= HitChance.High)
                    {
                        R.Cast(pred.CastPosition, true);
                    }
                }
            }
        }

        protected override void SetMenu()
        {
            ComboMenu = new Menu("combo", "Combo");
            {
                ComboMenu.Add(new MenuBool("useq", "Q"));
                ComboMenu.Add(new MenuBool("usew", "W"));
                ComboMenu.Add(new MenuBool("usee", "E"));
                ComboMenu.Add(new MenuBool("user", "R"));
                ComboMenu.Add(new MenuKeyBind("semir", "Semi R", Keys.T, KeyBindType.Press));


            }
            RootMenu.Add(ComboMenu);
            var HarassMenu = new Menu("harass", "Harass");
            {
                HarassMenu.Add(new MenuBool("useq", "Q"));
            }
            RootMenu.Add(HarassMenu);

            DrawMenu = new Menu("drawings", "Draw");
            {
                DrawMenu.Add(new MenuBool("drawq", "Q"));
                DrawMenu.Add(new MenuBool("drawr", "R"));
            }
            RootMenu.Add(DrawMenu);
        }

        protected override void Farming()
        {

        }
    }
}
