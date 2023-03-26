
using System;
using System.Collections.Generic;
using Xylem.Functional;

namespace Xylem.Framework.Layout
{
    public class GridFrame : GridFrame<Frame>
    {
        public GridFrame(int columns, int rows) : base(columns, rows)
        {
        }
    }

    public class GridFrame<V> : Frame where V : Frame
    {
        protected V[,] Items;

        protected int[] MaxRowHeights;
        protected int[] MaxColWidths;

        // How many expanders are in each row/column
        protected int[] RowExpanders;
        protected int[] ColExpanders;

        public int Columns, Rows;

        protected int SplitHorizontalSpace, SplitVerticalSpace;

        protected int _measuredWidth;
        public override int MeasuredWidth => _measuredWidth;

        protected int _measuredHeight;
        public override int MeasuredHeight => _measuredHeight;

        public GridFrame(int columns, int rows)
        {
            Columns = columns;
            Rows = rows;

            Items = new V[Columns, Rows];

            MaxRowHeights = new int[Rows];
            MaxColWidths = new int[Columns];

            RowExpanders = new int[Rows];
            ColExpanders = new int[Columns];
        }

        public bool Within(int x, int y) => 0 <= x && x < Columns && 0 <= y && y < Rows;

        public virtual V this[int x, int y]
        {
            get => Within(x, y) ? Items[x, y] : null;
            set 
            {
                if (Within(x, y))
                {
                    V prev = Items[x, y];

                    if (prev != null)
                    {
                        prev.Container = null;

                        if (prev.ExpandHorizontal)
                            ColExpanders[x] --;
                        if (prev.ExpandVertical)
                            RowExpanders[y] --;
                    }

                    Items[x, y] = value;

                    if (value == null)
                        return;

                    value.Container = this;

                    if (value.ExpandHorizontal)
                        ColExpanders[x] ++;
                    if (value.ExpandVertical)
                        RowExpanders[y] ++;
                }
            }
        }

        protected virtual void DetermineMaxDimensions()
        {
            for (int r = 0; r < Rows; r ++)
                MaxRowHeights[r] = 0;
            for (int c = 0; c < Columns; c ++)
                MaxColWidths[c] = 0;

            // First Pass:
            // 1. Determine which rows and columns will expand
            // 2. Determine the maximum measured width of frames in each column
            // 3. Determine the maximum measured height of frames in each row
            for (int y = 0; y < Rows; y ++)
            {
                for (int x = 0; x < Columns; x ++)
                {
                    Frame frame = Items[x, y];

                    if (frame == null)
                        continue;

                    frame.Resize();
                    
                    MaxRowHeights[y] = System.Math.Max(frame.MeasuredHeight, MaxRowHeights[y]);
                    MaxColWidths[x] = System.Math.Max(frame.MeasuredWidth, MaxColWidths[x]);
                }    
            }
        }

        // Returns the sum of all maximum row heights
        protected virtual int FlexibleRowSpace(out int space, out int expanders)
        {
            space = CH;
            expanders = 0;
            int total = 0;

            for (int y = 0; y < Rows; y ++)
            {
                if (RowExpanders[y] == 0)
                    space -= MaxRowHeights[y];
                else 
                    expanders ++;
                total += MaxRowHeights[y];
            }

            return total;
        }

        // Returns the sum of all maximum column widths
        protected virtual int FlexibleColumnSpace(out int space, out int expanders)
        {
            space = CW;
            expanders = 0;
            int total = 0;

            for (int x = 0; x < Columns; x ++)
            {
                if (ColExpanders[x] == 0)
                    space -= MaxColWidths[x];
                else 
                    expanders ++;
                total += MaxColWidths[x];
            }

            return total;
        }

