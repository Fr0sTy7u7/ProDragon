using EnsoulSharp;
using EnsoulSharp.SDK;
using System.ComponentModel.Composition;
using System;

namespace D_Jarvan.Damage
{
    [Export(typeof(IDamageItem))]
    [ExportMetadata("Item", DamageItems.BotRK)]
    public class DamageBotRK : DamageItem
    {
        public DamageBotRK()
        {
            base.ItemId = 3153;
            base.DamageType = DamageType.Physical;
        }
        public override double GetDamage(AIHeroClient source, AIBaseClient target)
        {
            return 100.0;
        }
        public override double GetPassiveDamage(AIHeroClient source, AIBaseClient target)
        {
            double num = Math.Max(15.0, 0.08 * (double)target.Health);
            if (target is AIMinionClient)
            {
                num = Math.Min(60.0, num);
            }
            return num;
        }
    }
}
