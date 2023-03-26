
using Xylem.Vectors;
using Vitreous.Component.Composite;
using Vitreous.Component.Spatial;

namespace Vitreous.Component.Utility
{
    public class AreaComparators
    {
        public static bool CompareStructures(Area area, Vec2i a, Vec2i b)
        {
            Structure sa = area.GetStructure(a);
            Structure sb = area.GetStructure(b);

            return sa.Reference == sb.Reference;
        }

        // Summary:
        //      Determines whether a structure has an extension that connects it to
        //      another structure.
        public static bool CompareStructureExtensions(Area area, Vec2i structureVec, Vec2i extensionVec)
        {
            Structure structure = area.GetStructure(structureVec);
            Structure extension = area.GetStructure(extensionVec);

            if (structure.IsEmpty || extension.IsEmpty)
                return false;

            Direction dir = Direction.Of(extensionVec - structureVec);

            return structure.Reference.HasExtraConnection(extension.Identifier) && extension.ConnectionAllowed(dir);
        }

        public static bool CompareFloors(Area area, Vec2i a, Vec2i b)
        {
            Floor sa = area.GetFloor(a);
            Floor sb = area.GetFloor(b);

            return sa.Reference == sb.Reference;
        }
    }
}