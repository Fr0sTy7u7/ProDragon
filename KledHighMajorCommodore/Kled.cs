using System;
using System.Linq;
using EnsoulSharp;
using EnsoulSharp.SDK;
using System.Collections.Generic;
using SharpDX;
using System.Drawing;
using EnsoulSharp.SDK.MenuUI;
using EnsoulSharp.SDK.Prediction;
using EnsoulSharp.SDK.MenuUI.Values;
using EnsoulSharp.SDK.Utility;

//This is an assembly for the champion Kled. I'd prefer it if you wouldn't copy this code without giving proper credits.
namespace KledHighMajorCommodore
{
    internal class Kled
    {
        #region Statics
        static Menu Config;
        static Spell Shotgun;
        static Spell FuckYou;
        static Spell Dash;
        static Spell Trap;
        static GameObject JoustObj;

        #endregion Statics

        #region Main Entrypoint
        static void Main(string[] args)
        {
            GameEvent.OnGameLoad += OnLoad;
        }
        #endregion Main Entrypoint

        #region Menu/OnLoad
        static void OnLoad() //It's not like I wanna initiate your assembly... b-baka
        {
            if (ObjectManager.Player.CharacterName != "Kled") return;

            Printmsg("Assembly Version: 1.2.0.0");
            Printmsg("Successfully Loaded | Have Fun!");

            Config = new Menu("HMC Kled", "Kled", true);

            //Spells
            Shotgun = new Spell(SpellSlot.Q, 750);
            Trap = new Spell(SpellSlot.Q, 750);
            Dash = new Spell(SpellSlot.E, 560);
            //3500 / 4000 / 4500 
            FuckYou = new Spell(SpellSlot.R, 3500);

            Shotgun.SetSkillshot(0.25f, 80, 3000, false, SkillshotType.Line);
            Trap.SetSkillshot(0.25f, 40, 1800, false, SkillshotType.Line);
            Dash.SetSkillshot(0.25f, 60, 2200, false, SkillshotType.Line);

            //Begin Menu Variables.
            var combat = Config.Add(new Menu("combat", "Combat Settings"));
            var killsteal = combat.Add(new Menu("killsteal", "Killsteal Settings"));
            var farm = Config.Add(new Menu("farm", "Farm Settings"));
            var lane = farm.Add(new Menu("laneclear", "Laneclear Farm Settings"));
            var jungle = farm.Add(new Menu("jungleclear", "Jungleclear Settings"));
            var dev = Config.Add(new Menu("dev", "Developer Menu"));
            var credits = Config.Add(new Menu("credit", "Credits"));

            //Combat
            combat.Add(new Menu("blank3", "Kled"));
            combat.Add(new MenuBool("Use.Shotgun", "Use Shotgun | Use Q").SetValue(true));
            combat.Add(new MenuBool("Use.ShotgunAntiGapCloser", "Gapclose with Shotgun").SetValue(true));
            combat.Add(new MenuBool("Use.ShotgunGapCloser", "Anti-Gapclose with Shotgun").SetValue(true));
            combat.Add(new Menu("blank2", "Skaarl"));
            combat.Add(new MenuBool("Use.Trap", "Use Trap | Use Q").SetValue(true));
            combat.Add(new MenuBool("Use.TrapAntiGapCloser", "Anti-Gapclose with Trap").SetValue(true));
            combat.Add(new MenuBool("Use.Dash", "Use Dash | Use E").SetValue(true));

            combat.Add(new Menu("blank4", "Harass/Mixed Mode"));
            combat.Add(new MenuBool("Use.ShotgunHarass", "Kled: Use Shotgun | Use Q").SetValue(false));
            combat.Add(new MenuBool("Use.TrapHarass", "Skaarl: Use Trap | Use Q").SetValue(true));

            //combat.Add(new MenuItem("blank4", "Offensive Items/Summoners").SetFontStyle(System.Drawing.FontStyle.Bold));
            //combat.Add(new MenuItem("Use.Tiamat", "Use Tiamat").SetValue(true));
            //combat.Add(new MenuItem("Use.Hydra", "Use Titanic/Ravenous Hydra").SetValue(true));
            //combat.Add(new MenuItem("Use.Ignite", "Use Ignite as Finisher").SetValue(true));

            killsteal.Add(new Menu("blank9", "Killsteal Settings"));
            killsteal.Add(new Menu("blank5", "Kled"));
            killsteal.Add(new MenuBool("ks.shotgun", "Use Shotgun | Use Q for KS").SetValue(true));
            killsteal.Add(new Menu("blank12", "Skaarl"));
            killsteal.Add(new MenuBool("ks.trap", "Use Trap | Use Q for KS").SetValue(true));
            killsteal.Add(new MenuBool("ks.dash", "Use Dash | Use E for KS").SetValue(false));
            killsteal.Add(new Menu("blank6", "Misc Settings"));
            killsteal.Add(new MenuBool("disable.ks", "Disable All Killsteal Methods").SetValue(false));

            //Lane
            lane.Add(new Menu("blank3", "Kled"));
            lane.Add(new MenuBool("laneclear.Shotgun", "Use Shotgun | Use Q").SetValue(true));
            lane.Add(new MenuSlider("laneclear.Shotgun.count", "[Shotgun] Minions Hit", 4, 8, 1));
            lane.Add(new Menu("blank2", "Skaarl"));
            lane.Add(new MenuBool("laneclear.Trap", "Use Trap | Use Q").SetValue(true));
            lane.Add(new MenuSlider("laneclear.Trap.count", "[Trap] Minions Hit", 4, 8, 1));
            lane.Add(new MenuBool("laneclear.Dash", "Use Dash | Use E").SetValue(false));
            lane.Add(new MenuSlider("laneclear.Dash.count", "[Dash] Minions Hit", 4, 8, 1));

            //Jungle
            jungle.Add(new Menu("blank3", "Kled"));
            jungle.Add(new MenuBool("jungleclear.Shotgun", "Use Shotgun | Use Q").SetValue(true));
            jungle.Add(new Menu("blank2", "Skaarl"));
            jungle.Add(new MenuBool("jungleclear.Trap", "Use Trap | Use Q").SetValue(true));
            jungle.Add(new MenuBool("jungleclear.Dash", "Use Dash | Use E").SetValue(true));


            //Dev
            dev.Add(new MenuBool("dev.on", "Enable Developer Mode").SetValue(true));
            dev.Add(new MenuBool("dev.chat", "Enable Debug in Chat").SetValue(false));

            //Credits
            credits.Add(new Menu("cred1", "Made By ScienceARK"));
            credits.Add(new Menu("cred2", "Playtesting/Menu: LazyMexican"));

            //Config.AddToMainMenu();
            Config.Attach();

            
            Gapcloser.OnGapcloser += StopDashingAtMeBro;
            Drawing.OnDraw += GottaDrawSpellRanges;
            GameObject.OnCreate += JoustObject;
            GameObject.OnDelete += JoustObjDelete; //potato tomato

        }
        #endregion Menu/OnLoad

