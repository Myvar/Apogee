using System;
using System.IO;

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
    }
}