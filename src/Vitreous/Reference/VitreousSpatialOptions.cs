
using Xylem.Data;

namespace Vitreous.Reference
{
    public static class SpatialOptions
    {
        public static readonly ValueOptionPackage SpatialValues = OptionPackage.Import<ValueOptionPackage>("vitreous_spatial_options");
    
        public static int GridWidth => SpatialValues.Get<int>("grid_width", 48);
        public static int GridHeight => SpatialValues.Get<int>("grid_height", 32);
        public static int TileWidth => SpatialValues.Get<int>("tile_width", 24);
        public static int TileHeight => SpatialValues.Get<int>("tile_height", 24);

        public static int SectorGridWidth => SpatialValues.Get<int>("sector_grid_width", 48);
        public static int SectorGridHeight => SpatialValues.Get<int>("sector_grid_height", 32);
        public static int SectorAreaWidth => SpatialValues.Get<int>("sector_area_width", 48);
        public static int SectorAreaHeight => SpatialValues.Get<int>("sector_area_height", 48);
    }
}