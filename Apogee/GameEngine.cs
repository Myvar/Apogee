using System;
using System.Diagnostics;
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
            
            var st = new Stopwatch();
            
            
            st.Reset();
            st.Start();
            
            //testing
            var s = Assets.Load<Scene>("mainscene.cs");
            
            st.Stop();
            Terminal.Log("Loaded Class Asset: " + st.ElapsedMilliseconds.ToString());
            
            st.Reset();
            st.Start();
            
            
            s.Update();
            
            st.Stop();
            Terminal.Log("Ran cold start Update: " + st.Elapsed.ToString());
            
           

            for (int i = 0; i < 10; i++)
            {
                st.Reset();
                st.Start();
                
                s.Update();
                
                st.Stop();
                Terminal.Log($"Hot Run #{i}: " + st.Elapsed.ToString());
            }
            
            return this;
        }
    }
}