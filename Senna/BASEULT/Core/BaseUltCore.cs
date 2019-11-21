using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI.Values;
using SharpDX;
using SharpDX.Direct3D9;
using Menu;
using Color = System.Drawing.Color;
using Damage = EnsoulSharp.SDK.Damage;
using Rectangle = SharpDX.Rectangle;
using Teleport = EnsoulSharp.SDK.Teleport;

namespace Core
{
    public class BaseUltCore
    {
        private const int Length = 260;
        private const int Height = 25;
        private const int LineThickness = 4;
        private static readonly List<Recall> Recalls = new List<Recall>();
        private static readonly List<BaseUltUnit> BaseUltUnits = new List<BaseUltUnit>();
        private static readonly List<BaseUltSpell> BaseUltSpells = new List<BaseUltSpell>();
        private static readonly AIHeroClient Player = ObjectManager.Player;
        public static int X = (int)TacticalMap.X - MenuManager.BarSettingsX;
        public static int Y = (int)TacticalMap.Y - MenuManager.BarSettingsY;

        public static void Initialize()
        {
            foreach (var hero in ObjectManager.Get<AIHeroClient>())
            {
                Recalls.Add(new Recall(hero, RecallStatus.Inactive));
            }

            #region Spells

            BaseUltSpells.Add(new BaseUltSpell("Ezreal", SpellSlot.R, 1000, 2000, 160, false));
            BaseUltSpells.Add(new BaseUltSpell("Jinx", SpellSlot.R, 600, 1700, 140, true));
            BaseUltSpells.Add(new BaseUltSpell("Ashe", SpellSlot.R, 250, 1600, 130, true));
            BaseUltSpells.Add(new BaseUltSpell("Draven", SpellSlot.R, 400, 2000, 160, true));
            BaseUltSpells.Add(new BaseUltSpell("Karthus", SpellSlot.R, 3125, 0, 0, false));
            BaseUltSpells.Add(new BaseUltSpell("Ziggs", SpellSlot.R, 250, 3100, 0, false));
            BaseUltSpells.Add(new BaseUltSpell("Lux", SpellSlot.R, 1375, 0, 0, false));
            BaseUltSpells.Add(new BaseUltSpell("Xerath", SpellSlot.R, 700, 600, 0, false));
            BaseUltSpells.Add(new BaseUltSpell("Senna", SpellSlot.R, 1150, 20000, 130, false));
            #endregion
        }


        public static void Game_OnUpdate()
        {
            foreach (var recall in Recalls)
            {
                if (recall.Status != RecallStatus.Inactive)
                {
                    var recallDuration = recall.Duration;
                    var cd = recall.Started + recallDuration - Game.Time;
                    var percent = (cd > 0 && Math.Abs(recallDuration) > float.Epsilon) ? 1f - (cd / recallDuration) : 1f;
                    var textLength = (recall.Unit.CharacterName.Length + 6) * 7;
                    var myLength = percent * Length;
                    var freeSpaceLength = myLength + textLength;
                    var freeSpacePercent = freeSpaceLength / Length > 1 ? 1 : freeSpaceLength / Length;
                    if (
                        Recalls.Any(
                            h =>
                                GetRecallPercent(h) > percent && GetRecallPercent(h) < freeSpacePercent &&
                                h.TextPos == recall.TextPos && recall.Started > h.Started))
                    {
                        recall.TextPos += 1;
                    }

                    if (recall.Status == RecallStatus.Finished &&
                        Recalls.Any(
                            h =>
                                h.Started > recall.Started && h.TextPos == recall.TextPos &&
                                recall.Started + 3 < h.Started + recall.Duration))
                    {
                        recall.TextPos += 1;
                    }
                }

                if (recall.Status == RecallStatus.Active)
                {
                    var compatibleChamps = new[] { "Jinx", "Ezreal", "Ashe", "Draven", "Karthus","Senna" }; //Ziggs, Xerath, Lux
                    if (recall.Unit.IsEnemy && compatibleChamps.Any(h => h == Player.CharacterName) &&
                        BaseUltUnits.All(h => h.Unit.NetworkId != recall.Unit.NetworkId))
                    {
                        var spell = BaseUltSpells.Find(h => h.Name == Player.CharacterName);
                        if (Player.Spellbook.GetSpell(spell.Slot).IsReady &&
                            Player.Spellbook.GetSpell(spell.Slot).Level > 0)
                        {
                            BaseUltCalcs(recall);
                        }
                    }
                }

                if (recall.Status != RecallStatus.Active)
                {
                    var baseultUnit = BaseUltUnits.Find(h => h.Unit.NetworkId == recall.Unit.NetworkId);
                    if (baseultUnit != null)
                    {
                        BaseUltUnits.Remove(baseultUnit);
                    }
                }
            }

            foreach (var unit in BaseUltUnits)
            {
                if (MenuManager.checkcollision && unit.Collision)
                {
                    continue;
                }

                if (unit.Unit.IsVisible)
                {
                    unit.LastSeen = Game.Time;
                }

                var timeLimit = MenuManager.timeLimit;
                if (Math.Round(unit.FireTime, 1) == Math.Round(Game.Time, 1) && Game.Time - timeLimit >= unit.LastSeen)
                {
                    var spell = Player.Spellbook.GetSpell(BaseUltSpells.Find(h => h.Name == Player.CharacterName).Slot);
                    if (spell.IsReady && !MenuManager.nobaseult)
                    {
                        Player.Spellbook.CastSpell(spell.Slot, GetFountainPos());
                    }
                }
            }
        }

