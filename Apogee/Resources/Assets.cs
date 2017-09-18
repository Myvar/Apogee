using System;
using System.Collections.Generic;
using System.IO;
using ImageSharp;

namespace Apogee.Resources
{
    public class Assets
    { 
        public string GameDirectory { get; set; }
        
        public Assets(GameEngine ge)
        {
            GameDirectory = ge.GameDirectory;
        }

        /// <summary>
        /// Load any resouve
        /// </summary>
        /// <param name="file">the resource file name</param>
        /// <typeparam name="T">the IResource</typeparam>
        /// <returns>the asset</returns>
        public T Load<T>(string file)  where T : IResource, new()
        {
            if (typeof(T) == typeof(Scene))
            {
                return (T)LoadScene(file);
            }
            
            if (typeof(T) == typeof(Model))
            {
                return (T)Model.LoadModel(Path.Combine(GameDirectory, file));
            }
            
            if (typeof(T) == typeof(Texture))
            {
                return (T)Texture.LoadTexture(Path.Combine(GameDirectory, file));
            }

            return new T();
        }

        /// <summary>
        /// Loads a scene from an cs file
        /// </summary>
        /// <param name="file">the cs file</param>
        /// <returns>the scene</returns>
        public IResource LoadScene(string file)
        {
            var c = CodeLoader.GetClass(
                File.ReadAllText(Path.Combine(GameDirectory, file)),
                typeof(Scene));
            
            return c as Scene;
        }

        public List<string> IndexModels()
        {
            var outp = new List<string>();
            IterateDirectory(new List<string>()
            {
                ".ogex"
            }, GameDirectory, outp);

            return outp;
        }
        
        public List<string> IndexTextures()
        {
            var outp = new List<string>();
            IterateDirectory(new List<string>()
            {
                ".png",
                ".jpg",
            }, GameDirectory, outp);

            return outp;
        }

        private void IterateDirectory(List<string> extentions, string dir, List<string> outp)
        {
            foreach (var file in Directory.GetFiles(dir))
            {
                foreach (var extention in extentions)
                {
                    if (file.EndsWith(extention))
                    {
                        outp.Add(file);
                        break;
                    }
                }
            }

            foreach (var directory in Directory.GetDirectories(dir))
            {
                IterateDirectory(extentions, directory, outp);
            }
        }
    }
}