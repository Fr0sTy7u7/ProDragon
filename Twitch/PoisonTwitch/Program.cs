using EnsoulSharp;
using EnsoulSharp.SDK;

namespace PoisonTwitch
{
    class Program
    {
        static void Main(string[] args)
        {
            GameEvent.OnGameLoad += OnGameLoad;
        }

        private static void OnGameLoad()
        {
            if (ObjectManager.Player.CharacterName != "Twitch")
            {
                return;

            }
            Twitch.OnLoad();
        }
    }
}
