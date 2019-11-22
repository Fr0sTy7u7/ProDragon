using System;
using System.Linq;
using System.Windows.Forms;
using D_Jarvan.Damage;
using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;
using EnsoulSharp.SDK.MenuUI.Values;
using EnsoulSharp.SDK.Prediction;
using EnsoulSharp.SDK.Utility;
using SharpDX;
using Menu = EnsoulSharp.SDK.MenuUI.Menu;

//gg
namespace D_Jarvan
{
    internal class Program
    {
        private const string ChampionName = "JarvanIV";

        private static Spell _q, _w, _e, _r;

        private static SpellSlot _igniteSlot;

        private static Items.Item _tiamat, _hydra, _blade, _bilge, _rand, _lotis;

        private static Menu _config;

        private static AIHeroClient _player;

        private static bool _haveulti;

        private static SpellDataInst _smiteSlot;

        private static void Main(string[] args)
        {
            Game.OnUpdate += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            _player = ObjectManager.Player;
            if (ObjectManager.Player.CharacterName != ChampionName) return;

            _q = new Spell(SpellSlot.Q, 770f);
            _w = new Spell(SpellSlot.W, 300f);
            _e = new Spell(SpellSlot.E, 830f);
            _r = new Spell(SpellSlot.R, 650f);

            _q.SetSkillshot(0.5f, 70f, float.MaxValue, false, SkillshotType.Line);
            _e.SetSkillshot(0.5f, 70f, float.MaxValue, false, SkillshotType.Circle);

            _smiteSlot = _player.Spellbook.GetSpell(_player.GetSpellSlot("summonersmite"));
            _igniteSlot = _player.GetSpellSlot("SummonerDot");

            _bilge = new Items.Item(3144, 475f);
            _blade = new Items.Item(3153, 425f);
            _hydra = new Items.Item(3074, 375f);
            _tiamat = new Items.Item(3077, 375f);
            _rand = new Items.Item(3143, 490f);
            _lotis = new Items.Item(3190, 590f);

            //D Jarvan
            _config = new Menu("jarvan", "D-Jarvan", true);



            //* var orbwalkerMenu = new Menu("LX-Orbwalker", "LX-Orbwalker");
            // LXOrbwalker.AddToMenu(orbwalkerMenu);
            //_config.AddSubMenu(orbwalkerMenu);*/

            //Combo
            var combo =_config.Add(new Menu("combo", "Combo"));
            combo.Add(new MenuBool("UseIgnite", "Use Ignite")).SetValue(true);
            combo.Add(new MenuBool("UseQC", "Use Q")).SetValue(true);
            combo.Add(new MenuBool("UseWC", "Use W")).SetValue(true);
            combo.Add(new MenuBool("UseEC", "Use E")).SetValue(true);
            combo.Add(new MenuBool("UseRC", "Use R(killable)")).SetValue(true);
            combo.Add(new MenuBool("UseRE", "AutoR Min Targ")).SetValue(true);
            combo.Add(new MenuSlider("MinTargets", "Ult when>=min enemy(COMBO)", 2, 1, 5));
            combo.Add(new MenuKeyBind("ActiveCombo", "Combo!", Keys.Space, KeyBindType.Press));
            combo.Add(new MenuKeyBind("ActiveComboEQR", "ComboEQ-R!", Keys.T, KeyBindType.Press));


            //Items public static Int32 Tiamat = 3077, Hydra = 3074, Blade = 3153, Bilge = 3144, Rand = 3143, lotis = 3190;
            var items = _config.Add(new Menu("items", "Items"));
            items.Add(new Menu("Offensive", "Offensive"));
            items.Add(new MenuBool("Tiamat", "Use Tiamat")).SetValue(true);
            items.Add(new MenuBool("Hydra", "Use Hydra")).SetValue(true);
            items.Add(new MenuBool("Bilge", "Use Bilge")).SetValue(true);
            items.Add(new MenuSlider("BilgeEnemyhp", "If Enemy Hp <", 85, 1, 100));
            items.Add(new MenuSlider("Bilgemyhp", "Or your Hp < ", 85, 1, 100));
            items.Add(new MenuBool("Blade", "Use Blade")).SetValue(true);
            items.Add(new MenuSlider("BladeEnemyhp", "If Enemy Hp <", 85, 1, 100));
            items.Add(new MenuSlider("Blademyhp", "Or Your  Hp <", 85, 1, 100));
            items.Add(new Menu("Deffensive", "Deffensive"));
            items.Add(new MenuBool("Omen", "Use Randuin Omen")).SetValue(true);
            items.Add(new MenuSlider("Omenenemys", "Randuin if enemys>", 2, 1, 5));
            items.Add(new MenuBool("lotis", "Use Iron Solari")).SetValue(true);
            items.Add(new MenuSlider("lotisminhp", "Solari if Ally Hp<", 35, 1, 100));
            /*_config.SubMenu("items").AddSubMenu(new Menu("Potions", "Potions"));
            _config.SubMenu("items").SubMenu("Potions").AddItem(new MenuItem("Hppotion", "Use Hp potion")).SetValue(true);
            _config.SubMenu("items").SubMenu("Potions").AddItem(new MenuItem("Hppotionuse", "Use Hp potion if HP<", 35, 1, 100)));
            _config.SubMenu("items").SubMenu("Potions").AddItem(new MenuItem("Mppotion", "Use Mp potion")).SetValue(true);
            _config.SubMenu("items").SubMenu("Potions").AddItem(new MenuItem("Mppotionuse", "Use Mp potion if HP<", 35, 1, 100)));
            */
            //Harass
            var harass = _config.Add(new Menu("harass", "Harass"));
            harass.Add(new MenuBool("UseQH", "Use Q")).SetValue(true);
            harass.Add(new MenuBool("UseEH", "Use E")).SetValue(true);
            harass.Add(new MenuBool("UseEQH", "Use EQ Combo")).SetValue(true);
            harass.Add(new MenuSlider("UseEQHHP", "EQ If Your Hp > ", 85, 1, 100));
            harass.Add(new MenuBool("UseItemsharass", "Use Tiamat/Hydra")).SetValue(true);
            harass.Add(new MenuSlider("harassmana", "Minimum Mana% >", 35, 1, 100));
            harass.Add(new MenuKeyBind("harasstoggle", "AutoHarass (toggle)", Keys.G, KeyBindType.Toggle));
            harass.Add(new MenuKeyBind("ActiveHarass", "Harass!", Keys.C, KeyBindType.Press));

            //LaneClear
            var laneclear = _config.Add(new Menu("farm", "Farm"));
            var lanefarm = laneclear.Add(new Menu("lanefarm", "LaneFarm"));
            lanefarm.Add(new MenuBool("UseItemslane", "Use Items in LaneClear")).SetValue(true);
            lanefarm.Add(new MenuBool("UseQL", "Q LaneClear")).SetValue(true);
            lanefarm.Add(new MenuBool("UseEL", "E LaneClear")).SetValue(true);
            lanefarm.Add(new MenuBool("UseWL", "W LaneClear")).SetValue(true);
            lanefarm.Add(new MenuSlider("UseWLHP", "use W if Hp% <", 35, 1, 100));
            lanefarm.Add(new MenuSlider("lanemana", "Minimum Mana% >", 35, 1, 100));
            lanefarm.Add(new MenuKeyBind("Activelane", "Jungle!", Keys.V, KeyBindType.Press));

            var lasthit = laneclear.Add(new Menu("lasthit", "LastHit"));
            lasthit.Add(new MenuBool("UseQLH", "Q LastHit")).SetValue(true);
            lasthit.Add(new MenuBool("UseELH", "E LastHit")).SetValue(true);
            lasthit.Add(new MenuBool("UseWLH", "W LaneClear")).SetValue(true);
            lasthit.Add(new MenuSlider("UseWLHHP", "use W if Hp% <", 35, 1, 100));
            lasthit.Add(new MenuSlider("lastmana", "Minimum Mana% >", 35, 1, 100));
            lasthit.Add(new MenuKeyBind("ActiveLast", "LastHit!", Keys.X, KeyBindType.Press));

            var jungle = laneclear.Add(new Menu("jungle", "Jungle"));
            jungle.Add(new MenuBool("UseItemsjungle", "Use Items in jungle")).SetValue(true);
            jungle.Add(new MenuKeyBind("Usesmite", "Use Smite(toggle)", Keys.H, KeyBindType.Toggle));
            jungle.Add(new MenuBool("UseQJ", "Q Jungle")).SetValue(true);
            jungle.Add(new MenuBool("UseEJ", "E Jungle")).SetValue(true);
            jungle.Add(new MenuBool("UseWJ", "W Jungle")).SetValue(true);
            jungle.Add(new MenuSlider("UseWJHP", "use W if Hp% <", 35, 1, 100));
            jungle.Add(new MenuSlider("junglemana", "Minimum Mana% >", 35, 1, 100));
            jungle.Add(new MenuKeyBind("Activejungle", "Jungle!", Keys.V, KeyBindType.Press));

            //Forest
            var forest = _config.Add(new Menu("forestgump", "Forest Gump"));
            forest.Add(new MenuBool("UseEQF", "Use EQ in Mouse ")).SetValue(true);
            forest.Add(new MenuBool("UseWF", "Use W ")).SetValue(true);
            forest.Add(new MenuKeyBind("Forest", "Active Forest Gump!", Keys.Z, KeyBindType.Press));


            //Misc
            var misc = _config.Add(new Menu("misc", "Misc"));
            misc.Add(new MenuBool("UseIgnitekill", "Use Ignite KillSteal")).SetValue(true);
            misc.Add(new MenuBool("UseQM", "Use Q KillSteal")).SetValue(true);
            misc.Add(new MenuBool("UseRM", "Use R KillSteal")).SetValue(true);
            misc.Add(new MenuBool("Gap_W", "W GapClosers")).SetValue(true);
            misc.Add(new MenuBool("UseEQInt", "EQ to Interrupt")).SetValue(true);
            misc.Add(new MenuBool("usePackets", "Use packets")).SetValue(true);

            //Misc
            var hitchance = misc.Add(new Menu("hitchance", "HitChance"));
            hitchance.Add(new MenuList("Echange", "E Hit", new[] { "Low", "Medium", "High", "Very High" }));

            //Drawings
            var drawings = _config.Add(new Menu("drawings", "Drawings"));
            drawings.Add(new MenuBool("DrawQ", "Draw Q")).SetValue(true);
            drawings.Add(new MenuBool("DrawW", "Draw W")).SetValue(true);
            drawings.Add(new MenuBool("DrawE", "Draw E")).SetValue(true);
            drawings.Add(new MenuBool("DrawR", "Draw R")).SetValue(true);
            drawings.Add(new MenuBool("DrawQR", "Draw EQ-R")).SetValue(true);
            drawings.Add(new MenuBool("Drawsmite", "Draw smite")).SetValue(true);
            drawings.Add(new MenuBool("CircleLag", "Lag Free Circles").SetValue(true));
            drawings.Add(new MenuSlider("CircleThickness", "Circles Thickness", 1, 10, 1));

            //_config.AddToMainMenu();
            _config.Attach();

            Game.Print("<font color='#881df2'>D-Jarvan by thienha1</font> Loaded.");
            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            GameObject.OnCreate += OnCreateObj;
            GameObject.OnDelete += OnDeleteObj;
            Gapcloser.OnGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Interrupter.OnInterrupterSpell += Interrupter_OnPossibleToInterrupt;
           
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (_config["combo"]["ActiveCombo"].GetValue<MenuKeyBind>().Active)
            {
                Combo();
            }
            if (_config["combo"]["ActiveComboEQR"].GetValue<MenuKeyBind>().Active)
            {
                ComboEqr();
            }
            if ((_config["harass"]["ActiveHarass"].GetValue<MenuKeyBind>().Active ||
                 _config["harass"]["harasstoggle"].GetValue<MenuKeyBind>().Active) &&
                (100 * (_player.Mana / _player.MaxMana)) > _config["harass"]["harassmana"].GetValue<MenuSlider>().Value)
            {
                Harass();

            }
            if (_config["farm"]["Activelane"].GetValue<MenuKeyBind>().Active &&
                (100 * (_player.Mana / _player.MaxMana)) > _config["farm"]["lanemana"].GetValue<MenuSlider>().Value)
            {
                Laneclear();
            }
            if (_config["farm"]["Activejungle"].GetValue<MenuKeyBind>().Active &&
                (100 * (_player.Mana / _player.MaxMana)) > _config["farm"]["junglemana"].GetValue<MenuSlider>().Value)
            {
                JungleClear();
            }
            if (_config["farm"]["ActiveLast"].GetValue<MenuKeyBind>().Active &&
                (100 * (_player.Mana / _player.MaxMana)) > _config["farm"]["lastmana"].GetValue<MenuSlider>().Value)
            {
                LastHit();
            }
            if (_config["forestgump"]["Forest"].GetValue<MenuKeyBind>().Active)
            {
                Forest();
            }
            if (_config["farm"]["Usesmite"].GetValue<MenuKeyBind>().Active)
            {
                Smiteuse();
            }

            _player = ObjectManager.Player;

            Orbwalker.AttackState = true;

            KillSteal();

        }
        private static void AntiGapcloser_OnEnemyGapcloser(AIBaseClient sender, Gapcloser.GapcloserArgs args)
        {
            if (_w.IsReady() && sender.IsValidTarget(_w.Range) && _config["misc"]["Gap_W"].GetValue<MenuBool>().Enabled)
            {
                _w.Cast(sender, Packets());
            }
        }
        private static void Interrupter_OnPossibleToInterrupt(AIBaseClient sender, Interrupter.InterruptSpellArgs args)
        {
            if (_e.IsReady() && _q.IsReady() && sender.IsValidTarget(_q.Range) && _config["misc"]["UseEQInt"].GetValue<MenuBool>().Enabled
                && _e.GetPrediction(sender).Hitchance >= Echange())
                _e.Cast(sender, Packets());
            _q.Cast(sender, Packets());
        }
        private static float ComboDamage(AIBaseClient enemy)
        {
            var damage = 0d;
            if (_igniteSlot != SpellSlot.Unknown &&
               _player.Spellbook.CanUseSpell(_igniteSlot) == SpellState.Ready)
                damage += ObjectManager.Player.GetSummonerSpellDamage(enemy, SummonerSpell.Ignite);
            if (Items.HasItem(_player, 3077) && Items.CanUseItem(_player, 3077))
                damage += _player.GetItemDamage(enemy, DamageItems.Tiamat);
            if (Items.HasItem(_player, 3074) && Items.CanUseItem(_player, 3074))
                damage += _player.GetItemDamage(enemy, DamageItems.RavenousHydra);
            if (Items.HasItem(_player, 3153) && Items.CanUseItem(_player, 3153))
                damage += _player.GetItemDamage(enemy, DamageItems.BotRK);
            if (Items.HasItem(_player, 3144) && Items.CanUseItem(_player, 3144))
                damage += _player.GetItemDamage(enemy, DamageItems.BilgewaterCutlass);
            if (_q.IsReady())
                damage += _player.GetSpellDamage(enemy, SpellSlot.Q) * 2 * 1.2;
            if (_e.IsReady())
                damage += _player.GetSpellDamage(enemy, SpellSlot.E);
            if (_r.IsReady())
                damage += _player.GetSpellDamage(enemy, SpellSlot.R);

            damage += _player.GetAutoAttackDamage(enemy) * 1.1;
            damage += _player.GetAutoAttackDamage(enemy);
            return (float)damage;
        }

