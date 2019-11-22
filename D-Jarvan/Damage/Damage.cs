using System;
using System.Collections.Generic;
using System.Linq;
using EnsoulSharp;

namespace D_Jarvan.Damage
{
    public class Damage
    {
        public double GetItemDamage(AIHeroClient source, AIBaseClient target, DamageItems item, ItemDamageType type = ItemDamageType.Default)
        {
            double num = 0.0;
            IDamageItem itemDamage = this.GetItemDamage(item);
            if (itemDamage != null)
            {
                if (type.HasFlag(ItemDamageType.Default))
                {
                    num += itemDamage.GetDamage(source, target);
                }
                if (type.HasFlag(ItemDamageType.Dot))
                {
                    num += itemDamage.GetDotDamage(source, target);
                }
                if (type.HasFlag(ItemDamageType.Passive))
                {
                    num += itemDamage.GetPassiveDamage(source, target);
                }
            }
            return num;
        }
        public IEnumerable<Lazy<IDamageItem, IDamageItemMetadata>> ItemLazies { get; protected set; }
        public IDamageItem GetItemDamage(DamageItems item)
        {
            Lazy<IDamageItem, IDamageItemMetadata> lazy = this.ItemLazies.FirstOrDefault((Lazy<IDamageItem, IDamageItemMetadata> i) => i.Metadata.Item == item);
            if (lazy == null)
            {
                return null;
            }
            return lazy.Value;
        }
    }
}
