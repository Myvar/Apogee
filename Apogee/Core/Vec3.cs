using System;
using Apogee.Components;

namespace Apogee.Core
{
    public class Vec3
    {
        public static Vec3 Zero = new Vec3(0f);

        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }


        public Vec3()
        {
            X = 0.0f;
            Y = 1.0f;
            Z = 0.0f;
        }

        public Vec3(float a)
        {
            X = a;
            Y = a;
            Z = a;
        }

        public Vec3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        
        public Vec3 Rotate(float angle, Vec3 axis)
        {
            float sinHalfAngle = (float)Math.Sin(OpenTK.MathHelper.DegreesToRadians(angle / 2));
            float cosHalfAngle = (float)Math.Cos(OpenTK.MathHelper.DegreesToRadians(angle / 2));

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

        public float DistanceTo(Vec3 b)
        {
            Vec3 vector = new Vec3(X - b.X, Y - b.Y, Z - b.Z);
            return (float) Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z);
        }

        public Vec3 Abs()
        {
            return new Vec3(Math.Abs(X), Math.Abs(Y), Math.Abs(Z));
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
            return MathF.Sqrt(Length2());
        }

        public float Normalize()
        {
            var l = Length();

            X /= l;
            Y /= l;
            Z /= l;

            return l;
        }

        public Vec3 Clone()
        {
            return new Vec3(X, Y, Z);
        }

        public Vec3 Normalized()
        {
            var v = Clone();
            v.Normalize();
            return v;
        }

        public double Dot(Vec3 v2)
        {
            return X * v2.X + Y * v2.Y + Z * v2.Z;
        }

        public Vec3 Cross(Vec3 v2)
        {
            return new Vec3(Y * v2.Z - Z * v2.Y,
                Z * v2.X - X * v2.Z,
                X * v2.Y - Y * v2.X);
        }


        public static Vec3 operator +(Vec3 c1, Vec3 c2)
        {
            return new Vec3(c1.X + c2.X, c1.Y + c2.Y, c1.Z + c2.Z);
        }

        public static Vec3 operator +(Vec3 c1, float c2)
        {
            return new Vec3(c1.X + c2, c1.Y + c2, c1.Z + c2);
        }

        public static Vec3 operator -(Vec3 c1, Vec3 c2)
        {
            return new Vec3(c1.X - c2.X, c1.Y - c2.Y, c1.Z - c2.Z);
        }

        public static Vec3 operator -(Vec3 c1, float c2)
        {
            return new Vec3(c1.X - c2, c1.Y - c2, c1.Z - c2);
        }

        public static Vec3 operator /(Vec3 c1, Vec3 c2)
        {
            return new Vec3(c1.X / c2.X, c1.Y / c2.Y, c1.Z / c2.Z);
        }

        public static Vec3 operator /(Vec3 c1, float c2)
        {
            return new Vec3(c1.X / c2, c1.Y / c2, c1.Z / c2);
        }

        public static Vec3 operator *(Vec3 c1, Vec3 c2)
        {
            return new Vec3(c1.X * c2.X, c1.Y * c2.Y, c1.Z * c2.Z);
        }

        public static Vec3 operator *(Vec3 c1, float c2)
        {
            return new Vec3(c1.X * c2, c1.Y * c2, c1.Z * c2);
        }

        public static Vec3 operator *(float c2, Vec3 c1)
        {
            return new Vec3(c1.X * c2, c1.Y * c2, c1.Z * c2);
        }

        public static Vec3 operator -(Vec3 c1)
        {
            return new Vec3(-c1.X, -c1.Y, -c1.Z);
        }
    }
}