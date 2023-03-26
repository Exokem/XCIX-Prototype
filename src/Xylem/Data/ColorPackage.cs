
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Microsoft.Xna.Framework;
using Xylem.Graphics;

namespace Xylem.Data
{
    public class ColorPackage : OptionPackage
    {
        private readonly Dictionary<string, Color> _entries;

        public ColorPackage() 
        {
            _entries = new Dictionary<string, Color>();
        }

        protected override void ImportEntry(string key, JToken token)
        {
            // if (token is JObject colorData)
            // {
            //     ColorFormat format = ColorFormat.Of(J.ReadString(colorData, K.Format, null));

            //     J.ReadArrayTokens(colorData, K.Value, )
            // }

            short h = 0;
            short s = 100;
            short v = 100;
            short a = 255;

            if (token is JArray values)
            {
                if (0 < values.Count)
                    h = (short) values[0];
                if (1 < values.Count)
                    s = (short) values[1];
                if (2 < values.Count)
                    v = (short) values[2];
                if (3 < values.Count)
                    a = (short) values[3];
            }

            _entries[key] = ColorFormat.FromHSV(h, s, v, a);
        }

        protected override IEnumerable<KeyValuePair<string, JToken>> ExportEntries()
        {
            throw new System.NotImplementedException();
        }

        public Color this[string key] => _entries[key];

        public override bool Has(string key) => _entries.ContainsKey(key);
    }
}