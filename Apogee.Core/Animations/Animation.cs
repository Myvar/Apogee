using System.Collections.Generic;
using System.IO;

namespace Apogee.Core.Animations
{
    public class Animation
    {
        public float Length { get; set; } //seconds
        public string Name { get; set; }
        public List<KeyFrame> KeyFrames { get; set; } = new List<KeyFrame>();

        public void Write(BinaryWriter writer)
        {
            writer.Write((double) Length);
            writer.Write(KeyFrames.Count);

            foreach (var keyFrame in KeyFrames)
            {
                keyFrame.Write(writer);
            }
        }

        public void Read(BinaryReader reader)
        {
            Length = (float) reader.ReadDouble();

            var fi = reader.ReadInt32();

            for (int i = 0; i < fi; i++)
            {
                var f = new KeyFrame();
                f.Read(reader);
                KeyFrames.Add(f);
            }
        }
    }
}