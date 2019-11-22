using System;
using System.Linq;
using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI.Values;
using EnsoulSharp.SDK.Utility;
using System.Windows.Forms;
using SharpDX;
using Color = System.Drawing.Color;
using Lib = PerfectDarius.PerfectLib;
using Menu = EnsoulSharp.SDK.MenuUI.Menu;

namespace PerfectDarius
{
    internal class PerfectDarius
    {
        internal static Menu Config;
        internal static int LastGrabTimeStamp;
        internal static int LastDunkTimeStamp;
        internal static HpBarIndicator HPi = new HpBarIndicator();
        static SpellSlot IgniteSlot;
        public PerfectDarius()
        {
            if (ObjectManager.Player.CharacterName == "Darius")
            {
                Game.Print("Perfect Darius v1.5");
                IgniteSlot = ObjectManager.Player.GetSpellSlot("summonerdot");
                Menu_OnLoad();
                Game.OnUpdate += Game_OnUpdate;

                Drawing.OnDraw += Drawing_OnDraw;
                Drawing.OnEndScene += Drawing_OnEndScene;

                Orbwalker.OnAction += Orbwalker_OnAction;

                AIBaseClient.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;

                Interrupter.OnInterrupterSpell += Interrupter2_OnInterruptableTarget;
            }
        }

        internal static void Interrupter2_OnInterruptableTarget(AIHeroClient sender, Interrupter.InterruptSpellArgs args)
        {
            if (sender.IsValidTarget(Lib.Spellbook["E"].Range) && Lib.Spellbook["E"].IsReady())
            {
                if (Config["rmenu"].GetValue<MenuBool>("useeint").Enabled)
                {
                    Lib.Spellbook["E"].Cast(sender.PreviousPosition);
                }
            }
        }


        internal static void Obj_AI_Base_OnProcessSpellCast(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe)
            {
                return;
            }

            switch (args.SData.Name.ToLower())
            {
                case "dariuscleave":
                    DelayAction.Add(Game.Ping + 800, Orbwalker.ResetAutoAttackTimer);
                    break;

                case "dariusaxegrabcone":
                    LastGrabTimeStamp = Variables.GameTimeTickCount;
                    DelayAction.Add(Game.Ping + 100, Orbwalker.ResetAutoAttackTimer);
                    break;

                case "dariusexecute":
                    LastDunkTimeStamp = Variables.GameTimeTickCount;
                    DelayAction.Add(Game.Ping + 350, Orbwalker.ResetAutoAttackTimer);
                    break;
            }
        }

        internal static float RModifier
        {
            get { return Config["rmenu"].GetValue<MenuSlider>("rmodi").Value; }
        }

        internal static float MordeShield(AIHeroClient unit)
        {
            if (unit.CharacterName != "Mordekaiser")
            {
                return 0f;
            }

            return unit.Mana;
        }

        internal static int PassiveCount(AIBaseClient unit)
        {
            return unit.GetBuffCount("DariusHemoMarker") > 0 ? unit.GetBuffCount("DariusHemoMarker") : 0;
        }