        #region random shit

        private static void JoustObjDelete(GameObject sender, EventArgs args)
        {
            if (sender.Name.Equals("Kled_Base_E_Mark.troy"))
            {
                JoustObj = null;
                Printchat("[E] Joust Mark Removed");
            }
        }

        private static void JoustObject(GameObject sender, EventArgs args)
        {
            if (sender.Name.Equals("Kled_Base_E_Mark.troy"))
            {
                JoustObj = sender;
                Printchat("[E] Joust Mark Detected");
            }

        }
        #endregion random shit


        #region Drawings
        //Spell Draw Ranges
        private static void GottaDrawSpellRanges(EventArgs args)
        {

            var EnemyTarget = TargetSelector.GetTarget(Shotgun.Range, DamageType.Magical);
            if (EnemyTarget != null)
            {


                var GapClosePos = (EnemyTarget.Position.Extend(ObjectManager.Player.Position, EnemyTarget.Position.Distance(ObjectManager.Player.Position) + 250));
                var Enemypos = Drawing.WorldToScreen(EnemyTarget.Position);
                var pos = Drawing.WorldToScreen(ObjectManager.Player.Position);

                if (Config["dev"]["dev.on"].GetValue<MenuBool>().Enabled)
                {
                    //Remember to remove knockback dot
                    if (EnemyTarget.IsValid && GapClosePos != null)
                       Render.Circle.DrawCircle(GapClosePos, 5, System.Drawing.Color.Gold, 5);

                    if (EnemyTarget.IsValid && EnemyTrapped(EnemyTarget))
                        Drawing.DrawText(Enemypos.X - 30, Enemypos.Y + 30, System.Drawing.Color.MediumPurple, "QMark Duration: " + QMarkDuration(EnemyTarget).ToString("#.#"));

                    if (EnemyTarget.IsValid && EnemyDashable(EnemyTarget))
                        Drawing.DrawText(Enemypos.X - 30, Enemypos.Y + 20, System.Drawing.Color.Purple, "Enemy Has Joust Mark");

                    if (EnemyTarget.IsValid && EnemyDashable(EnemyTarget))
                       Drawing.DrawText(pos.X - 30, pos.Y + 20, System.Drawing.Color.Orange, "2nd Dash Duration " + GetDashDuration().ToString("#.#"));

                  if (EnemyTarget.IsValid && EnemyTrapped(EnemyTarget))
                       Render.Circle.DrawCircle(EnemyTarget.Position, 20, System.Drawing.Color.MediumPurple, 5);
               }
           }

        }
        #endregion Drawings

