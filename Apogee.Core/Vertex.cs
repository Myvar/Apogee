using System;
using System.Collections.Generic;
using Apogee.Core.Math;

namespace Apogee.Core
{
     public class Vertex
    {
        public static int Size = 20;

        public Vertex(
            Vector3F position,
            Vector2F texCoord,
            Vector3F color,
            Vector3F normal,
            Vector3F tangent,
            Vector3I jointIDS,
            Vector3F jointWeights
        )
        {
            Position = position;
            TexCoord = texCoord;
            Color = color;
            Normal = normal;
            Tangent = tangent;
            JointIDS = jointIDS;
            JointWeights = jointWeights;
        }

        public Vertex(Vector3F position) : this(
            position,
            new Vector2F(0, 0),
            new Vector3F(0.5f, 0.5f, 0.5f),
            new Vector3F(0, 0, 0),
            new Vector3F(0, 0, 0),
            new Vector3I(0),
            new Vector3F(0))
        {
        }

        public Vertex(Vector3F position, Vector2F texCoord) : this(
            position,
            texCoord,
            new Vector3F(0.5f, 0.5f, 0.5f),
            new Vector3F(0, 0, 0),
            new Vector3F(0, 0, 0),
            new Vector3I(0),
            new Vector3F(0))
        {
        }

        public Vertex(Vector3F position, Vector2F texCoord, Vector3F color) : this(
            position,
            texCoord,
            color,
            new Vector3F(0, 0, 0),
            new Vector3F(0, 0, 0),
            new Vector3I(0),
            new Vector3F(0))
        {
        }

        public Vector3F Position { get; set; }
        public Vector2F TexCoord { get; set; }
        public Vector3F Color { get; set; }
        public Vector3F Normal { get; set; }
        public Vector3F Tangent { get; set; }

        public Vector3I JointIDS { get; set; }
        public Vector3F JointWeights { get; set; }

        public byte[] ToArray()
        {
            var re = new List<byte>();

            re.AddRange(BitConverter.GetBytes(Position.X));
            re.AddRange(BitConverter.GetBytes(Position.Y));
            re.AddRange(BitConverter.GetBytes(Position.Z));


            re.AddRange(BitConverter.GetBytes(Color.X));
            re.AddRange(BitConverter.GetBytes(Color.Y));
            re.AddRange(BitConverter.GetBytes(Color.Z));


            re.AddRange(BitConverter.GetBytes(Normal.X));
            re.AddRange(BitConverter.GetBytes(Normal.Y));
            re.AddRange(BitConverter.GetBytes(Normal.Z));

            re.AddRange(BitConverter.GetBytes(Tangent.X));
            re.AddRange(BitConverter.GetBytes(Tangent.Y));
            re.AddRange(BitConverter.GetBytes(Tangent.Z));

            re.AddRange(BitConverter.GetBytes(TexCoord.X));
            re.AddRange(BitConverter.GetBytes(TexCoord.Y));
            
            re.AddRange(BitConverter.GetBytes(JointIDS.X));
            re.AddRange(BitConverter.GetBytes(JointIDS.Y));
            re.AddRange(BitConverter.GetBytes(JointIDS.Z));
            
            re.AddRange(BitConverter.GetBytes(JointWeights.X));
            re.AddRange(BitConverter.GetBytes(JointWeights.Y));
            re.AddRange(BitConverter.GetBytes(JointWeights.Z));

            return re.ToArray();
        }
    }

    public static class VertexExt
    {
        public static byte[] ToByteArray(this Vertex[] data)
        {
            var vd = new List<byte>();

            foreach (var d in data)
            {
                vd.AddRange(d.ToArray());
            }

            return vd.ToArray();
        }
    }
}