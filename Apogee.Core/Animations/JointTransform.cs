using System.IO;
using Apogee.Core.Math;

namespace Apogee.Core.Animations
{
    public class JointTransform
    {
        public JointTransform(Vector3F position, Quaternion rotation)
        {
            Position = position;
            Rotation = rotation;
        }

        public JointTransform(BinaryReader reader)
        {
            Position = new Vector3F(
                (float) reader.ReadDouble(),
                (float) reader.ReadDouble(),
                (float) reader.ReadDouble());
            Rotation = new Quaternion(
                (float) reader.ReadDouble(),
                (float) reader.ReadDouble(),
                (float) reader.ReadDouble(),
                (float) reader.ReadDouble());
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write((double) Position.X);
            writer.Write((double) Position.Y);
            writer.Write((double) Position.Z);

            writer.Write((double) Rotation.X);
            writer.Write((double) Rotation.Y);
            writer.Write((double) Rotation.Z);
            writer.Write((double) Rotation.W);
        }

        public Vector3F Position { get; set; }
        public Quaternion Rotation { get; set; }

        public Matrix4F GetLocalTransform()
        {
            var matrix =
                new Matrix4F().InitTranslation(Position.X, Position.Y, Position.Z) * Rotation.ToRotationMatrix();
            return matrix;
        }

        public static JointTransform Interpolate(JointTransform frameA, JointTransform frameB, float progression)
        {
            var pos = Interpolate(frameA.Position, frameB.Position, progression);
            var rot = Quaternion.Interpolate(frameA.Rotation, frameB.Rotation, progression);
            return new JointTransform(pos, rot);
        }

        private static Vector3F Interpolate(Vector3F start, Vector3F end, float progression)
        {
            var x = start.X + (end.X - start.X) * progression;
            var y = start.Y + (end.Y - start.Y) * progression;
            var z = start.Z + (end.Z - start.Z) * progression;
            return new Vector3F(x, y, z);
        }
    }
}