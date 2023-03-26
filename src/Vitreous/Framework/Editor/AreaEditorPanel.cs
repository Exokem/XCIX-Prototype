
using Xylem.Data;
using Xylem.Framework;
using Xylem.Framework.Layout;
using Xylem.Framework.Control;
using Xylem.Registration;
using Vitreous.Framework.Layout;
using Vitreous.Component.Composite;
using Vitreous.Component.Spatial;
using Vitreous.Registration;
using Vitreous.Reference;

namespace Vitreous.Framework.Editor
{
    public sealed partial class AreaEditor
    {
        internal VerticalSplitFrame Panel;

        internal GridFrame AreaOptionsHolder;

        internal ListFrame<StructureEntry> StructureList;
        internal ListFrame<FloorEntry> FloorList;
        internal ListFrame<ElementEntry> ElementList;
        internal ListFrame<Area> AreaList;

        internal HorizontalSplitFrame ComponentSelectionHolder;

        internal void AssemblePanel()
        {
            int itemHeight = (int) (SpatialOptions.TileWidth * 1.5F);

            Panel = new VerticalSplitFrame()
            {
                Resizer = new WidthRatio(0.2F, new HeightRatio()),
                HorizontalAlignment = new RightAlignment(),
                BackgroundColor = VFK.EditorPanelBackground
            };

            ListFrameFactory<RegistryEntry> entryListFactory = new ListFrameFactory<RegistryEntry>();
            entryListFactory.ItemHeight(itemHeight)
                .ExpandHorizontal()
                .ExpandVertical()
                .BackgroundColor(VFK.EditorPanelListBackground);
            entryListFactory.LabelResizer(new RoundHeightResizer(16))
                .AllowDeselection(false)
                .ItemConstructor(v => new RegistryEntryItem<RegistryEntry>(v))
                .Resizer(new WidthRatio(secondary: new HeightRatio()))
                .Border(new SimpleBorder(0, 1, 0, 0));
            
            AreaList = entryListFactory.Assemble<Area>("Areas", OnAreaSelected, v => new RegistryEntryItem<Area>(v));
            AreaList.SpanColumns = 2;
            AreaList.Resizer = null;

            StructureList = entryListFactory.Assemble<StructureEntry>("Structures", OnStructureSelected, v => new VisibleEntryItem<StructureEntry>(v, v => v.Texture));

            FloorList = entryListFactory.Assemble<FloorEntry>("Floors", OnFloorSelected, v => new VisibleEntryItem<FloorEntry>(v, v => v.Texture));

            ElementList = entryListFactory.Assemble<ElementEntry>("Elements", OnElementSelected, v => new VisibleEntryItem<ElementEntry>(v, null));
            ElementList.Border = new SimpleBorder(0);

            AreaList.AddAllItems(LoadAreas());
            StructureList.AddAllItems(Registries.Structures.Entries());
            FloorList.AddAllItems(Registries.Floors.Entries());
            ElementList.AddAllItems(Registries.Elements.Entries());

            AreaList.SelectFirst();

            AssembleInspectorHolder();
            AssembleAreaOptions();
            AssembleComponentSelectionHolder();

            Panel.AddAll(AreaOptionsHolder, InspectorHolder);

            AreaList.Resize();
        }

        internal void AssembleComponentSelectionHolder()
        {
            ComponentSelectionHolder = new HorizontalSplitFrame()
            {
                Resizer = new HeightRatio(0.20F, secondary: new WidthRatio(0.8F)),
                HorizontalAlignment = new LeftAlignment(),
                VerticalAlignment = new BottomAlignment(),
                Border = new SimpleBorder(0, 0, 1, 1)
            };

            ComponentSelectionHolder.AddAll(StructureList, FloorList, ElementList);
        }

        internal void SetActiveTool(IEditorTool<Area> tool, IconButton button)
        {
            ViewportFrame.ActiveTool = tool;
            ToolFrame.Select(button);
        }

        internal void OnAreaSelected(Area entry)
        {
            SetArea(entry);
        }   

        internal void OnStructureSelected(StructureEntry entry)
        {
            if (!StructurePlaceTool.Structure.Equals(entry))
                StructurePlaceTool = new StructurePlaceTool(entry);
            ToolFrame.Select(StructurePlaceButton);
        }

        internal void OnFloorSelected(FloorEntry entry)
        {
            if (!FloorPlaceTool.Floor.Equals(entry))
                FloorPlaceTool = new FloorPlaceTool(entry);
            ToolFrame.Select(FloorPlaceButton);
        }

        internal void OnElementSelected(ElementEntry entry)
        {
            if (!ElementAddTool.Element.Equals(entry))
                ElementAddTool = new ElementAddTool(entry);
            ToolFrame.Select(ElementAddButton);
        }

        private void AssembleAreaOptions()
        {
            AreaOptionsHolder = new GridFrame(2, 3)
            {
                ExpandHorizontal = true, ExpandVertical = true,
                Borderless = true,
                Resizer = new WidthRatio()
            };

            AreaOptionsHolder[0, 0] = AreaList;

            TextInput newAreaName = new TextInput()
            {
                ContentInsets = new Insets(5),
                TooltipText = "New area identifier",
                ContentMargin = new Insets(0, 5, 0, 5),
                ExpandHorizontal = true,
                Border = new SimpleBorder(1, 1, 0, 0)
            };

            Button createNamedArea = new Button("New Area")
            {
                ContentInsets = new Insets(5),
                ActivationAction = frame => 
                {
                    if (newAreaName.LabelText.Length != 0)
                        AreaList.AddItem(new Area(newAreaName.LabelText));
                },
                BackgroundColor = VFK.EditorPanelAreaButtonBackground,
                ExpandHorizontal = true,
                Border = new SimpleBorder(1, 1, 0, 0)
            };

            AreaOptionsHolder[0, 1] = createNamedArea;
            AreaOptionsHolder[1, 1] = newAreaName;

            TextInput renamedAreaName = new TextInput()
            {
                ContentInsets = new Insets(5),
                TooltipText = "Rename current area",
                ContentMargin = new Insets(0, 5, 0, 5),
                ExpandHorizontal = true,
                Border = new SimpleBorder(1, 1, 0, 0)
            };

            Button renameCurrentArea = new Button("Rename Area")
            {
                ContentInsets = new Insets(5),
                ActivationAction = frame => 
                {
                    if (renamedAreaName.LabelText.Length != 0 && AreaList.SelectedItem != null)
                        AreaList.SelectedItem.SetIdentifier(renamedAreaName.LabelText);
                },
                BackgroundColor = VFK.EditorPanelAreaButtonBackground,
                ExpandHorizontal = true,
                Border = new SimpleBorder(1, 1, 0, 0)
            };

            AreaOptionsHolder[0, 2] = renameCurrentArea;
            AreaOptionsHolder[1, 2] = renamedAreaName;
        }
    }
}