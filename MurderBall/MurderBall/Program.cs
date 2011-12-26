using System;

namespace MurderBall
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (MurderBallGame game = new MurderBallGame())
            {
                game.Run();
            }
        }
    }
#endif
}

