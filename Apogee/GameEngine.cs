using System;
using System.IO;
using Apogee.Resources;

namespace Apogee
{
    public class GameEngine
    {
        public string GameDirectory { get; set; }
        public Assets Assets { get; set; }

        /// <summary>
        /// Init the GameEngine
        /// </summary>
        /// <param name="gd">The Directory containing the game data</param>
        public GameEngine(string gd)
        {
            GameDirectory = Path.GetFullPath(gd);
            
            //init
            Terminal.Debug($"GameDirectory: {GameDirectory}");
            Terminal.Debug($"WorkingDirectory: {Directory.GetCurrentDirectory()}");
            
            Assets = new Assets(this);
        }

        /// <summary>
        /// Use this Method to start the game
        /// </summary>
        /// <returns></returns>
        public GameEngine Start()
        {
            Terminal.Log("Starting Game");
            
            //testing
            var s = Assets.LoadScene("mainscene.cs");
            s.Update();
            
            return this;
        }
    }
}