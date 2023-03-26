
using System;
using Microsoft.Xna.Framework;
using Xylem.Data;

namespace Xylem.Reference
{
    public static class XFK
    {
        public static readonly ColorPackage FrameworkColors = OptionPackage.Import<ColorPackage>("framework_colors");

        public static Color Color(string key) => FrameworkColors[key];
        public static Color ColorOr(string key, Color defaultColor) => ColorOr(FrameworkColors, key, defaultColor);
        public static Color ColorOr(ColorPackage package, string key, Color defaultColor)
        {
            Color color = defaultColor; 

            if (package.Has(key))
                color = package[key];

            if (color == null)
                color = defaultColor;

            return color;
        }

        public static Color PrimaryBorder => Color("primary_border");
        public static Color PrimaryText => Color("primary_text");

        public static Color PrimaryAccent => Color("primary_accent"); // Light (Hover)
        public static Color SecondaryAccent => Color("secondary_accent"); // Dark

        public static Color Primary => Color("primary"); // Bright
        public static Color Secondary => Color("secondary"); // Light
        public static Color Tertiary => Color("tertiary"); // Dim
        public static Color Quaternary => Color("quaternary"); // Dark
        
        public static Color ListFrameLabelBackground =
            ColorOr("listframe_label_background", Primary);
        public static Color ListFrameItemAccent =
            ColorOr("listframe_item_accent", SecondaryAccent);

        public static Color ButtonBackground =
            ColorOr("button_background", Primary);
        public static Color ButtonBackgroundActivated =
            ColorOr("button_background_activated", SecondaryAccent);
        public static Color ButtonBackgroundHovered =
            ColorOr("button_background_hovered", PrimaryAccent);

        public static Color ButtonText =
            ColorOr("button_background", PrimaryText);
        public static Color ButtonTextActivated =
            ColorOr("button_background_activated", Secondary);
        public static Color ButtonTextHovered =
            ColorOr("button_background_hovered", SecondaryAccent);

        public static Color SwitchButtonBackgroundSelected =
            ColorOr("switch_button_background_selected", SecondaryAccent);
        public static Color SwitchButtonTextSelected =
            ColorOr("switch_button_text_selected", Secondary);

        public static Color TextInputTextBackgroundHovered = 
            ColorOr("textinput_text_background_hovered", PrimaryAccent);
        public static Color TextInputTextBackground = 
            ColorOr("textinput_text_background", Primary);
        public static Color TextInputBackground =
            ColorOr("textinput_background", Secondary);
        public static Color TextInputCursor =
            ColorOr("textinput_cursor", SecondaryAccent);

        public static Color IconColor = ColorOr("icon_color", PrimaryText);

        public static Color IconButtonBackground =
            ColorOr("iconbutton_background", Primary);
        public static Color IconButtonBackgroundActivated =
            ColorOr("iconbutton_background_activated", SecondaryAccent);
        public static Color IconButtonBackgroundHovered =
            ColorOr("iconbutton_background_hovered", PrimaryAccent);

        public static Color IconButtonIconActivated =
            ColorOr("iconbutton_icon_activated", Secondary);
        public static Color IconButtonIconHovered =
            ColorOr("iconbutton_icon_hovered", SecondaryAccent);

        public static Color TooltipBase = ColorOr("tooltip_base", Secondary);
        public static Color TooltipBackground = ColorOr("tooltip_background", PrimaryAccent);
        public static Color TooltipBorder = ColorOr("tooltip_border", SecondaryAccent);
        public static Color TooltipText = ColorOr("tooltip_text", SecondaryAccent);
    }
}