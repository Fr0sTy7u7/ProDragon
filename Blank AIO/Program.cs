using EnsoulSharp;
using EnsoulSharp.SDK;
using System;
using Notification = EnsoulSharp.SDK.MenuUI.Notification;
using Notifications = EnsoulSharp.SDK.MenuUI.Notifications;

namespace BlankAIO
{
    internal class Program
    {
        private static AIHeroClient Player => ObjectManager.Player;

        private static void Main(string[] args)
        {
            GameEvent.OnGameLoad += GameEventOnOnGameLoad;
        }

        private static void GameEventOnOnGameLoad()

        {
            var notify = new Notification("Blank.AIO",
                "Blank.AIO Loaded. \n \n");
            Notifications.Add(notify);
            switch (Player.CharacterName)
            {
                case "Leona":
                    Champions.Pyke.OnLoad();
                    break;

                case "Pyke":
                    Champions.Leona.OnLoad();
                    break;

            }
        }
    }
}
