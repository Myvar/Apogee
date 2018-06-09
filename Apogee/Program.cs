using System;
using System.Collections.Generic;
using System.IO;
using Apogee.Components;
using Apogee.Core;

namespace Apogee
{
    class Program
    {
        static void Main(string[] args)
        {
            GameEngine.Load("../TestGame");
            GameEngine.Container.LoadFile(Path.Combine(Container.AssetsPath, "test.map"));
            GameEngine.Start();
        }

        private static int _counter = 0;
    }
}