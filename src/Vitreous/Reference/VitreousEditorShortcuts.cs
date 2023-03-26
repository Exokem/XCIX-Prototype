
using Microsoft.Xna.Framework.Input;
using Xylem.Data;
using Xylem.Input;
using Xylem.Reflection;

namespace Vitreous.Reference
{
    [ModuleOptions("Vitreous")]
    public static class VES 
    {
        public static readonly ValueOptionPackage VitreousEditorShortcuts = OptionPackage.Import<ValueOptionPackage>("vitreous_editor_shortcuts");

        public static Keys LoadKey(string entry)
        {
            return KeyLookup.FromName(VitreousEditorShortcuts.Get<string>(entry));
        }

        public static InputClaim PlaceStructure = new InputClaim(LoadKey("structure_placement_tool"), clicked: true)
        {
            ExactModifiers = true
        };
        public static InputClaim PlaceFloor = new InputClaim(LoadKey("floor_placement_tool"), clicked: true)
        {
            ExactModifiers = true
        };
        public static InputClaim AddElement = new InputClaim(LoadKey("element_add_tool"), clicked: true)
        {
            ExactModifiers = true
        };
        public static InputClaim InspectTile = new InputClaim(LoadKey("tile_inspect_tool"), clicked: true)
        {
            ExactModifiers = true
        };
    }
}