        public static void Drawing_OnEndScene()
        {
            if (!MenuManager.showrecalls && !BaseUltUnits.Any())
            {
                return;
            }

            Drawing.DrawLine(X, Y, X + Length, Y, Height, ColorTranslator.FromHtml("#080d0a"));


            foreach (var recall in Recalls.OrderBy(h => h.Started))
            {
                if ((recall.Unit.IsAlly && !MenuManager.showallies) ||
                    (!recall.Unit.IsAlly && !MenuManager.showenemies))
                {
                    continue;
                }

                var recallDuration = recall.Duration;
                if (recall.Status == RecallStatus.Active)
                {
                    var isBaseUlt = BaseUltUnits.Any(h => h.Unit.NetworkId == recall.Unit.NetworkId);
                    var percent = GetRecallPercent(recall);
                    var colorIndicator = isBaseUlt
                        ? Color.OrangeRed
                        : (recall.Unit.IsAlly ? Color.DeepSkyBlue : Color.DarkViolet);
                    var colorText = isBaseUlt
                        ? Color.OrangeRed
                        : (recall.Unit.IsAlly ? Color.DeepSkyBlue : Color.PaleVioletRed);
                    var colorBar = isBaseUlt
                        ? Color.Red
                        : (recall.Unit.IsAlly ? Color.DodgerBlue : Color.MediumVioletRed);


                    Drawing.DrawLine((int)(percent * Length) + X - (float)(LineThickness * 0.5) + 4,
                        Y - (float)(Height * 0.5), (int)(percent * Length) + X - (float)(LineThickness * 0.5) + 4,
                        Y + (float)(Height * 0.5) + recall.TextPos * 20, LineThickness, colorIndicator);


                }

                if (recall.Status == RecallStatus.Abort || recall.Status == RecallStatus.Finished)
                {
                    const int fadeoutTime = 3;
                    var colorIndicator = recall.Status == RecallStatus.Abort ? Color.OrangeRed : Color.GreenYellow;
                    var colorText = recall.Status == RecallStatus.Abort ? Color.Orange : Color.GreenYellow;
                    var colorBar = recall.Status == RecallStatus.Abort ? Color.Yellow : Color.LawnGreen;
                    var fadeOutPercent = (recall.Ended + fadeoutTime - Game.Time) / fadeoutTime;
                    if (recall.Ended + fadeoutTime > Game.Time)
                    {
                        var timeUsed = recall.Ended - recall.Started;
                        var percent = timeUsed > recallDuration ? 1 : timeUsed / recallDuration;


                        Drawing.DrawLine((int)(percent * Length) + X - (float)(LineThickness * 0.5) + 4,
                            Y - (float)(Height * 0.5), (int)(percent * Length) + X - (float)(LineThickness * 0.5) + 4,
                            Y + (float)(Height * 0.5), LineThickness,
                            Color.FromArgb((int)(254 * fadeOutPercent), colorIndicator));

                    }
                    else
                    {
                        recall.Status = RecallStatus.Inactive;
                        recall.TextPos = 0;
                    }
                }
            }

            foreach (var unit in BaseUltUnits)
            {
                var duration = Recalls.Find(h => h.Unit.NetworkId == unit.Unit.NetworkId).Duration;
                var barPos = (unit.FireTime - Recalls.Find(h => unit.Unit.NetworkId == h.Unit.NetworkId).Started) /
                             duration;

                Drawing.DrawLine((int)(barPos * Length) + X - (float)(LineThickness * 0.5),
                    Y - (float)(Height * 0.5 + LineThickness), (int)(barPos * Length) + X - (float)(LineThickness * 0.5),
                    Y + (float)(Height * 0.5 + LineThickness), LineThickness, Color.Lime);
            }


        }

