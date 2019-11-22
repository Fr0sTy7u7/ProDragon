using EnsoulSharp;
using EnsoulSharp.SDK;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace D_Jarvan.Damage
{
    [Export(typeof(IDamageItem))]
    [ExportMetadata("Item", DamageItems.BilgewaterCutlass)]
    public class DamageBilgewaterCutlass : DamageItem
    {
        public DamageBilgewaterCutlass()
        {
            base.ItemId = 3144;
            base.DamageType = DamageType.Magical;
        }
        public override double GetDamage(AIHeroClient source, AIBaseClient target)
        {
            return 100.0;
        }
    }
}
