
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Xylem.Functional;

namespace Xylem.Graphics
{
    public static class GraphicsConfiguration
    {
        private static int _width, _height;
        private static readonly HashSet<DualReceiver<int, int>> _resizeReceivers = new HashSet<DualReceiver<int, int>>();

        public static bool Debug;

        static GraphicsConfiguration()
        {
            GraphicsContext.Window.ClientSizeChanged += (sender, args) =>
            {
                _width = GraphicsContext.Device.Viewport.Width;
                _height = GraphicsContext.Device.Viewport.Height;
                foreach (var receiver in _resizeReceivers)
                    receiver(_width, _height);
            };
        }

        internal static void PostInitialize()
        {
            foreach (var receiver in _resizeReceivers)
                receiver(_width, _height);
        }

        public static Rectangle RenderArea => new Rectangle(0, 0, Width, Height);

        public static float Scale
        {
            get 
            {
                float scaleW = ((float) Width / 1920F);
                float scaleH = ((float) Height / 1080F);

                return Math.Min(scaleW, scaleH);
            }
        }

        public static int PixelScale => (int) (Scale * 4F);

        public static int Width
        {
            get => Borderless ? GraphicsContext.Graphics.PreferredBackBufferWidth : _width;
            set 
            {
                _width = value;
                GraphicsContext.Graphics.PreferredBackBufferWidth = value;
            }
        }
        
        public static int Height
        {
            get => Borderless ? GraphicsContext.Graphics.PreferredBackBufferHeight : _height;
            set 
            {
                _height = value;
                GraphicsContext.Graphics.PreferredBackBufferHeight = value;
            }
        }

        public static bool Borderless
        {
            get => GraphicsContext.SourceInstance.Window.IsBorderless;
            set
            {
                GraphicsContext.SourceInstance.Window.IsBorderless = value;

                if (Borderless)
                {
                    GraphicsContext.Graphics.PreferredBackBufferWidth = GraphicsContext.DisplayWidth;
                    GraphicsContext.Graphics.PreferredBackBufferHeight = GraphicsContext.DisplayHeight;
                    GraphicsContext.SourceInstance.Window.Position = Point.Zero;
                    foreach (var receiver in _resizeReceivers)
                        receiver(GraphicsContext.DisplayWidth, GraphicsContext.DisplayHeight);
                }

                else
                {
                    GraphicsContext.Graphics.PreferredBackBufferWidth = _width;
                    GraphicsContext.Graphics.PreferredBackBufferHeight = _height;

                    int x = (GraphicsContext.DisplayWidth - Width) / 2;
                    int y = (GraphicsContext.DisplayHeight - Height) / 2;

                    GraphicsContext.SourceInstance.Window.Position = new Point(x, y);
                    foreach (var receiver in _resizeReceivers)
                        receiver(_width, _height);
                }

                GraphicsContext.Graphics.ApplyChanges();
            }
        }

        public static bool Fullscreen
        {
            get => GraphicsContext.Graphics.IsFullScreen;
            set
            {
                GraphicsContext.Graphics.IsFullScreen = value;

                if (Fullscreen)
                {
                    GraphicsContext.Graphics.PreferredBackBufferWidth = GraphicsContext.DisplayWidth;
                    GraphicsContext.Graphics.PreferredBackBufferHeight = GraphicsContext.DisplayHeight;
                    // GraphicsContext.SourceInstance.Window.Position = Point.Zero;
                }

                else
                {
                    GraphicsContext.Graphics.PreferredBackBufferWidth = _width;
                    GraphicsContext.Graphics.PreferredBackBufferHeight = _height;

                    // int x = (GraphicsContext.DisplayWidth - Width) / 2;
                    // int y = (GraphicsContext.DisplayHeight - Height) / 2;

                    // GraphicsContext.SourceInstance.Window.Position = new Point(x, y);
                }

                GraphicsContext.Graphics.ApplyChanges();
            }
        }

        public static void AddResizeReceiver(DualReceiver<int, int> receiver) => _resizeReceivers.Add(receiver);
    }
}