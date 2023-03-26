
using System;
using Xylem.Framework;
using Xylem.Graphics;
using Xylem.Component;
using Vitreous.Reference;

namespace Vitreous.Framework.Editor
{
    public static class EditorRulers
    {
        public static void RenderRulers(Frame frame, int gridWidth, int gridHeight, int grx, int gry, int spaceWidth, int spaceHeight)
        {
            int scale = GraphicsConfiguration.PixelScale;

            Typeface rulerType = Xylem.Reference.XylemOptions.DefaultTypeface;

            int rulerDimension = Math.Max((int) rulerType.StringWidth("12") + 2 * scale, (int) rulerType.CharHeight());

            Pixel.FillRect(frame.CX, frame.CY, rulerDimension, frame.CH, VFK.EditorRulerBackground);
            Pixel.FillRect(frame.CX, frame.CY, frame.CW, rulerDimension, VFK.EditorRulerBackground);

            for (int x = 0; x < gridWidth; x ++)
            {
                int rx = grx + (x * spaceWidth);
                Pixel.FillRect(rx, frame.CY, spaceWidth, rulerDimension, VFK.EditorRulerMidground);
                Pixel.FillRect(rx, frame.CY, 1, rulerDimension, VFK.EditorRulerForeground);
                Text.RenderTextAt(rulerType, $"{x}", rx + scale, frame.CY, color: VFK.EditorRulerForeground);
            }

            for (int y = 0; y < gridHeight; y ++)
            {
                int ry = gry + (y * spaceHeight);
                Pixel.FillRect(frame.CX, ry, rulerDimension, spaceHeight, VFK.EditorRulerMidground);
                Pixel.FillRect(frame.CX, ry, rulerDimension, 1, VFK.EditorRulerForeground);
                Text.RenderTextAt(rulerType, $"{y}", frame.CX + scale, ry + scale, color: VFK.EditorRulerForeground);
            }

            Pixel.FillRect(frame.CX, frame.CY, rulerDimension, rulerDimension, VFK.EditorRulerBackground);
        }
    }
}