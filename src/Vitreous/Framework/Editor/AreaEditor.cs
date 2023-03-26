
using System.Collections.Generic;

using Xylem.Framework;
using Xylem.Functional;
using Xylem.Registration;
using Xylem.Graphics;
using Xylem.Framework.Control;
using Xylem.Framework.Layout;

using Vitreous.Component.Spatial;
using Vitreous.Component.Core;
using Vitreous.Component.Composite;
using Vitreous.Reference;
using Vitreous.Framework.Layout;
using Vitreous.Framework.Extensions;

namespace Vitreous.Framework.Editor
{
    public partial class Editor : Frame
    {
        #region Component Fields

        // Layout Components
        PartitionGridFrame<Frame> AreaEditorHolder; // Layout Container
        EditorAreaFrame EditorAreaFrame; // Viewport
        VerticalSplitFrame AreaEditorPanel; // Right Side Panel
        HorizontalSplitFrame AreaEditorListPanel; // Bottom Panel
        ToolGridFrame<Area> AreaEditorToolFrame; // Left Side Panel (Tools)

        // Entry Lists
        ListFrame<StructureEntry> AreaStructures;
        ListFrame<FloorEntry> AreaFloors;
        ListFrame<ElementEntry> AreaElements;
        ListFrame<Area> AreaAreaList;

        // Inspector
        TabFrame AreaInspectorTabs;
        DynamicListFrame<Frame> AreaInspectorDescriptors;
        DynamicListFrame<Frame> AreaInspectorElements;

        // Editor Tools
        TileInspectorTool TileInspectorTool;
        StructurePlaceTool StructurePlaceTool;
        FloorPlaceTool FloorPlaceTool;
        ElementAddTool ElementAddTool;

        #endregion

        Tile InspectedTile;

        internal void ResizeAreaEditor()
        {
            float toolTabBaseProportion = 0.02F;

            float toolTabScaledProportion = toolTabBaseProportion / GraphicsConfiguration.Scale;

            float viewportScaledProportion = 0.8F - toolTabScaledProportion;

            AreaEditorHolder.SetColumnPartitions(toolTabScaledProportion, viewportScaledProportion, 0.2F);
        }

        internal void AssembleAreaEditorTools()
        {
            TileInspectorTool = new TileInspectorTool { TileReceiver = InspectTile };
            StructurePlaceTool = new StructurePlaceTool(StructureEntry.Empty);
            FloorPlaceTool = new FloorPlaceTool(FloorEntry.Empty);
            ElementAddTool = new ElementAddTool(ElementEntry.Empty);
        }

        private void SetAreaActiveTool(IEditorTool<Area> tool)
        {
            EditorAreaFrame.ActiveTool = tool;
        }

        #region Entry List Controls

        internal void AreaStructureSelected(StructureEntry structure)
        {
            if (StructurePlaceTool.Structure != structure)
                StructurePlaceTool = new StructurePlaceTool(structure);
            AreaEditorToolFrame.Select(0, 1);
            SetAreaActiveTool(StructurePlaceTool);
        }

        internal void AreaFloorSelected(FloorEntry floor)
        {
            if (FloorPlaceTool.Floor != floor)
                FloorPlaceTool = new FloorPlaceTool(floor);
            AreaEditorToolFrame.Select(0, 2);
            SetAreaActiveTool(FloorPlaceTool);
        }

        internal void AreaElementSelected(ElementEntry element)
        {
            if (ElementAddTool.Element != element)
                ElementAddTool = new ElementAddTool(element);
            AreaEditorToolFrame.Select(0, 3);
            SetAreaActiveTool(ElementAddTool);
        }

        #endregion

