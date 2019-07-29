using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using Apogee.Engine.Core;
using Csg;
using ObjLoader.Loader.Loaders;
using Veldrid;

namespace Apogee.Engine
{
    public class AMesh : IDisposable
    {
        private static DeviceBuffer _vertexBuffer;
        private static List<(uint leng, DeviceBuffer buf)> _indexBuffers = new List<(uint leng, DeviceBuffer buf)>();

        public AMesh(string name)
        {
            var factory = GameEngine.GraphicsDevice.ResourceFactory;

            /*AVertex[] quadVertices =
            {
                new AVertex(new Vector3(-.75f, .75f, 0), Vector2.Zero, RgbaFloat.Red.ToVector4(), Vector3.Zero,
                    Vector3.Zero),
                new AVertex(new Vector3(.75f, .75f, 0), Vector2.Zero, RgbaFloat.Green.ToVector4(), Vector3.Zero,
                    Vector3.Zero),
                new AVertex(new Vector3(-.75f, -.75f, 0), Vector2.Zero, RgbaFloat.Blue.ToVector4(), Vector3.Zero,
                    Vector3.Zero),
                new AVertex(new Vector3(.75f, -.75f, 0), Vector2.Zero, RgbaFloat.Yellow.ToVector4(), Vector3.Zero,
                    Vector3.Zero)
            };

            uint[] quadIndices = {0, 1, 2, 3};
            Load(quadVertices, new List<uint[]>() {quadIndices});*/
            var assembly = Assembly.GetEntryAssembly();

            var objLoaderFactory = new ObjLoaderFactory();
            var objLoader = objLoaderFactory.Create(new MaterialNullStreamProvider());
            using (var stream =
                assembly.GetManifestResourceStream(assembly.EntryPoint.DeclaringType.Namespace + "._res.Models." +
                                                   name +
                                                   ".obj"))
            {
                var result = objLoader.Load(stream);

                var verts = new List<AVertex>();

                var indices = new List<uint[]>();

                bool genNormals = !(result.Normals.Count > 0);

                foreach (var g in result.Groups)
                {
                    var tmp = new List<uint>();
                    foreach (var face in g.Faces)
                    {
                        for (int i = 0; i < face.Count; i++)
                        {
                            var f = face[i];

                            var vertex = result.Vertices[f.VertexIndex - 1];
                            var normal = result.Normals[f.NormalIndex - 1];
                            var text = result.Textures[f.TextureIndex - 1];

                            verts.Add(
                                new AVertex(
                                    new Vector3(vertex.X, vertex.Y, vertex.Z),
                                    new Vector2(text.X, text.Y),
                                    (i % 2 == 0 ? RgbaFloat.Blue.ToVector4() : RgbaFloat.Red.ToVector4()),
                                    new Vector3(normal.X, normal.Y, normal.Z),
                                    Vector3.Zero
                                )
                            );


                            tmp.Add((uint) verts.Count - 1);
                        }
                    }

                    indices.Add(tmp.ToArray());
                }

                Load(verts.ToArray(), indices);
            }
        }

        public AMesh(Solid mesh)
        {
            var verts = new List<AVertex>();
            var idxs = new List<uint[]>();
            var tmp = new List<uint>();

            foreach (var polygon in mesh.Polygons)
            {
                var offset = verts.Count ;
                for (var i = 0; i < polygon.Vertices.Count; i++)
                {
                    var vertex = polygon.Vertices[i];
                    var pos = vertex.Pos;
                    var tex = vertex.Tex;

                    verts.Add(
                        new AVertex(
                            new Vector3((float) pos.X, (float) pos.Y, (float) pos.Z),
                            new Vector2((float) tex.X, (float) tex.Y),
                            RgbaFloat.Grey.ToVector4(),
                            Vector3.Zero,
                            Vector3.Zero
                        )
                    );

                    //    tmp.Add((uint) verts.IndexOf(verts.Last()));
                }


                tmp.Add((uint) (offset + 0));
                tmp.Add((uint) (offset + 1));
                tmp.Add((uint) (offset + 2));

                tmp.Add((uint) (offset + 2));
                tmp.Add((uint) (offset + 3));
                tmp.Add((uint) (offset + 0));
            }

            idxs.Add(tmp.ToArray());
            Load(verts.ToArray(), idxs, true);
        }


