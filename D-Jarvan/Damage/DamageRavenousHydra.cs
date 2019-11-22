using EnsoulSharp;
using EnsoulSharp.SDK;
using D_Jarvan.Geometry;
using System.ComponentModel.Composition;

namespace D_Jarvan.Damage
{
    [Export(typeof(IDamageItem))]
    [ExportMetadata("Item", DamageItems.RavenousHydra)]
    public class DamageRavenousHydra : DamageItem
    {
        public DamageRavenousHydra()
		{
			base.ItemId = 3074;
			base.DamageType = DamageType.Physical;
		}
        public override double GetDamage(AIHeroClient source, AIBaseClient target)
        {
            return (target.InRange(100f, false) ? 1.0 : 0.6) * (double)source.TotalAttackDamage;
        }
        public override double GetPassiveDamage(AIHeroClient source, AIBaseClient target)
        {
            return (target.InRange(87.5f, false) ? 0.6 : 0.2) * (double)source.TotalAttackDamage;
        }
        private const float PassiveRange = 350f;
        private const float ActiveRange = 400f;
    }
}