        internal static void Drawing_OnEndScene(EventArgs args)
        {
            if (!Config["drawings"].GetValue<MenuBool>("drawfill").Enabled || Lib.Player.IsDead)
            {
                return;
            }

            foreach (var enemy in GameObjects.EnemyHeroes.Where(ene => ene.IsValidTarget() && ene.IsHPBarRendered))
            {
                HPi.unit = enemy;
                HPi.drawDmg(Lib.RDmg(enemy, PassiveCount(enemy)), new ColorBGRA(255, 255, 0, 90));
            }
        }
        internal static void Game_OnUpdate(EventArgs args)
        {
            if (ObjectManager.Player.IsDead)
            {
                return;
            }
            if (Lib.Spellbook["E"].IsReady()  && Config["rmenu"].GetValue<MenuBool>("tpcancel").Enabled )
            {
                var etarget = TargetSelector.GetTarget(Lib.Spellbook["E"].Range);
                if (etarget == null) return;
                if (etarget.IsValidTarget() && etarget.HasBuff("Teleport"))
                {
                    if (etarget.Distance(Lib.Player.PreviousPosition) > 250)
                    {
                        Lib.Spellbook["E"].Cast(etarget.PreviousPosition);
                    }
                }
            }

            if(IgniteSlot.IsReady() && Config["rmenu"].GetValue<MenuBool>("useIgn").Enabled)
            {
                var igtarget = TargetSelector.GetTarget(Lib.Player.GetRealAutoAttackRange(), DamageType.True);
                if (igtarget == null) return;
                var Ignitedmg = Damage.GetSummonerSpellDamage(ObjectManager.Player, igtarget, SummonerSpell.Ignite);
                if(igtarget.Health < Ignitedmg)
                {
                    //Cast Ignite
                }
            }

            if (Lib.Spellbook["R"].IsReady() && Config["rmenu"].GetValue<MenuBool>("ksr").Enabled)
            {
                foreach (var unit in GameObjects.EnemyHeroes.Where(ene => ene.IsValidTarget(Lib.Spellbook["R"].Range)))
                {
                    if (unit == null) return;
                    if (unit.CountEnemyHeroesInRange(1200) <= 1 && Config["rmenu"].GetValue<MenuBool>("ksr1").Enabled)
                    {
                        if (Lib.RDmg(unit, PassiveCount(unit)) + RModifier + 
                            Lib.Hemorrhage(unit, PassiveCount(unit)) >= unit.Health + MordeShield(unit))
                        {
                            if (!TargetSelector.GetTarget(Lib.Spellbook["R"].Range, DamageType.True).IsInvulnerable)
                            {
                                if (!unit.HasBuff("kindredrnodeathbuff"))
                                    Lib.Spellbook["R"].CastOnUnit(unit);
                            }
                        }
                    }

                    if (Lib.RDmg(unit, PassiveCount(unit)) + RModifier >= unit.Health +
                        Lib.Hemorrhage(unit, 1) + MordeShield(unit))
                    {
                        if (!TargetSelector.GetTarget(Lib.Spellbook["R"].Range, DamageType.True).IsInvulnerable)
                        {
                            if (!unit.HasBuff("kindredrnodeathbuff"))
                                Lib.Spellbook["R"].CastOnUnit(unit);
                        }
                    }
                }
            }

            switch (Orbwalker.ActiveMode)
            {
                case OrbwalkerMode.Combo:
                    Combo(Config["cmenu"].GetValue<MenuBool>("useq").Enabled, Config["cmenu"].GetValue<MenuBool>("usew").Enabled,
                          Config["cmenu"].GetValue<MenuBool>("usee").Enabled, Config["rmenu"].GetValue<MenuBool>("user").Enabled);
                    break;
                case OrbwalkerMode.Harass:
                    Harass();
                    break;
                case OrbwalkerMode.LaneClear:
                    Clear();
                    break;
                case OrbwalkerMode.LastHit:
                    LastHit();
                    break;
            }

            if (Config["cmenu"].GetValue<MenuKeyBind>("caste").Active)
            {
                Orbwalker.Orbwalk(null, Game.CursorPos);
                Combo(false, false, true, false);
            }
        }
        


        internal static void Drawing_OnDraw(EventArgs args)
        {
            if (Lib.Player.IsDead)
            {
                return;
            }

            var acircle = Config["drawings"].GetValue<MenuBool>("drawe");
            if (acircle.Enabled)
                Render.Circle.DrawCircle(Lib.Player.Position, Lib.Spellbook["E"].Range, Color.AliceBlue, 3);

            var rcircle = Config["drawings"].GetValue<MenuBool>("drawr");
            if (rcircle.Enabled)
                Render.Circle.DrawCircle(Lib.Player.Position, Lib.Spellbook["R"].Range, Color.LightGreen, 3);

            var qcircle = Config["drawings"].GetValue<MenuBool>("drawq");
            if (qcircle.Enabled)
                Render.Circle.DrawCircle(Lib.Player.Position, Lib.Spellbook["Q"].Range, Color.Red, 3);

            var plaz = Drawing.WorldToScreen(Lib.Player.Position);
            if (Lib.Player.GetBuffCount("dariusexecutemulticast") > 0)
            {
                var executetime = Lib.Player.GetBuff("dariusexecutemulticast").EndTime - Game.Time;
                Drawing.DrawText(plaz[0] - 15, plaz[1] + 55, System.Drawing.Color.OrangeRed, executetime.ToString("0.0"));
            }

            foreach (var enemy in GameObjects.EnemyHeroes.Where(ene => ene.IsValidTarget()))
            {
                var enez = Drawing.WorldToScreen(enemy.Position); 
                if (enemy.GetBuffCount("DariusHemoMarker") > 0)
                {
                    var endtime = enemy.GetBuff("DariusHemoMarker").EndTime - Game.Time;
                    // Drawing.DrawText(enez[0] - 50, enez[1], System.Drawing.Color.OrangeRed,  "Stack Count: " + enemy.GetBuffCount("dariushemo"));
                   // Drawing.DrawText(enez[0] - 25, enez[1] + 20, System.Drawing.Color.OrangeRed, endtime.ToString("0.0"));
                }
            }
        }


