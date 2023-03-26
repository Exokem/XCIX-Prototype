
using Microsoft.Xna.Framework;
using Xylem.Data;
using Xylem.Framework;
using Xylem.Framework.Control;
using Xylem.Framework.Layout;
using Xylem.Graphics;
using Xylem.Reference;
using Vitreous.Component.Composite;
using Vitreous.Component.Spatial;
using Vitreous.Component.Core;
using Vitreous.Reference;

namespace Vitreous.Framework.Editor
{
    public sealed partial class AreaEditor
    {
        internal GridFrame InspectorHolder;
        internal ListFrame<IconButton> InspectorModeSelector;
        internal DynamicListFrame<Frame> InspectorList;

        internal DynamicListFrame<Frame> InspectorDescriptorList;
        internal DynamicListFrame<Frame> InspectorElementList;

        private IconButton StructuresButton;
        private IconButton ElementsButton;

        internal TileInspectorTool Inspector;

        public static LabelFactory LabelFactory;
        public static InputFactory InputFactory;

        private void AssembleInspectorHolder()
        {
            Inspector = new TileInspectorTool()
            {
                TileReceiver = InspectTile
            };

            LabelFactory = new LabelFactory();
            LabelFactory.ContentInsets(new Insets(5)).BackgroundColor(VFK.EditorLabelPrimaryBackground).ExpandHorizontal().Border(new SimpleBorder(0, 1, 1, 0));

            InputFactory = new InputFactory();
            InputFactory.ContentInsets(new Insets(5))
                .ContentMargin(new Insets(0, 5, 0, 5))
                .BackgroundColor(VFK.EditorTextInputTextBackground)
                .ExpandHorizontal()
                .Border(new SimpleBorder(0, 1, 1, 0));

            InspectorHolder = new GridFrame(2, 2)
            {
                ExpandHorizontal = true, ExpandVertical = true,
                Borderless = true,
                Resizer = new WidthRatio()
            };

            InspectorModeSelector = new ListFrame<IconButton>()
            {
                Resizer = new HeightRatio(),
                HorizontalAlignment = new LeftAlignment(),
                ItemConstructor = v => new IconButtonItem(v),
                BackgroundColor = VFK.EditorIconListBackground,
                ExpandVertical = true,
                AllowDeselection = false,
                Border = new SimpleBorder(0, 1, 0, 0)
            };

            InspectorList = new DynamicListFrame<Frame>("Inspector")
            {
                ItemConstructor = v => new FrameListItem(v),
                ExpandHorizontal = true, ExpandVertical = true,
                BackgroundColor = VFK.EditorInspectorBackground,
                LabelResizer = new RoundHeightResizer(16),
                LabelBorder = new SimpleBorder(0, 0, 1, 0)
            };

            InspectorDescriptorList = new DynamicListFrame<Frame>("Descriptors")
            {
                ItemConstructor = v => new FrameListItem(v),
                ExpandHorizontal = true, ExpandVertical = true,
                BackgroundColor = VFK.EditorInspectorBackground,
                LabelResizer = new RoundHeightResizer(16),
                LabelBorder = new SimpleBorder(0, 1, 1, 0),
                Border = new SimpleBorder(0, 1, 1, 1)
            };

            InspectorElementList = new DynamicListFrame<Frame>("Elements")
            {
                ItemConstructor = v => new FrameListItem(v),
                ExpandHorizontal = true, ExpandVertical = true,
                BackgroundColor = VFK.EditorInspectorBackground,
                LabelResizer = new RoundHeightResizer(16),
                LabelBorder = new SimpleBorder(0, 0, 1, 0)
            };

            StructuresButton = IconButtonFactory.Assemble("Icon-Structures", "View Structure Information", InspectStructure);

            ElementsButton = IconButtonFactory.Assemble("Icon-Elements", "View Element Information", InspectElement);

            InspectorModeSelector.AddAllItems(StructuresButton, ElementsButton);

            InspectorModeSelector.SelectFirst();

            Label InspectorLabel = LabelFactory.Assemble("Tile Inspector");
            InspectorLabel.Border.Top = 1;
            InspectorLabel.SpanColumns = 2;

            InspectorHolder[0, 0] = InspectorLabel;
            InspectorHolder[0, 1] = InspectorDescriptorList;
            InspectorHolder[1, 1] = InspectorElementList;
        }