        private static void Combo()
        {
            var useQ = _config["combo"]["UseQC"].GetValue<MenuBool>();
            var useW = _config["combo"]["UseWC"].GetValue<MenuBool>();
            var useE = _config["combo"]["UseEC"].GetValue<MenuBool>();
            var useR = _config["combo"]["UseRC"].GetValue<MenuBool>();
            var autoR = _config["combo"]["UseRE"].GetValue<MenuBool>();

            var t = TargetSelector.GetTarget(_e.Range, DamageType.Magical);
            if (t != null && _config["combo"]["UseIgnite"].GetValue<MenuBool>() && _igniteSlot != SpellSlot.Unknown &&
                _player.Spellbook.CanUseSpell(_igniteSlot) == SpellState.Ready)
            {
                if (ComboDamage(t) > t.Health)
                {
                    _player.Spellbook.CastSpell(_igniteSlot, t);
                }
            }
            if (useR && _r.IsReady())
            {
                if (t != null && !_haveulti)
                    if (!t.HasBuff("JudicatorIntervention") && !t.HasBuff("Undying Rage") &&
                        ComboDamage(t) > t.Health)
                        _r.CastIfHitchanceEquals(t, HitChance.Medium, Packets());
            }
            if (useE && _e.IsReady() && _q.IsReady() && useQ)
            {
                //xsalice Code
                var vec = t.PreviousPosition - _player.PreviousPosition;
                var castBehind = _e.GetPrediction(t).CastPosition + Vector3.Normalize(vec) * 75;
                _e.Cast(castBehind, Packets());
                _q.Cast(t, Packets());
            }
            else
            {
                _e.Cast(t, Packets(), true);
                _q.Cast(t, Packets());
            }
            /* {

                if (t != null && t.Distance(_player.Position) <= _e.Range && _e.GetPrediction(t).Hitchance >= Echange())
                    _e.Cast(t, Packets(), true);
                _q.Cast(t, Packets(), true);
            }*/
            if (useW && _w.IsReady())
            {
                if (t != null && t.Distance(_player.Position) < _w.Range)
                    _w.Cast();

            }
            if (useQ && _q.IsReady() && !_e.IsReady() && !_w.IsReady())
            {
                if (t != null && t.Distance(_player.Position) < _q.Range)
                    _q.Cast(t, Packets(), true);

            }
            if (_r.IsReady() && autoR && !_haveulti)
            {
                if (ObjectManager.Get<AIHeroClient>().Count(hero => hero.IsValidTarget(_r.Range)) >=
                    _config["combo"]["MinTargets"].GetValue<MenuSlider>().Value)
                    _r.Cast(t, Packets(), true);
            }
            UseItemes(t);
        }