        internal static void Orbwalker_OnAction(object sender, OrbwalkerActionArgs args)
        {
            var hero = sender as AIHeroClient;
            if (hero == null || !hero.IsValid() || hero.Type != GameObjectType.AIHeroClient ||
                Orbwalker.ActiveMode != OrbwalkerMode.Combo)
            {
                return;
            }
            if (args.Type == OrbwalkerType.AfterAttack)
            {
                if (Lib.Spellbook["R"].IsReady() && Lib.Player.Mana - Lib.Spellbook["W"].Mana >
                    Lib.Spellbook["R"].Mana || !Lib.Spellbook["R"].IsReady())
                {
                    if (!hero.HasBuffOfType(BuffType.Slow) || Config["cmenu"].GetValue<MenuBool>("wwww").Enabled)
                        Lib.Spellbook["W"].Cast();
                }

                if (!Lib.Spellbook["W"].IsReady() && Config["rmenu"].GetValue<MenuBool>("iiii").Enabled)
                {
                    Lib.HandleItems();
                }
            }
        }
        public static void AutoQBlade(AIBaseClient unit)
        {
            if (Lib.Player.HasBuff("dariusqcast") && Lib.Player.CountEnemyHeroesInRange(650) < 3)
            {
                Orbwalker.AttackState = false;
                Orbwalker.MovementState = false;
                if (unit.DistanceToPlayer() <= 250)
                {
                    Lib.Player.IssueOrder(GameObjectOrder.MoveTo, Lib.Player.Position.Extend(unit.Position, -Lib.Spellbook["Q"].Range));
                }
                else
                {
                    Lib.Player.IssueOrder(GameObjectOrder.MoveTo, Lib.Player.Position);
                }
            }
            else
            {
                Orbwalker.AttackState = true;
                Orbwalker.MovementState = true;
            }
        }

        internal static bool CanQ(AIBaseClient unit)
        {
            if (unit == null)
            {
                return false;
            }
            if (!unit.IsValidTarget() ||
                TargetSelector.GetTarget(Lib.Spellbook["Q"].Range).IsInvulnerable)
            {
                return false;
            }

            if (unit.InAutoAttackRange())
            {
                return false;
            }

            if (Lib.Player.Distance(unit.PreviousPosition) < 175 ||
                Variables.GameTimeTickCount - LastGrabTimeStamp < 350)
            {
                return false;
            }

            if (Lib.Spellbook["R"].IsReady() &&
                Lib.Player.Mana - Lib.Spellbook["Q"].Mana < Lib.Spellbook["R"].Mana)
            {
                return false;
            }

            if (Lib.Spellbook["W"].IsReady() && Lib.WDmg(unit) >= unit.Health &&
                unit.Distance(Lib.Player.PreviousPosition) <= 200)
            {
                return false;
            }

            if (Lib.Spellbook["W"].IsReady() || Lib.Player.HasBuff("DariusNoxianTacticsONH") &&
                unit.Distance(Lib.Player.PreviousPosition) <= 225)
            {
                return false;
            }

            if (Lib.Spellbook["R"].IsReady() && Lib.Spellbook["R"].IsInRange(unit) &&
                Lib.RDmg(unit, PassiveCount(unit)) - Lib.Hemorrhage(unit, 1) >= unit.Health)
            {
                return false;
            }

            if (Lib.Player.GetAutoAttackDamage(unit) * 2 + Lib.Hemorrhage(unit, PassiveCount(unit)) >= unit.Health &&
                Lib.Player.Distance(unit.PreviousPosition) <= 180)
            {
                return false;
            }

            return true;
        }

        internal static void Harass()
        {
            if (Config["hmenu"].GetValue<MenuBool>("harassq").Enabled && Lib.Spellbook["Q"].IsReady())
            {
                var qtarget = TargetSelector.GetTarget(Lib.Spellbook["Q"].Range, DamageType.Physical);
                if (Lib.Player.Mana / Lib.Player.MaxMana * 100 > 60)
                {
                    if (CanQ(qtarget))
                    {
                        if (Config["rmenu"].GetValue<MenuBool>("autoblade").Enabled)
                        {
                            AutoQBlade(qtarget);
                        }
                        Lib.Spellbook["Q"].Cast();
                    }
                }
            }   
        }

