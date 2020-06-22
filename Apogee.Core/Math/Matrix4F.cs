using System.IO;

namespace Apogee.Core.Math
{
    public class Matrix4F
    {
         // @formatter:off
     
        public static Vector4F Transform(Matrix4F left, Vector4F right, Vector4F dest)
        {
            if (dest == null)
                dest = new Vector4F(0, 0, 0, 0);

            float x = left[0, 0] * right.X + left[1, 0] * right.Y + left[2, 0] * right.Z + left[3, 0] * right.W;
            float y = left[0, 1] * right.X + left[1, 1] * right.Y + left[2, 1] * right.Z + left[3, 1] * right.W;
            float z = left[0, 2] * right.X + left[1, 2] * right.Y + left[2, 2] * right.Z + left[3, 2] * right.W;
            float w = left[0, 3] * right.X + left[1, 3] * right.Y + left[2, 3] * right.Z + left[3, 3] * right.W;

            dest.X = x;
            dest.Y = y;
            dest.Z = z;
            dest.W = w;

            return dest;
        }

        public OpenToolkit.Mathematics.Matrix4 ToTKMattrix()
        {
            return new OpenToolkit.Mathematics.Matrix4(
                    new OpenToolkit.Mathematics.Vector4(this[0, 0], this[0, 1], this[0, 2], this[0, 3]),
                    new OpenToolkit.Mathematics.Vector4(this[1, 0], this[1, 1], this[1, 2], this[1, 3]),
                    new OpenToolkit.Mathematics.Vector4(this[2, 0], this[2, 1], this[2, 2], this[2, 3]),
                    new OpenToolkit.Mathematics.Vector4(this[3, 0], this[3, 1], this[3, 2], this[3, 3])
                );
        }

        public float[][] m;

        public Matrix4F()
        {
            m = new float[4][];
            for (int i = 0; i < m.Length; i++)
            {
                m[i] = new float[4];
            }
        }
        
        
        public void Write(BinaryWriter writer)
        {
            for (int i = 0; i < m.Length; i++)
            {
               writer.Write((double)m[i][0]);
               writer.Write((double)m[i][1]);
               writer.Write((double)m[i][2]);
               writer.Write((double)m[i][3]);
            }
        }
        
        public void Read(BinaryReader reader)
        {
            for (int i = 0; i < m.Length; i++)
            {
                m[i][0] =  (float)reader.ReadDouble();
                m[i][1] =  (float) reader.ReadDouble();
                m[i][2] =  (float)reader.ReadDouble();
                m[i][3] =  (float)reader.ReadDouble();
            }
        }
        
        
        public Matrix4F(OpenToolkit.Mathematics.Matrix4 x)
        {
            m = new float[4][];
            for (int i = 0; i < m.Length; i++)
            {
                m[i] = new float[4];
            }
            this.InitIdentity();
            
        
            m[0][0] = x.Row0[0];
            m[1][0] = x.Row1[0];
            m[2][0] = x.Row2[0];
            m[3][0] = x.Row3[0];

            m[0][1] = x.Row0[1];
            m[1][1] = x.Row1[1];
            m[2][1] = x.Row2[1];
            m[3][1] = x.Row3[1];

            m[0][2] = x.Row0[2];
            m[1][2] = x.Row1[2];
            m[2][2] = x.Row2[2];
            m[3][2] = x.Row3[2];

            m[0][3] = x.Row0[3];
            m[1][3] = x.Row1[3];
            m[2][3] = x.Row2[3];
            m[3][3] = x.Row3[3];
        }

        public Matrix4F InitIdentity()
        {
            m[0][0] = 1; m[0][1] = 0; m[0][2] = 0; m[0][3] = 0;
            m[1][0] = 0; m[1][1] = 1; m[1][2] = 0; m[1][3] = 0;
            m[2][0] = 0; m[2][1] = 0; m[2][2] = 1; m[2][3] = 0;
            m[3][0] = 0; m[3][1] = 0; m[3][2] = 0; m[3][3] = 1;

            return this;
        }
        