        private static void ComboEqr()
        {
            _player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            var manacheck = _player.Mana > _player.Spellbook.GetSpell(SpellSlot.Q).ManaCost + _player.Spellbook.GetSpell(SpellSlot.E).ManaCost + _player.Spellbook.GetSpell(SpellSlot.R).ManaCost;
            var t = TargetSelector.GetTarget(_q.Range + _r.Range, DamageType.Magical);

            if (_e.IsReady() && _q.IsReady() && manacheck)
            {
                if (t != null && _player.Distance(t) > _q.Range)
                    _e.Cast(t.PreviousPosition, Packets());
                _q.Cast(t.PreviousPosition, Packets());

            }
            if (t != null && _config["combo"]["UseIgnite"].GetValue<MenuBool>() && _igniteSlot != SpellSlot.Unknown &&
               _player.Spellbook.CanUseSpell(_igniteSlot) == SpellState.Ready)
            {
                if (ComboDamage(t) > t.Health)
                {
                    _player.Spellbook.CastSpell(_igniteSlot, t);
                }
            }
            if (_r.IsReady() && !_haveulti && t != null)
            {
                _r.CastIfHitchanceEquals(t, HitChance.Immobile, Packets());
            }
            if (_w.IsReady())
            {
                if (t != null && t.Distance(_player.Position) < _w.Range)
                    _w.Cast();
            }
            UseItemes(t);
        }