        protected override void RenderContentFrames()
        {
            foreach (Frame frame in ContentFrames)
            {
                frame.Render();
                if (frame == ViewportFrame && frame is EditorAreaFrame editorFrame)
                {
                    if (ViewportFrame.ActiveTool == Inspector && Inspector.InspectedTile != null)
                    {
                        int tx = Inspector.InspectedTile.Value.X;
                        int ty = Inspector.InspectedTile.Value.Y;

                        Rectangle inspectorOverlay = new Rectangle(ViewportFrame.GRX + (tx * ViewportFrame.SpaceWidth), ViewportFrame.GRY + (ty * ViewportFrame.SpaceHeight), ViewportFrame.SpaceWidth, ViewportFrame.SpaceHeight);

                        Pixel.FillRect(inspectorOverlay, VFK.EditorInspectorInspectedTileOverlay);
                        Pixel.FrameRect(inspectorOverlay.Shrink(GraphicsConfiguration.PixelScale), XFK.Secondary);
                    }

                    editorFrame.RenderRulers();
                }
            }
        }

        internal Tile InspectedTile;

        // Called when the inspector tool is used
        // Should update the panel display based on which panel button is selected
        private void InspectTile(Tile tile)
        {
            InspectedTile = tile;

            InspectStructure();
            InspectElement();

            InspectorHolder.Resize();
        }

        // Called by inspector panel buttons
        // Should update the panel display based on the currently inspected tile
        private void InspectStructure()
        {
            if (InspectedTile != null)
            {
                UpdateStructureInformation(InspectedTile.Structure);
            }
        }

        private void InspectElement()
        {
            if (InspectedTile != null)
            {
                UpdateElementInformation(InspectedTile.Structure.Elements);
            }
        }

        private void UpdateStructureInformation(Structure structure)
        {
            // InspectorList.Clear();
            InspectorDescriptorList.Clear();

            foreach (Attribute attribute in structure.Attributes)
            {
                // InspectorList.AddItem(AssembleAttributeFrame(attribute));
                InspectorDescriptorList.AddItem(AssembleAttributeFrame(attribute));
            }

            foreach (State state in structure.States)
            {
                // InspectorList.AddItem(AssembleStateFrame(state));
                InspectorDescriptorList.AddItem(AssembleStateFrame(state));
            }

            foreach (Qualifier qualifier in structure.Qualifiers)
            {
                // InspectorList.AddItem(AssembleQualifierFrame(qualifier));
                InspectorDescriptorList.AddItem(AssembleQualifierFrame(qualifier));
            }
        }

        private void UpdateElementInformation(ElementContainer elements)
        {
            // InspectorList.Clear();
            InspectorElementList.Clear();

            foreach (Element element in elements)
            {
                // InspectorList.AddItem(AssembleElementFrame(element));
                InspectorElementList.AddItem(AssembleElementFrame(element));
            }
        }

        public Frame AssembleElementFrame(Element element)
        {
            GridFrame grid = new GridFrame(2, 3)
            {
                ExpandHorizontal = true,
                BackgroundColor = VFK.EditorInspectorElementBackground,
                Border = new SimpleBorder(0, 1, 1, 0)
            };

            Label mainLabel = LabelFactory.Assemble("Element");
            mainLabel.SpanColumns = 2;
            mainLabel.BackgroundColor = VFK.EditorInspectorEntryTitleBackground;
            mainLabel.TextColor = VFK.EditorInspectorEntryTitleText;

            Button remove = new Button("Remove")
            {
                ContentInsets = new Insets(5),
                ActivationAction = frame => 
                {
                    InspectedTile.Structure.Elements.Remove(element);
                    UpdateElementInformation(InspectedTile.Structure.Elements);
                    InspectorHolder.Resize();
                },
                BackgroundColor = VFK.EditorPanelAreaButtonBackground,
                ExpandHorizontal = true,
                Border = new SimpleBorder(1, 1, 0, 0),
                SpanColumns = 2
            };

            grid[0, 0] = mainLabel;

            grid[0, 1] = LabelFactory.Assemble("Identifier:");
            grid[1, 1] = LabelFactory.Assemble($"{element.Identifier}");
            grid[0, 2] = remove;

            return grid;
        }

