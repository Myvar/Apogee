using System;

namespace Apogee
{
    public static class Utils
    {
        public static float ConvertToRadians(float angle)
        {
            return (MathF.PI / 180) * angle;
        }
    }

    public static class NumericExtensions
    {
        public static float ToRadians(this float val)
        {
            return (MathF.PI / 180) * val;
        }
    }
}