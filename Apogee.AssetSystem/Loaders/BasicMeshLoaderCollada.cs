using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Apogee.Core;
using Apogee.Core.Animations;
using Apogee.Core.Math;
using Apogee.Core.Resources;

namespace Apogee.AssetSystem.Loaders
{
    public class BasicMeshLoaderCollada : IAssetLoader
    {
        private class ColladaStructure
        {
            public Dictionary<string, ColladaSource> Sources { get; set; } = new Dictionary<string, ColladaSource>();
            public List<ColladaMesh> Meshes { get; set; } = new List<ColladaMesh>();
            public List<ColladaAnimation> Animations { get; set; } = new List<ColladaAnimation>();

            public ColladaSkin Skin { get; set; }
            public Joint RootJoint { get; set; }
            public int JointCount { get; set; }

            public ColladaSource GetSource(string id)
            {
                foreach ((string name, ColladaSource source) in Sources)
                {
                    if (name == id) return source;
                }

                throw new Exception();
            }
        }

        private class ColladaSource
        {
            public string Id { get; set; }
            public string SourceRaw { get; set; }
            public string[] SourceRawSplit { get; set; }
            public int SourceCount { get; set; }
            public int AccessorCount { get; set; }
            public int AccessorStride { get; set; }
            public Dictionary<string, string> Params { get; set; } = new Dictionary<string, string>();

            private object[] _source;

            public ColladaSource(XElement element)
            {
                //@FML @HACK @CleanUp this should realy be more oop and broken up 

                var fmt = new NumberFormatInfo();
                fmt.NegativeSign = "-";
                fmt.NumberDecimalSeparator = ".";

                if (element.Name != "source") throw new Exception();

                Id = element.Attribute("id").Value;
                var accessor = element.Element("technique_common").Element("accessor");

                var arrayID = accessor.Attribute("source").Value.Trim('#');
                var elementArray = element.Elements().First(x => x.Attribute("id").Value == arrayID);
                SourceCount = int.Parse(elementArray.Attribute("count").Value);
                SourceRaw = elementArray.Value;
                SourceRawSplit = SourceRaw.Trim().Split(' ');

                _source = new object[SourceCount];

                AccessorCount = int.Parse(accessor.Attribute("count").Value);
                AccessorStride = int.Parse(accessor.Attribute("stride").Value);

                foreach (var param in accessor.Elements("param"))
                {
                    Params.Add(param.Attribute("name").Value, param.Attribute("type").Value);
                }

                if (Params.ContainsValue("float4x4"))
                {
                    AccessorCount *= AccessorStride;
                    AccessorStride = 1;
                }

                for (var i = 0; i < AccessorCount; i++)
                {
                    var offset = i * AccessorStride;

                    for (var j = 0; j < Params.Count; j++)
                    {
                        var param = Params.Values.ToArray()[j];
                        switch (param)
                        {
                            case "float4x4":
                            case "float":
                                _source[offset + j] = (float) double.Parse(SourceRawSplit[offset + j], fmt);
                                break;
                            case "name":
                                _source[offset + j] = SourceRawSplit[offset + j];
                                break;
                            default:
                                Console.WriteLine($"[Collada] Unknown Source ParamType: {param}");
                                break;
                        }
                    }
                }
            }

            public float GetFloat(int index) => (float) _source[index];
            public string GetString(int index) => (string) _source[index];

            public Vector3F GetVector3F(int index)
            {
                var offset = AccessorStride * index;
                return new Vector3F(
                    (float) _source[offset],
                    (float) _source[offset + 1],
                    (float) _source[offset + 2]);
            }

            public Vector2F GetVector2F(int index)
            {
                var offset = AccessorStride * index;
                return new Vector2F(
                    (float) _source[offset],
                    (float) _source[offset + 1]);
            }
        }

        private class ColladaMesh
        {
            public Dictionary<string, string> Inputs { get; set; } = new Dictionary<string, string>();
            public int TrianglesCount { get; set; }
            public List<int> Indecies { get; set; } = new List<int>();

            public ColladaMesh(XElement e)
            {
                foreach (var descendant in e.Descendants())
                {
                    if (descendant.Name == "input")
                    {
                        Inputs.Add(descendant.Attribute("semantic").Value,
                            descendant.Attribute("source").Value.Trim('#'));
                    }
                    else if (descendant.Name == "triangles")
                    {
                        TrianglesCount = int.Parse(descendant.Attribute("count").Value);
                        foreach (var s in descendant.Element("p").Value.Trim().Split(' '))
                        {
                            Indecies.Add(int.Parse(s));
                        }
                    }
                }
            }
        }