        private static void Harass()
        {
            var target = TargetSelector.GetTarget(_e.Range, DamageType.Magical);
            var useQ = _config["harass"]["UseQH"].GetValue<MenuBool>();
            var useE = _config["harass"]["UseEH"].GetValue<MenuBool>();
            var useEq = _config["harass"]["UseEQH"].GetValue<MenuBool>();
            var useEqhp = (100 * (_player.Health / _player.MaxHealth)) > _config["harass"]["UseEQHHP"].GetValue<MenuSlider>().Value;
            var useItemsH = _config["harass"]["UseItemsharass"].GetValue<MenuBool>();
            if (useEqhp && useEq && _q.IsReady() && _e.IsReady())
            {
                var t = TargetSelector.GetTarget(_e.Range, DamageType.Magical);
                if (t != null && t.Distance(_player.Position) < _e.Range && _e.GetPrediction(t).Hitchance >= Echange())
                    _e.Cast(t, Packets());
                _q.Cast(t, Packets());
            }
            if (useQ && _q.IsReady())
            {
                var t = TargetSelector.GetTarget(_e.Range, DamageType.Magical);
                if (t != null && t.Distance(_player.Position) < _q.Range)
                    _q.Cast(t, Packets());
            }
            if (useE && _e.IsReady())
            {
                var t = TargetSelector.GetTarget(_e.Range, DamageType.Magical);
                if (t != null && t.Distance(_player.Position) < _e.Range && _e.GetPrediction(t).Hitchance >= Echange())
                    _e.Cast(t, Packets());
            }

            if (useItemsH && Items.CanUseItem(_player, 3077) && target.Distance(_player.Position) < _tiamat.Range)
            {
                _tiamat.Cast();
            }
            if (useItemsH && Items.CanUseItem(_player, 3074) && target.Distance(_player.Position) < _hydra.Range)
            {
                _hydra.Cast();
            }
        }

