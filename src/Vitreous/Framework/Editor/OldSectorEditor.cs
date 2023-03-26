
using System.IO;
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Xylem.Framework;
using Xylem.Data;
using Xylem;
using Xylem.Framework.Layout;
using Xylem.Framework.Control;
using Xylem.Functional;
using Xylem.Reference;
using Xylem.Input;
using Vitreous.Component.Spatial;
using Vitreous.Reference;

namespace Vitreous.Framework.Editor
{
    public sealed partial class OldSectorEditor : Frame
    {
        public static readonly FileInfo SectorEditorFile = AreaEditor.EditorDirectory.File("editor_sectors.json");

        readonly EditorSectorFrame EditorSectorFrame;

        ListFrame<Sector> SectorList;
        ListFrame<Area> AreaList;

        PartitionGridFrame<Frame> Holder;
        HorizontalSplitFrame EditorTabs;
        Button SectorEditorTab, AreaEditorTab;

        VerticalSplitFrame Panel;

        internal OldSectorEditor()
        {
            Borderless = true;

            EditorTabs = new HorizontalSplitFrame()
            {
                Borderless = true
            };

            EditorSectorFrame = new EditorSectorFrame()
            {
                ExpandVertical = true,
                RenderPriority = 99
            };

            Resizer = new WidthRatio(secondary: new HeightRatio());

            Panel = new VerticalSplitFrame()
            {
                HorizontalAlignment = new RightAlignment(),
                BackgroundColor = VFK.EditorPanelBackground,
                SpanRows = 2
            };

            ContextMenu sectorMenu = new ContextMenu()
            {
                IgnoreLayers = false
            };
            sectorMenu.Add("New Sector", PostNewSectorDialog);

            ContextMenu sectorEntryMenu = new ContextMenu();
            sectorEntryMenu.Add("New Sector", PostNewSectorDialog);
            sectorEntryMenu.Add("Rename Sector", PostRenameSectorDialog);

            SectorList = new ListFrame<Sector>("Sectors")
            {
                ExpandHorizontal = true, ExpandVertical = true,
                BackgroundColor = VFK.EditorPanelListBackground,
                ItemHeight = (int) (SpatialOptions.TileWidth * 1.5F),
                AllowDeselection = false,
                OnSelected = SectorSelected,
                ItemConstructor = v => new RegistryEntryItem<Sector>(v)
                {
                    ContextMenu = sectorEntryMenu
                },
                Resizer = new WidthRatio(secondary: new HeightRatio(0.5F))
            };

            SectorList.SetContextMenu(sectorMenu);

            AreaList = new ListFrame<Area>("Areas")
            {
                ExpandHorizontal = true, ExpandVertical = true,
                BackgroundColor = VFK.EditorPanelListBackground,
                ItemHeight = (int) (SpatialOptions.TileWidth * 1.5F),
                AllowDeselection = false,
                OnSelected = AreaSelected,
                ItemConstructor = v => new RegistryEntryItem<Area>(v),
                Resizer = new WidthRatio(secondary: new HeightRatio(0.5F))
            };

            SectorList.AddAllItems(LoadSectors());
            AreaList.AddAllItems(AreaEditor.LoadAreas());

            Panel.AddAll(AreaList, SectorList);

            Holder = new PartitionGridFrame<Frame>(2, 2)
            {
                Resizer = new WidthRatio(secondary: new HeightRatio()),
                RenderTopDown = false, PartitionRows = false
            };

            Holder.SetColumnPartitions(0.8F, 0.2F);

            SectorEditorTab = new Button("Sector Editor")
            {
                ContentInsets = new Insets(5),
                Border = new SimpleBorder(1, 0, 1, 1),
                ActivationAction = frame => XylemModule.SetContainedFrame(VitreousModule.SectorEditor)
            };
            AreaEditorTab = new Button("Area Editor")
            {
                ContentInsets = new Insets(5),
                Border = new SimpleBorder(1, 0, 1, 1),
                ActivationAction = frame => XylemModule.SetContainedFrame(VitreousModule.AreaEditor)
            };

            EditorTabs.AddAll(SectorEditorTab, AreaEditorTab);

            Holder[0, 0] = EditorTabs;
            Holder[0, 1] = EditorSectorFrame;
            Holder[1, 0] = Panel;

            // AddAll(EditorSectorFrame, Panel);
            AddAll(Holder);
        }

        internal void SectorSelected(Sector sector)
        {
            EditorSectorFrame.SectorComponent = sector;
        }

        internal void AreaSelected(Area area)
        {

        }

        internal void PostNewSectorDialog()
        {
            FormDialog dialog = new FormDialog(BlockingUpdater.CloseDialog, "New Sector", true, "Identifier")
            {
                Receiver = entries => 
                {
                    string identifier = entries.GetOrDefault("Identifier", "").Trim();

                    if (identifier.Length != 0)
                        SectorList.AddItem(new Sector(identifier));
                }
            };

            BlockingUpdater.PostDialog(dialog);
        }

        internal void PostRenameSectorDialog(ContextMenu menu, Frame menuItem, Frame source)
        {
            RegistryEntryItem<Sector> item = source as RegistryEntryItem<Sector>;

            FormDialog dialog = new FormDialog(BlockingUpdater.CloseDialog, "Rename Sector", new[]{"Identifier"}, new[]{$"{item.Item.Identifier}"}, true)
            {
                Receiver = entries => 
                {
                    string identifier = entries.GetOrDefault("Identifier", "").Trim();

                    if (source is RegistryEntryItem<Sector> item)
                    {
                        if (identifier.Length != 0)
                            item.Item.SetIdentifier(identifier);
                    }
                }
            };

            BlockingUpdater.PostDialog(dialog);
        }

        protected override void UpdateGlobalInputs()
        {
            if (I.CONTROL_S.RequestClaim())
                SaveSectors();
        }

        internal static IEnumerable<Sector> LoadSectors()
        {
            if (!AreaEditor.EditorDirectory.Exists)
                AreaEditor.EditorDirectory.Create();
            
            if (!SectorEditorFile.Exists)
                SectorEditorFile.Create();

            JObject data = JObject.Parse(File.ReadAllText(SectorEditorFile.FullName));

            if (data.ContainsKey(K.Entries) && data[K.Entries] is JArray array)
            {
                foreach (var token in array)
                {
                    if (token is JObject obj)
                    {
                        Sector sector = null;

                        try
                        {
                            sector = new Sector(obj);
                        }

                        catch (Exception e)
                        {
                            Output.Suggest($"Skipping invalid sector definition in '{SectorEditorFile.Name}':\n {e.StackTrace}", "Editor");
                        }

                        if (sector != null)
                            yield return sector;
                    }
                }
            }
        }

        public void SaveSectors()
        {
            if (!AreaEditor.EditorDirectory.Exists)
                AreaEditor.EditorDirectory.Create();

            JObject data = new JObject();
            JArray sectors = new JArray();

            foreach (Sector sector in SectorList.Items())
            {
                JObject sectorData = new JObject();
                sector.ExportSlim(sectorData);
                sectors.Add(sectorData);
            }

            data[K.Entries] = sectors;

            File.WriteAllText(SectorEditorFile.FullName, data.ToString());

            NotificationManager.EnqueueNotification("Saved Sectors");
        }
    }
}