        private class ColladaScene
        {
            public Joint Root { get; set; }
            public int JointCount { get; set; }

            public ColladaScene(XElement e)
            {
                Root = new Joint();
                Iterate(Root, e.Elements("node").First().Elements("node").First(), true);
            }

            private void Iterate(Joint j, XElement e, bool root = false)
            {
                var fmt = new NumberFormatInfo();
                fmt.NegativeSign = "-";
                fmt.NumberDecimalSeparator = ".";

                j.Name = e.Attribute("name").Value;
                j.ID = JointCount;
                JointCount++;

                var mat = e.Element("matrix");

                if (mat != null)
                {
                    var floats = mat.Value.Split(' ');
                    var dec = new OpenToolkit.Mathematics.Matrix4(
                        float.Parse(floats[0], fmt),
                        float.Parse(floats[1], fmt),
                        float.Parse(floats[2], fmt),
                        float.Parse(floats[3], fmt),
                        float.Parse(floats[4], fmt),
                        float.Parse(floats[5], fmt),
                        float.Parse(floats[6], fmt),
                        float.Parse(floats[7], fmt),
                        float.Parse(floats[8], fmt),
                        float.Parse(floats[9], fmt),
                        float.Parse(floats[10], fmt),
                        float.Parse(floats[11], fmt),
                        float.Parse(floats[12], fmt),
                        float.Parse(floats[13], fmt),
                        float.Parse(floats[14], fmt),
                        float.Parse(floats[15], fmt)
                    );

                    j.LocalBindTransform = new Matrix4F(dec);
                }

                foreach (var xElement in e.Elements("node"))
                {
                    var jj = new Joint();
                    Iterate(jj, xElement);
                    j.Children.Add(jj);
                }
            }
        }

        private class ColladaSkin
        {
            public Dictionary<string, string> Inputs { get; set; } = new Dictionary<string, string>();
            public int VertexCount { get; set; }
            public List<int> VCount { get; set; } = new List<int>();
            public List<int> V { get; set; } = new List<int>();

            public ColladaSkin(XElement e)
            {
                if (e == null) return;
                foreach (var descendant in e.Descendants())
                {
                    if (descendant.Name == "input")
                    {
                        if (!Inputs.ContainsKey(descendant.Attribute("semantic").Value))
                            Inputs.Add(descendant.Attribute("semantic").Value,
                                descendant.Attribute("source").Value.Trim('#'));
                    }
                    else if (descendant.Name == "vertex_weights")
                    {
                        VertexCount = int.Parse(descendant.Attribute("count").Value);
                        foreach (var s in descendant.Element("vcount").Value.Trim().Split(' '))
                        {
                            VCount.Add(int.Parse(s));
                        }

                        foreach (var s in descendant.Element("v").Value.Trim().Split(' '))
                        {
                            V.Add(int.Parse(s));
                        }
                    }
                }
            }
        }

        private class ColladaAnimation
        {
            public string Target { get; set; }
            public Dictionary<string, string> Inputs { get; set; } = new Dictionary<string, string>();

            public ColladaAnimation(XElement e)
            {
                Target = e.Element("channel").Attribute("target").Value.Split('/')[0];
                var c = Target.Split('_')[0].Length;
                Target = Target.Remove(0, c + 1);

                foreach (var descendant in e.Descendants())
                {
                    if (descendant.Name == "input")
                    {
                        if (!Inputs.ContainsKey(descendant.Attribute("semantic").Value))
                            Inputs.Add(descendant.Attribute("semantic").Value,
                                descendant.Attribute("source").Value.Trim('#'));
                    }
                }
            }
        }

