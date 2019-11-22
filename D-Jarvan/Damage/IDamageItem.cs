using EnsoulSharp;
using EnsoulSharp.SDK;

namespace D_Jarvan.Damage
{
    public interface IDamageItem
    {
        DamageType DamageType { get; }
        bool IsDot { get; }
        int ItemId { get; }
        double GetDamage(AIHeroClient source, AIBaseClient target);
        double GetDotDamage(AIHeroClient source, AIBaseClient target);
        double GetPassiveDamage(AIHeroClient source, AIBaseClient target);
    }
}
