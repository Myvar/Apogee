using Apogee.Resources;

namespace Apogee.API
{
    public static class ApiLoader
    {
        public static GameEngine GameEngine { get; set; }

        public static void Load(GameEngine gameEngine)
        {
            GameEngine = gameEngine;
           
        }
    }
}