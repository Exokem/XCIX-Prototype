
using System.Collections.Generic;
using Xylem.Framework;
using Xylem.Framework.Layout;
using Xylem.Framework.Control;
using Xylem.Functional;
using Xylem.Registration;
using Vitreous.Reference;
using Vitreous.Component.Spatial;

namespace Vitreous.Framework.Editor
{
    public partial class Editor : Frame
    {
        EditorSectorFrame EditorSectorFrame;
        PartitionGridFrame<Frame> SectorEditorHolder;
        VerticalSplitFrame SectorEditorPanel;

        ListFrame<Sector> SectorSectorList;
        ListFrame<Area> SectorAreaList;

        internal void AssembleSectorEditor()
        {
            SectorEditorHolder = new PartitionGridFrame<Frame>(2, 1)
            {
                Resizer = RatioResizer.ExpandBoth,
                RenderTopDown = false, PartitionRows = false
            };

            SectorEditorHolder.SetColumnPartitions(0.8F, 0.2F);

            EditorSectorFrame = new EditorSectorFrame()
            {
                RenderPriority = 99,
                ExpandVertical = true
            };

            SectorEditorPanel = new VerticalSplitFrame()
            {
                BackgroundColor = VFK.EditorPanelBackground,
                Border = new SimpleBorder(0, 0, 0, 1)
            };

            SectorSectorList = new ListFrame<Sector>("Sectors")
            {
                LabelBorder = new SimpleBorder(0, 1, 1, 1),
                ExpandHorizontal = true, ExpandVertical = true,
                BackgroundColor = VFK.EditorPanelListBackground,
                ItemHeight = (int) (SpatialOptions.TileWidth * 1.5F),
                AllowDeselection = false,
                OnSelected = SectorSelected,
                ItemConstructor = v => new RegistryEntryItem<Sector>(v)
                {
                    ContextMenu = SectorEntryMenu
                },
                ContextMenu = SectorListMenu
            };

            SectorAreaList = new ListFrame<Area>("Areas")
            {
                ExpandHorizontal = true, ExpandVertical = true,
                BackgroundColor = VFK.EditorPanelListBackground,
                ItemHeight = (int) (SpatialOptions.TileWidth * 1.5F),
                AllowDeselection = false,
                OnSelected = AreaSelected,
                ItemConstructor = v => new RegistryEntryItem<Area>(v)
                {
                    ContextMenu = AreaEntryMenu
                },
                ContextMenu = AreaListMenu
            };

            SectorEditorPanel.AddAll(SectorSectorList, SectorAreaList);

            SectorEditorHolder[0, 0] = EditorSectorFrame;
            SectorEditorHolder[1, 0] = SectorEditorPanel;
        }

        

    }
}