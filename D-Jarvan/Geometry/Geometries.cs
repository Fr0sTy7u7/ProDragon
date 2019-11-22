using EnsoulSharp;
using SharpDX;

namespace D_Jarvan.Geometry
{
    internal static class Geometries
    {
        public static Vector2 To2D(this Vector3 v)
        {
            return new Vector2(v.X, v.Y);
        }
        private static float GetRange(float range, bool squared)
        {
            if (!squared)
            {
                return range;
            }
            return range * range;
        }
        public static float Distance(this Vector2 v, Vector2 other, bool squared = false)
        {
            if (!squared)
            {
                return Vector2.Distance(v, other);
            }
            return Vector2.DistanceSquared(v, other);
        }
        public static float Distance<T, T1>(this T unit, T1 unit2, bool squared = false) where T : GameObject where T1 : GameObject
        {
            T t = unit;
            if (t == null)
            {
                return 0f;
            }
            Vector2 v = t.Position.To2D();
            T1 t2 = unit2;
            return v.Distance((t2 != null) ? t2.Position.To2D() : default, squared);
        }
        public static bool InRange<T, T1>(this T unit, T1 unit2, float range, bool squared = false) where T : GameObject where T1 : GameObject
        {
            return unit.Distance(unit2, squared) <= GetRange(range, squared);
        }
        public static bool InRange(this GameObject unit, float range, bool squared = false)
        {
            return ObjectManager.Player.InRange(unit, range, squared);
        }
    }
}
