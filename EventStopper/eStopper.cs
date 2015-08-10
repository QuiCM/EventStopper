using System;
using System.IO;
using System.Reflection;
using TShockAPI;

using Terraria;
using TerrariaApi.Server;
using TShockAPI.Hooks;

namespace EventStopper
{
	[ApiVersion(1, 21)]
	public class EStopper : TerrariaPlugin
	{
		private static Config Config { get; set; }

		public override string Author { get { return "White"; } }
		public override string Description { get { return "Attempts to stop config defined events when they start"; } }
		public override string Name { get { return "EventStopper"; } }
		public override Version Version { get { return Assembly.GetExecutingAssembly().GetName().Version; } }

		public EStopper(Main game)
			: base(game)
		{
			Order = 1;
			Config = new Config();
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
			GeneralHooks.ReloadEvent += OnReload;
		}

		private void OnReload(ReloadEventArgs e)
		{
			try
			{
				var configPath = Path.Combine(TShock.SavePath, "EventStop.json");
				(Config = Config.Read(configPath)).Write(configPath);

				e.Player.SendSuccessMessage("Reloaded event stopper plugin's configuration");
			}
			catch (Exception x)
			{
				TShock.Log.ConsoleError("Error occured on reloading event stopper plugin's configuration");
				TShock.Log.ConsoleError(x.ToString());
				e.Player.SendErrorMessage("Error occured on reloading event stopper plugin's configuration");
				e.Player.SendErrorMessage(x.Message);
			}
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

			if (Config.disableCultists)
			{
				WorldGen.GetRidOfCultists();
			}

			if (NPC.MoonLordCountdown > 0 && Config.disableLunar)
			{
				NPC.MoonLordCountdown = 0;
				NPC.LunarApocalypseIsUp = false;
				NPC.TowerActiveNebula = false;
				NPC.TowerActiveSolar = false;
				NPC.TowerActiveStardust = false;
				NPC.TowerActiveVortex = false;
			}

			if (Main.invasionType > 0)
			{
				bool sendData = true;
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
					case 4:
						{
							if (Config.disablePumpkinMoon)
							{
								Main.invasionType = 0;
								Main.invasionSize = 0;
							}
							break;
						}
					case 5:
						{
							if (Config.disableSnowMoon)
							{
								Main.invasionType = 0;
								Main.invasionSize = 0;
							}
							break;
						}
					case 6:
						{
							if (Config.disableEclipse)
							{
								Main.invasionType = 0;
								Main.invasionSize = 0;
							}
							break;
						}
					case 7:
						{
							if (Config.disableAliens)
							{
								Main.invasionType = 0;
								Main.invasionSize = 0;
							}
							break;
						}
					default:
						{
							sendData = false;
							break;
						}
				}

				if (sendData)
				{
					TSPlayer.All.SendData(PacketTypes.WorldInfo);
				}
			}
		}
	}
}