        private static void Laneclear()
        {
            var allMinionsQ = GameObjects.GetMinions(ObjectManager.Player.PreviousPosition, _q.Range, MinionTypes.All);
            var rangedMinionsQ = GameObjects.GetMinions(ObjectManager.Player.PreviousPosition, _q.Range + _q.Width,
                MinionTypes.Ranged);
            var rangedMinionsE = GameObjects.GetMinions(ObjectManager.Player.PreviousPosition, _e.Range + _e.Width,
                MinionTypes.Ranged);
            var allMinionsE = GameObjects.GetMinions(ObjectManager.Player.PreviousPosition, _e.Range + _e.Width,
                MinionTypes.All);
            var useItemsl = _config["farm"]["UseItemslane"].GetValue<MenuBool>();
            var useQl = _config["farm"]["UseQL"].GetValue<MenuBool>();
            var useEl = _config["farm"]["UseEL"].GetValue<MenuBool>();
            var useWl = _config["farm"]["UseWL"].GetValue<MenuBool>();
            var usewhp = (100 * (_player.Health / _player.MaxHealth)) < _config["farm"]["UseWLHP"].GetValue<MenuSlider>().Value;

            if (_q.IsReady() && useQl)
            {
                var fl1 = _e.GetLineFarmLocation(rangedMinionsQ, _e.Width);
                var fl2 = _e.GetLineFarmLocation(allMinionsQ, _e.Width);

                if (fl1.MinionsHit >= 3)
                {
                    _q.Cast(fl1.Position);
                }
                else if (fl2.MinionsHit >= 2 || allMinionsQ.Count == 1)
                {
                    _q.Cast(fl2.Position);
                }
                else
                    foreach (var minion in allMinionsE)
                        if (!minion.InAutoAttackRange() &&
                            minion.Health < 0.75 * _player.GetSpellDamage(minion, SpellSlot.Q))
                            _q.Cast(minion);
            }

            if (_e.IsReady() && useEl)
            {
                var fl1 = _e.GetCircularFarmLocation(rangedMinionsE, _e.Width);
                var fl2 = _e.GetCircularFarmLocation(allMinionsE, _e.Width);

                if (fl1.MinionsHit >= 3)
                {
                    _e.Cast(fl1.Position);
                }
                else if (fl2.MinionsHit >= 2 || allMinionsE.Count == 1)
                {
                    _e.Cast(fl2.Position);
                }
                else
                    foreach (var minion in allMinionsE)
                        if (!minion.InAutoAttackRange() &&
                            minion.Health < 0.75 * _player.GetSpellDamage(minion, SpellSlot.E))
                            _e.Cast(minion);
            }
            if (usewhp && useWl && _w.IsReady() && allMinionsQ.Count > 0)
            {
                _w.Cast();

            }
            if (useItemsl && Items.CanUseItem(_player, 3077) && allMinionsQ.Count > 2)
            {
                _tiamat.Cast();
            }
            if (useItemsl && Items.CanUseItem(_player, 3074) && allMinionsQ.Count > 2)
            {
                _hydra.Cast();
            }
        }

