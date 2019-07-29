using Apogee.Test;

namespace Apogee
{
    class Program
    {
        static void Main(string[] args)
        {
            var ge = new GameEngine(new TestGame());
            ge.Start();
        }
    }
}