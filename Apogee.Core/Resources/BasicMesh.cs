using System;
using System.Collections.Generic;
using System.Linq;
using Apogee.Core.Animations;
using Apogee.Core.Math;
using OpenToolkit.Graphics.OpenGL;

namespace Apogee.Core.Resources
{
    public class BasicMesh : IAsset
    {
        public int Size { get; set; }

        private int _vbo;
        private int ab;
        private List<int> _ibos = new List<int>();

        public BasicMesh()
        {
            ab = GL.GenVertexArray();
        }

        //skeliton
        public Joint RootJoint { get; set; }
        public int JointCount { get; set; }

        public Animator Animator { get; set; }
        public Animation Animation { get; set; }
        public List<string> JointsNames { get; set; } = new List<string>();

        public void Load(Joint root, int count)
        {
            RootJoint = root;
            JointCount = count;

            RootJoint.CalcInverseBindTransform(new Matrix4F().InitIdentity());
            Animator = new Animator(this);
        }


        public Matrix4F[] GetJointTransforms()
        {
            var jointMatrices = new Matrix4F[JointCount];
            AddJointsToArray(RootJoint, ref jointMatrices);
            return jointMatrices;
        }

        private void AddJointsToArray(Joint headJoint, ref Matrix4F[] jointMatrices)
        {
            jointMatrices[JointsNames.IndexOf(headJoint.Name.Replace(".", "_"))] = headJoint.AnimatedTransform;
            foreach (var childJoint in headJoint.Children)
            {
                AddJointsToArray(childJoint, ref jointMatrices);
            }
        }

        public Matrix4F[] GetLocalJointTransforms()
        {
            var jointMatrices = new Matrix4F[JointCount];
            AddLocalJointsToArray(RootJoint, ref jointMatrices);
            return jointMatrices;
        }

        private void AddLocalJointsToArray(Joint headJoint, ref Matrix4F[] jointMatrices)
        {
            jointMatrices[JointsNames.IndexOf(headJoint.Name.Replace(".", "_"))] = headJoint.LocalBindTransform;
            foreach (var childJoint in headJoint.Children)
            {
                AddLocalJointsToArray(childJoint, ref jointMatrices);
            }
        }

        public void Load(List<Vertex> vertexData, List<List<uint>> indices)
        {
            foreach (var index in indices)
            {
                CalcTangents(vertexData, index.ToArray());
            }

            Load(vertexData.ToArray().ToByteArray().ToList(), indices);
        }

        public void Load(List<byte> data, List<List<uint>> indices)
        {
            _vbo = GL.GenBuffer();
            _ibos = new List<int>();

            Size = indices.First().Count; // @Hack: is to correct
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, data.Count, data.ToArray(), BufferUsageHint.StaticDraw);


            for (var j = 0; j < indices.Count; j++)
            {
                var index = indices[j];

                var ibo = GL.GenBuffer();
                _ibos.Add(ibo);

                var buf = new List<byte>();
                for (var i1 = 0; i1 < index.Count; i1++)
                {
                    var i = index[i1];
                    buf.AddRange(BitConverter.GetBytes(i));
                }

                //indices[]
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, ibo);
                GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr) (buf.Count), buf.ToArray(),
                    BufferUsageHint.StaticDraw);

                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            }
        }

        public static void CalcNormals(Vertex[] data, uint[] indecies)
        {
            //@EdgeCase we should realy use normals if the mesh does not have any provided
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
                // ignored @Exception Handel it
            }
        }

        public static void CalcTangents(List<Vertex> data, uint[] indices)
        {
            for (var i = 0; i < indices.Length; i += 3)
            {
                var i0 = (int) indices[i];
                var i1 = (int) indices[i + 1];
                var i2 = (int) indices[i + 2];

                var edge1 = data[i1].Position - data[i0].Position;
                var edge2 = data[i2].Position - data[i0].Position;

                var deltaU1 = data[i1].TexCoord.X - data[i0].TexCoord.X;
                var deltaV1 = data[i1].TexCoord.Y - data[i0].TexCoord.Y;
                var deltaU2 = data[i2].TexCoord.X - data[i0].TexCoord.X;
                var deltaV2 = data[i2].TexCoord.Y - data[i0].TexCoord.Y;

                var dividend = (deltaU1 * deltaV2 - deltaU2 * deltaV1);
                //TODO: The first 0.0f may need to be changed to 1.0f here.
                var f = dividend == 0 ? 0.0f : 1.0f / dividend;

                var tangent = new Vector3F(0, 0, 0);
                tangent.X = f * (deltaV2 * edge1.X - deltaV1 * edge2.X);
                tangent.Y = f * (deltaV2 * edge1.Y - deltaV1 * edge2.Y);
                tangent.Z = f * (deltaV2 * edge1.Z - deltaV1 * edge2.Z);

                data[i0].Tangent = data[i0].Tangent + tangent;
                data[i1].Tangent = data[i1].Tangent + tangent;
                data[i2].Tangent = data[i2].Tangent + tangent;
            }

            for (var i = 0; i < data.Count; i++)
                data[i].Tangent.Normalize();
        }


        public void Dispose()
        {
            GL.DeleteBuffer(_vbo);
            foreach (var ibo in _ibos)
            {
                GL.DeleteBuffer(ibo);
            }
        }

        public void Draw()
        {
            GL.BindVertexArray(ab);
            
            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            GL.EnableVertexAttribArray(2);
            GL.EnableVertexAttribArray(3);
            GL.EnableVertexAttribArray(4);
            GL.EnableVertexAttribArray(5);
            GL.EnableVertexAttribArray(6);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Vertex.Size * 4, 0);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, Vertex.Size * 4, 12);
            GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, Vertex.Size * 4, 24);
            GL.VertexAttribPointer(3, 3, VertexAttribPointerType.Float, false, Vertex.Size * 4, 36);
            GL.VertexAttribPointer(4, 2, VertexAttribPointerType.Float, false, Vertex.Size * 4, 48);

            // GL.VertexAttribPointer(5, 3, VertexAttribPointerType.Int, false, Vertex.Size * 4, 56);
            GL.VertexAttribIPointer(5, 3, VertexAttribIntegerType.Int, Vertex.Size * 4, (IntPtr) 56);
            GL.VertexAttribPointer(6, 3, VertexAttribPointerType.Float, false, Vertex.Size * 4, 68);
            for (var i = 0;
                i < _ibos.Count;
                i++)
            {
                var ibo = _ibos[i];
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, ibo);
                GL.DrawElements(BeginMode.Triangles, Size, DrawElementsType.UnsignedInt, 0);
            }

            GL.DisableVertexAttribArray(0);
            GL.DisableVertexAttribArray(1);
            GL.DisableVertexAttribArray(2);
            GL.DisableVertexAttribArray(3);
            GL.DisableVertexAttribArray(4);
            GL.DisableVertexAttribArray(5);
            GL.DisableVertexAttribArray(6);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }
    }
}