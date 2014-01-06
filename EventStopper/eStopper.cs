using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using TShockAPI;

using Terraria;
using TerrariaApi;
using TerrariaApi.Server;

namespace EventStopper
{
    [ApiVersion(1,14)]
    public class eStopper : TerrariaPlugin
    {
        public static eConfig config { get; set; }
        public static string configPath { get { return Path.Combine(TShock.SavePath, "EventStop.json"); } }

        public override string Author { get { return "WhiteX"; } }
        public override string Description { get { return "Stops config defined events when they start"; } }
        public override string Name { get { return "EventStopper"; } }
        public override Version Version { get { return new Version(1, 0); } }

        public eStopper(Main game)
            : base(game)
        {
            Order = 1;
            config = new eConfig();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServerApi.Hooks.GameInitialize.Deregister(this, onInitialize);
                ServerApi.Hooks.GameUpdate.Deregister(this, onUpdate);
            }
            base.Dispose(disposing);
        }

        public override void Initialize()
        {
            ServerApi.Hooks.GameInitialize.Register(this, onInitialize);
            ServerApi.Hooks.GameUpdate.Register(this, onUpdate);
        }

        public void onInitialize(EventArgs args)
        {
            Commands.ChatCommands.Add(new Command("estopper.reload", reloadCom, "ereload")
            {
                HelpText = "Reloads the event stopper plugin's configuration file"
            });
            setUpConfig();
        }

        private void reloadCom(CommandArgs args)
        {
            args.Player.SendInfoMessage("Requesting configuration reload");
            setUpConfig(args.Player);
        }

        private void onUpdate(EventArgs args)
        {
            if (Main.moonPhase == 0 && config.disableFullMoon)
                TSServerPlayer.Server.SetFullMoon(false);

            if (Main.bloodMoon && config.disableBloodMoon)
                TSServerPlayer.Server.SetBloodMoon(false);

            if (Main.snowMoon && config.disableSnowMoon)
                TSServerPlayer.Server.SetSnowMoon(false);

            if (Main.pumpkinMoon && config.disablePumpkinMoon)
                TSServerPlayer.Server.SetPumpkinMoon(false);

            if (Main.eclipse && config.disableEclipse)
                TSServerPlayer.Server.SetEclipse(false);

            if (Main.raining && config.disableRain)
                Main.raining = false;

            if (Main.invasionType > 0)
                switch (Main.invasionType)
                {
                    case 1:
                        {
                            if (config.disableGoblinInvasion)
                            {
                                Main.invasionType = 0;
                                Main.invasionSize = 0;
                            }
                            break;
                        }
                    case 2:
                        {
                            if (config.disableFrostLegion)
                            {
                                Main.invasionType = 0;
                                Main.invasionSize = 0;
                            }
                            break;
                        }
                    case 3:
                        {
                            if (config.disablePirateInvasion)
                            {
                                Main.invasionType = 0;
                                Main.invasionSize = 0;
                            }
                            break;
                        }
                }
        }

        private void setUpConfig(TSPlayer player = null)
        {
            try
            {
                if (File.Exists(configPath))
                    config = eConfig.Read(configPath);

                else
                    config.Write(configPath);

                if (player != null)
                    player.SendSuccessMessage("Reloaded event stopper plugin's configuration");
            }
            catch (Exception x)
            {
                Log.ConsoleError("Error occured on reloading event stopper plugin's configuration");
                Log.ConsoleError(x.ToString());
                player.SendErrorMessage("Error occured on reloading event stopper plugin's configuration");
                player.SendErrorMessage(x.Message);
            }
        }
    }
}
