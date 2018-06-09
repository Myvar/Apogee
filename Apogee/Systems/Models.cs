using System.Collections.Generic;
using System.IO;
using Apogee.Components;
using Apogee.Core;

namespace Apogee.Systems
{
    public class Models : Ecs.System
    {
        public Dictionary<string, Model> Hashes { get; set; } = new Dictionary<string, Model>();

        public void LoadModel(Model model)
        {
            if (Hashes.ContainsKey(model.File))
            {
                model.Vbo = Hashes[model.File].Vbo;
                model.Ibos.AddRange(Hashes[model.File].Ibos);
                model.Size = Hashes[model.File].Size;
            }
            else
            {
                var ogexFile = new OpenGexLoader(Path.Combine(Container.AssetsPath, model.File));
                model.Load(ogexFile.GetVertexData(), ogexFile.GetIndeciesData());
                Hashes.Add(model.File, model);
            }
        }

        public Models()
        {
            RunOnce = true;
        }
    }
}