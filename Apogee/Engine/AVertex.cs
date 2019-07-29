using System.Numerics;
using Veldrid;

namespace Apogee.Engine
{
    public struct AVertex
    {
        public const uint SizeInBytes = 60;
        public Vector3 Position { get; set; }
        public Vector2 TexCoord { get; set; }
        public Vector4 Color { get; set; }
        public Vector3 Normal { get; set; }
        public Vector3 Tangent { get; set; }

        public AVertex(Vector3 position, Vector2 texCoord, Vector4 color, Vector3 normal, Vector3 tangent)
        {
            Position = position;
            TexCoord = texCoord;
            Color = color;
            Normal = normal;
            Tangent = tangent;
        }

        public static VertexLayoutDescription GetLayout()
        {
            return new VertexLayoutDescription(
                new VertexElementDescription("Position", VertexElementSemantic.Position,
                    VertexElementFormat.Float3),
                new VertexElementDescription("TexCoord", VertexElementSemantic.TextureCoordinate,
                    VertexElementFormat.Float2),
                new VertexElementDescription("Color", VertexElementSemantic.TextureCoordinate,
                    VertexElementFormat.Float4),
                new VertexElementDescription("Normal", VertexElementSemantic.Position,
                    VertexElementFormat.Float3),
                new VertexElementDescription("Tangent", VertexElementSemantic.Position,
                    VertexElementFormat.Float3));
        }
    }
}