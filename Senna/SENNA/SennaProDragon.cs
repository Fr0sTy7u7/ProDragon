namespace EnsoulSharp.Ashe
{
    using System;
    using System.Linq;

    using EnsoulSharp.SDK;
    using EnsoulSharp.SDK.MenuUI;
    using EnsoulSharp.SDK.Prediction;
    using EnsoulSharp.SDK.Utility;
    using Damage = EnsoulSharp.SDK.Damage;
    using Rectangle = SharpDX.Rectangle;
    using SharpDX;
    using SharpDX.Direct3D9;

    using Color = System.Drawing.Color;

    internal class Senna
    {
        private static Menu MyMenu;
        private static Spell Q, Q1, W, R;

        public static void OnLoad()
        {
            Q = new Spell(SpellSlot.Q, 660f);
            Q1 = new Spell(SpellSlot.Q, 1300);
            W = new Spell(SpellSlot.W, 1175f);
            R = new Spell(SpellSlot.R, 8000f);

            Q.SetTargetted(0.25f, 1400f);
            Q1.SetSkillshot(0.5f, 130, float.MaxValue, false, false, SkillshotType.Line);
            W.SetSkillshot(0.5f, 130, 1200, true, true, SkillshotType.Line, HitChance.High);
            R.SetSkillshot(0.25f, 130, 20000, true, false, SkillshotType.Line, HitChance.VeryHigh);

            MyMenu = new Menu(ObjectManager.Player.CharacterName, "Senna", true);

            var combo = new Menu("Combo", "Combo Settings")
            {
                MenuWrapper.Combat.Q,
                MenuWrapper.Combat.QAA,
                MenuWrapper.Combat.QW,
                //MenuWrapper.Combat.QC,
                MenuWrapper.Combat.W,
                MenuWrapper.Combat.WC,
                MenuWrapper.Combat.WAfterAA
            };
            MyMenu.Add(combo);

            var harass = new Menu("Harass", "Harass Settings")
            {
                MenuWrapper.Harass.Q,
                MenuWrapper.Harass.W,
                MenuWrapper.Harass.Mana
            };
            MyMenu.Add(harass);

            var clear = new Menu("clear", "Clear Settings")
            {
                MenuWrapper.Clear.C
            };
            MyMenu.Add(clear);

            var jungle = new Menu("JungleClear", "JungleClear Settings")
            {
                MenuWrapper.JungleClear.Q,
                MenuWrapper.JungleClear.Mana
            };
            MyMenu.Add(jungle);

            var killable = new Menu("KillSteal", "KillSteal Settings")
            {
                MenuWrapper.KillAble.Q,
                MenuWrapper.KillAble.QM,
                MenuWrapper.KillAble.W,
                MenuWrapper.KillAble.R
            };
            MyMenu.Add(killable);

            var misc = new Menu("Misc", "Misc Settings")
            {
                MenuWrapper.Misc.WA,
                MenuWrapper.Misc.WI
            };
            MyMenu.Add(misc);

            //var BU = new Menu("BU", "BaseUlt Settings")
            //{
            //    MenuWrapper.BU.BaseUlt
            //};
            //MyMenu.Add(BU);

            var draw = new Menu("Draw", "Draw Settings")
            {
                MenuWrapper.Draw.W,
                MenuWrapper.Draw.OnlyReady
            };
            MyMenu.Add(draw);

            MyMenu.Add(MenuWrapper.SemiR.Key);

            MyMenu.Attach();

            Game.OnUpdate += OnTick;
            Orbwalker.OnAction += OnOrbwalkerAction;
            Gapcloser.OnGapcloser += OnGapcloser;
            Interrupter.OnInterrupterSpell += OnInterrupterSpell;
            Drawing.OnDraw += OnDraw;
        }

        private static void SemiR()
        {
            var target = TargetSelector.GetTarget(R.Range);
            if (target != null && target.IsValidTarget())
            {
                var rPred = R.GetPrediction(target, false, 0, CollisionObjects.Heroes);
                if (rPred.Hitchance >= HitChance.VeryHigh)
                {
                    R.Cast(rPred.CastPosition);
                }
            }
        }
        
        private static void BaseUlt()
        {
            if(MenuWrapper.BU.BaseUlt.Enabled)
            {
                if(R.IsReady())
                {
                    
                }
            }
        }
        private static void KillAble()
        {
            if (MenuWrapper.KillAble.W.Enabled && W.IsReady())
            {
                foreach (var target in GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(W.Range) && !x.IsInvulnerable))
                {
                    if (target.IsValidTarget() && target.Health < W.GetDamage(target))
                    {
                        var wPred = W.GetPrediction(target, false, 0, CollisionObjects.Minions);
                        if (wPred.Hitchance >= HitChance.High)
                        {
                            W.Cast(wPred.CastPosition);
                        }
                    }
                }
            }

            if (MenuWrapper.KillAble.Q.Enabled && Q.IsReady())
            {
                foreach (var target in GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(Q.Range) && !x.IsInvulnerable))
                {
                    if (target.IsValidTarget() && target.Health < (Q.GetDamage(target) + 100))
                    {
                            Q.Cast(target);                        
                    }
                }
            }

            if (MenuWrapper.KillAble.R.Enabled && R.IsReady())
            {
                var target = TargetSelector.GetTarget(R.Range);
                if (target != null && target.IsValidTarget())
                {
                    var rPred = R.GetPrediction(target, false, 0, CollisionObjects.Heroes);
                    if (target.IsValidTarget() && target.Health < (R.GetDamage(target)))
                    {
                        if (rPred.Hitchance >= HitChance.VeryHigh)
                        {
                            R.Cast(rPred.CastPosition);
                        }
                    }
                }
            }
        }

        private static void Combat()
        {
            if (MenuWrapper.Combat.W.Enabled && W.IsReady())
            {
                var target = TargetSelector.GetTarget(W.Range);
                if (target != null && target.IsValidTarget(W.Range) &&
                    target.DistanceToPlayer() > ObjectManager.Player.GetRealAutoAttackRange(target))
                {
                    var wPred = W.GetPrediction(target, false, 0, CollisionObjects.Minions);
                    if (wPred.Hitchance >= HitChance.High)
                    {
                        W.Cast(wPred.CastPosition);
                    }
                }
            }

            if (MenuWrapper.Combat.Q.Enabled && Q.IsReady())
            {
                Q.CastOnBestTarget(extraRange: 0, areaOfEffect: false, minTargets: 1);
                Orbwalker.ResetAutoAttackTimer();
            }
        }

        private static void Harass()
        {
            if (MenuWrapper.Harass.W.Enabled && W.IsReady())
            {
                var target = TargetSelector.GetTarget(W.Range);
                if (target != null && target.IsValidTarget(W.Range))
                {
                    var wPred = W.GetPrediction(target, false, 0, CollisionObjects.Minions);
                    if (wPred.Hitchance >= HitChance.High)
                    {
                        W.Cast(wPred.CastPosition);
                    }
                }
            }

            if (MenuWrapper.Harass.Q.Enabled && Q.IsReady())
            {
                        Q.Cast();                  
            }
        }

        private static void OnTick(EventArgs args)
        {
            if (ObjectManager.Player.IsDead || ObjectManager.Player.IsRecalling() || MenuGUI.IsChatOpen || ObjectManager.Player.IsWindingUp)
            {
                return;
            }

            if (MenuWrapper.SemiR.Key.Active && R.IsReady())
            {
                SemiR();
            }

            KillAble();

            switch (Orbwalker.ActiveMode)
            {
                case OrbwalkerMode.Combo:
                    Combat();
                    break;
                case OrbwalkerMode.Harass:
                    Harass();
                    break;
            }
        }

        private static void OnOrbwalkerAction(object obj, OrbwalkerActionArgs args)
        {
            if (args.Type == OrbwalkerType.BeforeAttack)
            {
                if (Q.IsReady() && args.Target != null && args.Target.Type == GameObjectType.AIHeroClient)
                {
                    if (Orbwalker.ActiveMode == OrbwalkerMode.Combo)
                    {
                        Q.Cast();
                        Orbwalker.ResetAutoAttackTimer();
                    }
                    else if (Orbwalker.ActiveMode == OrbwalkerMode.Harass)
                    {
                        if (MenuWrapper.Harass.Q.Enabled && ObjectManager.Player.ManaPercent >= MenuWrapper.Harass.Mana.Value)
                        {
                            Q.Cast();
                            Orbwalker.ResetAutoAttackTimer();
                        }
                    }
                }
            }
            else if (args.Type == OrbwalkerType.BeforeAttack)
            {
                if (Orbwalker.ActiveMode == OrbwalkerMode.Combo)
                {
                    if (MenuWrapper.Combat.WAfterAA.Enabled && W.IsReady() && args.Target != null && args.Target.Type == GameObjectType.AIHeroClient)
                    {
                        var target = args.Target as AIHeroClient;
                        var wPred = W.GetPrediction(target);
                        if (wPred.Hitchance >= HitChance.High)
                        {
                            W.Cast(wPred.UnitPosition);
                        }
                    }
                }
                else if (Orbwalker.ActiveMode == OrbwalkerMode.LaneClear)
                {
                    if (ObjectManager.Player.ManaPercent >= MenuWrapper.JungleClear.Mana.Value && MenuWrapper.JungleClear.Q.Enabled && Q.IsReady())
                    {
                        if (args.Target != null && args.Target.Type == GameObjectType.AIMinionClient)
                        {
                            var mob = args.Target as AIMinionClient;
                            if (mob != null && mob.InAutoAttackRange() &&
                                (mob.GetJungleType() == JungleType.Large ||
                                 mob.GetJungleType() == JungleType.Legendary) &&
                                mob.Health > ObjectManager.Player.GetAutoAttackDamage(mob) * 4)
                            {
                                Q.Cast();
                                Orbwalker.ResetAutoAttackTimer();
                            }
                        }
                    }
                }
            }
        }

        private static void OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserArgs args)
        {
            if (MenuWrapper.Misc.WA.Enabled && W.IsReady() && args.EndPosition.DistanceToPlayer() < 600)
            {
                W.Cast(sender.Position);
            }
        }

        private static void OnInterrupterSpell(AIHeroClient target, Interrupter.InterruptSpellArgs args)
        {
            if (MenuWrapper.Misc.WI.Enabled && W.IsReady() && args.DangerLevel >= Interrupter.DangerLevel.Medium && target.DistanceToPlayer() < 660)
            {
                W.Cast(target.Position);
            }
        }

        private static void OnDraw(EventArgs args)
        {
            if (ObjectManager.Player.IsDead || MenuGUI.IsChatOpen)
            {
                return;
            }

            if (MenuWrapper.Draw.W.Enabled)
            {
                if (MenuWrapper.Draw.OnlyReady.Enabled && W.IsReady())
                {
                    Render.Circle.DrawCircle(GameObjects.Player.Position, W.Range, Color.FromArgb(255, 159, 0), 1);
                }
                else if (!MenuWrapper.Draw.OnlyReady.Enabled)
                {
                    Render.Circle.DrawCircle(GameObjects.Player.Position, W.Range, Color.FromArgb(255, 159, 0), 1);
                }
            }

            if (MenuWrapper.Draw.Q.Enabled)
            {
                if (MenuWrapper.Draw.OnlyReady.Enabled && Q.IsReady())
                {
                    Render.Circle.DrawCircle(GameObjects.Player.Position, Q.Range, Color.FromArgb(255, 0, 0), 1);
                }
                else if (!MenuWrapper.Draw.OnlyReady.Enabled)
                {
                    Render.Circle.DrawCircle(GameObjects.Player.Position, Q.Range, Color.FromArgb(255, 0, 0), 1);
                }
            }
        }
    }
}
