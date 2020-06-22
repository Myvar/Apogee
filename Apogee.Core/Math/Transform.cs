namespace Apogee.Core.Math
{
    public class Transform
    {
        public Vector3F Translation { get; set; } = new Vector3F(0);
        public Vector3F Rotation { get; set; } = new Vector3F(0);
        public Vector3F Scale { get; set; } = new Vector3F(1);


        public Matrix4F GetTranformation()
        {
            var trans = new Matrix4F().InitTranslation(
                Translation.X,
                Translation.Y,
                Translation.Z);

            var rot = new Matrix4F().InitRotation(
                Rotation.X,
                Rotation.Y,
                Rotation.Z);

            var scal = new Matrix4F().InitScale(
                Scale.X,
                Scale.Y,
                Scale.Z);

            return trans * rot * scal;
        }


        public Matrix4F GetRotMat()
        {
            var rot = new Matrix4F().InitRotation(
                Rotation.X,
                Rotation.Y,
                Rotation.Z);

            return rot;
        }
        
        public Matrix4F GetTranslationMat()
        {
            var trans = new Matrix4F().InitTranslation(
                Translation.X,
                Translation.Y,
                Translation.Z);


            return trans;
        }

        public static Transform operator +(Transform c1, Transform c2)
        {
            return new Transform()
            {
                Translation = c1.Translation + c2.Translation,
                Rotation = c1.Rotation + c2.Rotation,
                Scale = c1.Scale + (c2.Scale - new Vector3F(1))
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