        internal static void Combo(bool useq, bool usew, bool usee, bool user)
        {
            if (useq && Lib.Spellbook["Q"].IsReady())
            {
                var qtarget = TargetSelector.GetTarget(Lib.Spellbook["Q"].Range, DamageType.Physical);
                if (qtarget == null)
                {
                    return;
                }
                if (CanQ(qtarget) && Lib.Player.CanMove)
                {
                    Lib.Spellbook["Q"].Cast();
                    if (Config["rmenu"].GetValue<MenuBool>("autoblade").Enabled)
                    { 
                        AutoQBlade(qtarget);
                    }
                }
            }

            if (usew && Lib.Spellbook["W"].IsReady())
            {
                var wtarget = TargetSelector.GetTarget(Lib.Spellbook["W"].Range);
                if (wtarget  == null)
                {
                    return;
                }
                if (wtarget.IsValidTarget(Lib.Spellbook["W"].Range))
                {
                    if (Variables.GameTimeTickCount - LastDunkTimeStamp >= 500)
                    {
                        Lib.Spellbook["W"].CastOnUnit(wtarget);
                    }
                }
            }

            

            if (usee && Lib.Spellbook["E"].IsReady())
            {
                var etarget = TargetSelector.GetTarget(Lib.Spellbook["E"].Range);
                if (etarget == null)
                {
                    return;
                }
                if (etarget.IsValidTarget())
                {
                    if (etarget.Distance(Lib.Player.PreviousPosition) > 250)
                    {
                        if (Lib.Player.CountAllyHeroesInRange(1000) >= 1)
                            Lib.Spellbook["E"].Cast(etarget.PreviousPosition);

                        if (Lib.RDmg(etarget, PassiveCount(etarget)) - Lib.Hemorrhage(etarget, 1) >= etarget.Health)
                            Lib.Spellbook["E"].Cast(etarget.PreviousPosition);

                        if (Lib.Spellbook["W"].IsReady())
                            Lib.Spellbook["E"].Cast(etarget.PreviousPosition);

                        if (Lib.Player.GetAutoAttackDamage(etarget) + Lib.Hemorrhage(etarget, 3) * 3 >= etarget.Health)
                            Lib.Spellbook["E"].Cast(etarget.PreviousPosition);
                    }           
                }
            }

            if (user && Lib.Spellbook["R"].IsReady())
            {
                var unit = TargetSelector.GetTarget(Lib.Spellbook["R"].Range, DamageType.True);
                if (unit == null)
                {
                    return;
                }
                if (unit.IsValidTarget(Lib.Spellbook["R"].Range))
                {
                    if (!unit.HasBuffOfType(BuffType.Invulnerability) && !unit.HasBuffOfType(BuffType.SpellShield))
                    {
                        if (Lib.RDmg(unit, PassiveCount(unit)) + RModifier >= unit.Health +
                            Lib.Hemorrhage(unit, 1) + MordeShield(unit))
                        {
                            if (!unit.IsInvulnerable)
                            {
                                if (!unit.HasBuff("kindredrnodeathbuff"))
                                    Lib.Spellbook["R"].CastOnUnit(unit);
                            }
                        }
                    }
                }
            }
        }
        private static void Clear()
        {
            if (Lib.Player.IsWindingUp)
            {
                return;
            }
            if (Config["lmenu"].GetValue<MenuBool>("useqLC").Enabled && Lib.Spellbook["Q"].IsReady() && !Lib.Player.IsDashing())
            {
                var minis = GameObjects.EnemyMinions.Where(m => m.IsValidTarget(Lib.Player.AttackRange + 50)).Cast<AIBaseClient>().ToList();
                if (minis.Count() == 0)
                {
                    minis = GameObjects.Jungle.Where(m => m.IsValidTarget(Lib.Spellbook["Q"].Range)).Cast<AIBaseClient>().ToList();
                    if (GameObjects.Jungle.Where(m => m.IsValidTarget(Lib.Spellbook["Q"].Range)).Cast<AIBaseClient>().ToList().Count() >=
                        Config["lmenu"].GetValue<MenuSlider>("minimumMini").Value &&
                        minis.Count(m => m.Health - Lib.Spellbook["Q"].GetDamage(m) < 50 && m.Health - Lib.Spellbook["Q"].GetDamage(m) > 0) == 0 &&
                        (GameObjects.Jungle.Where(m => m.IsValidTarget(Lib.Player.AttackRange)).Cast<AIBaseClient>().ToList().Count() <= 0 || !Orbwalker.CanAttack()))
                    {
                        Lib.Spellbook["Q"].Cast();
                        return;
                    }
                }
                else
                {
                    if (GameObjects.EnemyMinions.Where(m => m.IsValidTarget(Lib.Spellbook["Q"].Range)).Cast<AIBaseClient>().ToList().Count() >=
                        Config["lmenu"].GetValue<MenuSlider>("minimumMini").Value &&
                        minis.Count(m => m.Health - Lib.Spellbook["Q"].GetDamage(m) < 50 && m.Health - Lib.Spellbook["Q"].GetDamage(m) > 0) == 0 &&
                        (GameObjects.EnemyMinions.Where(m => m.IsValidTarget(Lib.Player.AttackRange)).Cast<AIBaseClient>().ToList().Count() <= 0 || !Orbwalker.CanAttack()))
                    {
                        Lib.Spellbook["Q"].Cast();
                        return;
                    }
                }
            }
            if (Config["lmenu"].GetValue<MenuBool>("usewLC").Enabled && Lib.Spellbook["W"].IsReady())
            {
                var minionsForW = GameObjects.EnemyMinions.Where(m => m.IsValidTarget(Lib.Spellbook["W"].Range)).Cast<AIBaseClient>().ToList();
                if (minionsForW.Count() == 0)
                {
                    minionsForW = GameObjects.Jungle.Where(m => m.IsValidTarget(Lib.Spellbook["W"].Range)).Cast<AIBaseClient>().ToList();
                }

                foreach (var minion in minionsForW)
                {
                    if (minion.Health < Lib.Spellbook["W"].GetDamage(minion))
                    {
                        Lib.Spellbook["W"].CastOnUnit(minion);
                    }
                }
            }
        }
        private static void LastHit()
        {
            if (Config["lastmenu"].GetValue<MenuBool>("usewLH").Enabled && Lib.Spellbook["W"].IsReady())
            {
                var minionsForW = GameObjects.EnemyMinions.Where(m => m.IsValidTarget(Lib.Spellbook["W"].Range)).Cast<AIBaseClient>().ToList();
                if (minionsForW.Count() == 0)
                {
                    minionsForW = GameObjects.Jungle.Where(m => m.IsValidTarget(Lib.Spellbook["W"].Range)).Cast<AIBaseClient>().ToList();
                }

                foreach (var minion in minionsForW)
                {
                    if (minion.Health < Lib.Spellbook["W"].GetDamage(minion))
                    {
                        Lib.Spellbook["W"].CastOnUnit(minion);
                    }
                }
            }
        }

