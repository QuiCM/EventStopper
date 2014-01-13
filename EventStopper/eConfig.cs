using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

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

        public static eConfig Read(string path)
        {
            if (!File.Exists(path))
                return new eConfig();
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return Read(fs);
            }
        }

        public static eConfig Read(Stream stream)
        {
            using (var sr = new StreamReader(stream))
            {
                var cf = JsonConvert.DeserializeObject<eConfig>(sr.ReadToEnd());
                if (ConfigRead != null)
                    ConfigRead(cf);
                return cf;
            }
        }

        public void Write(string path)
        {
            using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Write))
            {
                Write(fs);
            }
        }
        public void Write(Stream stream)
        {
            var str = JsonConvert.SerializeObject(this, Formatting.Indented);
            using (var sw = new StreamWriter(stream))
            {
                sw.Write(str);
            }
        }

        public static Action<eConfig> ConfigRead;
    }
}
