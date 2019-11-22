using EnsoulSharp;

namespace D_Jarvan.Damage
{
    public static class DamageExtensions
    {
        public static double GetItemDamage(this AIHeroClient source, AIBaseClient target, DamageItems item)
        {
            Damage instance = DamageExtensions.Instance;
            if (instance == null)
            {
                return 0.0;
            }
            return instance.GetItemDamage(source, target, item, ItemDamageType.Default);
        }
        private static Damage Instance
        {
            get
            {
                return Instances.Damage;
            }
        }
    }
}
