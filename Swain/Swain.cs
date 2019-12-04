using System;
using System.Linq;
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
            var not1 = new Notification("ProDragon Swain", "Swain loaded \n And don't forget to give me feedback :) ");
            var not2 = new Notification("Combo", "Q, W, E, R auto active");
            var not3 = new Notification("killsteal", "Q, W, E auto active");
            Notifications.Add(not1);
            Notifications.Add(not2);
            Notifications.Add(not3);
        }
    }
    internal class MenuSettings
    {
        public class Combo
        {
            public static Menu set = new Menu("setcombo", "Combo");
        }
        public class Harass
        {
            public static Menu set = new Menu("setharass", "Harass");
        }
        public class LaneClear
        {
            public static Menu set = new Menu("setlc", "Lane Clear");
        }
        public class JungleClear
        {
            public static Menu set = new Menu("setjc", "Jungle Clear");
        }
        public class LastHit
        {
            public static Menu set = new Menu("setlh", "Last Hit");
        }
        public class Misc
        {
            public static Menu set = new Menu("setm", "Some Misc");
        }
        public class Drawing
        {
            public static Menu set = new Menu("setd", "Drawing");
        }
        public class Credits
        {
        }
        public class Keys
        {
        }
    }
    internal class Swain
    {
        private static SpellSlot summonerIgnite;
        private static Spell Q, W, E, R;
        private static AIHeroClient objPlayer = ObjectManager.Player;
        private static Menu myMenu;

        public static void OnLoad()
        {
            Q = new Spell(SpellSlot.Q, 650f);
            Q.SetSkillshot(0.25f, 200f, 9999f, true, SkillshotType.Cone);

            W = new Spell(SpellSlot.W, 3500f);
            W.SetSkillshot(2f, 225f, 0, false, SkillshotType.Circle);

            E = new Spell(SpellSlot.E, 700f);
            E.SetSkillshot(0.5f, 140f, 1200, false, SkillshotType.Line);

            R = new Spell(SpellSlot.R, 600f);

            #region Menu Init

            myMenu = new Menu(objPlayer.CharacterName, "Swain", true);

            var comboMenu = new Menu("comboMenu", "Combo")
            {
                MenuSettings.Combo.set
            };
            myMenu.Add(comboMenu);

            var harassMenu = new Menu("harassMenu", "Harass")
            {
                MenuSettings.Harass.set
            };
            myMenu.Add(harassMenu);

            var laneClearMenu = new Menu("laneClearMenu", "Lane Clear")
            {
                MenuSettings.LaneClear.set
            };
            myMenu.Add(laneClearMenu);

            var jungleClearMenu = new Menu("jungleClearMenu", "Jungle Clear")
            {
                MenuSettings.JungleClear.set
            };
            myMenu.Add(jungleClearMenu);

            var lastHitMenu = new Menu("lastHitMenu", "Last Hit")
            {
                MenuSettings.LastHit.set
            };
            myMenu.Add(lastHitMenu);

            var miscMenu = new Menu("miscMenu", "Misc")
            {
                MenuSettings.Misc.set
            };
            myMenu.Add(miscMenu);

            var drawingMenu = new Menu("drawingMenu", "Drawings")
            {
                MenuSettings.Drawing.set
            };
            myMenu.Add(drawingMenu);



            myMenu.Attach();

            #endregion

            Tick.OnTick                     += OnUpdate;
            Drawing.OnDraw                  += OnDraw;
            Drawing.OnEndScene              += OnEndScene;
            Orbwalker.OnAction              += OnAction;
            Interrupter.OnInterrupterSpell  += OnInterrupterSpell;
            Gapcloser.OnGapcloser           += OnGapcloser;
        }
        private static void OnUpdate(EventArgs args)
        {
            if (objPlayer.IsDead || objPlayer.IsRecalling())
                return;
            if (MenuGUI.IsChatOpen || MenuGUI.IsShopOpen)
                return;

                KillSteal();

            switch (Orbwalker.ActiveMode)
            {
                case OrbwalkerMode.Combo:
                    Combo();
                    break;
                case OrbwalkerMode.Harass:
                    Harass();
                    break;
                case OrbwalkerMode.LaneClear:
                    LaneClear();
                    JungleClear();
                    break;
                case OrbwalkerMode.LastHit:
                    LastHit();
                    break;
            }
        }

        #region Orbwalker Modes

        private static void Combo()
        {
            var target = TargetSelector.GetTarget(1000);

            if (target == null || target.IsDead || target.IsAlly)
                return;

            var getPrediction = Q.GetPrediction(target, false, 0, CollisionObjects.YasuoWall);
            if (Q.IsReady() && target.IsValidTarget(Q.Range))
            {
                if (getPrediction.Hitchance >= HitChance.Medium)
                {
                    Q.Cast(getPrediction.CastPosition);
                }
                return;
            }

            var getPredictione = E.GetPrediction(target, false, 0, CollisionObjects.YasuoWall);
            if(E.IsReady() && target.IsValidTarget(E.Range))
            {
                if(W.IsReady() && objPlayer.Mana >= Q.Mana + E.Mana)
                {
                    W.Cast(getPredictione.CastPosition - 50);
                    return;
                }
                if (getPredictione.Hitchance >= HitChance.High)
                {
                    E.Cast(getPrediction.CastPosition);
                }
                return;
            }

            if(target.IsValidTarget(R.Range) && R.IsReady())
            {
                if (objPlayer.HasBuff("SwainR")) { return; }
                if(objPlayer.CountEnemyHeroesInRange(R.Range) >2)
                {
                    R.Cast();
                }
            }            
        }
        private static void Harass()
        {
            var t = TargetSelector.GetTarget(Q.Range);
            if (objPlayer.ManaPercent < 40)
                return;
            if (t.IsDead || t == null) { return; }

            if(Q.IsReady() && t.IsValidTarget(Q.Range - 100))
            {
                Q.Cast(t.PreviousPosition);
            }
            var getPrediction = E.GetPrediction(t, false, 0, CollisionObjects.YasuoWall);
            if (E.IsReady() && t.IsValidTarget(E.Range - 100) && getPrediction.Hitchance >= HitChance.High)
            {
                E.Cast(getPrediction.CastPosition);
            }            
        }
        private static void LaneClear()
        {
            if (objPlayer.ManaPercent < 30)
                return;
            var allMinions = GameObjects.EnemyMinions.Where(x => x.IsMinion() && !x.IsDead).OrderBy(x => x.Distance(objPlayer.Position));
            foreach (var min in allMinions.Where(x => x.IsValidTarget(Q.Range - 15) && x.Health * 3 < Q.GetDamage(x)))
            {
                var getPrediction = Q.GetPrediction(min, false, 0, CollisionObjects.YasuoWall);
                var getCollisions = getPrediction.CollisionObjects.ToList();

                if (getCollisions.Any() && (getCollisions.Count() == 1 && getCollisions.FirstOrDefault().Health * 2 < Q.GetDamage(getCollisions.FirstOrDefault())))
                {
                    Q.Cast(getPrediction.CastPosition);
                }
                else if (getCollisions.Count() == 2 && getCollisions[0].Health * 3 < Q.GetDamage(getCollisions[0]) && getCollisions[1].Health * 3 < Q.GetDamage(getCollisions[1]))
                {
                    Q.Cast(getPrediction.CastPosition);
                }
            }           
        }

        //wait 
        private static void JungleClear()
        {
            
        }
        private static void LastHit()
        {
            
        }

        #endregion

        #region Events

        private static void OnAction(object sender, OrbwalkerActionArgs args)
        {
            
        }
        private static void OnInterrupterSpell(AIHeroClient sender, Interrupter.InterruptSpellArgs arg)
        {
            
        }
        private static void OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserArgs args)
        {            
            if (args != null && args.EndPosition.DistanceToPlayer() < 350)
                E.Cast(objPlayer.Position);
        }

        #endregion

        #region Drawings

        private static void OnDraw(EventArgs args)
        {

            if (Q.IsReady())
            {
                Render.Circle.DrawCircle(objPlayer.Position, Q.Range, System.Drawing.Color.AliceBlue);
            }
            if (W.IsReady())
            {
                Render.Circle.DrawCircle(objPlayer.Position, W.Range, System.Drawing.Color.Beige);
            }
            if (E.IsReady())
            {
                Render.Circle.DrawCircle(objPlayer.Position, W.Range, System.Drawing.Color.DodgerBlue);
            }
            if (R.IsReady())
            {
                Drawing.DrawCircle(objPlayer.Position, R.Range, System.Drawing.Color.DarkBlue);
            }
        }
        private static void OnEndScene(EventArgs args)
        {
            foreach (var target in GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(2000) && !x.IsDead && x.IsHPBarRendered))
            {
                Vector2 pos = Drawing.WorldToScreen(target.Position);

                if (!pos.IsOnScreen())
                    return;

                var damage = getDamage(target, true, true, true, true);

                var hpBar = target.HPBarPosition;

                if (damage > target.Health)
                {
                    Drawing.DrawText(hpBar.X + 69, hpBar.Y - 45, System.Drawing.Color.White, "KILLABLE");
                    Drawing.DrawText(hpBar.X + 69, hpBar.Y + 45, System.Drawing.Color.Red, "HE IS RUNNING");
                }

                var damagePercentage = ((target.Health - damage) > 0 ? (target.Health - damage) : 0) / target.MaxHealth;
                var currentHealthPercentage = target.Health / target.MaxHealth;

                var startPoint = new Vector2(hpBar.X - 45 + damagePercentage * 104, hpBar.Y - 18);
                var endPoint = new Vector2(hpBar.X - 45 + currentHealthPercentage * 104, hpBar.Y - 18);

                Drawing.DrawLine(startPoint, endPoint, 12, System.Drawing.Color.Yellow);
            }
        }

        #endregion

        #region Misc

        private static void KillSteal()
        {
            foreach (var target in GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(Q.Range)))
            {
                if (summonerIgnite.IsReady() && target.IsValidTarget(600))
                {
                    if (target.Health + target.MagicalShield < objPlayer.GetSummonerSpellDamage(target, SummonerSpell.Ignite))
                    {
                        objPlayer.Spellbook.CastSpell(summonerIgnite, target);
                    }
                }

                if (Q.IsReady() && target.IsValidTarget(Q.Range) && target.Health + target.MagicalShield < Q.GetDamage(target))
                {
                    Q.Cast(target.Position);
                }

                if (W.IsReady() && target.IsValidTarget(W.Range) && target.Health + target.MagicalShield < W.GetDamage(target))
                {
                    var getPrediction = W.GetPrediction(target);
                    W.Cast(getPrediction.CastPosition);
                }

                if (E.IsReady() && target.IsValidTarget(E.Range) && target.Health + target.MagicalShield < E.GetDamage(target))
                {
                    var getPrediction = E.GetPrediction(target, false, 0, CollisionObjects.YasuoWall);
                    E.Cast(getPrediction.CastPosition);
                }

                if (R.IsReady() && objPlayer.HasBuff("SwainR") && target.Health + target.MagicalShield < R.GetDamage(target) && target.IsValidTarget(600))
                {
                    R.Cast();
                }
            }               
        }

        #region Extensions

        private static float getDamage(AIBaseClient target, bool q = false, bool w = false, bool r = false, bool ignite = false)
        {
            float damage = 0;

            if (target == null || target.IsDead)
                return 0;
            if (target.HasBuffOfType(BuffType.Invulnerability))
                return 0;

            if (q && Q.IsReady())
                damage += (float)Damage.GetSpellDamage(objPlayer, target, SpellSlot.Q);
            if (w && W.IsReady())
                damage += (float)Damage.GetSpellDamage(objPlayer, target, SpellSlot.W);
            if (r && R.IsReady())
                damage += (float)Damage.GetSpellDamage(objPlayer, target, SpellSlot.R);

            if (ignite && summonerIgnite.IsReady())
                damage += (float)objPlayer.GetSummonerSpellDamage(target, SummonerSpell.Ignite);

            if (objPlayer.GetBuffCount("itemmagicshankcharge") == 100) // oktw sebby
                damage += (float)objPlayer.CalculateMagicDamage(target, 100 + 0.1 * objPlayer.TotalMagicalDamage);

            if (target.HasBuff("ManaBarrier") && target.HasBuff("BlitzcrankManaBarrierCO"))
                damage += target.Mana / 2f;
            if (target.HasBuff("GarenW"))
                damage = damage * 0.7f;
            if (target.HasBuff("ferocioushowl"))
            {
                damage = damage * 0.7f;
            }

            return damage;
        }

        #endregion

        #endregion
    }
}
