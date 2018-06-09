using System.Collections.Generic;
using System.IO;
using Apogee.Components;

namespace Apogee.Systems
{
    public class Shaders : Ecs.System
    {
        public Dictionary<string, Shader> Hashes { get; set; } = new Dictionary<string, Shader>();


        public void LoadShader(Shader shader)
        {
            if (Hashes.ContainsKey(shader.File))
            {
                shader.Program = Hashes[shader.File].Program;
            }
            else
            {
                shader.Load(Path.Combine(Container.AssetsPath, shader.File));
                Hashes.Add(shader.File, shader);
            }
        }

        public Shaders()
        {
            RunOnce = true;
        }
    }
}