using System;
using System.Linq;
using System.Collections.Generic;
using SharpDX;
using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.Events;
using EnsoulSharp.SDK.MenuUI;
using EnsoulSharp.SDK.MenuUI.Values;
using EnsoulSharp.SDK.Prediction;
using EnsoulSharp.SDK.Utility;

using Color = System.Drawing.Color;
using static EnsoulSharp.SDK.Items;

namespace Irelia
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            GameEvent.OnGameLoad += OnGameLoad;
        }
        private static void OnGameLoad()
        {
            if (ObjectManager.Player.CharacterName != "Irelia")
                return;

            Irelia.OnLoad();
            Game.Print("<font color=\"#05FAAC\"><b>Irelia Loaded</b></font>");
            Game.Print("<font color=\"#001FFF\"><b>Auto Setting by ProDragon!!</b></font>");

            Game.Print("<font color=\"#FF0000\"><b>Enjoy your game! xD</b></font>");
        }
    }
    internal class MenuSettings
    {
        public class Combo
        {
            public static Menu setcombo                     = new Menu("c", "Combo active");
            public static MenuSeparator comboSeparator          = new MenuSeparator("comboSeparator", "Combo Settings");
            public static MenuBool useQ                         = new MenuBool("useQ", "Use Q");
            public static MenuBool useW                         = new MenuBool("useW", "Use W");
            public static MenuBool useE                         = new MenuBool("useE", "Use E");
            public static MenuBool useR                         = new MenuBool("useR", "Use R");
            public static MenuBool useIgnite                    = new MenuBool("useIgnite", "Use Ignite");
            public static MenuSeparator wSettingsSeparator      = new MenuSeparator("wSettingsSeparator", "W Settings");
            public static MenuList wMode                        = new MenuList("wMode", "Select Mode", new[] { "Common Prediction", "Only On Stunned Enemies" }, 0);
            public static MenuSeparator eSettingsSeparator      = new MenuSeparator("eSettingsSeparator", "E Settings");
            public static MenuBool eImmobile                    = new MenuBool("eImmobile", "Don't Use E on Immobile Enemies");
            public static MenuList eMode                        = new MenuList("eMode", "Select Mode:", new[] { "Target On The Center", "Target On The Edge" }, 1);
        }
        public class Harass
        {
            public static Menu setharass                    = new Menu("h", "Harass active");
            public static MenuSeparator harassSeparator         = new MenuSeparator("harassSeparator", "Harass Settings");
            public static MenuBool useQ                         = new MenuBool("useQ", "Use Q");
            public static MenuBool useW                         = new MenuBool("useW", "Use W");
            public static MenuSeparator wSettingsSeparator      = new MenuSeparator("wSettingsSeparator", "W Settings");
            public static MenuList wMode                        = new MenuList("wMode", "Select Mode", new[] { "Common Prediction", "Only On Stunned Enemies" }, 0);
            public static MenuSeparator manaSeperator           = new MenuSeparator("manaSeperator", "Mana Manager");
            public static MenuSlider minMana                    = new MenuSlider("minMana", "Min Mana Percent", 50, 0, 100);
        }
        public class LaneClear
        {
            public static Menu setlaneclear = new Menu("l", "LaneClear active");
            public static MenuSeparator laneClearSeperator      = new MenuSeparator("laneClearSeperator", "Lane Clear Settings");
            public static MenuBool useQ                         = new MenuBool("useQ", "Use Q");
            public static MenuBool useW                         = new MenuBool("useW", "Use W");
            public static MenuSeparator qSettingsSeperator      = new MenuSeparator("qSettingsSeperator", "Q Settings");
            public static MenuBool autostackClear               = new MenuBool("autostackClear", "Auto Stack");
            public static MenuList qStackMode                   = new MenuList("qStackMode", "Select Mode:", new[] { "LastHit 1 Minion", "LastHit >= 2 Minions" }, 1);
            public static MenuSeparator wSettingsSeperator      = new MenuSeparator("wSettingsSeperator", "W Settings");
            public static MenuSlider minHitsW                   = new MenuSlider("minHitsW", "Min Minions Hit", 3, 1, 6);
            public static MenuSeparator manaSeperator           = new MenuSeparator("manaSeperator", "Mana Manager");
            public static MenuSlider minManaQ                   = new MenuSlider("minManaQ", "Min Mana Percent For Q", 40, 0, 100);
            public static MenuSlider minManaW                   = new MenuSlider("minManaW", "Min Mana Percent For W", 60, 0, 100);
        }
        public class JungleClear
        {
            public static Menu setjungleclear = new Menu("j", "JungleClear active");
            public static MenuSeparator jungleClearSeparator    = new MenuSeparator("jungleClearSeparator", "Jungle Clear Settings");
            public static MenuBool useQ                         = new MenuBool("useQ", "Use Q");
            public static MenuBool useW                         = new MenuBool("useW", "Use W");
            public static MenuBool useE                         = new MenuBool("useE", "Use E");
            public static MenuSeparator qSettingsSeparator      = new MenuSeparator("qSettingsSeparator", "Q Settings");
            public static MenuList qMode                        = new MenuList("qMode", "Select Mode:", new[] { "All Monsters", "Big Monsters" }, 0);
            public static MenuSeparator wSettingsSeparator      = new MenuSeparator("wSettingsSeparator", "W Settings");
            public static MenuList wMode                        = new MenuList("wMode", "Select Mode:", new[] { "All Monsters", "Big Monsters" }, 0);
            public static MenuSeparator eSettingsSeparator      = new MenuSeparator("eSettingsSeparator", "E Settings");
            public static MenuList eMode                        = new MenuList("eMode", "Select Mode:", new[] { "All Monsters", "Big Monsters" }, 0);
            public static MenuSeparator manaSeperator           = new MenuSeparator("manaSeperator", "Mana Manager");
            public static MenuSlider minManaQ                   = new MenuSlider("minManaQ", "Min Mana Percent For Q", 30, 0, 100);
            public static MenuSlider minManaW                   = new MenuSlider("minManaW", "Min Mana Percent For W", 30, 0, 100);
            public static MenuSlider minManaE                   = new MenuSlider("minManaE", "Min Mana Percent For E", 60, 0, 100);
        }
        public class LastHit
        {
            public static Menu setlasthit = new Menu("lh", "LastHit active");
            public static MenuSeparator lastHitSeparator        = new MenuSeparator("lastHitSeparator", "Last Hit Settings");
            public static MenuBool useQ                         = new MenuBool("useQ", "Use Q");
            public static MenuSeparator qSettingsSeperator      = new MenuSeparator("qSettingsSeperator", "Q Settings");
            public static MenuBool autostackClear               = new MenuBool("autostackClear", "Auto Stack");
            public static MenuList qStackMode                   = new MenuList("qStackMode", "Select Mode:", new[] { "LastHit 1 Minion", "LastHit >= 2 Minions" }, 0);

        }
        public class Misc
        {
            public static Menu setmisc = new Menu("m", "Misc active");
            public static MenuSeparator miscSeparator           = new MenuSeparator("miscSeparator", "Misc Settings");
            public static MenuBool interrupter                  = new MenuBool("interrupter", "Interrupter");
            public static MenuBool gapcloser                    = new MenuBool("gapcloser", "Gapcloser");
            public static MenuSeparator killStealSeparator      = new MenuSeparator("killStealSeparator", "KillSteal Settings");
            public static MenuBool killstealEnable              = new MenuBool("killstealEnable", "Enable");
            public static MenuBool killstealQ                   = new MenuBool("killstealQ", "Use Q");
            public static MenuBool killstealW                   = new MenuBool("killstealW", "Use W");
            public static MenuBool killstealR                   = new MenuBool("killstealR", "Use R");
            public static MenuBool killstealIgnite              = new MenuBool("killstealIgnite", "Use Ignite");
        }

        public class Item
        {
            public static MenuSeparator a = new MenuSeparator("Items Settings", "Items Settings");
            public static MenuBool b = new MenuBool("hydra", "Use [Hydra] Reset AA");
            public static MenuBool c = new MenuBool("titanic", "Use [Titanic]");
            public static MenuBool d = new MenuBool("BOTRK", "Use [Botrk]");
            public static MenuSlider e = new MenuSlider("ihp", "My HP Use BOTRK <=", 50);
            public static MenuSlider f = new MenuSlider("ihpp", "Enemy HP Use BOTRK <=", 50);


        }
        public class Drawing
        {
            public static MenuBool setdraw = new MenuBool("d", "Drawing active");
            public static MenuSeparator drawingSeparator        = new MenuSeparator("drawingSeparator", "Drawings");
            public static MenuBool disableDrawings              = new MenuBool("disableDrawings", "Disable", false);
            public static MenuBool drawAutoStack                = new MenuBool("drawAutoStack", "Auto Stack Text");
            public static MenuBool drawDmg                      = new MenuBool("drawDmg", "Damage Indicator");
            public static MenuSeparator rangesSeperator         = new MenuSeparator("rangesSeperator", "Spell Ranges");
            public static MenuBool drawQ                        = new MenuBool("drawQ", "Q Range");
            public static MenuBool drawW                        = new MenuBool("drawW", "W Range");
            public static MenuBool drawE                        = new MenuBool("drawE", "E Range");
            public static MenuBool drawR                        = new MenuBool("drawR", "R Range");
        }

        public class Keys
        {
            public static MenuSeparator keysSeperator           = new MenuSeparator("keysSeperator", "Keys Settings");
            public static MenuKeyBind harassToggle              = new MenuKeyBind("harassToggle", "Harass Key", System.Windows.Forms.Keys.H, KeyBindType.Toggle);
            public static MenuKeyBind farmToggle                = new MenuKeyBind("farmToggle", "Spell Farm Key", System.Windows.Forms.Keys.J, KeyBindType.Toggle);        }
    }
    internal class Irelia
    {
        private static SpellSlot summonerIgnite;
        private static SpellSlot summonerFlash;
        private static AIHeroClient objPlayer = ObjectManager.Player;
        private static Menu myMenu;
        private static Menu mMenu;
        public static Spell Ignite;
        public static Item Hydra;
        public static Item Tiamat;
        public static Item Titanic;
        public static Item Botrk;
        public static Item Bil;
        public static Menu Menu, Items, Rset;
        public static Spell Q, W, E, R;
        private static AIBaseClient target;

        public static void OnLoad()
        {
            Q = new Spell(SpellSlot.Q, 600);
            Q.SetTargetted(0f, float.MaxValue);

            W = new Spell(SpellSlot.W, 300);

            E = new Spell(SpellSlot.E, 775);



            R = new Spell(SpellSlot.R, 900);
            R.SetSkillshot(0.5f, 260, 2000, false, SkillshotType.Line, HitChance.VeryHigh);

            Ignite = new Spell(ObjectManager.Player.GetSpellSlot("summonerdot"), 600);
            Tiamat = new Item(ItemId.Tiamat_Melee_Only, 400);
            Hydra = new Item(ItemId.Ravenous_Hydra_Melee_Only, 400);
            Titanic = new Item(ItemId.Titanic_Hydra, objPlayer.GetRealAutoAttackRange());
            Botrk = new Item(ItemId.Blade_of_the_Ruined_King, 400);
            Bil = new Item(3144, 475f);
            summonerFlash = objPlayer.GetSpellSlot("SummonerFlash");


            #region Menu Init

            myMenu = new Menu(objPlayer.CharacterName, "Irelia Beta version", true);
            mMenu = new Menu(objPlayer.CharacterName, "Feed Back", true);
            var f1 = new Menu("f1", "All FeedBack send to my FaceBook:");
            mMenu.Add(f1);
            var f2 = new Menu("f2", "FB.COM/Vinh.Kevin195");
            var comboMenu = new Menu("comboMenu", "Combo")
            {
                MenuSettings.Combo.setcombo
                //MenuSettings.Combo.comboSeparator,
                //MenuSettings.Combo.useQ,
                //MenuSettings.Combo.useW,
                //MenuSettings.Combo.useE,
                //MenuSettings.Combo.useR,
                //MenuSettings.Combo.useIgnite,
                //MenuSettings.Combo.wSettingsSeparator,
                //MenuSettings.Combo.wMode,
                //MenuSettings.Combo.eSettingsSeparator,
                //MenuSettings.Combo.eImmobile,
                //MenuSettings.Combo.eMode,
            };
            myMenu.Add(comboMenu);

            var harassMenu = new Menu("harassMenu", "Harass")
            {
                MenuSettings.Harass.setharass
                //MenuSettings.Harass.harassSeparator,
                //MenuSettings.Harass.useQ,
                //MenuSettings.Harass.useW,
                //MenuSettings.Harass.wSettingsSeparator,
                //MenuSettings.Harass.wMode,
                //MenuSettings.Harass.manaSeperator,
                //MenuSettings.Harass.minMana,
            };
            myMenu.Add(harassMenu);

            var laneClearMenu = new Menu("laneClearMenu", "Lane Clear")
            {
                MenuSettings.LaneClear.setlaneclear
                //MenuSettings.LaneClear.laneClearSeperator,
                //MenuSettings.LaneClear.useQ,
                //MenuSettings.LaneClear.useW,
                //MenuSettings.LaneClear.qSettingsSeperator,
                //MenuSettings.LaneClear.autostackClear,
                //MenuSettings.LaneClear.qStackMode
                //MenuSettings.LaneClear.wSettingsSeperator,
                //MenuSettings.LaneClear.minHitsW,
                //MenuSettings.LaneClear.manaSeperator,
                //MenuSettings.LaneClear.minManaQ,
                //MenuSettings.LaneClear.minManaW,
            };
            myMenu.Add(laneClearMenu);

            var jungleClearMenu = new Menu("jungleClearMenu", "Jungle Clear")
            {
                MenuSettings.JungleClear.setjungleclear
                //MenuSettings.JungleClear.jungleClearSeparator,
                //MenuSettings.JungleClear.useQ,
                //MenuSettings.JungleClear.useW,
                //MenuSettings.JungleClear.useE,
                //MenuSettings.JungleClear.qSettingsSeparator,
                //MenuSettings.JungleClear.qMode,
                //MenuSettings.JungleClear.wSettingsSeparator,
                //MenuSettings.JungleClear.wMode,
                //MenuSettings.JungleClear.eSettingsSeparator,
                //MenuSettings.JungleClear.eMode,
                //MenuSettings.JungleClear.manaSeperator,
                //MenuSettings.JungleClear.minManaQ,
                //MenuSettings.JungleClear.minManaW,
                //MenuSettings.JungleClear.minManaE,
            };
            myMenu.Add(jungleClearMenu);

            var lastHitMenu = new Menu("lastHitMenu", "Last Hit")
            {
                MenuSettings.LastHit.setlasthit
                //MenuSettings.LastHit.lastHitSeparator,
                //MenuSettings.LastHit.useQ,
                //MenuSettings.LastHit.qSettingsSeperator,
                //MenuSettings.LastHit.autostackClear,
                //MenuSettings.LastHit.qStackMode
            };
            myMenu.Add(lastHitMenu);

            var miscMenu = new Menu("miscMenu", "Misc")
            {
                MenuSettings.Misc.setmisc,
                //MenuSettings.Misc.miscSeparator,
                //MenuSettings.Misc.interrupter,
                //MenuSettings.Misc.gapcloser,

                new Menu("item", "Item")
                {

                }

            //new Menu("killStealMenu", "KillSteal")
            //{
            //MenuSettings.Misc.killStealSeparator,
            //MenuSettings.Misc.killstealEnable,
            //MenuSettings.Misc.killstealQ,
            //MenuSettings.Misc.killstealW,
            //MenuSettings.Misc.killstealR,
            //MenuSettings.Misc.killstealIgnite,
            //},
        };
            myMenu.Add(miscMenu);

            var drawingMenu = new Menu("drawingMenu", "Drawings")
            {
                MenuSettings.Drawing.setdraw
               //MenuSettings.Drawing.drawingSeparator,
               //MenuSettings.Drawing.disableDrawings,
               //MenuSettings.Drawing.drawDmg,
               //MenuSettings.Drawing.rangesSeperator,
               //MenuSettings.Drawing.drawQ,
               //MenuSettings.Drawing.drawW,
               //MenuSettings.Drawing.drawE,
               //MenuSettings.Drawing.drawR,
            };
            myMenu.Add(drawingMenu);

            myMenu.Attach();

            #endregion

            Game.OnUpdate                     += OnUpdate;
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


            switch (Orbwalker.ActiveMode)
            {
                case OrbwalkerMode.Combo:
                    Combo();
                    logicE();
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
        private static void Combo()
        {
            var target = TargetSelector.GetTarget(1500);
            foreach (var minion in GetEnemyLaneMinionsTargetsInRange(Q.Range))
            {
                if(minion.Health <= objPlayer.GetSpellDamage(minion, SpellSlot.Q))
                {
                    if(minion.Distance(target) < objPlayer.Distance(target))
                    {
                        if (Q.IsReady())
                        {
                            Q.CastOnUnit(minion);
                        }
                    }
                }

                if (objPlayer.HealthPercent < 70)
                {
                    if (minion.Distance(target) < 300 && minion.Health < objPlayer.GetSpellDamage(minion, SpellSlot.Q))
                    {
                        if(minion.DistanceToPlayer() > 200)
                        {
                            Q.CastOnUnit(minion);
                        }
                        if(minion.Distance(target) > 300)
                        {
                            Q.CastOnUnit(minion);
                        }
                    }
                    if(objPlayer.Mana <= Q.Mana *2)
                    {
                        return;
                    }
                }

                if(objPlayer.HealthPercent < 40)
                {
                    if(objPlayer.Mana > Q.Mana *2)
                    {
                        if(minion.Distance(target) < 300)
                        {
                            Q.CastOnUnit(minion);
                        }
                    }
                }
            }

            if(E.IsReady())
            {
                if(target.IsValidTarget(E.Range))
                {
                    E.Cast(target.Position);
                }

            }
            if (E.IsReady() && objPlayer.HasBuff("IreliaE"))
            {
                if (target.IsValidTarget(E.Range))
                {
                    E.Cast(target.Position);
                }
            }


            if(Q.IsReady())
            {
                if(target.HasBuff("ireliamark"))
                {
                    if(objPlayer.Distance(target) < Q.Range)
                    {
                        Q.CastOnUnit(target);
                    }
                }
            }

            if(R.IsReady())
            {
                if(target.Health < 700)
                {
                    if(objPlayer.Mana > R.Mana + Q.Mana - 20)
                    {
                        R.Cast(target.PreviousPosition);
                    }
                }
            }

            if (target == null || target.IsDead || target.IsAlly)
                return;

            /*if (E.IsReady() && objPlayer.Mana >= Q.Mana)
            {
                if (!target.IsValidTarget(E.Range))
                    return;

                var getPrediction = E.GetPrediction(target);

                        if (getPrediction.Hitchance >= HitChance.Medium)
                        {
                            E.Cast(target.PreviousPosition);
                        }
            }
            if (Q.IsReady())
            {
                if (!target.IsValidTarget(Q.Range))
                    return;

                var getPrediction = Q.GetPrediction(target);

                if (getPrediction.CollisionObjects.Count() < 2 && getPrediction.Hitchance >= HitChance.High)
                    Q.Cast(target.PreviousPosition);
            }*/
            /*if (MenuSettings.Combo.useW.Enabled && W.IsReady())
            {
                if (!target.IsValidTarget(W.Range - 50))
                    return;
                if (E.IsReady() && objPlayer.ManaPercent > Q.Mana + W.Mana)
                    return;

                var getPrediction = W.GetPrediction(target);

                switch (MenuSettings.Combo.wMode.SelectedValue)
                {
                    case "Common Prediction":
                        if (getPrediction.Hitchance >= HitChance.High || target.HasBuffOfType(BuffType.Slow))
                        {
                            W.Cast(getPrediction.CastPosition);
                        }
                        break;
                    case "Only On Stunned Enemies":
                        if (getPrediction.Hitchance == HitChance.Immobile)
                        {
                            W.Cast(getPrediction.CastPosition);
                        }
                        break;
                }
            }*/
            /*if (R.IsReady())
            {
                var getPrediction = R.GetPrediction(target);

                if (!target.IsValidTarget(R.Range + 100))
                    return;
                if (Q.IsReady() && getDamage(target, true, false, false, false) > target.Health)
                    return;

                if (E.IsReady() && getDamage(target, false, false, true, false) > target.Health)
                    return;

                if(target.IsValidTarget(R.Range))
                {
                    if(objPlayer.Mana >= R.Mana + Q.Mana)
                    {
                        if(target.Health <= getDamage(target, true,false, false, true))
                        {
                            R.Cast();
                            DelayAction.Add(1100, () =>
                            {
                                Q.Cast(target.PreviousPosition);
                            });
                        }
                    }

                    if (objPlayer.Mana >= R.Mana + E.Mana)
                    {
                        if (target.Health <= getDamage(target, false, false, true, true))
                        {
                            R.Cast();
                            DelayAction.Add(100, () =>
                            {
                                E.Cast(target.PreviousPosition);
                            });
                        }
                    }

                    if (summonerIgnite.IsReady())
                    {
                        if (target.Health < getDamage(target, false, false, true, true))
                            R.Cast();
                    }
                }

                if (target.Health < getDamage(target, false, false, true, false))
                {
                    R.Cast();
                    DelayAction.Add(0, () =>
                    {
                        W.Cast(target.PreviousPosition);
                    });
                }

                if (target != null && objPlayer.CountEnemyHeroesInRange(R.Range - 200) >= 2 )
                {
                    R.Cast();
                }


                if (!target.IsValidTarget(800))
                {

                    if (objPlayer.CountEnemyHeroesInRange(700) <= 2)
                    {

                        var t = TargetSelector.GetTarget(950);
                        if (summonerFlash.IsReady())
                        {
                            R.Cast();
                            DelayAction.Add(1000, () =>
                            {
                                if (target.IsValidTarget(900))
                                {
                                    if(target.IsUnderEnemyTurret(200))
                                    {
                                        return;
                                    }

                                    if (target.DistanceToPlayer() > R.Range)
                                    {
                                        objPlayer.Spellbook.CastSpell(summonerFlash, target.Position);
                                    }
                                }
                            });
                        }
                    }
                }

            }*/

            if (summonerIgnite.IsReady())
            {
                if (!target.IsValidTarget(600))
                    return;

                if (target.Health < getDamage(target, true, false, false, true))
                    objPlayer.Spellbook.CastSpell(summonerIgnite, target);
                if (target.Health < getDamage(target, false, false, false, true))
                    objPlayer.Spellbook.CastSpell(summonerIgnite, target);
            }
        }

        private static void logicE()
        {
            var t = TargetSelector.GetTarget(E.Range, DamageType.Magical);
            var vec = t.PreviousPosition - objPlayer.PreviousPosition;
            var castBehind = E.GetPrediction(t).CastPosition + Vector3.Normalize(vec) * 75;
            var castinform = E.GetPrediction(t).CastPosition - Vector3.Normalize(vec) * 75;
            if (E.IsReady() && objPlayer.HasBuff("IreliaE"))
            {
                E.Cast(castBehind);
            }
            else
            {
                E.Cast(castinform);
            }
        }

        private static void Harass()
        {
            /*if (objPlayer.ManaPercent < 30)
                return;

            var target = TargetSelector.GetTarget(Q.Range);

            if (target == null || target.IsDead)
                return;

            if (Q.IsReady())
            {
                if (!target.IsValidTarget(E1.Range - 50))
                    return;

                var getPrediction = E.GetPrediction(target);

                if (getPrediction.CollisionObjects.Count() < 2 && getPrediction.Hitchance >= HitChance.Medium)
                {
                    E.Cast(target);
                }
            }
            if (Q.IsReady())
            {
                if (!target.IsValidTarget(Q.Range))
                    return;

                var getPrediction = Q.GetPrediction(target);

                        if (getPrediction.Hitchance >= HitChance.Low)
                        {
                            Q.Cast(getPrediction.CastPosition);
                        }               
            }*/
        }
        private static void LaneClear()
        {
            /*var allMinions = GameObjects.EnemyMinions.Where(x => x.IsMinion() && !x.IsDead).OrderBy(x => x.Distance(objPlayer.Position));

            if (allMinions.Count() == 0)
                return;
            
            if (Q.IsReady())
            {
                if (objPlayer.ManaPercent < 30)
                    return;

                    foreach (var min in allMinions.Where(x => x.IsValidTarget(Q.Range) && x.Health < Q.GetDamage(x)))
                    {
                        var getPrediction = Q.GetPrediction(min, true);
                        var getCollisions = getPrediction.CollisionObjects.ToList();

                        switch (MenuSettings.LaneClear.qStackMode.SelectedValue)
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
                
            }*/

            foreach (var minion in GetEnemyLaneMinionsTargetsInRange(Q.Range))
            {
                if (minion.Health <= GameObjects.Player.GetSpellDamage(minion, SpellSlot.Q) && Q.IsReady() && objPlayer.ManaPercent > 30)
                {
                    Q.Cast(minion);                                          
                }
            }
        }
        private static void JungleClear()
        {
            var mob = GameObjects.Jungle
                .Where(x => x.IsValidTarget(Q.Range) && x.GetJungleType() != JungleType.Unknown)
                .OrderByDescending(x => x.MaxHealth).FirstOrDefault();

            if (mob != null)
            {
                if(E.IsReady() && mob.IsValidTarget(E.Range))
                {
                    E.Cast(mob.Position);
                }
                if(E.IsReady() && mob.IsValidTarget(E.Range) && objPlayer.HasBuff("IreliaE"))
                {
                    E.Cast(mob.Position);
                }
                if(Q.IsReady() && mob.HasBuff("ireliamark"))
                {
                    Q.CastOnUnit(mob);
                }
            }
        }
        private static void LastHit()
        {
            
        }

        #endregion

        #region Events

        private static void OnAction(object sender, OrbwalkerActionArgs args)
        {
            if (Orbwalker.ActiveMode == OrbwalkerMode.LaneClear || Orbwalker.ActiveMode == OrbwalkerMode.LastHit)
            {
                if (args.Type != OrbwalkerType.BeforeAttack)
                    return;

                switch (args.Target.Type)
                {
                    case GameObjectType.AIMinionClient:
                        if (args.Target != null)
                        {
                            var minTarget = (AIMinionClient)args.Target;

                            if (Q.Instance.CooldownExpires < 0.3f)
                            {
                                if (minTarget.Health > Q.GetDamage(minTarget))
                                    return;

                                args.Process = false;
                            }
                        }
                        break;
                    case GameObjectType.AITurretClient:
                        break;
                }
            }
        }
        private static void OnInterrupterSpell(AIHeroClient sender, Interrupter.InterruptSpellArgs arg)
        {
        }
        private static void OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserArgs args)
        {
        }

        #endregion

        #region Drawings

        private static void OnDraw(EventArgs args)
        {
            if (MenuSettings.Drawing.disableDrawings.Enabled)
                return;

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
            foreach (var target in GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(5000) && !x.IsDead && x.IsHPBarRendered))
            {
                Vector2 pos = Drawing.WorldToScreen(target.Position);

                if (!pos.IsOnScreen())
                    return;

                var damage = getDamage(target, true, true, true, true);

                var hpBar = target.HPBarPosition;

                if (damage > target.Health)
                {
                    Drawing.DrawText(hpBar.X + 69, hpBar.Y - 45, System.Drawing.Color.Red, "KILLABLE");
                    Drawing.DrawText(hpBar.X + 69, hpBar.Y + 45, System.Drawing.Color.YellowGreen, "KILL HIM! HE IS RUNNING OUT!");
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
                var target = TargetSelector.GetTarget(1500);
                if (Q.IsReady())
                {
                    if(target.IsValidTarget(600))
                    {
                    Q.CastOnUnit(target);           
                    }

                }                
                if (summonerIgnite.IsReady() && target.IsValidTarget(600))
                {
                    if (target.Health + target.MagicalShield < objPlayer.GetSummonerSpellDamage(target, SummonerSpell.Ignite))
                    {
                        objPlayer.Spellbook.CastSpell(summonerIgnite, target);
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
            if (target.CharacterName == "Morderkaiser")
            {
                damage -= target.Mana;
            }
            if (objPlayer.HasBuff("SummonerExhaust"))
            {
                damage = damage * 0.6f;
            }
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
