using System;
using System.IO;
using System.Reflection;
using System.Text;
using Veldrid;
using Veldrid.SPIRV;

namespace Apogee.Engine
{
    public class AShader : IDisposable
    {
        private string _glsl;
        private Veldrid.Shader[] _shaders;

        public AShader(string name)
        {
            var assembly = Assembly.GetEntryAssembly();

            using (var stream =
                assembly.GetManifestResourceStream(assembly.EntryPoint.DeclaringType.Namespace + "._res.Shaders." + name +
                                                   ".glsl"))
            using (var textReader = new StreamReader(stream))
            {
                _glsl = textReader.ReadToEnd();
            }

            var vertexShaderDesc = new ShaderDescription(
                ShaderStages.Vertex,
                Encoding.UTF8.GetBytes("#version 450\n #define VERTEX\n" + _glsl),
                "main");
            var fragmentShaderDesc = new ShaderDescription(
                ShaderStages.Fragment,
                Encoding.UTF8.GetBytes("#version 450\n #define FRAGMENT\n" + _glsl),
                "main");

            _shaders = GameEngine.GraphicsDevice.ResourceFactory.CreateFromSpirv(vertexShaderDesc, fragmentShaderDesc);
        }


        public void Dispose()
        {
            foreach (Shader shader in _shaders)
            {
                shader.Dispose();
            }
        }

        public ShaderSetDescription GetShaderSetDescription()
        {
            return new ShaderSetDescription(
                vertexLayouts: new VertexLayoutDescription[] {AVertex.GetLayout()},
                shaders: _shaders);
        }

        public static implicit operator ShaderSetDescription(AShader shader)
        {
            return shader.GetShaderSetDescription();
        }
    }
}