        public Frame AssembleAttributeFrame(Attribute attribute)
        {
            GridFrame grid = new GridFrame(2, 3)
            {
                ExpandHorizontal = true,
                BackgroundColor = VFK.EditorInspectorAttributeBackground,
                Border = new SimpleBorder(0, 1, 1, 0)
            };

            Label mainLabel = LabelFactory.Assemble("Attribute");
            mainLabel.SpanColumns = 2;
            mainLabel.BackgroundColor = VFK.EditorInspectorEntryTitleBackground;
            mainLabel.TextColor = VFK.EditorInspectorEntryTitleText;
            grid[0, 0] = mainLabel;

            grid[0, 1] = LabelFactory.Assemble("Identifier:");
            grid[1, 1] = LabelFactory.Assemble($"{attribute.Identifier}");

            TextInput valueInput = InputFactory.Assemble($"{attribute.Value}", true);
            valueInput.OnTextChanged = v => 
            {
                attribute.Value = int.Parse(v);
                InspectedTile.Invalidated = true;
            };
            grid[0, 2] = LabelFactory.Assemble("Value:");
            grid[1, 2] = valueInput;

            return grid;
        }

        public Frame AssembleStateFrame(State state)
        {
            GridFrame grid = new GridFrame(2, 3)
            {
                ExpandHorizontal = true,
                BackgroundColor = VFK.EditorInspectorStateBackground,
                Border = new SimpleBorder(0, 1, 1, 0)
            };
            
            Label mainLabel = LabelFactory.Assemble("State");
            mainLabel.SpanColumns = 2;
            mainLabel.BackgroundColor = VFK.EditorInspectorEntryTitleBackground;
            mainLabel.TextColor = VFK.EditorInspectorEntryTitleText;
            grid[0, 0] = mainLabel;

            grid[0, 1] = LabelFactory.Assemble("Identifier:");
            grid[1, 1] = LabelFactory.Assemble($"{state.Identifier}");

            Menu valueMenu = new Menu(state.Value.Identifier)
            {
                ExpandHorizontal = true,
                Borderless = true
            };

            DynamicListFrame<SwitchButton> valueList = new DynamicListFrame<SwitchButton>()
            {
                ItemConstructor = v => new SwitchButtonItem(v),
                Resizer = new WidthRatio(),
                AllowDeselection = false,
                AutoSize = true,
                ItemOffset = -1
            };

            foreach (StateValue value in state.Reference.Values())
            {
                SwitchButton valueButton = new SwitchButton(value.Identifier)
                {
                    OnSelected = button =>
                    {
                        valueMenu.LabelText = value.Identifier;
                        state.Value = value;
                        InspectedTile.Invalidated = true;
                        InspectedTile.InvalidateAdjacencies = true;
                    },
                    ContentInsets = new Insets(5),
                    BackgroundColor = VFK.EditorInspectorStateSelectionBackground
                };
                valueList.AddItem(valueButton);
                valueButton.Border.Right = 0;
            }
            
            valueMenu.Add(valueList);

            grid[0, 2] = LabelFactory.Assemble("Value:");
            grid[1, 2] = valueMenu;

            return grid;
        }

        public Frame AssembleQualifierFrame(Qualifier qualifier)
        {
            GridFrame grid = new GridFrame(2, 3)
            {
                ExpandHorizontal = true,
                BackgroundColor = VFK.EditorInspectorQualifierBackground,
                Border = new SimpleBorder(0, 1, 1, 0)
            };

            Label mainLabel = LabelFactory.Assemble("Qualifier");
            mainLabel.SpanColumns = 2;
            mainLabel.BackgroundColor = VFK.EditorInspectorEntryTitleBackground;
            mainLabel.TextColor = VFK.EditorInspectorEntryTitleText;
            grid[0, 0] = mainLabel;

            grid[0, 1] = LabelFactory.Assemble("Identifier:");
            grid[1, 1] = LabelFactory.Assemble($"{qualifier.Identifier}");

            Button valueButton = new Button($"{qualifier.Value}")
            {
                ActivationAction = button =>
                {
                    qualifier.Value = !qualifier.Value;
                    ((Button) button).LabelText = $"{qualifier.Value}";
                    InspectedTile.Invalidated = true;
                },
                ContentInsets = new Insets(5),
                BackgroundColor = VFK.EditorInspectorQualifierValueBackground,
                Borderless = true
            };

            grid[0, 2] = LabelFactory.Assemble("Value:");
            grid[1, 2] = valueButton;

            return grid;
        }
    }
}