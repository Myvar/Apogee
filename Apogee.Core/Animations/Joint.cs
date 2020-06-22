using System.Collections.Generic;
using System.IO;
using Apogee.Core.Math;

namespace Apogee.Core.Animations
{
    public class Joint
    {
        public int ID { get; set; }
        public string Name { get; set; }

        public List<Joint> Children { get; set; } = new List<Joint>();

        public Matrix4F AnimatedTransform { get; set; } = new Matrix4F().InitIdentity();
        public Matrix4F LocalBindTransform { get; set; } = new Matrix4F().InitIdentity();
        public Matrix4F InverseBindTransform { get; set; } = new Matrix4F().InitIdentity();


        public void Read(BinaryReader reader)
        {
            ID = reader.ReadInt32();
            Name = reader.ReadString();

            AnimatedTransform.Read(reader);
            LocalBindTransform.Read(reader);
            InverseBindTransform.Read(reader);

            var ci = reader.ReadInt32();
            for (int i = 0; i < ci; i++)
            {
                var c = new Joint();
                c.Read(reader);
                Children.Add(c);
            }
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(ID);
            writer.Write(Name);

            AnimatedTransform.Write(writer);
            LocalBindTransform.Write(writer);
            InverseBindTransform.Write(writer);

            writer.Write(Children.Count);
            foreach (var child in Children)
            {
                child.Write(writer);
            }
        }

        public void CalcInverseBindTransform(Matrix4F parent)
        {
            var bindTransform = parent.Clone() * LocalBindTransform.Clone();
            InverseBindTransform = bindTransform.Clone().Invert();

            foreach (var child in Children)
            {
                child.CalcInverseBindTransform(bindTransform);
            }
        }
    }
}