        public Matrix4F InitCamera(Vector3F forward, Vector3F up)
        {
            Vector3F f = forward;
            f.Normalize();

            Vector3F r = up;
            r.Normalize();
            r = r.Cross(f);

            Vector3F u = f.Cross(r);

            m[0][0] = r.X; m[0][1] = r.Y; m[0][2] = r.Z; m[0][3] = 0;
            m[1][0] = u.X; m[1][1] = u.Y; m[1][2] = u.Z; m[1][3] = 0;
            m[2][0] = f.X; m[2][1] = f.Y; m[2][2] = f.Z; m[2][3] = 0;
            m[3][0] = 0; m[3][1] = 0; m[3][2] = 0; m[3][3] = 1;

            return this;
        }

        public Matrix4F InitProjection(float aFov, float aWidth, float aHeight, float Znear, float ZFar)
        {
            float ar = aWidth / aHeight;
            float tanHalfFOV = (float)System.Math.Tan(OpenToolkit.Mathematics.MathHelper.DegreesToRadians(aFov / 2));
            float zRange = Znear - ZFar;

            m[0][0] = 1.0f / (tanHalfFOV * ar); m[0][1] = 0; m[0][2] = 0; m[0][3] = 0;
            m[1][0] = 0; m[1][1] = 1.0f / tanHalfFOV; m[1][2] = 0; m[1][3] = 0;
            m[2][0] = 0; m[2][1] = 0; m[2][2] = (-Znear - ZFar) / zRange; m[2][3] = 2 * ZFar * Znear / zRange;
            m[3][0] = 0; m[3][1] = 0; m[3][2] = 1; m[3][3] = 0;

            return this;
        }

        public Matrix4F InitOrthographic(float left, float right, float bottom, float top, float near, float far)
        {
            float width = right - left;
            float height = top - bottom;
            float depth = far - near;

            m[0][0] = 2 / width; m[0][1] = 0; m[0][2] = 0; m[0][3] = -(right + left) / width;
            m[1][0] = 0; m[1][1] = 2 / height; m[1][2] = 0; m[1][3] = -(top + bottom) / height;
            m[2][0] = 0; m[2][1] = 0; m[2][2] = -2 / depth; m[2][3] = -(far + near) / depth;
            m[3][0] = 0; m[3][1] = 0; m[3][2] = 0; m[3][3] = 1;

            return this;
        }

        public Matrix4F InitTranslation(float x, float y, float z)
        {
            m[0][0] = 1; m[0][1] = 0; m[0][2] = 0; m[0][3] = x;
            m[1][0] = 0; m[1][1] = 1; m[1][2] = 0; m[1][3] = y;
            m[2][0] = 0; m[2][1] = 0; m[2][2] = 1; m[2][3] = z;
            m[3][0] = 0; m[3][1] = 0; m[3][2] = 0; m[3][3] = 1;

            return this;
        }

        public Matrix4F InitRotation(float x, float y, float z)
        {
            Matrix4F rx = new Matrix4F();
            Matrix4F ry = new Matrix4F();
            Matrix4F rz = new Matrix4F();

            x = (float)OpenToolkit.Mathematics.MathHelper.DegreesToRadians(x);
            y = (float)OpenToolkit.Mathematics.MathHelper.DegreesToRadians(y);
            z = (float)OpenToolkit.Mathematics.MathHelper.DegreesToRadians(z);

            rz.m[0][0] = (float)System.Math.Cos(z); rz.m[0][1] = -((float)System.Math.Sin(z)); rz.m[0][2] = 0; rz.m[0][3] = 0;
            rz.m[1][0] = (float)System.Math.Sin(z); rz.m[1][1] = (float)System.Math.Cos(z); rz.m[1][2] = 0; rz.m[1][3] = 0;
            rz.m[2][0] = 0; rz.m[2][1] = 0; rz.m[2][2] = 1; rz.m[2][3] = 0;
            rz.m[3][0] = 0; rz.m[3][1] = 0; rz.m[3][2] = 0; rz.m[3][3] = 1;

            rx.m[0][0] = 1; rx.m[0][1] = 0; rx.m[0][2] = 0; rx.m[0][3] = 0;
            rx.m[1][0] = 0; rx.m[1][1] = (float)System.Math.Cos(x); rx.m[1][2] = -((float)System.Math.Sin(x)); rx.m[1][3] = 0;
            rx.m[2][0] = 0; rx.m[2][1] = (float)System.Math.Sin(x); rx.m[2][2] = (float)System.Math.Cos(x); rx.m[2][3] = 0;
            rx.m[3][0] = 0; rx.m[3][1] = 0; rx.m[3][2] = 0; rx.m[3][3] = 1;

            ry.m[0][0] = (float)System.Math.Cos(y); ry.m[0][1] = 0; ry.m[0][2] = -((float)System.Math.Sin(y)); ry.m[0][3] = 0;
            ry.m[1][0] = 0; ry.m[1][1] = 1; ry.m[1][2] = 0; ry.m[1][3] = 0;
            ry.m[2][0] = (float)System.Math.Sin(y); ry.m[2][1] = 0; ry.m[2][2] = (float)System.Math.Cos(y); ry.m[2][3] = 0;
            ry.m[3][0] = 0; ry.m[3][1] = 0; ry.m[3][2] = 0; ry.m[3][3] = 1;

            m = rz.Mul(ry.Mul(rx)).m;
            return this;
        }

