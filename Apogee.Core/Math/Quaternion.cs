namespace Apogee.Core.Math
{
    public class Quaternion
    {
         public float X, Y, Z, W;

        public Quaternion(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public float Length()
        {
            return (float)System.Math.Sqrt(X * X + Y * Y + Z * Z + W * W);
        }

        public Quaternion Normalize()
        {
            var len = Length();

            X /= len;
            Y /= len;
            Z /= len;
            W /= len;

            return this;
        }

        public Quaternion Conjugate()
        {
            return new Quaternion(-X, -Y, -Z, W);
        }

        public Quaternion Mul(Quaternion r)
        {
            float w = W * r.W - X * r.X - Y * r.Y - Z * r.Z;
            float x = X * r.W + W * r.X + Y * r.Z - Z * r.Y;
            float y = Y * r.W + W * r.Y + Z * r.X - X * r.Z;
            float z = Z * r.W + W * r.Z + X * r.Y - Y * r.X;

            return new Quaternion(x, y, z, w);
        }

        public Quaternion Mul(Vector3F r)
        {
            float w = -X * r.X - Y * r.Y - Z * r.Z;
            float x = W * r.X + Y * r.Z - Z * r.Y;
            float y = W * r.Y + Z * r.X - X * r.Z;
            float z = W * r.Z + X * r.Y - Y * r.X;

            return new Quaternion(x, y, z, w);
        }

        public static Quaternion operator *(Quaternion c1, Quaternion c2)
        {
            return c1.Mul(c2);
        }

        /*public static Quaternion CreateFromRotationMatrix(Mat4 matrix)
        {
            var quaternion = new Quaternion(0, 0, 0, 0);
            float sqrt;
            float half;
            float scale = matrix[0, 0] + matrix[1, 1] + matrix[2, 2];

            if (scale > 0.0f)
            {
                sqrt = (float) Math.Sqrt(scale + 1.0f);
                quaternion.W = sqrt * 0.5f;
                sqrt = 0.5f / sqrt;

                quaternion.X = (matrix[1, 2] - matrix[2, 1]) * sqrt;
                quaternion.Y = (matrix[2, 0] - matrix[0, 2]) * sqrt;
                quaternion.Z = (matrix[0, 1] - matrix[1, 0]) * sqrt;

                return quaternion;
            }

            if ((matrix[0, 0] >= matrix[1, 1]) && (matrix[0, 0] >= matrix[2, 2]))
            {
                sqrt = (float) Math.Sqrt(1.0f + matrix[0, 0] - matrix[1, 1] - matrix[2, 2]);
                half = 0.5f / sqrt;

                quaternion.X = 0.5f * sqrt;
                quaternion.Y = (matrix[0, 1] + matrix[1, 0]) * half;
                quaternion.Z = (matrix[0, 2] + matrix[2, 0]) * half;
                quaternion.W = (matrix[1, 2] - matrix[2, 1]) * half;

                return quaternion;
            }

            if (matrix[1, 1] > matrix[2, 2])
            {
                sqrt = (float) Math.Sqrt(1.0f + matrix[1, 1] - matrix[0, 0] - matrix[2, 2]);
                half = 0.5f / sqrt;

                quaternion.X = (matrix[1, 0] + matrix[0, 1]) * half;
                quaternion.Y = 0.5f * sqrt;
                quaternion.Z = (matrix[2, 1] + matrix[1, 2]) * half;
                quaternion.W = (matrix[2, 0] - matrix[0, 2]) * half;

                return quaternion;
            }

            sqrt = (float) Math.Sqrt(1.0f + matrix[2, 2] - matrix[0, 0] - matrix[1, 1]);
            half = 0.5f / sqrt;

            quaternion.X = (matrix[2, 0] + matrix[0, 2]) * half;
            quaternion.Y = (matrix[2, 1] + matrix[1, 2]) * half;
            quaternion.Z = 0.5f * sqrt;
            quaternion.W = (matrix[0, 1] - matrix[1, 0]) * half;

            return quaternion;
        }*/
        public Matrix4F ToRotationMatrix()
        {
            var qat = new OpenToolkit.Mathematics.Quaternion(X, Y, Z, W);
            var mat = OpenToolkit.Mathematics.Matrix4.CreateFromQuaternion(qat);
            return new Matrix4F(mat);
        }

        public static Quaternion Interpolate(Quaternion frameARotation, Quaternion frameBRotation, float progression)
        {
            var a = new OpenToolkit.Mathematics.Quaternion(frameARotation.X, frameARotation.Y, frameARotation.Z, frameARotation.W);
            var b = new OpenToolkit.Mathematics.Quaternion(frameBRotation.X, frameBRotation.Y, frameBRotation.Z, frameBRotation.W);
            var slerp = OpenToolkit.Mathematics.Quaternion.Slerp(a, b, progression);
            return new Quaternion(slerp.X, slerp.Y, slerp.Z, slerp.W);
        }
    }
}