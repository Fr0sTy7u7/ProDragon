using System;
using EnsoulSharp.SDK;

namespace PerfectDarius
{
    class Program
    {
        static void Main(string[] args)
        {
            GameEvent.OnGameLoad += Game_OnGameLoad;
        }

        static void Game_OnGameLoad()
        {
            new PerfectDarius();
        }
    }
}