        public void Draw(CommandList commandList)
        {
            // Set all relevant state to draw our quad.
            commandList.SetVertexBuffer(0, _vertexBuffer);

            foreach (var indexBuffer in _indexBuffers)
            {
                commandList.SetIndexBuffer(indexBuffer.buf, IndexFormat.UInt32);

                // Issue a Draw command for a single instance with 4 indices.
                commandList.DrawIndexed(indexBuffer.leng);
            }
        }

        public void Load(AVertex[] vertexData, List<uint[]> indices, bool genNormals = false)
        {
            var factory = GameEngine.GraphicsDevice.ResourceFactory;

            if (genNormals)
            {
                foreach (var index in indices)
                {
                    CalcNormals(vertexData, index.ToArray());
                }
            }


            var vbDescription = new BufferDescription(
                (uint) (vertexData.Length * AVertex.SizeInBytes),
                BufferUsage.VertexBuffer);
            _vertexBuffer = factory.CreateBuffer(vbDescription);
            GameEngine.GraphicsDevice.UpdateBuffer(_vertexBuffer, 0, vertexData);


            foreach (var index in indices)
            {
                var ibDescription = new BufferDescription(
                    (uint) (index.Length * sizeof(uint)),
                    BufferUsage.IndexBuffer);
                _indexBuffers.Add(((uint) index.Length, factory.CreateBuffer(ibDescription)));
                GameEngine.GraphicsDevice.UpdateBuffer(_indexBuffers.Last().buf, 0, index);
            }
        }

        private void CalcNormals(AVertex[] data, uint[] indecies)
        {
            try
            {
                for (var i = 0; i < indecies.Length; i += 3)
                {
                    var i0 = indecies[i];
                    var i1 = indecies[i + 1];
                    var i2 = indecies[i + 2];

                    var v1 = data[i1].Position - data[i0].Position;
                    var v2 = data[i2].Position - data[i0].Position;

                    var normal = v1.Cross(v2);
                    normal.Normalize();

                    data[i0].Normal = data[i0].Normal + normal;
                    data[i1].Normal = data[i1].Normal + normal;
                    data[i2].Normal = data[i2].Normal + normal;
                }

                for (var i = 0; i < data.Length; i++)
                {
                    data[i].Normal = data[i].Normal.Normalized();
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private void CalcTangents(AVertex[] data, uint[] indices)
        {
            for (var i = 0; i < indices.Length; i += 3)
            {
                var i0 = indices[i];
                var i1 = indices[i + 1];
                var i2 = indices[i + 2];

                var edge1 = data[i1].Position - data[i0].Position;
                var edge2 = data[i2].Position - data[i0].Position;

                var deltaU1 = data[i1].TexCoord.X - data[i0].TexCoord.X;
                var deltaV1 = data[i1].TexCoord.Y - data[i0].TexCoord.Y;
                var deltaU2 = data[i2].TexCoord.X - data[i0].TexCoord.X;
                var deltaV2 = data[i2].TexCoord.Y - data[i0].TexCoord.Y;

                var dividend = (deltaU1 * deltaV2 - deltaU2 * deltaV1);
                //TODO: The first 0.0f may need to be changed to 1.0f here.
                var f = dividend == 0 ? 0.0f : 1.0f / dividend;

                var tangent = new Vector3(0, 0, 0);
                tangent.X = f * (deltaV2 * edge1.X - deltaV1 * edge2.X);
                tangent.Y = f * (deltaV2 * edge1.Y - deltaV1 * edge2.Y);
                tangent.Z = f * (deltaV2 * edge1.Z - deltaV1 * edge2.Z);

                data[i0].Tangent = data[i0].Tangent + tangent;
                data[i1].Tangent = data[i1].Tangent + tangent;
                data[i2].Tangent = data[i2].Tangent + tangent;
            }

            for (var i = 0; i < data.Length; i++)
                data[i].Tangent.Normalize();
        }


        public void Dispose()
        {
            _vertexBuffer.Dispose();
            foreach (var buffer in _indexBuffers)
            {
                buffer.buf.Dispose();
            }
        }
    }
}