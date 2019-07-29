using System.Numerics;
using Apogee.Engine;
using Apogee.Engine.Core;

namespace Apogee.Test
{
    public class TestGame : AGame
    {
        private static AMesh _mesh;
        private Transform _transform;
        private static RenderPass _renderPass;
        private static ProcessedTexture _texture;

        public override void Load()
        {
            //   _mesh = new AMesh("cube");

            var s = new Servo();
            var solid = s.GetSolid();

            _mesh = new AMesh(solid);

            _transform = new Transform {Scale = new Vector3(0.03f)};
            _renderPass = new RenderPass("basic");
            _texture = LoadEmbeddedAsset<ProcessedTexture>("Textures.bricks.jpg");
        }

        public override void CleanUp()
        {
            _renderPass.Dispose();
            _mesh.Dispose();
        }

        public override void Render()
        {
            _renderPass
                .NewFrame()
                .SetCamera()
                .Transfrom(_transform)
                .SetTexture(_texture)
                .DrawMesh(_mesh)
                .Flush();
        }

        public override void Update()
        {
            _transform.Rotation += new Vector3(0.1f, 0.2f, 0);
        }
    }
}