        //@Incomplete: we should support triangulating the mesh our self and support materials
        private
            (
            List<Vertex> vertices,
            List<List<uint>> indecies,
            List<string> JointsNames,
            Joint RootJoint,
            int JointCount,
            Animation animation
            ) ReadCollada(Stream s)
        {
            var JointsNames = new List<string>();
            var xml = XDocument.Load(s);
            //remove name space crap
            xml.Descendants()
                .Attributes()
                .Where(x => x.IsNamespaceDeclaration)
                .Remove();

            //remove name space crap 2.0
            foreach (var elem in xml.Descendants())
            {
                elem.Name = elem.Name.LocalName;
            }

            var file = new ColladaStructure();

            foreach (var element in xml.Descendants())
            {
                switch (element.Name.ToString())
                {
                    case "source":
                    {
                        var source = new ColladaSource(element);
                        file.Sources.Add(source.Id, source);
                        break;
                    }
                    case "mesh":
                    {
                        var m = new ColladaMesh(element);
                        file.Meshes.Add(m);
                        break;
                    }
                    case "visual_scene":
                    {
                        if (element.Descendants().All(x => x.Attribute("type")?.Value != "JOINT")) continue;

                        var sc = new ColladaScene(element);
                        file.RootJoint = sc.Root;
                        file.JointCount = sc.JointCount;
                        break;
                    }
                    case "library_controllers":
                    {
                        file.Skin = new ColladaSkin(element.Element("controller")?.Element("skin"));
                        break;
                    }
                    case "animation":
                        file.Animations.Add(new ColladaAnimation(element));
                        break;
                }
            }

            /*
            * First we will load the skin data
            */
            var vertices = new List<Vertex>();
            var indicesList = new List<List<uint>>();

            var mesh = file.Meshes[0];

            var indices = new List<uint>();

            var posInput = mesh.Inputs["POSITION"];
            var posVertexData = file.GetSource(posInput);

            var normaInput = mesh.Inputs["NORMAL"];
            var normalVertexData = file.GetSource(normaInput);

            ColladaSource txtCoordVertexData = null;
            ColladaSource colorVertexData = null;
            ColladaSource skinJointsVertexData = null;
            ColladaSource skinWeightsVertexData = null;
            ColladaSource namesVertexData = null;

            if (mesh.Inputs.ContainsKey("TEXCOORD"))
            {
                var txtCoordInput = mesh.Inputs["TEXCOORD"];
                txtCoordVertexData = file.GetSource(txtCoordInput);
            }

            if (mesh.Inputs.ContainsKey("COLOR"))
            {
                var colorInput = mesh.Inputs["COLOR"];
                colorVertexData = file.GetSource(colorInput);
            }

            if (file.Skin != null)
            {
                if (file.Skin.Inputs.ContainsKey("JOINT"))
                {
                    var skinJointsInput = file.Skin.Inputs["JOINT"];
                    skinJointsVertexData = file.GetSource(skinJointsInput);
                }

                if (file.Skin.Inputs.ContainsKey("WEIGHT"))
                {
                    var skinWeightsInput = file.Skin.Inputs["WEIGHT"];
                    skinWeightsVertexData = file.GetSource(skinWeightsInput);
                }

                if (file.Skin.Inputs.ContainsKey("JOINT"))
                {
                    var namesInput = file.Skin.Inputs["JOINT"];
                    namesVertexData = file.GetSource(namesInput);

                    for (int i = 0; i < namesVertexData.AccessorCount; i++)
                    {
                        JointsNames.Add(namesVertexData.GetString(i));
                    }
                }
            }

            var offset = 0;

            for (var i = 0; i < posVertexData.AccessorCount; i++)
            {
                var vert = new Vertex(posVertexData.GetVector3F(i));

                if (file.JointCount > 0)
                {
                    var vcount = file.Skin.VCount[i];


                    var c = vcount;
                    var lst = new List<(float waight, int offset)>();

                    for (int j = 0; j < c; j++)
                    {
                        var noff = file.Skin.V[offset + (j * 2)];
                        lst.Add((skinWeightsVertexData.GetFloat(file.Skin.V[offset + (j * 2) + 1]), noff));
                    }

                    lst.Sort();
                    lst.Reverse();

                    for (int j = 0; j < (lst.Count > 3 ? 3 : lst.Count); j++)
                    {
                        vert.JointWeights[j] = lst[j].waight;
                        vert.JointIDS[j] = lst[j].offset;
                    }

                    offset += vcount * 2;
                }

                vertices.Add(vert);
            }

            var indexStride = mesh.Inputs.Count - 1; //remove VERTEX
            for (var i = 0;
                i < mesh.TrianglesCount;
                i++)
            {
                var a = (i * indexStride) * 3;
                var b = ((i * indexStride) * 3) + indexStride;
                var c = ((i * indexStride) * 3) + indexStride * 2;

                vertices[mesh.Indecies[a]].Normal = normalVertexData.GetVector3F(mesh.Indecies[a + 1]);
                vertices[mesh.Indecies[b]].Normal = normalVertexData.GetVector3F(mesh.Indecies[b + 1]);
                vertices[mesh.Indecies[c]].Normal = normalVertexData.GetVector3F(mesh.Indecies[c + 1]);

                if (txtCoordVertexData != null)
                {
                    vertices[mesh.Indecies[a]].TexCoord = txtCoordVertexData.GetVector2F(mesh.Indecies[a + 2]);
                    vertices[mesh.Indecies[b]].TexCoord = txtCoordVertexData.GetVector2F(mesh.Indecies[b + 2]);
                    vertices[mesh.Indecies[c]].TexCoord = txtCoordVertexData.GetVector2F(mesh.Indecies[c + 2]);
                }

                if (colorVertexData != null)
                {
                    vertices[mesh.Indecies[a]].Color = colorVertexData.GetVector3F(mesh.Indecies[a + 3]);
                    vertices[mesh.Indecies[b]].Color = colorVertexData.GetVector3F(mesh.Indecies[b + 3]);
                    vertices[mesh.Indecies[c]].Color = colorVertexData.GetVector3F(mesh.Indecies[c + 3]);
                }

                indices.Add((uint) mesh.Indecies[a]);
                indices.Add((uint) mesh.Indecies[b]);
                indices.Add((uint) mesh.Indecies[c]);
            }

            indicesList.Add(indices);

            var ani = new Animation();

            if (file.JointCount > 0)
            {
                var f = file.Sources[file.Animations[0].Inputs["INPUT"]];
                var frameCount = f.SourceCount;
                for (int i = 0; i < frameCount; i++)
                {
                    ani.KeyFrames.Add(new KeyFrame()
                    {
                        TimeStamp = f.GetFloat(i)
                    });
                }

                ani.Length = ani.KeyFrames.Last().TimeStamp;
            }

            foreach (var animation in file.Animations)
            {
                var outputs = file.Sources[animation.Inputs["OUTPUT"]];

                for (var i = 0; i < ani.KeyFrames.Count; i++)
                {
                    var keyFrame = ani.KeyFrames[i];

                    var matOffset = i * 16;


                    var dec = new OpenToolkit.Mathematics.Matrix4(
                        new OpenToolkit.Mathematics.Vector4(
                            outputs.GetFloat(matOffset + 0),
                            outputs.GetFloat(matOffset + 1),
                            outputs.GetFloat(matOffset + 2),
                            outputs.GetFloat(matOffset + 3)),
                        new OpenToolkit.Mathematics.Vector4(
                            outputs.GetFloat(matOffset + 4),
                            outputs.GetFloat(matOffset + 5),
                            outputs.GetFloat(matOffset + 6),
                            outputs.GetFloat(matOffset + 7)),
                        new OpenToolkit.Mathematics.Vector4(
                            outputs.GetFloat(matOffset + 8),
                            outputs.GetFloat(matOffset + 9),
                            outputs.GetFloat(matOffset + 10),
                            outputs.GetFloat(matOffset + 11)),
                        new OpenToolkit.Mathematics.Vector4(
                            outputs.GetFloat(matOffset + 12),
                            outputs.GetFloat(matOffset + 13),
                            outputs.GetFloat(matOffset + 14),
                            outputs.GetFloat(matOffset + 15)));

                    var decRot = dec.ExtractRotation();
                    var rot = new Quaternion(decRot.X, decRot.Y, decRot.Z, decRot.W);
                    dec.Transpose();

                    var decTrans = dec.ExtractTranslation();
                    var pos = new Vector3F(decTrans.X, decTrans.Y, decTrans.Z);
                    keyFrame.Pose.Add(animation.Target, new JointTransform(pos, rot, new Vector3F(1)));
                }
            }

            return (vertices, indicesList, JointsNames, file.RootJoint, file.JointCount, ani);
        }


        public IAsset LoadFromFile(RawAssetSource source)
        {
            var re = new BasicMesh();
            (
                List<Vertex> vertices,
                List<List<uint>> indecies,
                List<string> JointsNames,
                Joint RootJoint,
                int JointCount,
                Animation animation
            ) = ReadCollada(File.OpenRead(source.File));


            re.Load(vertices, indecies);
            if (JointCount > 0)
            {
                re.Load(RootJoint, JointCount);
                re.JointsNames = JointsNames;

                re.Animation = animation;
                re.Animator.DoAnimation(re.Animation);
            }

            return re;
        }
    }
}