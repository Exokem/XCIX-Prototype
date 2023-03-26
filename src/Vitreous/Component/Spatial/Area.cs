
using Newtonsoft.Json.Linq;
using Xylem.Vectors;
using Xylem.Registration;
using Xylem.Reference;
using Vitreous.Component.Composite;
using Vitreous.Reference;
using Vitreous.Registration;

namespace Vitreous.Component.Spatial
{
    public class Area : RegistryEntry
    {
        public static int GridWidth => SpatialOptions.GridWidth;
        public static int GridHeight => SpatialOptions.GridHeight;

        private readonly Tile[,] _tiles = new Tile[GridWidth, GridHeight];

        public readonly AreaType AreaType;

        public Area(string identifier) : base()
        {
            Identifier = identifier;
            AreaType = AreaType.Unknown;

            for (int y = 0; y < GridHeight; y ++)
            {
                for (int x = 0; x < GridWidth; x ++)
                {
                    this[x, y] = new Tile();
                }
            }
        }

        public Area(JObject data) : base(data)
        {
            int y = 0;

            AreaType = Registries.AreaTypes[J.ReadString(data, VK.AreaType, K.Unknown)];

            J.ReadArrayTokens(data, VK.Tiles, rowToken => 
            {
                JArray row = rowToken as JArray;
                int x = 0;

                foreach (JToken tileToken in row)
                {
                    JObject tileData = tileToken as JObject;
                    _tiles[x++, y] = new Tile(tileData);
                }

                y++;
            });

            JArray rows = data[VK.Tiles] as JArray;
        }

        public override void Export(JObject data)
        {
            base.Export(data);

            JArray rows = new JArray();
            for (int y = 0; y < GridHeight; y ++)
            {
                JArray row = new JArray();
                for (int x = 0; x < GridWidth; x ++)
                {
                    row.Add(this[x, y].Export());
                }
                rows.Add(row);
            }

            data[VK.Tiles] = rows;
            data[VK.AreaType] = AreaType.Identifier;
        }

        internal void SetIdentifier(string identifier) => Identifier = identifier;

        public bool ContainsPosition(Vec2i vec) => ContainsPosition(vec.X, vec.Y);
        public bool ContainsPosition(int x, int y)
        {
            return 0 <= x && x < GridWidth && 0 <= y && y < GridHeight;
        }

        public Tile this[Vec2i vec] => this[vec.X, vec.Y];
        // Nonnull
        public Tile this[int x, int y]
        {
            get 
            {
                if (!ContainsPosition(x, y))
                    return null;

                if (_tiles[x, y] == null)
                    _tiles[x, y] = new Tile();

                return _tiles[x, y];
            }

            set 
            {
                if (value != null)
                {
                    _tiles[x, y] = value;
                    value.Invalidated = true;
                }
            }
        }

        public Structure GetStructure(Vec2i v)
        {
            if (ContainsPosition(v))
                return this[v].Structure;
            else 
                return Structure.Empty;
        }

        public void SetStructure(Vec2i v, Structure structure)
        {
            this[v].Structure = structure;
            InvalidateNeighbors(v);
        }

        public Floor GetFloor(Vec2i v)
        {
            if (ContainsPosition(v))
                return this[v].Floor;
            else 
                return Floor.Empty;
        }

        public void SetFloor(Vec2i v, Floor floor)
        {
            this[v].Floor = floor;
            InvalidateNeighbors(v);
        }

        public void InvalidateNeighbors(Vec2i v)
        {
            this[v].InvalidateAdjacencies = false;

            for (int i = v.X - 1; i <= v.X + 1; i ++)
            {
                for (int j = v.Y - 1; j <= v.Y + 1; j ++)
                {
                    Tile state = this[i, j];

                    if (state != null)
                        state.Invalidated = true;
                }
            }
        }

        public bool AddElement(Vec2i v, Element element)
        {
            ElementContainer container = this[v].Structure.Elements;

            int pc = container.Size();
            container.Add(element);

            return container.Size() != pc;
        }

        public bool RemoveElement(Vec2i v, Element element)
        {
            ElementContainer container = this[v].Structure.Elements;

            int pc = container.Size();
            container.Remove(element);

            return container.Size() != pc;
        }
    }
}