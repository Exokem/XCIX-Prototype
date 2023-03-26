
using Microsoft.Xna.Framework;
using Xylem.Reference;
using Xylem.Data;

namespace Vitreous.Reference
{
    public static class VFK
    {
        public static Color ColorOr(string key, Color defaultColor) => XFK.ColorOr(VitreousFrameworkColors, key, defaultColor);

        public static readonly ColorPackage VitreousFrameworkColors = OptionPackage.Import<ColorPackage>("vitreous_framework_colors");

        static VFK()
        {
        }

        public static Color SectorEditorEmptyArea =
            ColorOr("sector_editor_empty_area", XFK.Tertiary);
        public static Color SectorEditorArea =
            ColorOr("sector_editor_area", XFK.Secondary);

        public static Color EditorRulerBackground = 
            ColorOr("editor_ruler_background", XFK.Tertiary);
        public static Color EditorRulerMidground = 
            ColorOr("editor_ruler_midground", XFK.Secondary);
        public static Color EditorRulerForeground = 
            ColorOr("editor_ruler_foreground", XFK.PrimaryText);

        public static Color EditorTileHighlightBorder = 
            ColorOr("editor_tile_hover_border", XFK.SecondaryAccent);
        
        public static Color EditorTextInputTextBackground =
            ColorOr("editor_textinput_text_background", XFK.Primary);

        public static Color EditorLabelPrimaryBackground = 
            ColorOr("editor_label_primary_background", XFK.Primary);

        public static Color EditorIconListBackground =
            ColorOr("editor_icon_list_background", XFK.Secondary);

        public static Color EditorInspectorBackground =
            ColorOr("editor_inspector_background", XFK.Tertiary);

        public static Color EditorInspectorEntryTitleBackground =
            ColorOr("editor_inspector_entry_title_background", XFK.SecondaryAccent);
        public static Color EditorInspectorEntryTitleText =
            ColorOr("editor_inspector_entry_title_text", XFK.Primary);

        public static Color EditorInspectorElementBackground =
            ColorOr("editor_inspector_element_background", XFK.Secondary);

        public static Color EditorInspectorAttributeBackground =
            ColorOr("editor_inspector_attribute_background", XFK.Secondary);
        public static Color EditorInspectorStateBackground =
            ColorOr("editor_inspector_state_background", XFK.Secondary);
        public static Color EditorInspectorQualifierBackground =
            ColorOr("editor_inspector_qualifier_background", XFK.Secondary);

        public static Color EditorInspectorStateSelectionBackground =
            ColorOr("editor_inspector_state_selection_background", XFK.Secondary);
        public static Color EditorInspectorQualifierValueBackground =
            ColorOr("editor_inspector_qualifier_value_background", XFK.Secondary);

        public static Color EditorInspectorInspectedTileOverlay =
            ColorOr("editor_inspector_inspected_tile_overlay", XFK.Secondary);

        public static Color EditorAreaBackground =
            ColorOr("editor_area_background", XFK.Tertiary);

        public static Color EditorViewportBackground =
            ColorOr("editor_viewport_background", XFK.Quaternary);

        public static Color EditorPanelBackground =
            ColorOr("editor_panel_background", XFK.Secondary);

        public static Color EditorPanelAreaButtonBackground =
            ColorOr("editor_panel_area_button_background", XFK.Primary);

        public static Color EditorPanelListBackground =
            ColorOr("editor_panel_list_background", XFK.Tertiary);
    }
}