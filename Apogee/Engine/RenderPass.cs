using System;
using System.Numerics;
using Apogee.Engine.Core;
using Veldrid;

namespace Apogee.Engine
{
    public class RenderPass : IDisposable
    {
        private CommandList _commandList;

        private DeviceBuffer _projectionBuffer;
        private DeviceBuffer _viewBuffer;
        private DeviceBuffer _worldBuffer;
        private ResourceSet _projViewSet;
        private ResourceSet _worldTextureSet;
        private Pipeline _pipeline;
        private AShader _shader;


        private ResourceLayout _worldTextureLayout;

        public RenderPass(string shader)
        {
            var factory = GameEngine.GraphicsDevice.ResourceFactory;
            _shader = new AShader(shader);


            _commandList = factory.CreateCommandList();
            _projectionBuffer = factory.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer));
            _viewBuffer = factory.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer));
            _worldBuffer = factory.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer));


            var projViewLayout = factory.CreateResourceLayout(
                new ResourceLayoutDescription(
                    new ResourceLayoutElementDescription("ProjectionBuffer", ResourceKind.UniformBuffer,
                        ShaderStages.Vertex),
                    new ResourceLayoutElementDescription("ViewBuffer", ResourceKind.UniformBuffer,
                        ShaderStages.Vertex)));

            _worldTextureLayout = factory.CreateResourceLayout(
                new ResourceLayoutDescription(
                    new ResourceLayoutElementDescription("WorldBuffer", ResourceKind.UniformBuffer,
                        ShaderStages.Vertex),
                    new ResourceLayoutElementDescription("SurfaceTexture", ResourceKind.TextureReadOnly,
                        ShaderStages.Fragment),
                    new ResourceLayoutElementDescription("SurfaceSampler", ResourceKind.Sampler,
                        ShaderStages.Fragment)));

            _projViewSet = factory.CreateResourceSet(new ResourceSetDescription(
                projViewLayout,
                _projectionBuffer,
                _viewBuffer));


            // Create pipeline
            var pipelineDescription = new GraphicsPipelineDescription()
            {
                BlendState = BlendStateDescription.SingleOverrideBlend,
                DepthStencilState = new DepthStencilStateDescription(
                    true,
                    true,
                    ComparisonKind.LessEqual),
                RasterizerState = new RasterizerStateDescription(
                    FaceCullMode.None, //@Speed with cgs it must be solid
                    PolygonFillMode.Solid,
                    FrontFace.Clockwise,
                    true,
                    false),
                PrimitiveTopology = PrimitiveTopology.TriangleList
            };
            pipelineDescription.ShaderSet = _shader;
            pipelineDescription.Outputs = GameEngine.GraphicsDevice.SwapchainFramebuffer.OutputDescription;
            pipelineDescription.ResourceLayouts = new[] {projViewLayout, _worldTextureLayout};
            _pipeline = factory.CreateGraphicsPipeline(pipelineDescription);
        }

        public RenderPass SetCamera()
        {
            _commandList.UpdateBuffer(_projectionBuffer, 0, Matrix4x4.CreatePerspectiveFieldOfView(
                1.0f,
                (float) GameEngine.Window.Width / GameEngine.Window.Height,
                0.5f,
                100f));

            _commandList.UpdateBuffer(_viewBuffer, 0,
                Matrix4x4.CreateLookAt(Vector3.UnitZ * 2.5f, Vector3.Zero, Vector3.UnitY));

            return this;
        }

        public RenderPass Transfrom(Transform t)
        {
            _commandList.UpdateBuffer(_worldBuffer, 0, (Matrix4x4) t);

            return this;
        }

        public RenderPass SetTexture(ProcessedTexture t)
        {
            var factory = GameEngine.GraphicsDevice.ResourceFactory;


            _worldTextureSet = factory.CreateResourceSet(new ResourceSetDescription(
                _worldTextureLayout,
                _worldBuffer,
                t.SurfaceTexture,
                GameEngine.GraphicsDevice.Aniso4xSampler));


            _commandList.SetGraphicsResourceSet(1, _worldTextureSet);
            return this;
        }

        public RenderPass DrawMesh(AMesh mesh)
        {
            mesh.Draw(_commandList);
            return this;
        }

        public RenderPass NewFrame(Framebuffer fb = null)
        {
            _commandList.Begin();


            if (fb == null)
            {
                fb = GameEngine.GraphicsDevice.SwapchainFramebuffer;
            }

            // We want to render directly to the output window.
            _commandList.SetFramebuffer(fb);
            _commandList.ClearColorTarget(0, RgbaFloat.Black);
            _commandList.ClearDepthStencil(1f);
            _commandList.SetPipeline(_pipeline);

            _commandList.SetGraphicsResourceSet(0, _projViewSet);


            return this;
        }

        public void Flush()
        {
            _commandList.End();
            _worldTextureSet.Dispose();
            GameEngine.GraphicsDevice.SubmitCommands(_commandList);
        }

        public void Dispose()
        {
            _commandList?.Dispose();
            _projectionBuffer?.Dispose();
            _viewBuffer?.Dispose();
            _worldBuffer?.Dispose();
            _projViewSet?.Dispose();
            _worldTextureSet?.Dispose();
        }
    }
}