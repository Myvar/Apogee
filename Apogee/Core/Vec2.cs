namespace Apogee.Core
{
    public class Vec2
    {
        public Vec2(float x, float y)
        {
            X = x;
            Y = y;
        }

        public float X { get; set; }
        public float Y { get; set; }
        
        public static Vec2 operator -(Vec2 c1, Vec2 c2)
        {
            return new Vec2(c1.X - c2.X, c1.Y - c2.Y);
        }
        
        public static Vec2 operator +(Vec2 c1, Vec2 c2)
        {
            return new Vec2(c1.X + c2.X, c1.Y + c2.Y);
        }
        
        public static Vec2 operator *(Vec2 c1, Vec2 c2)
        {
            return new Vec2(c1.X * c2.X, c1.Y * c2.Y);
        }
        
        public static Vec2 operator /(Vec2 c1, Vec2 c2)
        {
            return new Vec2(c1.X / c2.X, c1.Y / c2.Y);
        }
    }
}