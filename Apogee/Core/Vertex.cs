namespace Apogee.Core
{
    public class Vertex
    {
        public static int Size = 14;

        public Vertex(Vec3 position, Vec2 texCoord, Vec3 color, Vec3 normal, Vec3 tangent)
        {
            this.Position = position;
            this.TexCoord = texCoord;
            this.Color = color;
            this.Normal = normal;
            this.Tangent = tangent;

        }

        public Vertex(Vec3 position) : this(
            position,
            new Vec2(0, 0),
            new Vec3(0.5f, 0.5f, 0.5f),
            new Vec3(0, 0, 0),
            new Vec3(0, 0, 0))
        {

        }
        
        public Vertex(Vec3 position, Vec2 texCoord) : this(
            position,
            texCoord,
            new Vec3(0.5f, 0.5f, 0.5f),
            new Vec3(0, 0, 0),
            new Vec3(0, 0, 0))
        {

        }

        public Vec3 Position { get; set; }
        public Vec2 TexCoord { get; set; }
        public Vec3 Color { get; set; }
        public Vec3 Normal { get; set; }
        public Vec3 Tangent { get; set; }

    }
}