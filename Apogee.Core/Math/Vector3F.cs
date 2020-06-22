using OpenToolkit.Mathematics;

namespace Apogee.Core.Math
{
    public class Vector3F
    {
         // apply all this to vec2 and vec4
        //  Vector comarison is scrwed @Bug
        public static readonly Vector3F Zero = new Vector3F(0f);

        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public float this[int x]
        {
            get
            {
                switch (x)
                {
                    case 0:
                        return X;
                    case 1:
                        return Y;
                    case 2:
                        return Z;
                }

                return -1;
            }
            set
            {
                switch (x)
                {
                    case 0:
                        X = value;
                        break;
                    case 1:
                        Y = value;
                        break;
                    case 2:
                        Z = value;
                        break;
                }
            }
        }


        public bool Eq(Vector3F vec)
        {
            return (int) (float)System.Math.Round(X) == (int) (float)System.Math.Round(vec.X) &&
                   (int) (float)System.Math.Round(Y) == (int) (float)System.Math.Round(vec.Y) &&
                   (int) (float)System.Math.Round(Z) == (int) (float)System.Math.Round(vec.Z);
        }

        public Vector3F()
        {
        }

        protected bool Equals(Vector3F other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Vector3F) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = X.GetHashCode();
                hashCode = (hashCode * 397) ^ Y.GetHashCode();
                hashCode = (hashCode * 397) ^ Z.GetHashCode();
                return hashCode;
            }
        }

        public Vector3F(float a)
        {
            X = a;
            Y = a;
            Z = a;
        }

        public Vector3F(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Vector3F Rotate(float angle, Vector3F axis)
        {
            float sinHalfAngle = (float) System.Math.Sin(MathHelper.DegreesToRadians(angle / 2));
            float cosHalfAngle = (float) System.Math.Cos(MathHelper.DegreesToRadians(angle / 2));

            float rx = axis.X * sinHalfAngle;
            float ry = axis.Y * sinHalfAngle;
            float rz = axis.Z * sinHalfAngle;
            float rw = cosHalfAngle;

            var rotation = new Quaternion(rx, ry, rz, rw);
            var conjugate = rotation.Conjugate();

            var w = rotation.Mul(this).Mul(conjugate);

            X = w.X;
            Y = w.Y;
            Z = w.Z;

            return this;
        }

        public float DistanceTo(Vector3F b)
        {
            Vector3F vector = new Vector3F(X - b.X, Y - b.Y, Z - b.Z);
            return (float) System.Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z);
        }

        public Vector3F Abs()
        {
            return new Vector3F(System.Math.Abs(X), System.Math.Abs(Y), System.Math.Abs(Z));
        }

        private float sqr(float n)
        {
            return n * n;
        }

        public float Length2()
        {
            return sqr(X) + sqr(Y) + sqr(Z);
        }

        public float Length()
        {
            return (float)System.Math.Sqrt(Length2());
        }

        public float Normalize()
        {
            var l = Length();

            X /= l;
            Y /= l;
            Z /= l;

            return l;
        }

        public void Reflect(Vector3F normal)
        {
            // I is the original array
            // N is the normal of the incident plane
            // R = I - (2 * N * ( DotProduct[ I,N] ))
            // inline the dotProduct here instead of calling method
            float dotProduct = ((X * normal.X) + (Y * normal.Y)) + (Z * normal.Z);
            X = X - (2.0f * normal.X) * dotProduct;
            Y = Y - (2.0f * normal.Y) * dotProduct;
            Z = Z - (2.0f * normal.Z) * dotProduct;
        }

        private float Lerp(float firstFloat, float secondFloat, float by)
        {
            return firstFloat * (1 - by) + secondFloat * by;
        }

        public static float LerpAngle(float a, float b, float t)
        {
            float num = Repeat(b - a, 360f);
            if ((double) num > 180.0)
                num -= 360f;
            return a + num * Clamp01(t);
        }

        public static float Clamp01(float value)
        {
            if ((double) value < 0.0)
                return 0.0f;
            if ((double) value > 1.0)
                return 1f;
            return value;
        }

        public static float Repeat(float t, float length)
        {
            return t - (float)System.Math.Floor(t / length) * length;
        }

        public Vector3F Lerp(Vector3F secondVector, float by)
        {
            X = Lerp(X, secondVector.X, by);
            Y = Lerp(Y, secondVector.Y, by);
            Z = Lerp(Z, secondVector.Z, by);
            return this;
        }

        public Vector3F LerpAngle(Vector3F secondVector, float by)
        {
            X = LerpAngle(X, secondVector.X, by);
            Y = LerpAngle(Y, secondVector.Y, by);
            Z = LerpAngle(Z, secondVector.Z, by);
            return this;
        }

        public Vector3F Clone()
        {
            return new Vector3F(X, Y, Z);
        }

        public Vector3F Normalized()
        {
            var v = Clone();
            v.Normalize();
            return v;
        }

        public double Dot(Vector3F v2)
        {
            return X * v2.X + Y * v2.Y + Z * v2.Z;
        }

        public Vector3F Cross(Vector3F v2)
        {
            return new Vector3F(Y * v2.Z - Z * v2.Y,
                Z * v2.X - X * v2.Z,
                X * v2.Y - Y * v2.X);
        }


        public static Vector3F operator +(Vector3F c1, Vector3F c2)
        {
            return new Vector3F(c1.X + c2.X, c1.Y + c2.Y, c1.Z + c2.Z);
        }

        public static Vector3F operator +(Vector3F c1, float c2)
        {
            return new Vector3F(c1.X + c2, c1.Y + c2, c1.Z + c2);
        }

        public static Vector3F operator -(Vector3F c1, Vector3F c2)
        {
            return new Vector3F(c1.X - c2.X, c1.Y - c2.Y, c1.Z - c2.Z);
        }

        public static Vector3F operator -(Vector3F c1, float c2)
        {
            return new Vector3F(c1.X - c2, c1.Y - c2, c1.Z - c2);
        }

        public static Vector3F operator /(Vector3F c1, Vector3F c2)
        {
            return new Vector3F(c1.X / c2.X, c1.Y / c2.Y, c1.Z / c2.Z);
        }

        public static Vector3F operator /(Vector3F c1, float c2)
        {
            return new Vector3F(c1.X / c2, c1.Y / c2, c1.Z / c2);
        }

        public static Vector3F operator *(Vector3F c1, Vector3F c2)
        {
            return new Vector3F(c1.X * c2.X, c1.Y * c2.Y, c1.Z * c2.Z);
        }

        public static Vector3F operator *(Vector3F c1, float c2)
        {
            return new Vector3F(c1.X * c2, c1.Y * c2, c1.Z * c2);
        }

        public static Vector3F operator *(float c2, Vector3F c1)
        {
            return new Vector3F(c1.X * c2, c1.Y * c2, c1.Z * c2);
        }

        public static Vector3F operator -(Vector3F c1)
        {
            return new Vector3F(-c1.X, -c1.Y, -c1.Z);
        }

        public static explicit operator Vector3(Vector3F b) => new Vector3(b.X, b.Y, b.Z);
        public static explicit operator Vector3F(Vector3 b) => new Vector3F(b.X, b.Y, b.Z);

        public void Round()
        {
            X = (float)System.Math.Round(X);
            Y = (float)System.Math.Round(Y);
            Z = (float)System.Math.Round(Z);
        }
    }
}