using System.IO;
using Newtonsoft.Json;

namespace EventStopper
{
    public class Config
    {
        public bool disableFullMoon = false;
        public bool disableFrostMoon = false;
        public bool disableBloodMoon = false;
        public bool disablePumpkinMoon = false;
        public bool disableSolarEclipse = false;
        public bool disableRain = false;
        public bool disableSlimeRain = false;
        public bool disableGoblinInvasion = false;
        public bool disablePirateInvasion = false;
        public bool disableFrostLegion = false;
        public bool disableMeteors = false;
		public bool disableMartianInvasion = false;
		public bool disableLunarInvasion = false;
		public bool disableCultists = false;

        public void Write(string path)
        {
            File.WriteAllText(path, JsonConvert.SerializeObject(this, Formatting.Indented));
        }

        public static Config Read(string path)
        {
            if (!File.Exists(path))
                return new Config();
            return JsonConvert.DeserializeObject<Config>(File.ReadAllText(path));
        }
    }
}
