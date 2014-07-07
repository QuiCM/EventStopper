using System;
using System.IO;
using TShockAPI;

using Terraria;
using TerrariaApi.Server;
using TShockAPI.Hooks;

namespace EventStopper
{
    [ApiVersion(1,16)]
    public class EStopper : TerrariaPlugin
    {
        private static eConfig Config { get; set; }

        public override string Author { get { return "WhiteX"; } }
        public override string Description { get { return "Stops config defined events when they start"; } }
        public override string Name { get { return "EventStopper"; } }
        public override Version Version { get { return new Version(1, 0); } }

        public EStopper(Main game)
            : base(game)
        {
            Order = 1;
            Config = new eConfig();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServerApi.Hooks.GameInitialize.Deregister(this, OnInitialize);
                ServerApi.Hooks.GameUpdate.Deregister(this, OnUpdate);
            }
            base.Dispose(disposing);
        }

        public override void Initialize()
        {
            ServerApi.Hooks.GameInitialize.Register(this, OnInitialize);
            ServerApi.Hooks.GameUpdate.Register(this, OnUpdate);
        }

        private void OnInitialize(EventArgs args)
        {
            Commands.ChatCommands.Add(new Command("estopper.reload", ReloadCom, "ereload")
            {
                HelpText = "Reloads the event stopper plugin's configuration file"
            });
            SetUpConfig(new ReloadEventArgs(TSPlayer.Server));
        }

        private void ReloadCom(CommandArgs args)
        {
            args.Player.SendInfoMessage("Requesting configuration reload");
            SetUpConfig(new ReloadEventArgs(args.Player));
        }

        private static void OnUpdate(EventArgs args)
        {
            if (WorldGen.spawnMeteor && Config.disableMeteors)
                WorldGen.spawnMeteor = false;

            if (Main.moonPhase == 0 && Config.disableFullMoon)
                TSPlayer.Server.SetTime(false, 0.0);

            if (Main.bloodMoon && Config.disableBloodMoon)
                TSPlayer.Server.SetBloodMoon(false);

            if (Main.snowMoon && Config.disableSnowMoon)
                TSPlayer.Server.SetFrostMoon(false);

            if (Main.pumpkinMoon && Config.disablePumpkinMoon)
                TSPlayer.Server.SetPumpkinMoon(false);

            if (Main.eclipse && Config.disableEclipse)
                TSPlayer.Server.SetEclipse(false);

            if (Main.raining && Config.disableRain)
            {
                Main.rainTime = 0;
                Main.raining = false;
                Main.maxRaining = 0f;
            }

            if (Main.invasionType > 0)
                switch (Main.invasionType)
                {
                    case 1:
                        {
                            if (Config.disableGoblinInvasion)
                            {
                                Main.invasionType = 0;
                                Main.invasionSize = 0;
                            }
                            break;
                        }
                    case 2:
                        {
                            if (Config.disableFrostLegion)
                            {
                                Main.invasionType = 0;
                                Main.invasionSize = 0;
                            }
                            break;
                        }
                    case 3:
                        {
                            if (Config.disablePirateInvasion)
                            {
                                Main.invasionType = 0;
                                Main.invasionSize = 0;
                            }
                            break;
                        }
                }
        }

        private static void SetUpConfig(ReloadEventArgs args)
        {
            try
            {
                var configPath = Path.Combine(TShock.SavePath, "EventStop.json");
                (Config = eConfig.Read(configPath)).Write(configPath);

                args.Player.SendSuccessMessage("Reloaded event stopper plugin's configuration");
            }
            catch (Exception x)
            {
                Log.ConsoleError("Error occured on reloading event stopper plugin's configuration");
                Log.ConsoleError(x.ToString());
                args.Player.SendErrorMessage("Error occured on reloading event stopper plugin's configuration");
                args.Player.SendErrorMessage(x.Message);
            }
        }
    }
}
