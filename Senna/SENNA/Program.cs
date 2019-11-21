namespace EnsoulSharp.Ashe
{
    using System;
    using EnsoulSharp.SDK;

    internal class Program
    {
        private static void Main(string[] args)
        {
            GameEvent.OnGameLoad += OnGameLoad;
        }

        private static void OnGameLoad()
        {
            if (ObjectManager.Player.CharacterName != "Senna")
                return;
            Senna.OnLoad();

            Game.Print("Senna Loaded");
            Game.Print("Enjoy your game");
        }


    }
}