        #region AntiGapCloser
        private static void StopDashingAtMeBro(AIHeroClient sender, Gapcloser.GapcloserArgs args)
        {
            var UseShotgunAntiGapCloser = Config["combat"]["Use.ShotgunAntiGapCloser"].GetValue<MenuBool>().Enabled;
            var UseTrapAntiGapCloser = Config["combat"]["Use.TrapAntiGapCloser"].GetValue<MenuBool>().Enabled;
            if (ObjectManager.Player.IsDead || sender == null)
                return;

            var targetpos = Drawing.WorldToScreen(sender.Position);
            if (sender.IsValidTarget(Shotgun.Range))
            {
                Render.Circle.DrawCircle(sender.Position, sender.BoundingRadius,
                System.Drawing.Color.DeepPink);
                Drawing.DrawText(targetpos[0] - 40, targetpos[1] + 20, System.Drawing.Color.DodgerBlue, "GAPCLOSER");
            }

            if (Shotgun.IsReady() && sender.IsValidTarget(Shotgun.Range) && !Mounted() && UseShotgunAntiGapCloser && args.EndPosition.Distance(ObjectManager.Player.Position) < 350)
            {
                Shotgun.Cast(sender);
            }

            if (Trap.IsReady() && sender.IsValidTarget(Trap.Range) && Mounted() && UseTrapAntiGapCloser && args.EndPosition.Distance(ObjectManager.Player.Position) < 350 )
            {
                Trap.Cast(sender);
            }
        }
        #endregion AntiGapCloser

        #region OrbwalkerManager
        private static void orbwalkerManager(EventArgs args)
        {

            //Switch orbwalkersmode depending on pressed button, baka.
            switch (Orbwalker.ActiveMode)
            {
                case OrbwalkerMode.Combo:
                    FuckEmUp();
                    break;
                case OrbwalkerMode.Harass:
                    SlapTheShitOutOfEm();
                    break;
                case OrbwalkerMode.LaneClear:
                    KillDemMinions();
                    WhoGoesKledJungle();
                    break;
            }

            if (!Config["killsteal"]["disable.ks"].GetValue<MenuBool>().Enabled)
            {
                BitchThatWasMyKill();
            }

            //Quick and Dirty R range ~ ur waifu is shit
            if (FuckYou.Level == 1) FuckYou.Range = 3500;
            if (FuckYou.Level == 2) FuckYou.Range = 4000;
            if (FuckYou.Level == 3) FuckYou.Range = 4500;

        }
        #endregion OrbwalkerManager

