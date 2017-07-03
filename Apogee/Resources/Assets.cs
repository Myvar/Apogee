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

        public Scene LoadScene(string file)
        {
            var c = CodeLoader.GetClass(
                File.ReadAllText(Path.Combine(GameDirectory, file)),
                typeof(Scene));
            
            return c as Scene;
        }
    }
}