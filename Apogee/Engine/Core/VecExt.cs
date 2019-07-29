using System.Numerics;

namespace Apogee.Engine.Core
{
    public static class VecExt
    {
        public static Vector3 Normalized(this Vector3 vec)
        {
            var v = vec.Clone();
            v.Normalize();
            return v;
        }

        public static Vector3 Clone(this Vector3 vec)
        {
            return new Vector3(vec.X, vec.Y, vec.Z);
        }

        public static float Normalize(this Vector3 vec)
        {
            var l = vec.Length();

            vec.X /= l;
            vec.Y /= l;
            vec.Z /= l;

            return l;
        }

        public static float Dot(this Vector3 v1, Vector3 v2)
        {
            return v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z;
        }


        public static Vector3 Cross(this Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.Y * v2.Z - v1.Z * v2.Y,
                v1.Z * v2.X - v1.X * v2.Z,
                v1.X * v2.Y - v1.Y * v2.X);
        }

     
    }
}