        public Matrix4F InitScale(float x, float y, float z)
        {
            m[0][0] = x; m[0][1] = 0; m[0][2] = 0; m[0][3] = 0;
            m[1][0] = 0; m[1][1] = y; m[1][2] = 0; m[1][3] = 0;
            m[2][0] = 0; m[2][1] = 0; m[2][2] = z; m[2][3] = 0;
            m[3][0] = 0; m[3][1] = 0; m[3][2] = 0; m[3][3] = 1;

            return this;
        }
        
        // @formatter:on

        public static Matrix4F Clone(Matrix4F matrix)
        {
            // allocates/creates a duplicate of a matrix.
            var result = new Matrix4F();
            for (int i = 0; i < 4; ++i) // copy the values
            for (int j = 0; j < 4; ++j)
                result.m[i][j] = matrix.m[i][j];
            return result;
        }

        public Matrix4F Clone()
        {
            // allocates/creates a duplicate of a matrix.
            var result = new Matrix4F();
            for (int i = 0; i < 4; ++i) // copy the values
            for (int j = 0; j < 4; ++j)
                result.m[i][j] = m[i][j];
            return result;
        }

        public Matrix4F Mul(Matrix4F r)
        {
            Matrix4F re = new Matrix4F();

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    re[i, j] =
                        m[i][0] * r[0, j] +
                        m[i][1] * r[1, j] +
                        m[i][2] * r[2, j] +
                        m[i][3] * r[3, j];
                }
            }

            return re;
        }

        public float this[int x, int y]
        {
            get { return m[x][y]; }
            set { m[x][y] = value; }
        }

        public static Matrix4F operator *(Matrix4F c1, Matrix4F c2)
        {
            return c1.Mul(c2);
        }

        public static Vector4F operator *(Vector4F c1, Matrix4F c2)
        {
            var vec = new OpenToolkit.Mathematics.Vector4(c1.X, c1.Y, c1.Z, c1.W) * c2.ToTKMattrix();

            return new Vector4F(vec.X, vec.Y, vec.Z, vec.W);
        }

        public static Vector3F operator *(Vector3F c1, Matrix4F c2)
        {
            var vec = new OpenToolkit.Mathematics.Vector4(c1.X, c1.Y, c1.Z, 1) * c2.ToTKMattrix();

            return new Vector3F(vec.X, vec.Y, vec.Z);
        }


        public Matrix4F Invert()
        {
            var tk = ToTKMattrix();
            var x = tk.Inverted();

            m[0][0] = x.Row0[0];
            m[1][0] = x.Row1[0];
            m[2][0] = x.Row2[0];
            m[3][0] = x.Row3[0];

            m[0][1] = x.Row0[1];
            m[1][1] = x.Row1[1];
            m[2][1] = x.Row2[1];
            m[3][1] = x.Row3[1];

            m[0][2] = x.Row0[2];
            m[1][2] = x.Row1[2];
            m[2][2] = x.Row2[2];
            m[3][2] = x.Row3[2];

            m[0][3] = x.Row0[3];
            m[1][3] = x.Row1[3];
            m[2][3] = x.Row2[3];
            m[3][3] = x.Row3[3];

            return this;
        }
    }
}