        private static Vector3 GetFountainPos()
        {
            switch (Game.MapId)
            {
                case GameMapId.SummonersRift:
                    {
                        return Player.Team == GameObjectTeam.Order
                            ? new Vector3(14296, 14362, 171)
                            : new Vector3(408, 414, 182);
                    }
                case GameMapId.HowlingAbyss:
                    {
                        return Player.Team == GameObjectTeam.Order
                            ? new Vector3(524, 4164, 35)
                            : new Vector3(13323, 4105, 36);
                    }
                case GameMapId.TwistedTreeline:
                    {
                        return Player.Team == GameObjectTeam.Order
                            ? new Vector3(1060, 7297, 150)
                            : new Vector3(14353, 7297, 150);
                    }
            }

            return new Vector3();
        }

        private static double GetRecallPercent(Recall recall)
        {
            var recallDuration = recall.Duration;
            var cd = recall.Started + recallDuration - Game.Time;
            var percent = (cd > 0 && Math.Abs(recallDuration) > float.Epsilon) ? 1f - (cd / recallDuration) : 1f;
            return percent;
        }

        private static float GetBaseUltTravelTime(float delay, float speed)
        {
            if (Player.CharacterName == "Karthus")
            {
                return delay / 1000;
            }

            var distance = Vector3.Distance(Player.Position, GetFountainPos());
            var missilespeed = speed;
            if (Player.CharacterName == "Jinx" && distance > 1350)
            {
                const float accelerationrate = 0.3f;
                var acceldifference = distance - 1350f;
                if (acceldifference > 150f)
                {
                    acceldifference = 150f;
                }

                var difference = distance - 1500f;
                missilespeed = (1350f * speed + acceldifference * (speed + accelerationrate * acceldifference) +
                                difference * 2200f) / distance;
            }

            return (distance / missilespeed + ((delay - 65) / 1000));
        }

        private static double GetBaseUltSpellDamage(BaseUltSpell spell, AIHeroClient target)
        {
            var level = Player.Spellbook.GetSpell(spell.Slot).Level - 1;
            switch (spell.Name)
            {
                case "Jinx":
                    {
                        var damage = new float[] { 250, 350, 450 }[level] +
                                     new float[] { 25, 30, 35 }[level] / 100 * (target.MaxHealth - target.Health) +
                                     1 * Player.FlatPhysicalDamageMod;
                        return Damage.CalculateDamage(Player, target, DamageType.Physical, damage);
                    }
                case "Ezreal":
                    {
                        var damage = new float[] { 350, 500, 650 }[level] + 0.9f * Player.FlatMagicDamageMod +
                                     1 * Player.FlatPhysicalDamageMod;
                        return Damage.CalculateDamage(Player, target, DamageType.Magical, damage) * 0.7;
                    }
                case "Senna":
                    {
                        var damage = new float[] { 250, 350, 475 }[level] + 0.7f * Player.FlatMagicDamageMod +
                                     1 * Player.FlatPhysicalDamageMod;
                        return Damage.CalculateDamage(Player, target, DamageType.Magical, damage) * 0.7;
                    }
                case "Ashe":
                    {
                        var damage = new float[] { 250, 425, 600 }[level] + 1 * Player.FlatMagicDamageMod;
                        return Damage.CalculateDamage(Player, target, DamageType.Magical, damage);
                    }
                case "Draven":
                    {
                        var damage = new float[] { 175, 275, 375 }[level] + 1.1f * Player.FlatPhysicalDamageMod;
                        return Damage.CalculateDamage(Player, target, DamageType.Physical, damage) * 0.7;
                    }
                case "Karthus":
                    {
                        var damage = new float[] { 250, 400, 550 }[level] + 0.6f * Player.FlatMagicDamageMod;
                        return Damage.CalculateDamage(Player, target, DamageType.Magical, damage);
                    }
                case "Lux":
                    {
                        var damage = new float[] { 300, 400, 500 }[level] + 0.75f * Player.FlatMagicDamageMod;
                        return Damage.CalculateDamage(Player, target, DamageType.Magical, damage);
                    }
                case "Xerath":
                    {
                        var damage = new float[] { 190, 245, 300 }[level] + 0.43f * Player.FlatMagicDamageMod;
                        return Damage.CalculateDamage(Player, target, DamageType.Magical, damage);
                    }
                case "Ziggs":
                    {
                        var damage = new float[] { 250, 375, 500 }[level] + 0.9f * Player.FlatMagicDamageMod;
                        return Damage.CalculateDamage(Player, target, DamageType.Magical, damage);
                    }
            }

            return 0;
        }

