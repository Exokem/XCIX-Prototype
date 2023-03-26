
using Xylem.Framework;
using Xylem.Framework.Layout;
using Xylem.Reference;
using Xylem.Framework.Control;
using Xylem.Functional;

using Vitreous.Component.Core;
using Vitreous.Component.Composite;
using Vitreous.Component.Spatial;
using Vitreous.Reference;

namespace Vitreous.Framework.Extensions
{
    public static class ComponentExtensions
    {
        public static Frame GetFrame(this Attribute attribute, Tile inspectedTile)
        {
            GridFrame grid = new GridFrame(2, 1)
            {
                ExpandHorizontal = true,
                BackgroundColor = VFK.EditorInspectorAttributeBackground,
                Border = new SimpleBorder(0, 1, 1, 0)
            };

            Label identifierLabel = new Label($"{attribute.Identifier}", XFK.PrimaryBorder)
            {
                ContentInsets = new Insets(5),
                ExpandHorizontal = true,
                Border = new SimpleBorder(0, 1, 1, 0),
            };

            TextInput valueInput = new TextInput($"{attribute.Value}")
            {
                ContentInsets = new Insets(5),
                ExpandHorizontal = true,
                Numeric = true,
                Border = new SimpleBorder(0, 1, 1, 0),
                TextColor = VFK.EditorInspectorEntryTitleText,
                OnTextChanged = v => 
                {
                    attribute.Value = int.Parse(v);
                    inspectedTile.Invalidated = true;
                }
            };

            grid[0, 0] = identifierLabel;
            grid[1, 0] = valueInput;

            return grid;
        }

        public static Frame GetFrame(this State state, Tile inspectedTile)
        {
            GridFrame grid = new GridFrame(2, 1)
            {
                ExpandHorizontal = true,
                BackgroundColor = VFK.EditorInspectorAttributeBackground,
                Border = new SimpleBorder(0, 1, 1, 0)
            };

            Label identifierLabel = new Label($"{state.Identifier}", XFK.PrimaryBorder)
            {
                ContentInsets = new Insets(5),
                ExpandHorizontal = true,
                Border = new SimpleBorder(0, 1, 1, 0),
            };

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
                        inspectedTile.Invalidated = true;
                        inspectedTile.InvalidateAdjacencies = true;
                    },
                    ContentInsets = new Insets(5),
                    BackgroundColor = VFK.EditorInspectorStateSelectionBackground
                };
                valueList.AddItem(valueButton);
                valueButton.Border.Right = 0;
            }
            
            valueMenu.Add(valueList);

            grid[0, 0] = identifierLabel;
            grid[1, 0] = valueMenu;

            return grid;
        }

        public static Frame GetFrame(this Qualifier qualifier, Tile inspectedTile)
        {
            PartitionGridFrame<Frame> grid = new PartitionGridFrame<Frame>(2, 1)
            {
                ExpandHorizontal = true,
                BackgroundColor = VFK.EditorInspectorAttributeBackground,
                Border = new SimpleBorder(0, 1, 1, 0)
            };

            grid.SetColumnPartitions(0.5F, 0.5F);

            Label identifierLabel = new Label($"{qualifier.Identifier}", XFK.PrimaryBorder)
            {
                ContentInsets = new Insets(5),
                ExpandHorizontal = true,
                Border = new SimpleBorder(0, 1, 1, 0),
            };

            Button valueButton = new Button($"{qualifier.Value}")
            {
                ActivationAction = button =>
                {
                    qualifier.Value = !qualifier.Value;
                    (button as Button).LabelText = $"{qualifier.Value}";
                    inspectedTile.Invalidated = true;
                },
                ContentInsets = new Insets(5),
                BackgroundColor = VFK.EditorInspectorQualifierValueBackground,
                Border = new SimpleBorder(0, 0, 1, 0)
            };

            grid[0, 0] = identifierLabel;
            grid[1, 0] = valueButton;

            return grid;
        }

        public static Frame GetFrame(this Element element, Update actionCallback)
        {
            GridFrame grid = new GridFrame(2, 1)
            {
                ExpandHorizontal = true,
                BackgroundColor = VFK.EditorInspectorAttributeBackground,
                Border = new SimpleBorder(0, 1, 1, 0)
            };

            Label identifierLabel = new Label($"{element.Identifier}", XFK.PrimaryBorder)
            {
                ContentInsets = new Insets(5),
                ExpandHorizontal = true,
                Border = new SimpleBorder(0, 1, 1, 0),
            };

            Button remove = new Button("Remove")
            {
                ContentInsets = new Insets(5),
                ActivationAction = frame => actionCallback(),
                BackgroundColor = VFK.EditorPanelAreaButtonBackground,
                ExpandHorizontal = true,
                Border = new SimpleBorder(1, 1, 0, 0),
                SpanColumns = 2
            };

            grid[0, 0] = identifierLabel;
            grid[1, 0] = remove;

            return grid;
        }
    }
}