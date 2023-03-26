
using System.Collections.Generic;
using System.IO;
using System;
using Newtonsoft.Json.Linq;
using Microsoft.Xna.Framework.Input;
using Xylem.Framework;
using Xylem;
using Xylem.Data;
using Xylem.Reference;
using Xylem.Input;
using Vitreous.Component.Spatial;
using Vitreous.Reference;

namespace Vitreous.Framework.Editor
{
    [Obsolete]
    public sealed partial class AreaEditor : Frame
    {
        public static DirectoryInfo EditorDirectory;
        public static FileInfo EditorAreaFile;
        
        public static IconButtonFactory IconButtonFactory;

        static AreaEditor()
        {
            EditorDirectory = Importer.ExecutionDirectory.Directory("Editor");
            EditorAreaFile = EditorDirectory.File("editor_areas.json");

            IconButtonFactory = new IconButtonFactory();
            IconButtonFactory.IconScale(2F).Border(new SimpleBorder(0, 0, 1, 0));
        }

        internal AreaEditor()
        {
            Borderless = true;

            Resizer = new WidthRatio(secondary: new HeightRatio());

            AssembleViewport();
            AssemblePanel();

            FocusManager.SetFocused(ViewportFrame);

            AddAll(ViewportFrame, Panel, ToolFrame, ComponentSelectionHolder);
        }

        private static int ListItemHeight => (int) (SpatialOptions.TileWidth * 1.5F);

        public static IEnumerable<Area> LoadAreas()
        {
            if (!EditorDirectory.Exists)
                EditorDirectory.Create();
            
            if (!EditorAreaFile.Exists)
                EditorAreaFile.Create();

            JObject data = JObject.Parse(File.ReadAllText(EditorAreaFile.FullName));

            if (data.ContainsKey(K.Entries) && data[K.Entries] is JArray array)
            {
                foreach (var token in array)
                {
                    if (token is JObject obj)
                    {
                        Area area = null;

                        try
                        {
                            area = new Area(obj);
                        }

                        catch (Exception e)
                        {
                            Output.Suggest($"Skipping invalid area definition in '{EditorAreaFile.Name}':\n {e.StackTrace}", "Editor");
                        }

                        if (area != null)
                            yield return area;
                    }
                }
            }
        }

        protected override void UpdateGlobalInputs()
        {
            if (I.CONTROL_S.RequestClaim())
                SaveAreas();

            if (VES.PlaceStructure.RequestClaim())
                SetActiveTool(StructurePlaceTool, StructurePlaceButton);
            else if (VES.PlaceFloor.RequestClaim())
                SetActiveTool(FloorPlaceTool, FloorPlaceButton);
            else if (VES.AddElement.RequestClaim())
                SetActiveTool(ElementAddTool, ElementAddButton);
            else if (VES.InspectTile.RequestClaim())
                SetActiveTool(Inspector, InspectorButton);
        }

        public void SaveAreas()
        {
            if (!EditorDirectory.Exists)
                EditorDirectory.Create();

            JObject data = new JObject();

            data[K.Entries] = J.WriteArray(AreaList.Items());

            File.WriteAllText(EditorAreaFile.FullName, data.ToString());

            NotificationManager.EnqueueNotification("Saved areas");
        }
    }
}