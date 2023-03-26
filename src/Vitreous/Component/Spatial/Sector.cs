
using System;
using Newtonsoft.Json.Linq;
using Xylem.Registration;
using Xylem.Reference;
using Vitreous.Reference;
using Vitreous.Registration;

namespace Vitreous.Component.Spatial
{
    public class Sector : RegistryEntry
    {
        public static int GridWidth => SpatialOptions.SectorGridWidth;
        public static int GridHeight => SpatialOptions.SectorGridHeight;

        private readonly Area[,] _areas = new Area[GridWidth, GridHeight];

        public Sector(string identifier) : base()
        {
            Identifier = identifier;
        }

        public Sector(JObject data) : base(data)
        {
            J.ReadGrid(data, VK.Areas, (x, y, areaToken) =>
            {
                if (areaToken is JObject areaData)
                    _areas[x, y] = new Area(areaData);
                else if (areaToken.Type == JTokenType.String) 
                {
                    string identifier = (string) areaToken;

                    if (Registries.Areas.Has(identifier))
                        _areas[x, y] = Registries.Areas[identifier];
                }
            });
        }

        internal void SetIdentifier(string identifier) => Identifier = identifier;

        public override void Export(JObject data)
        {
            base.Export(data);

            J.WriteGrid(data, VK.Areas, _areas, area => area.Export(), GridHeight, GridWidth);
        }

        public override void ExportSlim(JObject data)
        {
            base.Export(data);

            J.WriteGrid(data, VK.Areas, _areas, area => 
            {

                return area == null ? JToken.FromObject(K.Empty) : JToken.FromObject(area.Identifier);
            }, GridHeight, GridWidth);
        }

        public Area this[int x, int y]
        {
            get => ContainsCoordinate(x, y) ? _areas[x, y] : null;
        }

        public bool ContainsCoordinate(int x, int y)
        {
            return x == Math.Clamp(x, 0, GridWidth) && y == Math.Clamp(y, 0, GridHeight);
        }
    }
}