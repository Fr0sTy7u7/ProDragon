using EnsoulSharp;
using EnsoulSharp.SDK;

namespace D_Jarvan.Damage
{
    public class DamageItem : IDamageItem
    {
        public DamageType DamageType { get; protected set; }
        public bool IsDot { get; protected set; }
        public int ItemId { get; protected set; }
        public virtual double GetDamage(AIHeroClient source, AIBaseClient target)
        {
            return 0.0;
        }
        public virtual double GetDotDamage(AIHeroClient source, AIBaseClient target)
        {
            return 0.0;
        }
        public virtual double GetPassiveDamage(AIHeroClient source, AIBaseClient target)
        {
            return this.GetDamage(source, target);
        }
    }
}