        private static void LastHit()
        {
            var allMinions = GameObjects.GetMinions(ObjectManager.Player.PreviousPosition, _q.Range, MinionTypes.All);
            var useQ = _config["farm"]["UseQLH"].GetValue<MenuBool>();
            var useW = _config["farm"]["UseQWH"].GetValue<MenuBool>();
            var useE = _config["farm"]["UseQEH"].GetValue<MenuBool>();
            var usewhp = (100 * (_player.Health / _player.MaxHealth)) < _config["farm"]["UseWLHHP"].GetValue<MenuSlider>().Value;
            foreach (var minion in allMinions)
            {
                if (useQ && _q.IsReady() && _player.Distance(minion) < _q.Range &&
                    minion.Health < 0.95 * _player.GetSpellDamage(minion, SpellSlot.Q))
                {
                    _q.Cast(minion, Packets());
                }

                if (_e.IsReady() && useE && _player.Distance(minion) < _e.Range &&
                    minion.Health < 0.95 * _player.GetSpellDamage(minion, SpellSlot.E))
                {
                    _e.Cast(minion, Packets());
                }
                if (usewhp && useW && _w.IsReady() && allMinions.Count > 0)
                {
                    _w.Cast();

                }
            }
        }

        private static void JungleClear()
        {
            var mobs = GameObjects.GetJungles(_player.PreviousPosition, _q.Range,
                JungleType.All, JungleOrderTypes.MaxHealth);
            var useItemsJ = _config["farm"]["UseItemsjungle"].GetValue<MenuBool>();
            var useQ = _config["farm"]["UseQJ"].GetValue<MenuBool>();
            var useW = _config["farm"]["UseWJ"].GetValue<MenuBool>();
            var useE = _config["farm"]["UseEJ"].GetValue<MenuBool>();
            var usewhp = (100 * (_player.Health / _player.MaxHealth)) < _config["farm"]["UseWJHP"].GetValue<MenuSlider>().Value;

            if (mobs.Count > 0)
            {
                var mob = mobs[0];
                if (useQ && _q.IsReady() && _player.Distance(mob) < _q.Range)
                {
                    _q.Cast(mob, Packets());
                }
                if (_e.IsReady() && useE && _player.Distance(mob) < _q.Range)
                {
                    _e.Cast(mob, Packets());
                }
                if (_w.IsReady() && useW && usewhp && _player.Distance(mob) < _w.Range)
                {
                    _w.Cast();
                }
                if (useItemsJ && Items.CanUseItem(_player, 3077) && _player.Distance(mob) < _tiamat.Range)
                {
                    _tiamat.Cast();
                }
                if (useItemsJ && Items.CanUseItem(_player, 3074) && _player.Distance(mob) < _hydra.Range)
                {
                    _hydra.Cast();
                }
            }

        }

        private static MenuBool Packets()
        {
            return _config["misc"]["usePackets"].GetValue<MenuBool>();
        }

        private static HitChance Echange()
        {
            switch (_config["hitchance"]["Echange"].GetValue<MenuList>().HoveringIndex)
            {
                case 0:
                    return HitChance.Low;
                case 1:
                    return HitChance.Medium;
                case 2:
                    return HitChance.High;
                case 3:
                    return HitChance.VeryHigh;
                default:
                    return HitChance.Medium;
            }
        }

        private static int getSmiteDmg()
        {
            int level = _player.Level;
            int index = _player.Level / 5;
            float[] dmgs = { 370 + 20 * level, 330 + 30 * level, 240 + 40 * level, 100 + 50 * level };
            return (int)dmgs[index];
        }

        private static void Smiteuse()
        {
            string[] jungleMinions;
            if (Map.GetMap().Type.Equals(Map.MapType.TwistedTreeline))
            {
                jungleMinions = new string[] { "TT_Spiderboss", "TT_NWraith", "TT_NGolem", "TT_NWolf" };
            }
            else
            {
                jungleMinions = new string[] { "AncientGolem", "LizardElder", "Worm", "Dragon" };
            }

            var minions = GameObjects.GetJungles(_player.Position, 1000, JungleType.All, JungleOrderTypes.MaxHealth);
            if (minions.Count() > 0)
            {
                int smiteDmg = getSmiteDmg();
                foreach (AIBaseClient minion in minions)
                {

                    Boolean b;
                    if (Map.GetMap().Type.Equals(Map.MapType.TwistedTreeline))
                    {
                        b = minion.Health <= smiteDmg &&
                            jungleMinions.Any(name => minion.Name.Substring(0, minion.Name.Length - 5).Equals(name));
                    }
                    else
                    {
                        b = minion.Health <= smiteDmg && jungleMinions.Any(name => minion.Name.StartsWith(name));
                    }

                    if (b)
                    {
                        _player.Spellbook.CastSpell(_smiteSlot.Slot, minion);
                    }
                }
            }
        }

