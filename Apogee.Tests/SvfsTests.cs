using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Apogee.SVFS;
using Apogee.SVFS.Structures;
using NUnit.Framework;

namespace Apogee.Tests
{
    public class Tests
    {
        public static Dictionary<string, byte[]> RandomDataSets { get; set; } = new Dictionary<string, byte[]>();

        [SetUp]
        public void Setup()
        {
            var rng = new Random();

            for (int i = 0; i < 100; i++)
            {
                var data = new byte[rng.Next(100, 10000)];
                rng.NextBytes(data);
                RandomDataSets.Add(data.Md5Hash(), data);
            }
        }

        [Test]
        public void BasicTest()
        {
            var sw = new Stopwatch();
            using (var mem = new MemoryStream())
            {
                var sfs = new SimpleVfs();
                sfs.ScopeGroup("test");

                foreach (var randomDataSet in RandomDataSets)
                {
                    var data = new MemoryStream(randomDataSet.Value);
                    {
                        sfs.AddAsset(new AssetEntry(randomDataSet.Key), data);
                    }
                }

                sw.Restart();
                sfs.Write(mem);
                sw.Stop();

                Console.WriteLine($"Writing Data took {sw.ElapsedMilliseconds}MS");

                mem.Position = 0;

                sfs = new SimpleVfs();
                sfs.Read(mem);

                foreach (var sfsGroup in sfs.Groups)
                {
                    sfs.ScopeGroup(sfsGroup);
                    foreach (var asset in sfs.Assets)
                    {
                        sw.Restart();
                        using (var data = sfs.OpenRead(asset))
                        {
                            sw.Stop();

                            Console.WriteLine($"Reading Data took {sw.ElapsedMilliseconds}MS");

                            Assert.True(RandomDataSets.ContainsKey(data.Md5Hash()),
                                "Bad Data or Failed to find dataset for entry");
                        }
                    }
                }

                Console.WriteLine($"Hash for file: {mem.ToArray().Md5Hash()}");
            }
        }
    }
}