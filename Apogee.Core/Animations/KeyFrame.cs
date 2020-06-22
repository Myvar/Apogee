using System.Collections.Generic;
using System.IO;

namespace Apogee.Core.Animations
{
    public class KeyFrame
    {
        public float TimeStamp { get; set; }
        public Dictionary<string, JointTransform> Pose { get; set; } = new Dictionary<string, JointTransform>();


        public void Write(BinaryWriter writer)
        {
            writer.Write((double) TimeStamp);
            writer.Write(Pose.Count);

            foreach ((string name, JointTransform transform) in Pose)
            {
                writer.Write(name);
                transform.Write(writer);
            }
        }

        public void Read(BinaryReader reader)
        {
            TimeStamp = (float) reader.ReadDouble();

            var pi = reader.ReadInt32();

            for (int i = 0; i < pi; i++)
            {
                var name = reader.ReadString();
                var jt = new JointTransform(reader);
                Pose.Add(name, jt);
            }
        }
    }
}