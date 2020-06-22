namespace Apogee.Core.Math
{
    public class Vector2F
    {
        public Vector2F(float x, float y)
        {
            X = x;
            Y = y;
        }

        public Vector2F(float x)
        {
            X = x;
            Y = x;
        }

        public float X { get; set; }
        public float Y { get; set; }

        private float Lerp(float firstFloat, float secondFloat, float by)
        {
            return firstFloat * (1 - by) + secondFloat * by;
        }

        public Vector2F Clone()
        {
            return new Vector2F(X, Y);
        }

        public Vector2F Lerp(Vector2F secondVector, float by)
        {
            X = Lerp(X, secondVector.X, by);
            Y = Lerp(Y, secondVector.Y, by);
            return this;
        }

        public static Vector2F operator -(Vector2F c1, Vector2F c2)
        {
            return new Vector2F(c1.X - c2.X, c1.Y - c2.Y);
        }

        public static Vector2F operator +(Vector2F c1, Vector2F c2)
        {
            return new Vector2F(c1.X + c2.X, c1.Y + c2.Y);
        }

        public static Vector2F operator *(Vector2F c1, Vector2F c2)
        {
            return new Vector2F(c1.X * c2.X, c1.Y * c2.Y);
        }

        public static Vector2F operator /(Vector2F c1, Vector2F c2)
        {
            return new Vector2F(c1.X / c2.X, c1.Y / c2.Y);
        }
    }
}