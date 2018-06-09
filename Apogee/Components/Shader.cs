using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Apogee.Core;
using Apogee.Ecs;
using Newtonsoft.Json;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Apogee.Components
{
    public class Shader : Component
    {
        [JsonIgnore]
        public int Program;
        
        private Dictionary<string, int> Uniforms = new Dictionary<string, int>();
        
        public string File { get; set; }

        public void Load(string fileName)
        {
            Program = GL.CreateProgram();

            if (Program == 0)
            {
                Console.WriteLine("Failed to create Shader.");
                Environment.Exit(0);
            }

            GL.BindAttribLocation(Program, 0, "position");
            GL.BindAttribLocation(Program, 1, "color");
            GL.BindAttribLocation(Program, 2, "normal");
            GL.BindAttribLocation(Program, 3, "tangent");
            GL.BindAttribLocation(Program, 4, "textCoord");

            AddVertexShader(System.IO.File.ReadAllText(fileName + ".vs.glsl"));
            AddFragmentShader(System.IO.File.ReadAllText(fileName + ".fs.glsl"));

            CompileShader();
        }


        public void AddUniform(string name)
        {
            int uniformLocation = GL.GetUniformLocation(Program, name);

            if (uniformLocation == -1)
            {
                Console.WriteLine("Failed to find uniform: " + name);
                //Environment.Exit(1);
            }

            Uniforms.Add(name, uniformLocation);
        }

        public virtual void SetUniform(object value)
        {
            var type = value.GetType().GetTypeInfo();
            var name = type.Name;

            foreach (var i in type.GetProperties())
            {
                SetUniform(name + "_" + i.Name, i.GetValue(value));
            }
        }

        public virtual void SetUniform(string name, object value)
        {
            if (!Uniforms.ContainsKey(name))
            {
                AddUniform(name);
            }

            if (value.GetType() == typeof(int))
            {
                GL.Uniform1(Uniforms[name], (int) value);
            }
            else if (value.GetType() == typeof(float))
            {
                GL.Uniform1(Uniforms[name], (float) value);
            }
            else if (value.GetType() == typeof(Vec3))
            {
                var val = (Vec3) value;
                GL.Uniform3(Uniforms[name], val.X, val.Y, val.Z);
            }
            else if (value.GetType() == typeof(Color))
            {
                var val = (Color) value;
                GL.Uniform3(Uniforms[name], val.R + 0.00001f / 255, val.G + 0.00001f / 255, val.B + 0.00001f / 255);
            }
            else if (value.GetType() == typeof(Mat4))
            {
                var val = (Mat4) value;
                var mat4 = new Matrix4(
                    new Vector4(val[0, 0], val[0, 1], val[0, 2], val[0, 3]),
                    new Vector4(val[1, 0], val[1, 1], val[1, 2], val[1, 3]),
                    new Vector4(val[2, 0], val[2, 1], val[2, 2], val[2, 3]),
                    new Vector4(val[3, 0], val[3, 1], val[3, 2], val[3, 3])
                );

                GL.UniformMatrix4(Uniforms[name], true, ref mat4);
            }
        }

        public void Apply()
        {
            GL.UseProgram(Program);
        }

        public void AddVertexShader(string text)
        {
            AddProgram(text, ShaderType.VertexShader);
        }

        public void AddFragmentShader(string text)
        {
            AddProgram(text, ShaderType.FragmentShader);
        }

        public void AddGeometryShader(string text)
        {
            AddProgram(text, ShaderType.GeometryShader);
        }

        public void CompileShader()
        {
            GL.LinkProgram(Program);

            int outs = 0;

            GL.GetProgram(Program, GetProgramParameterName.LinkStatus, out outs);

            if (outs == 0)
            {
                Console.WriteLine(GL.GetProgramInfoLog(Program));
                Environment.Exit(0);
            }


            GL.ValidateProgram(Program);

            GL.GetProgram(Program, GetProgramParameterName.ValidateStatus, out outs);

            if (outs == 0)
            {
                Console.WriteLine(GL.GetProgramInfoLog(Program));
                Environment.Exit(0);
            }
        }

        private void AddProgram(string text, ShaderType type)
        {
            int shader = GL.CreateShader(type);

            if (shader == 0)
            {
                Console.WriteLine("Failed to add Shader.");
                Environment.Exit(0);
            }

            GL.ShaderSource(shader, text);
            GL.CompileShader(shader);

            int outs = 0;

            GL.GetShader(shader, ShaderParameter.CompileStatus, out outs);

            if (outs == 0)
            {
                Console.WriteLine(type.ToString() + ":");
                Console.WriteLine(GL.GetShaderInfoLog(shader));
                Environment.Exit(0);
            }

            GL.AttachShader(Program, shader);
        }

        public void Update(Mat4 Model, Mat4 MVP)
        {
            SetUniform("sampler", 0);
            SetUniform("Model", Model);
            SetUniform("MVP", MVP);
        }
    }
}