        private static void UseItemes(AIHeroClient target)
        {
            var iBilge = _config["items"]["Bilge"].GetValue<MenuBool>();
            var iBilgeEnemyhp = target.Health <=
                                (target.MaxHealth * (_config["items"]["BilgeEnemyhp"].GetValue<MenuSlider>().Value) / 100);
            var iBilgemyhp = _player.Health <=
                             (_player.MaxHealth * (_config["items"]["Bilgemyhp"].GetValue<MenuSlider>().Value) / 100);
            var iBlade = _config["items"]["Blade"].GetValue<MenuBool>();
            var iBladeEnemyhp = target.Health <=
                                (target.MaxHealth * (_config["items"]["BladeEnemyhp"].GetValue<MenuSlider>().Value) / 100);
            var iBlademyhp = _player.Health <=
                             (_player.MaxHealth * (_config["items"]["Blademyhp"].GetValue<MenuSlider>().Value) / 100);
            var iOmen = _config["items"]["Omen"].GetValue<MenuBool>();
            var iOmenenemys = ObjectManager.Get<AIHeroClient>().Count(hero => hero.IsValidTarget(450)) >=
                              _config["items"]["Omenenemys"].GetValue<MenuSlider>().Value;
            var iTiamat = _config["items"]["Tiamat"].GetValue<MenuBool>();
            var iHydra = _config["items"]["Hydra"].GetValue<MenuBool>();
            var ilotis = _config["items"]["lotis"].GetValue<MenuBool>();
            //var ihp = _config.Item("Hppotion").GetValue<MenuBool>();
            // var ihpuse = _player.Health <= (_player.MaxHealth * (_config.Item("Hppotionuse").GetValue<MenuSlider>().Value) / 100);
            //var imp = _config.Item("Mppotion").GetValue<MenuBool>();
            //var impuse = _player.Health <= (_player.MaxHealth * (_config.Item("Mppotionuse").GetValue<MenuSlider>().Value) / 100);

            if (_player.Distance(target) <= 450 && iBilge && (iBilgeEnemyhp || iBilgemyhp) && Items.CanUseItem(_player, 3144))
            {
                _bilge.Cast(target);

            }
            if (_player.Distance(target) <= 450 && iBlade && (iBladeEnemyhp || iBlademyhp) && Items.CanUseItem(_player, 3153))
            {
                _blade.Cast(target);

            }
            if (_player.CountEnemyHeroesInRange(350) >= 1 && iTiamat && Items.CanUseItem(_player, 3077))
            {
                _tiamat.Cast();

            }
            if (_player.CountEnemyHeroesInRange(350) >= 1 && iHydra && Items.CanUseItem(_player, 3074))
            {
                _hydra.Cast();

            }
            if (iOmenenemys && iOmen && Items.CanUseItem(_player, 3143))
            {
                _rand.Cast();

            }
            if (ilotis)
            {
                foreach (var hero in ObjectManager.Get<AIHeroClient>().Where(hero => hero.IsAlly || hero.IsMe))
                {
                    if (hero.Health <= (hero.MaxHealth * (_config["items"]["lotisminhp"].GetValue<MenuSlider>().Value) / 100) &&
                        hero.Distance(_player.PreviousPosition) <= _lotis.Range && Items.CanUseItem(_player, 3190))
                        _lotis.Cast();
                }
            }
        }


        private static void KillSteal()
        {
            var target = TargetSelector.GetTarget(_q.Range, DamageType.Magical);
            var igniteDmg = _player.GetSummonerSpellDamage(target, SummonerSpell.Ignite);
            if (target != null && _config["misc"]["UseIgnitekill"].GetValue<MenuBool>() && _igniteSlot != SpellSlot.Unknown &&
            _player.Spellbook.CanUseSpell(_igniteSlot) == SpellState.Ready)
            {
                if (igniteDmg > target.Health)
                {
                    _player.Spellbook.CastSpell(_igniteSlot, target);
                }
            }
            if (_q.IsReady() && _config["misc"]["UseQM"].GetValue<MenuBool>())
            {
                var t = TargetSelector.GetTarget(_q.Range, DamageType.Magical);
                if (_q.GetDamage(t) > t.Health && _player.Distance(t) <= _q.Range)
                {
                    _q.Cast(t, Packets());
                }
            }
            if (_r.IsReady() && _config["misc"]["UseRM"].GetValue<MenuBool>())
            {
                var t = TargetSelector.GetTarget(_r.Range, DamageType.Magical);
                if (t != null)
                    if (!t.HasBuff("JudicatorIntervention") && !t.HasBuff("Undying Rage") && _r.GetDamage(t) > t.Health)
                        _r.Cast(t, Packets(), true);
            }
        }