        internal static void Menu_OnLoad()
        {
            Config = new Menu("darius","Perfect Darius", true);

            var cmenu = new Menu("cmenu", "Combo Settings");
            cmenu.Add(new MenuBool("useq", "Use Q in Combo"));
            cmenu.Add(new MenuBool("usee", "Use E in Combo")).SetValue(true);
            cmenu.Add(new MenuKeyBind("caste", "Use E to Assist", Keys.T, KeyBindType.Press));
            cmenu.Add(new MenuBool("usew", "Use W to AA Reset")).SetValue(true);
            cmenu.Add(new MenuBool("wwww", "Use W on Slowed Targets")).SetValue(true);
            cmenu.Add(new MenuBool("www2w", "Please Select The Target to Use The Ult"));
            Config.Add(cmenu);

            var hmenu = new Menu("hmenu", "Harass Settings");
            hmenu.Add(new MenuBool("harassq", "Use Q in harass")).SetValue(true);
            Config.Add(hmenu);

            var lmenu = new Menu("lmenu", "Lane/Jungle Clear Settings");
            lmenu.Add(new MenuBool("useqLC", "Use Q", true));
            lmenu.Add(new MenuSlider("minimumMini", "Use Q min minion", 2, 1, 6));
            lmenu.Add(new MenuBool("usewLC", "Use W for Last Hit", true));
            Config.Add(lmenu);

            var lastmenu = new Menu("lastmenu", "Last Hit Settings");
            lastmenu.Add(new MenuBool("usewLH", "Use W for Last Hit", true));
            Config.Add(lastmenu);


            var amenu = new Menu("amenu", "Activator");
            amenu.Add(new MenuBool("useIgn", "Use Ignite")).SetValue(true);
            amenu.Add(new MenuBool("iiii", "Use Hydra")).SetValue(true);
            Config.Add(amenu);

            var rmenu = new Menu("rmenu", "More Settings");
            
            rmenu.Add(new MenuBool("useeint", "Interrupt Spells with E")).SetValue(true);
            rmenu.Add(new MenuBool("autoblade", "Auto move hit blade")).SetValue(true);
            rmenu.Add(new MenuBool("tpcancel", "E to Cancel the Enemy's TP")).SetValue(true);
            rmenu.Add(new MenuBool("user", "Use R")).SetValue(true);
            rmenu.Add(new MenuBool("ksr", "Use R in KS")).SetValue(true);
            rmenu.Add(new MenuBool("ksr1", "Use R Max Dmg")).SetValue(false);
            //rmenu.AddItem(new MenuItem("userlast", "Use R before buff expiry").SetValue(true).SetTooltip("After a successful ult, will not waste R if buff will Expire"));
            rmenu.Add(new MenuSlider("rmodi", "R Delay", 0, 0, 500));
            rmenu.Add(new MenuBool("useeflee", "Auto E Fleeing Targets")).SetValue(true);
            Config.Add(rmenu);

            

            var drmenu = new Menu("drawings", "Draw Settings");
            drmenu.Add(new MenuBool("drawq", "Draw Q")).SetValue(true);
            drmenu.Add(new MenuBool("drawe", "Draw E")).SetValue(true);
            drmenu.Add(new MenuBool("drawr", "Draw R")).SetValue(true);

            drmenu.Add(new MenuBool("drawfill", "Draw R Damage Fill")).SetValue(true);
          //  drmenu.AddItem(new MenuItem("drawstack", "Draw Stack Count")).SetValue(true);
            Config.Add(drmenu);

            Config.Attach();
        }
    }
    internal class PerfectLib
    {
        internal static AIHeroClient Player = ObjectManager.Player;
        internal static System.Collections.Generic.Dictionary<string, Spell> Spellbook = new System.Collections.Generic.Dictionary<string, Spell>
        {
            { "Q", new Spell(SpellSlot.Q, 425f) },
            { "W", new Spell(SpellSlot.W, 200f) },
            { "E", new Spell(SpellSlot.E, 490f) },
            { "R", new Spell(SpellSlot.R, 460f) }
      
            
        };
        


        internal static void HandleItems()
        {
            //HYDRA
            if (Items.HasItem(Lib.Player, 3077) && Items.CanUseItem(Lib.Player, 3077))
                Items.UseItem(Lib.Player, 3077);

            //TİAMAT
            if (Items.HasItem(Lib.Player, 3074) && Items.CanUseItem(Lib.Player, 3074))
                Items.UseItem(Lib.Player, 3074);

            //NEW ITEM
            if (Items.HasItem(Lib.Player, 3748) && Items.CanUseItem(Lib.Player, 3748))
                Items.UseItem(Lib.Player, 3748);
        }

        internal static float QDmg(AIBaseClient unit)
        {
            return
                (float)
                    Player.CalculateDamage(unit, DamageType.Physical,
                        new[] { 50, 50, 80, 110, 140, 170 }[Spellbook["Q"].Level] +
                       (new[] { 1.0, 1.0, 1.1, 1.2, 1.3, 1.4 }[Spellbook["Q"].Level] * Player.FlatPhysicalDamageMod));
        }

        internal static float WDmg(AIBaseClient unit)
        {
            return
                (float)
                    Player.CalculateDamage(unit, DamageType.Physical,
                       Player.TotalAttackDamage + (0.42 * Player.TotalAttackDamage));
        }

        internal static float RDmg(AIBaseClient unit, int stackcount)
        {
            var bonus = (new[] { 20, 20, 40, 60 }[Spellbook["R"].Level] +
                            (0.25 * Player.FlatPhysicalDamageMod) * stackcount);
            return
                (float)(bonus + (Player.CalculateDamage(unit, DamageType.True,
                        new[] { 100, 100, 200, 300 }[Spellbook["R"].Level] + (0.75 * Player.FlatPhysicalDamageMod))));
        }

        internal static float Hemorrhage(AIBaseClient unit, int stackcount)
        {
            if (stackcount < 1)
                stackcount = 1;

            return
                (float)
                    Player.CalculateDamage(unit, DamageType.Physical,
                        (13 + Player.Level) + (0.3 * Player.FlatPhysicalDamageMod)) * stackcount;
        }
    }
}
