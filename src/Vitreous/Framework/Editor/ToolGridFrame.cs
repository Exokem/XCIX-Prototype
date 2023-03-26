
using System.Collections.Generic;
using Xylem.Framework;
using Xylem.Registration;
using Xylem.Functional;
using Xylem.Vectors;
using Xylem.Framework.Layout;
using Xylem.Framework.Control;

namespace Vitreous.Framework.Editor
{
    public class ToolGridFrame<V> : SelectionGridFrame<IconButton>
    {
        public ToolGridFrame(int columns, int rows) : base(columns, rows)
        {
            AllowDeselection = false;
        }

        public ToolGridFrame<V> WithToolButton(int x, int y, IEditorTool<V> toolBasis, Update selectionAction)
        {
            IconButton button = new IconButton(R.Textures[toolBasis.IconResourceIdentifier])
            {
                ResourceScale = 2F,
                Selectable = true,
                TooltipText = toolBasis.Description,
                OnSelected = () => 
                {
                    Select(x, y);
                    selectionAction();
                },
                AutoScale = true,
                ExpandHorizontal = true,
                Border = new SimpleBorder(0, 1, 1, 1)
            };

            this[x, y] = button;

            return this;
        }

        internal override void Select(IconButton item)
        {
            base.Select(item);
        }

        public void Select(int x, int y)
        {
            if (!Within(x, y))
                return;

            if (this[x, y] != null)
                Select(this[x, y]);
        }
    }
}