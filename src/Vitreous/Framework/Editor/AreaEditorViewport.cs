
using Xylem.Input;
using Xylem.Framework.Layout;
using Xylem.Framework.Control;
using Xylem.Framework;
using Xylem.Functional;
using Xylem.Registration;
using Vitreous.Component.Composite;
using Vitreous.Component.Spatial;
using Vitreous.Reference;

namespace Vitreous.Framework.Editor
{
    public sealed partial class AreaEditor
    {
        internal EditorAreaFrame ViewportFrame;

        internal SelectionGridFrame<IconButton> ToolFrame;

        internal StructurePlaceTool StructurePlaceTool;
        internal FloorPlaceTool FloorPlaceTool;
        internal ElementAddTool ElementAddTool;

        IconButton StructurePlaceButton, FloorPlaceButton, ElementAddButton, InspectorButton;

        private void AssembleViewport()
        {
            ViewportFrame = new EditorAreaFrame()
            {
                Resizer = new WidthRatio(0.8F, new HeightRatio(0.8F)),
                BackgroundColor = VFK.EditorViewportBackground
            };

            AssembleTools();
            AssembleToolFrame();

            ViewportFrame.Add(ToolFrame);
        }

        internal void SetArea(Area area) => ViewportFrame.AreaComponent = area;

        private void AssembleTools()
        {
            StructurePlaceTool = new StructurePlaceTool(StructureEntry.Empty);
            FloorPlaceTool = new FloorPlaceTool(FloorEntry.Empty);
            ElementAddTool = new ElementAddTool(ElementEntry.Empty);
        }

        private void AssembleToolFrame()
        {
            ToolFrame = new SelectionGridFrame<IconButton>(1, 4)
            {
                VerticalAlignment = new CenteredVerticalAlignment(),
                AllowDeselection = false,
                Focusable = true,
                OX = 30,
                Relocator = new MouseDragRelocator(InputProcessor.MouseMiddle)
                {
                    LimitX = frame => (int) (0.8F * (float) frame.Container.RW),
                    LimitY = frame => (int) (0.8F * (float) frame.Container.RH)
                }
            };

            UpdateDispatcher.EnqueueDelayedUpdate(() => ToolFrame.VerticalAlignment = null, 10);

            StructurePlaceButton = new IconButton(R.Textures["Icon-Structures"])
            {
                ResourceScale = 2F,
                Selectable = true,
                TooltipText = "Place Structures",
                OnSelected = () => ViewportFrame.ActiveTool = StructurePlaceTool
            };

            FloorPlaceButton = new IconButton(R.Textures["Icon-Floors"])
            {
                ResourceScale = 2F,
                Selectable = true,
                TooltipText = "Place Floors",
                OnSelected = () => ViewportFrame.ActiveTool = FloorPlaceTool
            };

            ElementAddButton = new IconButton(R.Textures["Icon-Elements"])
            {
                ResourceScale = 2F,
                Selectable = true,
                TooltipText = "Add Elements",
                OnSelected = () => ViewportFrame.ActiveTool = ElementAddTool
            };

            InspectorButton = new IconButton(R.Textures["Icon-Inspector"])
            {
                ResourceScale = 2F,
                Selectable = true,
                TooltipText = "Inspect Tiles",
                OnSelected = () => ViewportFrame.ActiveTool = Inspector
            };

            ToolFrame.RenderIndex = 99;

            ToolFrame[0, 0] = StructurePlaceButton;
            ToolFrame[0, 1] = FloorPlaceButton;
            ToolFrame[0, 2] = ElementAddButton;
            ToolFrame[0, 3] = InspectorButton;
        }
    }
}