        private static void Forest()
        {
            var manacheck = _player.Mana > _player.Spellbook.GetSpell(SpellSlot.Q).ManaCost + _player.Spellbook.GetSpell(SpellSlot.E).ManaCost;
            _player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            var target = TargetSelector.GetTarget(_e.Range, DamageType.Magical);

            if (_config["forestgump"]["UseEQF"].GetValue<MenuBool>() && _q.IsReady() && _e.IsReady() && manacheck)
            {
                _e.Cast(Game.CursorPos, Packets());
                _q.Cast(Game.CursorPos, Packets());
            }
            if (_config["forestgump"]["UseWF"].GetValue<MenuBool>() && _w.IsReady() && target != null &&
                _player.Distance(target) < _w.Range)
            {
                _w.Cast();
            }

        }

        private static void OnCreateObj(GameObject sender, EventArgs args)
        {
            if (!(sender is EffectEmitter)) return;
            var obj = (EffectEmitter)sender;
            if (obj != null && obj.IsMe && obj.Name == "JarvanCataclysm_tar")

            //debug
            //if (unit == ObjectManager.Player.Name)
            {
                // Game.PrintChat("Spell: " + name);
                _haveulti = true;
                return;
            }
        }

        private static void OnDeleteObj(GameObject sender, EventArgs args)
        {
            if (!(sender is EffectEmitter)) return;
            var obj = (EffectEmitter)sender;
            if (obj != null && obj.IsMe && obj.Name == "JarvanCataclysm_tar")
            {
                _haveulti = false;
                return;
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (_config["drawings"]["Drawsmite"].GetValue<MenuBool>())
            {
                if (_config["drawings"]["Usesmite"].GetValue<MenuKeyBind>().Active)
                {
                    Drawing.DrawText(Drawing.Width * 0.90f, Drawing.Height * 0.68f, System.Drawing.Color.DarkOrange,
                        "Smite Is On");
                }
                else
                    Drawing.DrawText(Drawing.Width * 0.90f, Drawing.Height * 0.68f, System.Drawing.Color.DarkRed,
                        "Smite Is Off");
            }
            if (_config["drawings"]["CircleLag"].GetValue<MenuBool>())
            {
                if (_config["drawings"]["DrawQ"].GetValue<MenuBool>())
                {
                    MiniMap.DrawCircle(ObjectManager.Player.Position, _q.Range, System.Drawing.Color.Gray,
                        _config["drawings"]["CircleThickness"].GetValue<MenuSlider>().Value);
                }
                if (_config["drawings"]["DrawW"].GetValue<MenuBool>())
                {
                    MiniMap.DrawCircle(ObjectManager.Player.Position, _w.Range, System.Drawing.Color.Gray,
                        _config["drawings"]["CircleThickness"].GetValue<MenuSlider>().Value);
                }
                if (_config["drawings"]["DrawE"].GetValue<MenuBool>())
                {
                    MiniMap.DrawCircle(ObjectManager.Player.Position, _e.Range, System.Drawing.Color.Gray,
                        _config["drawings"]["CircleThickness"].GetValue<MenuSlider>().Value);
                }
                if (_config["drawings"]["DrawR"].GetValue<MenuBool>())
                {
                    MiniMap.DrawCircle(ObjectManager.Player.Position, _r.Range, System.Drawing.Color.Gray,
                        _config["drawings"]["CircleThickness"].GetValue<MenuSlider>().Value);
                }
                if (_config["drawings"]["DrawQR"].GetValue<MenuBool>())
                {
                    MiniMap.DrawCircle(ObjectManager.Player.Position, _q.Range + _r.Range, System.Drawing.Color.Gray,
                        _config["drawings"]["CircleThickness"].GetValue<MenuSlider>().Value);
                }
            }
            else
            {
                if (_config["drawings"]["DrawQ"].GetValue<MenuBool>())
                {
                    Drawing.DrawCircle(ObjectManager.Player.Position, _q.Range, System.Drawing.Color.White);
                }
                if (_config["drawings"]["DrawW"].GetValue<MenuBool>())
                {
                    Drawing.DrawCircle(ObjectManager.Player.Position, _w.Range, System.Drawing.Color.White);
                }
                if (_config["drawings"]["DrawE"].GetValue<MenuBool>())
                {
                    Drawing.DrawCircle(ObjectManager.Player.Position, _e.Range, System.Drawing.Color.White);
                }

                if (_config["drawings"]["DrawR"].GetValue<MenuBool>())
                {
                    Drawing.DrawCircle(ObjectManager.Player.Position, _r.Range, System.Drawing.Color.White);
                }
                if (_config["drawings"]["DrawQR"].GetValue<MenuBool>())
                {
                    Drawing.DrawCircle(ObjectManager.Player.Position, _q.Range + _r.Range, System.Drawing.Color.White);
                }
            }
        }
    }
}




