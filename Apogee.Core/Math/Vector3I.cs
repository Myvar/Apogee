namespace Apogee.Core.Math
{
    public class Vector3I
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }

        public int this[int x]
        {
            get
            {
                switch (x)
                {
                    case 0:
                        return X;
                    case 1:
                        return Y;
                    case 2:
                        return Z;
                }

                return -1;
            }
            set
            {
                switch (x)
                {
                    case 0:
                        X = value;
                        break;
                    case 1:
                        Y = value;
                        break;
                    case 2:
                        Z = value;
                        break;
                }
            }
        }

        public Vector3I()
        {
        }

        public Vector3I(int a)
        {
            X = a;
            Y = a;
            Z = a;
        }

        public Vector3I(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }
}