        #region Buff Checks
        //IsMounted
        public static bool Mounted()
        {
            if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).Name != "KledRiderQ")
            {
                return true;
            }
            else return false;
        }

        //EnemyHasQDebuff
        public static bool EnemyTrapped(AIHeroClient enemy)
        {
            if (enemy == null) return false;
            if (enemy.HasBuff("kledqmark"))
            {
                return true;
            }
            else return false;
        }

        //Check W Passive ON
        public static bool KledWOn()
        {
            if (ObjectManager.Player.HasBuff("kledwactive"))
            {
                return true;
            }
            else return false;
        }

        //CheckEDebuff
        public static bool EnemyDashable(AIHeroClient enemy)
        {
            if (enemy == null) return false;
            if (enemy.HasBuff("klede2target"))
            {
                return true;
            }
            else return false;
        }
        //Check Second Dash
        public static bool PlayerDashUp()
        {
            if (ObjectManager.Player.HasBuff("KledE2"))
            {
                return true;
            }
            else return false;
        }
        #endregion Buff Checks

        #region math
        //Dash Buff Duration (Expiration date)
        public static float GetDashDuration()
        {
            if (ObjectManager.Player == null || !ObjectManager.Player.HasBuff("KledE2")) return 0;
            float DashBuffDuration = ObjectManager.Player.GetBuff("KledE2").EndTime - Game.Time;
            return DashBuffDuration;

        }
        public static float QMarkDuration(AIHeroClient enemy)
        {
            if (enemy == null || !enemy.HasBuff("kledqmark")) return 0;
            float TrapBuffDuration = enemy.GetBuff("kledqmark").EndTime - Game.Time;
            return TrapBuffDuration;

        }
        #endregion math


        #region Combo
        //Combo
        private static void FuckEmUp()
        {
            var EnemyTarget = TargetSelector.GetTarget(Trap.Range, DamageType.Magical);
            if (EnemyTarget == null) return;

            //DashBuff
            var DashBuff = ObjectManager.Player.GetBuff("KledE2");

            //Prediction
            var ShotgunPred = Shotgun.GetPrediction(EnemyTarget).Hitchance >= HitChance.Medium;
            var TrapPred = Trap.GetPrediction(EnemyTarget).Hitchance >= HitChance.High;
            var DashPred = Dash.GetPrediction(EnemyTarget).Hitchance >= HitChance.Low;

            //Damages
            double DashDmg = DashDamage(EnemyTarget);
            double AADamage = ObjectManager.Player.GetAutoAttackDamage(EnemyTarget);
            double ShotgunDmg = ShotgunDamage(EnemyTarget);
            double TrapDmg = TrapDamage(EnemyTarget);

            //Menu
            var UseShotgun = Config["combat"]["Use.Shotgun"].GetValue<MenuBool>().Enabled;
            var UseGapShotgun = Config["combat"]["Use.ShotgunGapCloser"].GetValue<MenuBool>().Enabled;
            var UseTrap = Config["combat"]["Use.Trap"].GetValue<MenuBool>().Enabled;
            var UseDash = Config["combat"]["Use.Dash"].GetValue<MenuBool>().Enabled;

            if (!Mounted())
            {

                //Use Shotgun to get Skaarl
                if (ObjectManager.Player.Mana >= 80 && ObjectManager.Player.Mana < 100 && Shotgun.IsReady() && EnemyTarget.IsValidTarget(Shotgun.Range) && UseShotgun && ShotgunPred)
                {
                    Shotgun.Cast(EnemyTarget);
                }

                //Shotguncast while walking away | Sort of Fleeing?
                if (Shotgun.IsReady() && !Mounted() && EnemyTarget.IsValidTarget(Shotgun.Range) && !ObjectManager.Player.IsFacing(EnemyTarget) && EnemyTarget.IsMoving && UseShotgun && ShotgunPred)
                {
                    Shotgun.Cast(EnemyTarget);
                }

                var GapClosePos = (EnemyTarget.Position.Extend(ObjectManager.Player.Position, EnemyTarget.Position.Distance(ObjectManager.Player.Position) + 250));
                //Shotgun Gapclose | Gapclose while facing target
                if (ObjectManager.Player.Distance(EnemyTarget.Position) <= 450 + ObjectManager.Player.AttackRange && Shotgun.IsReady()
                     && ObjectManager.Player.IsFacing(EnemyTarget) && EnemyTarget.Distance(ObjectManager.Player.Position) > 200 && UseGapShotgun && ObjectManager.Player.Health > EnemyTarget.Health) //Dash towards enemy with Q.
                {
                    Shotgun.Cast(GapClosePos);
                    Printchat("[Q] Gapcloser - To Target");
                }
            }

            if (Mounted())
            {
                //Basic Trap cast
                if (Trap.IsReady() && Mounted() && EnemyTarget.IsValidTarget(Trap.Range) && UseTrap && TrapPred && !ObjectManager.Player.IsDashing())
                {
                    Trap.Cast(EnemyTarget);
                }

                //2nd Dash if duration is almost expired || Use E when duration is almost over
                if (EnemyTarget.IsValidTarget(Dash.Range * 2) && PlayerDashUp() && EnemyDashable(EnemyTarget) && GetDashDuration() < 1 && UseDash && DashPred)
                {
                    Dash.Cast(EnemyTarget);
                }

                if (Dash.IsReady())
                {

                    //Dash if enemy is in range || Use E in Range
                    if (Dash.IsReady() && EnemyTarget.IsValidTarget(Dash.Range) && !PlayerDashUp() && !EnemyTrapped(EnemyTarget) && UseDash)
                    {
                        Dash.Cast(EnemyTarget.Position);
                        Printchat("[E] Dash if enemy is in range || Use E in Range");
                    }

                    //Cast if Killable with E+AA*2 - First dash
                    if (EnemyTarget.Health < DashDmg + (AADamage * 3) && !PlayerDashUp() && UseDash && DashPred && ObjectManager.Player.Health > EnemyTarget.Health && EnemyTarget.Distance(ObjectManager.Player.Position) > 280)
                    {
                        Dash.Cast(EnemyTarget);
                        Printchat("[E] Cast if Killable with E+AA*2 - First dash");
                    }

                    //Cast if killable with E+AA*3 - First Dash + Passive
                    if (EnemyTarget.Health < DashDmg + (AADamage * 4) && !PlayerDashUp() && UseDash && DashPred && KledWOn() && EnemyTarget.Distance(ObjectManager.Player.Position) > 280)
                    {
                        Dash.Cast(EnemyTarget);
                        Printchat("Cast if killable with E+AA*3 - First Dash + Passive");
                    }

                    //Dash if Killable
                    if (EnemyTrapped(EnemyTarget) && !PlayerDashUp() && EnemyTarget.IsValidTarget(Trap.Range) && EnemyTarget.Health < TrapDmg + AADamage * 4 && EnemyTarget.Distance(ObjectManager.Player.Position) >= Dash.Range && Dash.IsReady() && UseDash && DashPred && EnemyTarget.Distance(ObjectManager.Player.Position) > 280)
                    {
                        Dash.Cast(EnemyTarget.Position);
                        Printchat("[E] Dash if Killable");
                    }

                    //Dash if W is Active and Q hit thus ignoring the ranged requirement - After Dashable target died.
                    if (EnemyTrapped(EnemyTarget) && KledWOn() && !PlayerDashUp() && UseDash && DashPred && EnemyTarget.Distance(ObjectManager.Player.Position) > 250)
                    {
                        Dash.Cast(EnemyTarget);
                        Printchat("[E] Dash if W is Active and Q hit thus ignoring the ranged requirement - After Dashable target died.");

                    }
                    //Dash if W is Active and Q hit thus ignoring the ranged ruquirement 
                    if (EnemyTrapped(EnemyTarget) && KledWOn() && PlayerDashUp() && EnemyDashable(EnemyTarget) && UseDash && DashPred && EnemyTarget.Distance(ObjectManager.Player.Position) > 250)
                    {
                        Dash.Cast(EnemyTarget);
                        Printchat("[E] Dash if W is Active and Q hit thus ignoring the ranged ruquirement ");
                    }

                    //Dash if target dashes. | ignores other logic.
                    if (EnemyTarget.IsValidTarget(Dash.Range) && PlayerDashUp() && UseDash && DashPred && EnemyTarget.IsDashing() && EnemyTarget.Distance(ObjectManager.Player.Position) > 280)
                    {
                        Dash.Cast(EnemyTarget);
                        Printchat("[E] Dash if target dashes. | ignores other logic. ");
                    }

                    //Dash if enemy is ranged (Doesn't check Q debuff) || E if enemy is melee (Ignores Q debuff)
                    if (Dash.IsReady() && EnemyTarget.IsValidTarget(Dash.Range) && !PlayerDashUp() && EnemyTarget.IsRanged && UseDash && DashPred)
                    {
                        Dash.Cast(EnemyTarget);
                        Printchat("Dash if enemy is ranged (Doesn't check Q debuff) || E if enemy is melee (Ignores Q debuff)");
                    }

                    if (JoustObj != null)
                    {

                        //2nd Dash if duration is almost expired || Use E when duration is almost over - After Enemy dies with Dash Buff
                        if (PlayerDashUp() && JoustObj.IsValid & EnemyTarget.Position.Distance(JoustObj.Position) <= 200 && GetDashDuration() < 1 && UseDash)
                        {
                            Dash.Cast(EnemyTarget.Position);
                            Printchat("2nd Dash if duration is almost expired");
                        }
                        //2nd Dash if enemy is out of AA range || Out of AA E dash - After Enemy dies with Dash Buff
                        if (PlayerDashUp() && JoustObj.IsValid & EnemyTarget.Position.Distance(JoustObj.Position) < 150 && EnemyTarget.Distance(ObjectManager.Player.Position) > 290 && UseDash)
                        {
                            Dash.Cast(EnemyTarget.Position);
                            Printchat("2nd Dash if enemy is out of AA range");
                        }

                        if (EnemyTrapped(EnemyTarget) && KledWOn() && PlayerDashUp() && JoustObj.IsValid & EnemyTarget.Position.Distance(JoustObj.Position) < 150 && UseDash && EnemyTarget.Distance(ObjectManager.Player.Position) > 280)
                        {
                            Dash.Cast(EnemyTarget.Position);
                            Printchat("2nd Dash if enemy is out of AA range");
                        }

                        if (KledWOn() && PlayerDashUp() && JoustObj.IsValid & EnemyTarget.Position.Distance(JoustObj.Position) < 150 && UseDash && EnemyTarget.Distance(ObjectManager.Player.Position) > 290)
                        {
                            Dash.Cast(EnemyTarget.Position);
                            Printchat("2nd Dash if enemy is out of AA range");
                        }

                        //Cast if Killable with E+AA*2 - 2nd dash
                        if (EnemyTarget.Health < DashDmg + (AADamage * 2) && PlayerDashUp() && UseDash && ObjectManager.Player.Health > EnemyTarget.Health && JoustObj.IsValid && EnemyTarget.Position.Distance(JoustObj.Position) < 100 && EnemyTarget.CountEnemyHeroesInRange(400) <= 1)
                        {
                            Dash.Cast(EnemyTarget.Position);
                            Printchat("2nd Dash Cast if Killable with E+AA*2 - 2nd dash");
                        }

                        //Cast if killable with E+AA*3 - 2nd Dash + Passive
                        if (EnemyTarget.Health < DashDmg + (AADamage * 3) && PlayerDashUp() && UseDash && KledWOn() 
                            && JoustObj.IsValid & EnemyTarget.Position.Distance(JoustObj.Position) < 100 && EnemyTarget.Position.Distance(JoustObj.Position) < 100 && EnemyTarget.CountEnemyHeroesInRange(400) <= 1)
                        {
                            Dash.Cast(EnemyTarget.Position);
                            Printchat("Cast if killable with E+AA*3 - 2nd Dash + Passive");
                        }
                    }


                }
            }
        }
        #endregion combo
        #region Killsteal
        private static void BitchThatWasMyKill()
        {
            var enemies = GameObjects.Enemy.Where(e => e.IsValidTarget(Shotgun.Range) && e != null);
            //Menu
            var UseShotgun = Config["killsteal"]["ks.shotgun"].GetValue<MenuBool>().Enabled;
            var UseTrap = Config["killsteal"]["ks.trap"].GetValue<MenuBool>().Enabled;
            var UseDash = Config["killsteal"]["ks.dash"].GetValue<MenuBool>().Enabled;

            foreach (var EnemyTarget in enemies)
            {
                if (EnemyTarget.IsValid && EnemyTarget != null)
                {
                    //Prediction
                    var ShotgunPred = Shotgun.GetPrediction(EnemyTarget).Hitchance >= HitChance.High;
                    var TrapPred = Trap.GetPrediction(EnemyTarget).Hitchance >= HitChance.High;
                    var DashPred = Dash.GetPrediction(EnemyTarget).Hitchance >= HitChance.High;
                    //Damages
                    double DashDmg = DashDamage(EnemyTarget);
                    double AADamage = ObjectManager.Player.GetAutoAttackDamage(EnemyTarget);
                    double ShotgunDmg = ShotgunDamage(EnemyTarget);
                    double TrapDmg = TrapDamage(EnemyTarget);

                    if (!Mounted() && EnemyTarget.Health < ShotgunDmg && UseShotgun && EnemyTarget.IsValidTarget(Shotgun.Range) && ShotgunPred) Shotgun.Cast(EnemyTarget);

                    if (Mounted())
                    {
                        if (EnemyTarget.Health < TrapDmg && UseTrap && EnemyTarget.IsValidTarget(Trap.Range) && TrapPred) Trap.Cast(EnemyTarget);
                        if (EnemyTarget.Health < TrapDmg && UseDash && EnemyTarget.IsValidTarget(Dash.Range) && DashPred) Dash.Cast(EnemyTarget);
                    }


                }


            }


        }
        #endregion Killsteal

        #region Mixed
        //Mixed
        private static void SlapTheShitOutOfEm()
        {
            var EnemyTarget = TargetSelector.GetTarget(Trap.Range, DamageType.Magical);
            if (EnemyTarget == null || !EnemyTarget.IsValid) return;

            //Prediction
            var ShotgunPred = Shotgun.GetPrediction(EnemyTarget).Hitchance >= HitChance.Medium;
            var TrapPred = Trap.GetPrediction(EnemyTarget).Hitchance >= HitChance.High;
            var DashPred = Dash.GetPrediction(EnemyTarget).Hitchance >= HitChance.Low;

            //Menu
            var UseShotgun = Config["combat"]["Use.ShotgunHarass"].GetValue<MenuBool>().Enabled;
            var UseTrap = Config["combat"]["Use.TrapHarass"].GetValue<MenuBool>().Enabled;


            if (Mounted())
            {
                if (UseTrap && Trap.IsReady() && EnemyTarget.IsValidTarget(Trap.Range) && TrapPred)
                    Trap.Cast(EnemyTarget);
            }
            if (!Mounted())
            {
                if (UseShotgun && Shotgun.IsReady() && EnemyTarget.IsValidTarget(Shotgun.Range) && ShotgunPred)
                    Shotgun.Cast(EnemyTarget);
            }
        }
        #endregion Mixed

        #region Laneclear
        //Lane
        private static void KillDemMinions()
        {
            //Menu
            var LaneclearShotgun = Config["laneclear"]["laneclear.Shotgun"].GetValue<MenuBool>();
            var LaneclearTrap = Config["laneclear"]["laneclear.Trap"].GetValue<MenuBool>();
            var LaneclearDash = Config["laneclear"]["laneclear.Dash"].GetValue<MenuBool>();

            var Shotguncount = Config["laneclear"]["laneclear.Shotgun.count"].GetValue<MenuSlider>();
            var Trapcount = Config["laneclear"]["laneclear.Trap.count"].GetValue<MenuSlider>();
            var Dashcount = Config["laneclear"]["laneclear.Dash.count"].GetValue<MenuSlider>();

            //Shotgun
            var MinionsShotGunRange = GameObjects.GetMinions(Shotgun.Range, MinionTypes.All, MinionTeam.Enemy).Where(m => m.IsValid
            && m.Distance(ObjectManager.Player) < Shotgun.Range).ToList();
            var ShotgunFarmPosition = Shotgun.GetLineFarmLocation(new List<AIBaseClient>(MinionsShotGunRange), Shotgun.Width);

            //Trap
            var MinionsTrapRange = GameObjects.GetMinions(Trap.Range, MinionTypes.All, MinionTeam.Enemy).Where(m => m.IsValid
            && m.Distance(ObjectManager.Player) < Trap.Range).ToList();
            var TrapFarmPosition = Trap.GetLineFarmLocation(new List<AIBaseClient>(MinionsTrapRange), Trap.Width);

            //Dash
            var MinionsDashRange = GameObjects.GetMinions(Dash.Range, MinionTypes.All, MinionTeam.Enemy).Where(m => m.IsValid
            && m.Distance(ObjectManager.Player) < Dash.Range).ToList();
            var DashFarmPosition = Dash.GetLineFarmLocation(new List<AIBaseClient>(MinionsDashRange), Dash.Width);


            //Checks for Kled
            if (!Mounted())
            {
                foreach (var minion in MinionsShotGunRange)
                {
                    if (minion == null || !minion.IsValid || !LaneclearShotgun) return;

                    if (Shotgun.IsReady() && ShotgunFarmPosition.MinionsHit >= Shotguncount.Value)
                    {
                        Shotgun.Cast(ShotgunFarmPosition.Position);
                    }

                }
            }

            //Checks for Skaarl
            if (Mounted())
            {
                foreach (var minion in MinionsTrapRange)
                {
                    if (minion == null || !minion.IsValid || !LaneclearTrap) return;

                    if (Trap.IsReady() && TrapFarmPosition.MinionsHit >= Trapcount.Value && !ObjectManager.Player.IsDashing())
                    {
                        Trap.Cast(TrapFarmPosition.Position);
                    }

                }
                foreach (var minion in MinionsDashRange)
                {
                    if (minion == null || !minion.IsValid || !LaneclearDash) return;

                    if (Dash.IsReady() && DashFarmPosition.MinionsHit >= Dashcount.Value)
                    {
                        Dash.Cast(DashFarmPosition.Position);
                    }

                }
            }


        }
        #endregion laneclear

        #region Jungle
        //Jungle
        private static void WhoGoesKledJungle()
        {

            //Menu
            var JungleclearShotgun = Config["jungleclear"]["jungleclear.Shotgun"].GetValue<MenuBool>();
            var JungleclearTrap = Config["jungleclear"]["jungleclear.Trap"].GetValue<MenuBool>();
            var JungleclearDash = Config["jungleclear"]["jungleclear.Dash"].GetValue<MenuBool>();


            //Shotgun
            var MinionsShotGunRange = GameObjects.GetJungles(Shotgun.Range, JungleType.All, JungleOrderTypes.MaxHealth).Where(m => m.IsValid
            && m.Distance(ObjectManager.Player) < Shotgun.Range).ToList().OrderBy(m => m.MaxHealth);
            var ShotgunFarmPosition = Shotgun.GetLineFarmLocation(new List<AIBaseClient>(MinionsShotGunRange), Shotgun.Width);

            //Trap
            var MinionsTrapRange = GameObjects.GetJungles(Shotgun.Range, JungleType.All, JungleOrderTypes.MaxHealth).Where(m => m.IsValid
            && m.Distance(ObjectManager.Player) < Trap.Range).ToList().OrderBy(m => m.MaxHealth);
            var TrapFarmPosition = Trap.GetLineFarmLocation(new List<AIBaseClient>(MinionsTrapRange), Trap.Width);

            //Dash
            var MinionsDashRange = GameObjects.GetJungles(Shotgun.Range, JungleType.All, JungleOrderTypes.MaxHealth).Where(m => m.IsValid
            && m.Distance(ObjectManager.Player) < Dash.Range).ToList().OrderBy(m => m.MaxHealth);
            var DashFarmPosition = Dash.GetLineFarmLocation(new List<AIBaseClient>(MinionsDashRange), Dash.Width);


            //Checks for Kled
            if (!Mounted())
            {
                foreach (var minion in MinionsShotGunRange)
                {
                    if (minion == null || !minion.IsValid || !JungleclearShotgun) return;

                    if (Shotgun.IsReady())
                    {
                        Shotgun.Cast(ShotgunFarmPosition.Position);
                    }

                }
            }

            //Checks for Skaarl
            if (Mounted())
            {
                foreach (var minion in MinionsTrapRange)
                {
                    if (minion == null || !minion.IsValid || !JungleclearTrap) return;

                    if (Trap.IsReady() && !ObjectManager.Player.IsDashing())
                    {
                        Trap.Cast(ShotgunFarmPosition.Position);
                    }

                }
                foreach (var minion in MinionsDashRange)
                {
                    if (minion == null || !minion.IsValid || !JungleclearDash) return;

                    if (Dash.IsReady())
                    {
                        Dash.Cast(ShotgunFarmPosition.Position);
                    }

                }
            }
        }
        #endregion Jungle

        #region Chat
        static void Printchat(string message)
        {
            if (!Config["dev"]["dev.chat"].GetValue<MenuBool>() || !Config["dev"]["dev.on"].GetValue<MenuBool>())
                return;

            Game.Print(
                "<font color='#00ff00'>[DEBUG]:</font> <font color='#FFFFFF'>" + message + "</font>");
        }
        static void Printmsg(string message)
        {
            Game.Print(
                "<font color='#FFB90F'>[High Major Commodore Kled]:</font> <font color='#FFFFFF'>" + message + "</font>");
        }
        #endregion Chat

        #region Items
        private static void GottaUseItems()
        {

        }
        #endregion Items

        #region DamageCalculations..

        static double ShotgunDamage(AIBaseClient target)
        {
            return
              ObjectManager.Player.CalculateDamage(target, DamageType.Physical,
                    new[] { 0, 30, 45, 60, 75, 90 }[Shotgun.Level] + 0.8 * ObjectManager.Player.FlatPhysicalDamageMod);
            
        }
        static double TrapDamage(AIBaseClient target)
        {
            return
              ObjectManager.Player.CalculateDamage(target, DamageType.Physical,
                    new[] { 0, 25, 50, 75, 100, 125 }[Trap.Level] + 0.6 * ObjectManager.Player.FlatPhysicalDamageMod);

        }
        static double DashDamage(AIBaseClient target)
        {
            return
              ObjectManager.Player.CalculateDamage(target, DamageType.Physical,
                    new[] { 0, 25, 45, 70, 95, 120 }[Dash.Level] + 0.6 * ObjectManager.Player.FlatPhysicalDamageMod);

        }
        #endregion DamageCalculations..

        //nobody will ever see this, teehee.
    }
}
