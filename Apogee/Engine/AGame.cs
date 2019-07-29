using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using Apogee.Engine.AssetProcessor;

namespace Apogee.Engine
{
    public abstract class AGame
    {
        public abstract void Load();
        public abstract void CleanUp();
        public abstract void Render();
        public abstract void Update();

        public T LoadEmbeddedAsset<T>(string name) where T : ProcessedTexture
        {
            var assembly = Assembly.GetEntryAssembly();
            using (Stream stream = GetType().Assembly
                .GetManifestResourceStream(assembly.EntryPoint.DeclaringType.Namespace + "._res." + name))
            {
                if (stream == null)
                {
                    throw new InvalidOperationException("No embedded asset with the name " + name);
                }

                return (T) new ImageSharpProcessor().ProcessT(stream, "." + name.Split('.').Last());
            }
        }
    }
}