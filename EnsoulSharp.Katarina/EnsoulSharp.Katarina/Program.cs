namespace EnsoulSharp.Katarina
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
            EnsoulSharp.SDK.Events.Tick.OnTick += DelayTime;
            Menu tick = new Menu("tick", "Tick Per Second", true);
            tickpersecond.ValueChanged += onTickChange;
            tick.Add(tickpersecond);
            tick.Add(new Menu("notice", "Decrease it will make script work better but you also has high chance get disconnect issues"));
            tick.Add(new Menu("notice2", "It should is higher than 30, increase it if you get disconnect issues"));
            tick.Attach();
            if (ObjectManager.Player.CharacterName != "Katarina")
                return;
            Katarina.OnLoad();
            Game.Print("<font color = '#FFFFFF' > [Nicky -> Katarina]");
            Game.Print("<font color = '#FFFFFF' > Thanks for using..!");
        }
        private static void onTickChange(object sender, EventArgs e)
        {
            EnsoulSharp.SDK.Events.Tick.TickPreSecond = tickpersecond.Value;
        }
    }
}