        protected virtual void ResizeItems()
        {
            // Second Pass:
            // 1. Set height of frames in non-expanding rows to the max height of that row
            // 2. Set width of frames in non-expanding cols meet the max width of that col
            // 3. Set height of frames in expanding rows to the split vertical space
            // 4. Set width of frames in expanding cols to the split horizontal space
            for (int y = 0; y < Rows; y ++)
            {
                for (int x = 0; x < Columns; x ++)
                {
                    V frame = Items[x, y];

                    if (frame == null)
                        continue;

                    frame.W = ColumnWidth(x);
                    frame.H = RowHeight(y);

                    if (1 < frame.SpanColumns)
                    {
                        for (int sx = x + 1; sx < Columns; sx ++)
                            frame.W += ColumnWidth(sx);
                    }

                    if (1 < frame.SpanRows)
                    {
                        for (int sy = y + 1; sy < Rows; sy ++)
                            frame.H += RowHeight(sy);
                    }
                }    
            }
        }

        public override void Resize()
        {
            base.Resize();
            // Find max width of each column using measured widths of non-expanders
            // Find max height of each row using measured heights of non-expanders
            // Auto-size frames in each row/column to the maximum dimensions
            // Divide remaining space between expanders

            DetermineMaxDimensions();

            // Intermediate Step: 
            // 1. Determine horizontal space available for division between expanding columns
            // 2. Determine vertical space available for division between expanding rows
            // 3. Split horizontal and vertical space
            int expandingRows = 0, expandingColumns = 0;

            int spaceHorizontal = CW;
            int spaceVertical = CH;

            _measuredHeight = 0;
            _measuredWidth = 0;

            _measuredHeight = FlexibleRowSpace(out spaceVertical, out expandingRows);
            _measuredWidth = FlexibleColumnSpace(out spaceHorizontal, out expandingColumns);

            if (W == 0)
            {
                W = _measuredWidth + ContentInsets.Width;
                spaceHorizontal += W;
            }

            if (H == 0)
            {
                H = _measuredHeight + ContentInsets.Height;
                spaceVertical += H;
            }

            SplitHorizontalSpace = spaceHorizontal / System.Math.Max(expandingColumns, 1);
            SplitVerticalSpace = spaceVertical / System.Math.Max(expandingRows, 1);

            ResizeItems();
        }

        protected virtual int ColumnWidth(int x)
        {
            if (ColExpanders[x] != 0)
                return System.Math.Max(SplitHorizontalSpace, MaxColWidths[x]);
            else 
                return MaxColWidths[x];
        }

        protected virtual int RowHeight(int y)
        {
            if (RowExpanders[y] != 0)
                return System.Math.Max(SplitVerticalSpace, MaxRowHeights[y]);
            else
                return MaxRowHeights[y];
        }

        protected virtual void UpdateGridItem(int x, int y, V item)
        {
            item?.Update();
        }

        protected override void UpdateContentFrames()
        {
            for (int y = 0; y < Rows; y ++)
            {
                for (int x = 0; x < Columns; x ++)
                {
                    V item = Items[x, y];
                    if (item != null)
                        UpdateGridItem(x, y, item);
                }
            }
        }

        protected override void RenderContentFrames()
        {
            int oy = 0;

            // This fixed an issue where a grid within a directional layout may not be
            // resized appropriately, leading to item offsets that are out of bounds of
            // the grid. 
            // TODO: investigate why overall grid size is accurate but size of items is not (grid within a directional layout)
            bool shouldResize = false;

            for (int y = 0; y < Rows; y ++)
            {
                int ox = 0;
                
                for (int x = 0; x < Columns; x ++)
                {
                    V frame = Items[x, y];

                    if (frame != null)
                    {
                        frame.OX = ox;
                        frame.OY = oy;
                        frame.Render();
                    }

                    if (CH < oy || CW < ox)
                        shouldResize = true;

                    ox += ColExpanders[x] == 0 ? MaxColWidths[x] : SplitHorizontalSpace;
                }

                oy += RowExpanders[y] == 0 ? MaxRowHeights[y] : SplitVerticalSpace;
            }

            if (shouldResize)
                UpdateDispatcher.EnqueueRenderAction(Resize);
        }
    }
}