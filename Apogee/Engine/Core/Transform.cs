using System.Numerics;

namespace Apogee.Engine.Core
{
    public struct Transform
    {
        public Transform(float z = 0)
        {
            Translation = new Vector3(0);
            Rotation = new Vector3(0);
            Scale = new Vector3(1);
        }

        public Vector3 Translation { get; set; }
        public Vector3 Rotation { get; set; }
        public Vector3 Scale { get; set; }

        public static explicit operator Matrix4x4(Transform b)
        {
            var trans = Matrix4x4.CreateTranslation(
                b.Translation.X,
                b.Translation.Y,
                b.Translation.Z);

            var rot = Matrix4x4.CreateRotationX(b.Rotation.X.ToRadians()) *
                      Matrix4x4.CreateRotationY(b.Rotation.Y.ToRadians()) *
                      Matrix4x4.CreateRotationZ(b.Rotation.Z.ToRadians());

            var scal = Matrix4x4.CreateScale(
                b.Scale.X,
                b.Scale.Y,
                b.Scale.Z);

            return trans * rot * scal;
        }


        public static Transform operator +(Transform c1, Transform c2)
        {
            return new Transform()
            {
                Translation = c1.Translation + c2.Translation,
                Rotation = c1.Rotation + c2.Rotation,
                Scale = c1.Scale + (c2.Scale - new Vector3(1))
            };
        }

        public Transform Clone()
        {
            return new Transform()
            {
                Translation = Translation.Clone(),
                Rotation = Rotation.Clone(),
                Scale = Scale.Clone()
            };
        }
    }
}