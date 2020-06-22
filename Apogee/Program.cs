using System;
using System.Diagnostics;
using System.IO;
using Apogee.Core;
using Apogee.Engine;
using Apogee.SVFS;
using Apogee.SVFS.Structures;

namespace Apogee
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
             * We have 4 main problems to solve in this experimental game engine
             * Hosting
             * Asset Compilation vfs loading & writing
             * Shaders
             * Multithreading
             */

            /*
             * Hosting options
             * A a new exe is made that refrances apogee
             * B some how use generators
             * C Apogee loads game but this complicates circular dependensies
             */

            /*
             * Assets
             * Every folder is a group you load groups
             * 
             */

            /*
             * Shaders Mono.Cecill and code generators to make syntax valid as well as code completion later
             */
            
            ApogeeEngine.Run();
        }
    }
}