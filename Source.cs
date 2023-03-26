using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using System.Text;
using System.Collections.Generic;
using Xylem.Data;
using Xylem.Component;
using Xylem.Graphics;
using Xylem.Framework;
using Xylem.Framework.Layout;
using Xylem.Framework.Control;
using Xylem.Input;
using Xylem.Functional;
using Xylem.Registration;
using Xylem.Reflection;
using Xylem.Reference;

namespace Xylem
{
    public class Source : Game
    {
        public static Source Instance;

        public Source()
        {
            GraphicsContext.Graphics = new GraphicsDeviceManager(this);
            GraphicsContext.SourceInstance = this;
            
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Instance = this;
        }

        protected override void Initialize()
        {
            GraphicsConfiguration.Width = 1920;
            GraphicsConfiguration.Height = 1080;
            GraphicsContext.Graphics.ApplyChanges();

            Window.AllowUserResizing = true;
            Window.Title = "XCIX";

            Output.WriteRaw("\n===== Indexing Modules =====");
            Importer.IndexModules();
            Output.WriteRaw("\n===== Initializing Registries =====");
            Initializer.Initialize<ModuleRegistryInitializer>("registry");
            Output.WriteRaw("\n===== Importing Modules =====");
            Importer.ImportModules();
            Output.WriteRaw("\n===== Initializing Modules =====");
            Initializer.Initialize<ModuleSource>("source");

            Output.WriteRaw("\n===== Initializing Module Options =====");
            Initializer.Initialize<ModuleOptions>("options");

            GraphicsConfiguration.AddResizeReceiver((w, h) =>
            {
                XylemModule.Container.Resize();
            });
            GraphicsConfiguration.PostInitialize();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            GraphicsContext.StartRenderer();

            Window.TextInput += FocusManager.ReceiveTextInput;

            // DialogUpdater.PostDialog(new FormDialog(DialogUpdater.CloseDialog, "Test Dialog", true, "test", "option"));
        }

        private int samples = 0;
        private float totalFrames = 0F;

        private int initialTicks = 0;
        private const int initialResizeLimit = 3;

        protected override void Update(GameTime gameTime)
        {
            if (!IsActive)
                return;

            samples ++;
            totalFrames += 1.0F / (float) gameTime.ElapsedGameTime.TotalSeconds;

            InputProcessor.Update();
            IntervalPublisher<bool>.UpdatePublishers();

            if (InputProcessor.Clicked(Keys.Escape))
                Exit();
            if (InputProcessor.Clicked(Keys.F11))
                GraphicsConfiguration.Borderless = !GraphicsConfiguration.Borderless;
            if (InputProcessor.Clicked(Keys.F10))
                GraphicsConfiguration.Debug = !GraphicsConfiguration.Debug;

            UpdateDispatcher.DispatchUpdates();
            
            // _mainFrame.Resize();
            if (BlockingUpdater.Update())
                XylemModule.Container.Update();

            UpdateDispatcher.ProcessUpdateQueue();
            UpdateDispatcher.ProcessDelayedUpdateQueue();

            NotificationManager.UpdateNotifications();

            if (initialTicks ++ < initialResizeLimit)
                XylemModule.Container.Resize();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            if (!IsActive)
                return;

            GraphicsContext.Device.Clear(Color.Black);
            GraphicsContext.Renderer.Begin(samplerState: SamplerState.PointClamp, blendState: BlendState.AlphaBlend);

            XylemModule.Container.Render();

            int debugHeight = GraphicsConfiguration.Height - 5;
            int debugTextHeight = XylemOptions.DefaultTypeface.ScaledHeight();

            // FPS display
            if (GraphicsConfiguration.Debug)
                Text.RenderTextAt(XylemOptions.DefaultTypeface, $"{totalFrames / samples}", 7, debugHeight - debugTextHeight);

            UpdateDispatcher.ProcessRenderQueue();

            // Text.RenderTextAt(XylemOptions.DefaultTypeface, $"M: {InputProcessor.MDX}, {InputProcessor.MDY}", 7, debugHeight - debugTextHeight * 2);
            // Text.RenderTextAt(Options.InterfaceTypeface, $"Z: {InputProcessor.State(Keys.Z)}", 7, debugHeight - debugTextHeight * 3);

            NotificationManager.RenderNotifications();

            BlockingUpdater.Render();

            GraphicsContext.Renderer.End();

            if (samples == 10)
            {
                samples = 0;
                totalFrames = 0;
            }

            base.Draw(gameTime);
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
            foreach (TextureResource resource in R.Textures.Entries())
            {
                resource.Texture?.Dispose();
            }
        }
    }

    public static class Output
    {
        public static void Write(string output, string module = null)
        {
            Log("INFO", output, module);
        }

        private static void Log(string prefix, string output, string module)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append($"[{prefix}");
            if (module != null)
                builder.Append($"/{module}");
            builder.Append($"] {output}");

            Debug.WriteLine(builder.ToString());
            Console.WriteLine(builder.ToString());
        }

        public static void Suggest(string output, string module = null)
        {
            Log("WARN", output, module);
        }

        public static void Error(string output)
        {
            Debug.Fail($"[ERROR] {output}");
        }

        public static void WriteRaw(string output)
        {
            Debug.WriteLine(output);
            Console.WriteLine(output);
        }
    }
}
