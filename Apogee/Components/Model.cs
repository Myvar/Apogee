﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using Apogee.Core;
using Apogee.Ecs;
using Newtonsoft.Json;
using OpenTK.Graphics.OpenGL;

namespace Apogee.Components
{
    
    public class Model : Component
    {
        public string File { get; set; }

        [JsonIgnore]
        public int Vbo;
        [JsonIgnore]
        public List<int> Ibos = new List<int>();
        [JsonIgnore]
        public int Size { get; set; }

        public Model()
        {
            Vbo = GL.GenBuffer();   
        }        
        
        public void Load(Vertex[] vertexsData, List<List<uint>> indices)
        {
            foreach (var index in indices)
            {
                calcNormals(vertexsData, index.ToArray());
            }

            var vertexData = new List<byte>();


            foreach (var d in vertexsData)
            {
                vertexData.AddRange(BitConverter.GetBytes(d.Position.X));
                vertexData.AddRange(BitConverter.GetBytes(d.Position.Y));
                vertexData.AddRange(BitConverter.GetBytes(d.Position.Z));


                vertexData.AddRange(BitConverter.GetBytes(d.Color.X));
                vertexData.AddRange(BitConverter.GetBytes(d.Color.Y));
                vertexData.AddRange(BitConverter.GetBytes(d.Color.Z));


                vertexData.AddRange(BitConverter.GetBytes(d.Normal.X));
                vertexData.AddRange(BitConverter.GetBytes(d.Normal.Y));
                vertexData.AddRange(BitConverter.GetBytes(d.Normal.Z));

                vertexData.AddRange(BitConverter.GetBytes(d.Tangent.X));
                vertexData.AddRange(BitConverter.GetBytes(d.Tangent.Y));
                vertexData.AddRange(BitConverter.GetBytes(d.Tangent.Z));

                vertexData.AddRange(BitConverter.GetBytes(d.TexCoord.X));
                vertexData.AddRange(BitConverter.GetBytes(d.TexCoord.Y));
            }


            var data = vertexData.ToArray();

            Size = indices.First().Count;
            GL.BindBuffer(BufferTarget.ArrayBuffer, Vbo);
            GL.BufferData<byte>(BufferTarget.ArrayBuffer, (IntPtr) (data.Length), data, BufferUsageHint.StaticDraw);


            for (int j = 0; j < indices.Count; j++)
            {
                var index = indices[j];
                var ibo = GL.GenBuffer();
                Ibos.Add(ibo);

                var buf = new List<byte>();
                foreach (var i in index)
                {
                    buf.AddRange(BitConverter.GetBytes(i));
                }


                //indices[]
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, ibo);
                GL.BufferData<byte>(BufferTarget.ElementArrayBuffer, (IntPtr) (buf.Count), buf.ToArray(),
                    BufferUsageHint.StaticDraw);

                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            }
        }

        private void calcNormals(Vertex[] data, uint[] indecies)
        {
            try
            {
                for (int i = 0; i < indecies.Length; i += 3)
                {
                    uint i0 = indecies[i];
                    uint i1 = indecies[i + 1];
                    uint i2 = indecies[i + 2];

                    Vec3 v1 = data[i1].Position - data[i0].Position;
                    Vec3 v2 = data[i2].Position - data[i0].Position;

                    Vec3 normal = v1.Cross(v2);
                    normal.Normalize();

                    data[i0].Normal = data[i0].Normal + normal;
                    data[i1].Normal = data[i1].Normal + normal;
                    data[i2].Normal = data[i2].Normal + normal;
                }

                for (int i = 0; i < data.Length; i++)
                {
                    data[i].Normal = data[i].Normal.Normalized();
                }
            }
            catch
            {
            }
        }

        private void CalcTangents(Vertex[] data, uint[] indecies)
        {
            for (int i = 0; i < indecies.Length; i += 3)
            {
                uint i0 = indecies[i];
                uint i1 = indecies[i + 1];
                uint i2 = indecies[i + 2];

                Vec3 edge1 = data[i1].Position - data[i0].Position;
                Vec3 edge2 = data[i2].Position - data[i0].Position;

                float deltaU1 = data[i1].TexCoord.X - data[i0].TexCoord.X;
                float deltaV1 = data[i1].TexCoord.Y - data[i0].TexCoord.Y;
                float deltaU2 = data[i2].TexCoord.X - data[i0].TexCoord.X;
                float deltaV2 = data[i2].TexCoord.Y - data[i0].TexCoord.Y;

                float dividend = (deltaU1 * deltaV2 - deltaU2 * deltaV1);
                //TODO: The first 0.0f may need to be changed to 1.0f here.
                float f = dividend == 0 ? 0.0f : 1.0f / dividend;

                Vec3 tangent = new Vec3(0, 0, 0);
                tangent.X = f * (deltaV2 * edge1.X - deltaV1 * edge2.X);
                tangent.Y = f * (deltaV2 * edge1.Y - deltaV1 * edge2.Y);
                tangent.Z = f * (deltaV2 * edge1.Z - deltaV1 * edge2.Z);

                data[i0].Tangent = data[i0].Tangent + tangent;
                data[i1].Tangent = data[i1].Tangent + tangent;
                data[i2].Tangent = data[i2].Tangent + tangent;
            }

            for (int i = 0; i < data.Length; i++)
                data[i].Tangent.Normalize();
        }

        public void Dispose()
        {
            GL.DeleteBuffer(Vbo);

            foreach (var ibo in Ibos)
            {
                GL.DeleteBuffer(ibo);
            }
        }

        public void Draw()
        {
            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            GL.EnableVertexAttribArray(2);
            GL.EnableVertexAttribArray(3);
            GL.EnableVertexAttribArray(4);

            GL.BindBuffer(BufferTarget.ArrayBuffer, Vbo);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Vertex.Size * 4, 0);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, Vertex.Size * 4, 12);
            GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, Vertex.Size * 4, 24);
            GL.VertexAttribPointer(3, 3, VertexAttribPointerType.Float, false, Vertex.Size * 4, 36);
            GL.VertexAttribPointer(4, 2, VertexAttribPointerType.Float, false, Vertex.Size * 4, 48);

            foreach (var ibo in Ibos)
            {
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, ibo);
                GL.DrawElements(BeginMode.Triangles, Size, DrawElementsType.UnsignedInt, 0);
            }

            GL.DisableVertexAttribArray(0);
            GL.DisableVertexAttribArray(1);
            GL.DisableVertexAttribArray(2);
            GL.DisableVertexAttribArray(3);
            GL.DisableVertexAttribArray(4);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }
    }
}