        internal void AssembleAreaEditor()
        {
            AssembleAreaEditorTools();

            EditorAreaFrame = new EditorAreaFrame()
            {
                RenderPriority = 99
            };

            AreaEditorToolFrame = new ToolGridFrame<Area>(1, 5)
            {
                BackgroundColor = VFK.EditorPanelBackground,
                Border = new SimpleBorder(0, 1, 0, 1)
            }
            .WithToolButton(0, 0, TileInspectorTool, () => SetAreaActiveTool(TileInspectorTool))
            .WithToolButton(0, 1, StructurePlaceTool, () => SetAreaActiveTool(StructurePlaceTool))
            .WithToolButton(0, 2, FloorPlaceTool, () => SetAreaActiveTool(FloorPlaceTool))
            .WithToolButton(0, 3, ElementAddTool, () => SetAreaActiveTool(ElementAddTool));

            AreaEditorHolder = new PartitionGridFrame<Frame>(3, 2)
            {
                Resizer = RatioResizer.ExpandBoth,
                RenderTopDown = false
            };

            AreaEditorHolder.SetColumnPartitions(0.05F, 0.8F, 0.15F);
            AreaEditorHolder.SetRowPartitions(0.75F, 0.25F);

            #region Panel Initialization

            AreaEditorPanel = new VerticalSplitFrame()
            {
                BackgroundColor = VFK.EditorPanelBackground,
                Border = new SimpleBorder(0, 0, 0, 1),
                SpanRows = 2
            };

            AreaEditorListPanel = new HorizontalSplitFrame()
            {
                BackgroundColor = VFK.EditorPanelBackground,
                Border = new SimpleBorder(1, 0, 0, 0),
                SpanColumns = 2
            };

            AreaAreaList = new ListFrame<Area>("Areas")
            {
                LabelBorder = new SimpleBorder(0, 1, 1, 1),
                ExpandHorizontal = true, ExpandVertical = true,
                BackgroundColor = VFK.EditorPanelListBackground,
                ItemHeight = (int) (SpatialOptions.TileWidth * 1.5F),
                AllowDeselection = false,
                OnSelected = area => EditorAreaFrame.AreaComponent = area,
                ItemConstructor = v => new RegistryEntryItem<Area>(v)
                {
                    ContextMenu = AreaEntryMenu
                },
                ContextMenu = AreaListMenu
            };

            AreaInspectorDescriptors = new DynamicListFrame<Frame>()
            {
                ExpandHorizontal = true, ExpandVertical = true,
                BackgroundColor = VFK.EditorInspectorBackground,
                ItemConstructor = v => new FrameListItem(v)
            };

            AreaInspectorElements = new DynamicListFrame<Frame>()
            {
                ExpandHorizontal = true, ExpandVertical = true,
                BackgroundColor = VFK.EditorInspectorBackground,
                ItemConstructor = v => new FrameListItem(v)
            };

            AreaInspectorTabs = new TabFrame(VK.Structure, "Tile Structure", AreaInspectorDescriptors)
            {
                SizeTabsAutomatically = false,
                TabProportion = 0.5F
            }
            .WithTab(VK.Elements, "Tile Elements", AreaInspectorElements);

            #endregion

            AreaEditorPanel.AddAll(AreaAreaList, AreaInspectorTabs);

            #region Entry List Initialization
            AreaStructures = new ListFrame<StructureEntry>("Structures")
            {
                BackgroundColor = VFK.EditorPanelListBackground,
                ItemHeight = (int) (SpatialOptions.TileWidth * 1.5F),
                OnSelected = AreaStructureSelected,
                ItemConstructor = v => new VisibleEntryItem<StructureEntry>(v, v => v.Texture)
                {
                },
                Border = new SimpleBorder(0, 1, 0, 1)
            };

            AreaFloors = new ListFrame<FloorEntry>("Floors")
            {
                BackgroundColor = VFK.EditorPanelListBackground,
                ItemHeight = (int) (SpatialOptions.TileWidth * 1.5F),
                OnSelected = AreaFloorSelected,
                ItemConstructor = v => new VisibleEntryItem<FloorEntry>(v, v => v.Texture)
                {
                },
                Border = new SimpleBorder(0, 1, 0, 0)
            };

            AreaElements = new ListFrame<ElementEntry>("Elements")
            {
                BackgroundColor = VFK.EditorPanelListBackground,
                ItemHeight = (int) (SpatialOptions.TileWidth * 1.5F),
                OnSelected = AreaElementSelected,
                ItemConstructor = v => new VisibleEntryItem<ElementEntry>(v, null)
                {
                },
                Border = new SimpleBorder(0, 0, 0, 0)
            };
            #endregion

            AreaEditorListPanel.AddAll(AreaStructures, AreaFloors, AreaElements);

            AreaEditorHolder[0, 0] = AreaEditorToolFrame;
            AreaEditorHolder[1, 0] = EditorAreaFrame;
            AreaEditorHolder[0, 1] = AreaEditorListPanel;
            AreaEditorHolder[2, 0] = AreaEditorPanel;
        }
        
        void InspectTile(Tile tile)
        {
            InspectedTile = tile;

            Structure structure = tile.Structure;

            AreaInspectorDescriptors.Clear();

            foreach (Attribute attribute in structure.Attributes)
                AreaInspectorDescriptors.AddItem(attribute.GetFrame(tile));

            foreach (State state in structure.States)
                AreaInspectorDescriptors.AddItem(state.GetFrame(tile));

            foreach (Qualifier qualifier in structure.Qualifiers)
                AreaInspectorDescriptors.AddItem(qualifier.GetFrame(tile));

            AreaInspectorTabs.Resize();

            RefreshElements();
        }   

        void RefreshElements()
        {
            AreaInspectorElements.Clear();

            foreach (Element element in InspectedTile.Structure.Elements)
                AreaInspectorElements.AddItem(element.GetFrame(() => RemoveElement(element)));

            AreaInspectorElements.Resize();
        }

        void RemoveElement(Element element)
        {
            InspectedTile.Structure.Elements.Remove(element);
            RefreshElements();
        }
    }
}