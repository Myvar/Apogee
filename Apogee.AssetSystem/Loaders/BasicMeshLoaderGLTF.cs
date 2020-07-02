using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using Apogee.Core;
using Apogee.Core.Animations;
using Apogee.Core.Math;
using Apogee.Core.Resources;
using SharpGLTF.Animations;
using SharpGLTF.Schema2;
using SharpGLTF.Transforms;
using Animation = SharpGLTF.Schema2.Animation;
using Quaternion = Apogee.Core.Math.Quaternion;

namespace Apogee.AssetSystem.Loaders
{
    public class BasicMeshLoaderGLTF : IAssetLoader
    {
        public IAsset LoadFromFile(RawAssetSource source)
        {
            var re = new BasicMesh();

            var model = SharpGLTF.Schema2.ModelRoot.Load(source.File);

            var indecies = new List<List<uint>>();
            var vertexes = new List<Vertex>();

            var root = model.LogicalNodes.First(x => x.VisualParent == null);
            var JointsNames = new List<string>();
            var rcount = 1;

            foreach (var node in model.LogicalNodes)
            {
                JointsNames.Add(node.Name);
                Console.WriteLine(node.Name);
            }

            Joint iterate(Node top)
            {
                if (top.Name == null) top.Name = rcount.ToString();
                var re = new Joint();

                re.Name = top.Name;
                // JointsNames.Add(top.Name);

                var jts = new List<int>();

                foreach (var skin in model.LogicalSkins)
                {
                    foreach (var f in skin.GetType().GetRuntimeFields())
                    {
                        if (f.Name == "_joints")
                        {
                            jts = (List<int>) f.GetValue(skin);

                            /*File.WriteAllLines("lol.txt",
                                jts.ConvertAll<string>((x) => x.ToString())
                                    .ToArray());*/
                            break;
                        }
                    }
                }

                re.LocalBindTransform =
                    new Matrix4F().InitIdentity().InitTranslation(
                        top.LocalTransform.Translation.X,
                        top.LocalTransform.Translation.Y,
                        top.LocalTransform.Translation.Z) *
                    new Quaternion(
                        top.LocalTransform.Rotation.X,
                        top.LocalTransform.Rotation.Y,
                        top.LocalTransform.Rotation.Z,
                        top.LocalTransform.Rotation.W).ToRotationMatrix() *
                    new Matrix4F().InitIdentity().InitScale(
                        top.LocalTransform.Scale.X,
                        top.LocalTransform.Scale.Y,
                        top.LocalTransform.Scale.Z);

                var accessor = model.LogicalSkins.First().GetInverseBindMatricesAccessor();
                var ars = accessor.AsMatrix4x4Array();

                re.InverseBindTransform =
                    new Matrix4F(
                ars[jts.IndexOf(model.LogicalNodes.ToList().IndexOf(top))]);

                foreach (var child in top.VisualChildren)
                {
                    re.Children.Add(iterate(child));
                    rcount++;
                }

                return re;
            }

            var jointRoot = iterate(root);
            var ColourValues =
                new string[]
                {
                    "FF0000", "00FF00", "0000FF", "FFFF00", "FF00FF", "00FFFF", "000000",
                    "800000", "008000", "000080", "808000", "800080", "008080", "808080",
                    "C00000", "00C000", "0000C0", "C0C000", "C000C0", "00C0C0", "C0C0C0",
                    "400000", "004000", "000040", "404000", "400040", "004040", "404040",
                    "200000", "002000", "000020", "202000", "200020", "002020", "202020",
                    "600000", "006000", "000060", "606000", "600060", "006060", "606060",
                    "A00000", "00A000", "0000A0", "A0A000", "A000A0", "00A0A0", "A0A0A0",
                    "E00000", "00E000", "0000E0", "E0E000", "E000E0", "00E0E0", "E0E0E0",
                };
            foreach (var mesh in model.LogicalMeshes)
            {
                foreach (var primitive in mesh.Primitives)
                {
                    indecies.Add(primitive.GetIndices().ToList());


                    var accessors = primitive.WithIndicesAutomatic(PrimitiveType.TRIANGLES);

                    foreach (var (name, accessor) in accessors.VertexAccessors)
                    {
                        Console.WriteLine(name + ": " + accessor.Count);
                        switch (accessor.Dimensions)
                        {
                            case DimensionType.SCALAR:
                                break;
                            case DimensionType.VEC2:
                            {
                                var list = accessor.AsVector2Array();
                                for (var i = 0; i < list.Count; i++)
                                {
                                    var vector2 = list[i];

                                    if (vertexes.Count < i)
                                    {
                                        vertexes.Add(new Vertex());
                                    }

                                    if (name == "TEXCOORD_0")
                                    {
                                        vertexes[i].TexCoord = new Vector2F(vector2.X, vector2.Y);
                                        var c = primitive.Material.Channels.First().Parameter;

                                        //vertexes[i].Color = new Vector3F(c.X, c.Y, c.Z);
                                    }
                                }

                                break;
                            }
                            case DimensionType.VEC3:
                            {
                                var list = accessor.AsVector3Array();
                                for (var i = 0; i < list.Count; i++)
                                {
                                    var vector3 = list[i];

                                    if (vertexes.Count <= i)
                                    {
                                        vertexes.Add(new Vertex());
                                    }

                                    if (name == "POSITION")
                                    {
                                        vertexes[i].Position = new Vector3F(vector3.X, vector3.Y, vector3.Z);
                                    }

                                    if (name == "NORMAL")
                                    {
                                        vertexes[i].Normal = new Vector3F(vector3.X, vector3.Y, vector3.Z);
                                    }
                                }
                            }
                                break;
                            case DimensionType.VEC4:
                            {
                                var list = accessor.AsVector4Array();
                                for (var i = 0; i < list.Count; i++)
                                {
                                    var vector4 = list[i];

                                    if (vertexes.Count <= i)
                                    {
                                        vertexes.Add(new Vertex());
                                    }


                                    if (name == "JOINTS_0")
                                    {
                                        var jts = new List<int>();

                                        foreach (var skin in model.LogicalSkins)
                                        {
                                            foreach (var f in skin.GetType().GetRuntimeFields())
                                            {
                                                if (f.Name == "_joints")
                                                {
                                                    jts = (List<int>) f.GetValue(skin);

                                                    /*File.WriteAllLines("lol.txt",
                                                        jts.ConvertAll<string>((x) => x.ToString())
                                                            .ToArray());*/
                                                    break;
                                                }
                                            }
                                        }

                                        var c = Color.FromArgb(
                                            int.Parse(ColourValues[jts[(int) vector4.X] + 1],
                                                System.Globalization.NumberStyles.HexNumber));

                                        /*c = Color.FromArgb(
                                           int.Parse(
                                               ColourValues[(int) vector4.X],
                                               System.Globalization.NumberStyles.HexNumber));*/
                                        var j = model.LogicalNodes[(int) vector4.X];


                                        // if (JointsNames.IndexOf("Head") == (int) vector4.X)
                                        {
                                            vertexes[i].Color = new Vector3F(
                                                c.R / 255f,
                                                c.G / 255f,
                                                c.B / 255f
                                            );
                                        }

                                        /*
                                        Console.WriteLine(
                                            $"[Loader] Mapping: {model.LogicalNodes[(int) vector4.X].Name} to {JointsNames.IndexOf(model.LogicalNodes[(int) vector4.X].Name)}");
                                            */

                                        vertexes[i].JointIDS = new Vector3I(
                                            jts[(int) vector4.X],
                                            jts[(int) vector4.Y],
                                            jts[(int) vector4.Z]
                                        );
                                    }

                                    if (name == "WEIGHTS_0")
                                    {
                                        vertexes[i].JointWeights = new Vector3F(vector4.X, vector4.Y, vector4.Z);
                                        //vertexes[i].JointWeights = new Vector3F(0, vector4.Y,  0);

                                        /*vertexes[i].Color.X *= vector4.X;
                                        vertexes[i].Color.Y *= vector4.Y;
                                        vertexes[i].Color.Z *= vector4.Z;*/

                                        /*if (vertexes[i].JointIDS.X == 0 && vector4.X > 0)
                                            Console.WriteLine("index zero id: " + JointsNames[vertexes[i].JointIDS.X]);
                                        if (vertexes[i].JointIDS.Y == 0 && vector4.X > 0)
                                            Console.WriteLine("index zero id: " + JointsNames[vertexes[i].JointIDS.Y]);
                                        if (vertexes[i].JointIDS.Z == 0 && vector4.X > 0)
                                            Console.WriteLine("index zero id: " + JointsNames[vertexes[i].JointIDS.Z]);*/
                                    }
                                }
                            }
                                break;
                            case DimensionType.MAT2:
                                break;
                            case DimensionType.MAT3:
                                break;
                            case DimensionType.MAT4:
                                break;
                        }
                    }
                }
            }


            foreach (var animation in model.LogicalAnimations)
            {
                var ani = new Apogee.Core.Animations.Animation();

                var frames = animation.FindTranslationSampler(root.VisualChildren.First())
                    .GetLinearKeys().Count() - 1;

                ani.Length = animation.Duration;
                ani.Name = animation.Name;


                var stepsize = ani.Length / frames;

                for (float f = 0; f < frames; f++)
                {
                    var i = stepsize * f;

                    var keyframe = new KeyFrame();

                    keyframe.TimeStamp = i;

                    AffineTransform GetLocalTransform(Animation ani, Node node, float time)
                    {
                        AffineTransform localTransform = node.LocalTransform;
                        var curveSampler1 = ani.FindScaleSampler(node)?.CreateCurveSampler(false);
                        var curveSampler2 = ani.FindRotationSampler(node)?.CreateCurveSampler(false);
                        var curveSampler3 = ani.FindTranslationSampler(node)?.CreateCurveSampler(false);
                        /*if (curveSampler1 != null) localTransform.Scale = curveSampler1.GetPoint(time);
                        if (curveSampler2 != null) localTransform.Rotation = curveSampler2.GetPoint(time);
                        if (curveSampler3 != null) localTransform.Translation = curveSampler3.GetPoint(time);*/


                        return localTransform;
                    }

                    foreach (var name in JointsNames)
                    {
                        if (name == "Head") Debugger.Break();
                        var node = model.LogicalNodes.First(x => x.Name == name);

                        //  var t = ani.Length / i + 0.000001f;


                        var scaleSampler = animation.FindScaleSampler(node);
                        var rotationSampler = animation.FindRotationSampler(node);
                        var translationSampler = animation.FindTranslationSampler(node);

                        var scales = scaleSampler?.GetLinearKeys().ToArray();
                        var rotates = rotationSampler?.GetLinearKeys().ToArray();
                        var translates = translationSampler?.GetLinearKeys().ToArray();

                        var samplerTransform = GetLocalTransform(animation, node, i);


                        var transform = new JointTransform(
                            new Vector3F(
                                samplerTransform.Translation.X,
                                samplerTransform.Translation.Y,
                                samplerTransform.Translation.Z
                            ),
                            new Quaternion(
                                rotates == null ? samplerTransform.Rotation.X : rotates[(int) f].Item2.X,
                                rotates == null ? samplerTransform.Rotation.Y : rotates[(int) f].Item2.Y,
                                rotates == null ? samplerTransform.Rotation.Z : rotates[(int) f].Item2.Z,
                                rotates == null ? samplerTransform.Rotation.W : rotates[(int) f].Item2.W
                            ),
                            new Vector3F(
                                scales == null ? samplerTransform.Scale.X : scales[(int) f].Item2.X,
                                scales == null ? samplerTransform.Scale.Y : scales[(int) f].Item2.Y,
                                scales == null ? samplerTransform.Scale.Z : scales[(int) f].Item2.Z
                            ));

                        keyframe.Pose[name] = transform;
                    }

                    ani.KeyFrames.Add(keyframe);
                }


                re.Animation = ani;
            }

            if (rcount > 0)
            {
                re.JointsNames = JointsNames;
                re.Load(jointRoot, rcount);


                // re.Animation = animation;
                re.Animator.DoAnimation(re.Animation);
                Console.WriteLine(re.Animation.Name);
            }

            re.Load(vertexes, indecies);
            return re;
        }
    }
}