namespace Apogee
{
    class Program
    {
        static void Main(string[] args)
        {
            new GameEngine(args[0], args[1]).Start();
        }
    }
}
