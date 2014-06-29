using System.IO;
using Newtonsoft.Json;

namespace EventStopper
{
    public class eConfig
    {
        public bool disableFullMoon = false;
        public bool disableSnowMoon = false;
        public bool disableBloodMoon = false;
        public bool disablePumpkinMoon = false;
        public bool disableEclipse = false;
        public bool disableRain = false;
        public bool disableGoblinInvasion = false;
        public bool disablePirateInvasion = false;
        public bool disableFrostLegion = false;
        public bool disableMeteors = false;

        public void Write(string path)
        {
            File.WriteAllText(path, JsonConvert.SerializeObject(this, Formatting.Indented));
        }

        public static eConfig Read(string path)
        {
            if (!File.Exists(path))
                return new eConfig();
            return JsonConvert.DeserializeObject<eConfig>(File.ReadAllText(path));
        }
    }
}