        private static void BaseUltCalcs(Recall recall)
        {
            var finishedRecall = recall.Started + recall.Duration;
            var spellData = BaseUltSpells.Find(h => h.Name == Player.CharacterName);
            var timeNeeded = GetBaseUltTravelTime(spellData.Delay, spellData.Speed);
            var fireTime = finishedRecall - timeNeeded;
            var spellDmg = GetBaseUltSpellDamage(spellData, recall.Unit);
            // var collision = GetCollision(spellData.Radius, spellData).Any();
            if (fireTime > Game.Time && fireTime < recall.Started + recall.Duration && recall.Unit.Health < spellDmg &&
                MenuManager.BaseUltMenu["Setttings"]["target" + recall.Unit.CharacterName].GetValue<MenuBool>().Enabled &&
                MenuManager.baseult &&
                !MenuManager.nobaseult)
            {
                BaseUltUnits.Add(new BaseUltUnit(recall.Unit, fireTime, false));
            }
            else if (BaseUltUnits.Any(h => h.Unit.NetworkId == recall.Unit.NetworkId))
            {
                BaseUltUnits.Remove(BaseUltUnits.Find(h => h.Unit.NetworkId == recall.Unit.NetworkId));
            }
        }

        public static void Teleport_OnTeleport(AIBaseClient sender, Teleport.TeleportEventArgs args)
        {
            var unit = Recalls.Find(h => h.Unit.NetworkId == sender.NetworkId);
            if (unit == null || args.Type != Teleport.TeleportType.Recall)
            {
                return;
            }

            switch (args.Status)
            {
                case Teleport.TeleportStatus.Start:
                    {
                        unit.Status = RecallStatus.Active;
                        unit.Started = Game.Time;
                        unit.TextPos = 0;
                        unit.Duration = (float)args.Duration / 1000;
                        break;
                    }

                case Teleport.TeleportStatus.Abort:
                    {
                        unit.Status = RecallStatus.Abort;
                        unit.Ended = Game.Time;
                        break;
                    }

                case Teleport.TeleportStatus.Finish:
                    {
                        unit.Status = RecallStatus.Finished;
                        unit.Ended = Game.Time;
                        break;
                    }
            }
        }

        //        private static IEnumerable<AIBaseClient> GetCollision(float spellwidth, BaseUltSpell spell)
        //        {
        //            return (from unit in GameObjects.EnemyHeroes.Where(h => Player.Distance(h) < 2000)
        //                let pred =
        //                    Prediction.Position.PredictLinearMissile(unit, 2000, (int) spell.Radius, (int) spell.Delay,
        //                        spell.Speed, -1)
        //                let endpos = Player.Position.Extend(GetFountainPos(), 2000)
        //                let projectOn = pred.UnitPosition.To2D().ProjectOn(Player.Position.ToVector2(), endpos)
        //                where projectOn.SegmentPoint.Distance(endpos) < spellwidth + unit.BoundingRadius
        //                select unit).Cast<AIBaseClient>().ToList();
        //        }
    }
    public class Recall
    {
        public int TextPos;

        public Recall(AIHeroClient unit, RecallStatus status)
        {
            Unit = unit;
            Status = status;
        }

        public AIHeroClient Unit { get; set; }
        public RecallStatus Status { get; set; }
        public float Started { get; set; }
        public float Ended { get; set; }
        public float Duration { get; set; }
    }

    public class BaseUltUnit
    {
        public BaseUltUnit(AIHeroClient unit, float fireTime, bool collision)
        {
            Unit = unit;
            FireTime = fireTime;
            Collision = collision;
        }

        public AIHeroClient Unit { get; set; }
        public float FireTime { get; set; }
        public bool Collision { get; set; }
        public float LastSeen { get; set; }
    }

    public class BaseUltSpell
    {
        public BaseUltSpell(string name, SpellSlot slot, float delay, float speed, float radius, bool collision)
        {
            Name = name;
            Slot = slot;
            Delay = delay;
            Speed = speed;
            Radius = radius;
            Collision = collision;
        }

        public string Name { get; set; }
        public SpellSlot Slot { get; set; }
        public float Delay { get; set; }
        public float Speed { get; set; }
        public float Radius { get; set; }
        public bool Collision { get; set; }
    }

    public enum RecallStatus
    {
        Active,
        Inactive,
        Finished,
        Abort
    }
}
