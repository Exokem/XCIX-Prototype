
using Newtonsoft.Json.Linq;
using Microsoft.Xna.Framework;
using Xylem.Vectors;
using Xylem.Graphics.Patchwork;
using Xylem.Reference;
using Vitreous.Registration;
using Vitreous.Reference;
using Vitreous.Component.Composite;
using Vitreous.Component.Utility;

namespace Vitreous.Component.Spatial
{
    public class Tile : JsonComposite
    {
        public Floor Floor = Floor.Empty;
        public Structure Structure = Structure.Empty;
        

        public bool Invalidated { get; set; } = true;
        public bool InvalidateAdjacencies { get; set; } = false;

        public readonly FunctionalConnectionProcessor StructureConnectionProcessor;
        public readonly FunctionalConnectionProcessor FloorConnectionProcessor;
        public readonly OrnateConnectionProcessor<Structure> StructureExtraConnectionProcessor;

        public Tile() 
        {
            StructureConnectionProcessor = new ConnectionProcessor();
            FloorConnectionProcessor = new ConnectionProcessor();
            StructureExtraConnectionProcessor = new ExtraConnectionProcessor<Structure>();
        }

        public Tile(JObject data) : this()
        {
            if (data.ContainsKey(VK.Floor) && data[VK.Floor] is JObject floorData)
            {
                string floorRef = (string) floorData[K.Reference];

                Floor = new Floor(Registries.Floors[floorRef]);
                Floor.ImportExtraData(floorData);
            }

            if (data.ContainsKey(VK.Structure) && data[VK.Structure] is JObject structureData)
            {
                // Create a new Structure using the reference identifier
                string structureRef = (string) structureData[K.Reference];

                Structure = new Structure(Registries.Structures[structureRef]);

                // Then import extra data to overwrite attribute/state/qualifier values
                Structure.ImportExtraData(structureData);
            }
        }

        public void UpdateConnections(Area area, Vec2i position)
        {
            StructureConnectionProcessor.Process(position, (a, b) => AreaComparators.CompareStructures(area, a, b));
            if (Structure.Reference.HasAnyExtraConnections())
                StructureExtraConnectionProcessor.Process(position, (a, b) => AreaComparators.CompareStructureExtensions(area, a, b), area.GetStructure);
            if (!Floor.IsEmpty)
                FloorConnectionProcessor.Process(position, (a, b) => AreaComparators.CompareFloors(area, a, b));
        }

        public override void Export(JObject data)
        {
            data[VK.Structure] = Structure?.Export();
            data[VK.Floor] = Floor?.Export();
        }

        public void RenderWithin(Rectangle area)
        {
            Floor?.Reference?.RenderWithin(area, FloorConnectionProcessor.Connections());
            Structure.Reference.RenderWithin(area, Structure, StructureConnectionProcessor.Connections(), StructureExtraConnectionProcessor.Connections());
        }
    }
}