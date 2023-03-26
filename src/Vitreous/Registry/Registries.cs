
using Xylem.Interaction;
using Xylem.Registration;
using Xylem.Reflection;

using Vitreous.Reference;
using Vitreous.Component.Core;
using Vitreous.Component.Composite;
using Vitreous.Component.Spatial;

namespace Vitreous.Registration
{
    [ModuleRegistryInitializer("Vitreous")]
    public static class Registries
    {
        public static readonly Registry<AttributeEntry> Attributes;
        public static readonly Registry<StateEntry> States;
        public static readonly Registry<QualifierEntry> Qualifiers;

        public static readonly Registry<PartEntry> Parts;
        public static readonly Registry<ElementEntry> Elements;

        public static readonly Registry<StructureEntry> Structures;
        public static readonly Registry<FloorEntry> Floors;

        public static readonly Registry<AreaType> AreaTypes;
        public static readonly Registry<Area> Areas;

        public static readonly Registry<Sector> Sectors;

        static Registries()
        {
            Attributes = new Registry<AttributeEntry>("Data\\Attributes", VK.Attribute);
            Qualifiers = new Registry<QualifierEntry>("Data\\Qualifiers", VK.Qualifier);
            States = new Registry<StateEntry>("Data\\States", VK.State);

            Parts = new Registry<PartEntry>("Data\\Composites\\Parts", VK.Part);
            Elements = new Registry<ElementEntry>("Data\\Composites\\Elements", VK.Element);

            Structures = new Registry<StructureEntry>("Data\\Composites\\Structures", VK.Structure);
            Floors = new Registry<FloorEntry>("Data\\Composites\\Floors", VK.Floor);

            AreaTypes = new Registry<AreaType>("Data\\Spatial\\AreaTypes", VK.AreaType);
            Areas = new Registry<Area>("Data\\Spatial\\Areas", VK.Area);

            Sectors = new Registry<Sector>("Data\\Spatial\\Sectors", VK.Sector);
        }

        public static void Initialize() {}
    }
}