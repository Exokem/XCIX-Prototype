
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Xylem.Functional;
using Xylem.Vectors;
using Xylem.Graphics;

namespace Xylem.Framework.Layout
{
    // A grid frame where each row/column is sized to a specific partition of the whole
    // width or height of the grid
    public class PartitionGridFrame<V> : GridFrame<V> where V : Frame
    {
        protected readonly float[] ColumnPartitions;
        protected readonly float[] RowPartitions;

        public readonly int[] ColumnWidths;
        public readonly int[] RowHeights;

        public bool PartitionRows { get; set; } = true;
        public bool PartitionColumns { get; set; } = true;

        public bool RenderTopDown { get; set; } = true;

        public override int MeasuredWidth => _measuredWidth;
        public override int MeasuredHeight => _measuredHeight;

        protected readonly SortedList<V, V> PriorityRenderOrder;

        public PartitionGridFrame(int columns, int rows) : base(columns, rows)
        {
            Borderless = true;

            Columns = columns;
            Rows = rows;

            ColumnPartitions = new float[Columns];
            RowPartitions = new float[Rows];

            ColumnWidths = new int[Columns];
            RowHeights = new int[Rows];

            PriorityRenderOrder = new SortedList<V, V>(new RenderPriorityComparer());
        }

        public PartitionGridFrame<V> SetRowPartitions(params float[] partitions)
        {
            for (int r = 0; r < Math.Min(partitions.Length, Rows); r ++)
                RowPartitions[r] = partitions[r];
            return this;
        }

        public PartitionGridFrame<V> SetColumnPartitions(params float[] partitions)
        {
            for (int c = 0; c < Math.Min(partitions.Length, Columns); c ++)
                ColumnPartitions[c] = partitions[c];
            return this;
        }

        public int PartitionColumnWidth(int x) => (int) (ColumnPartitions[x] * (float) CW);
        public int PartitionRowHeight(int y) => (int) (RowPartitions[y] * (float) CH);

        protected override int ColumnWidth(int x) => ColumnWidths[x];
        protected override int RowHeight(int y) => RowHeights[y];

        public override void Resize()
        {
            base.Resize();

            // Max dimensions are based on resized items (they will be resized again later)
            DetermineMaxDimensions();

            CalculateMeasuredSize();

            W = Math.Max(W, MeasuredWidth);
            H = Math.Max(H, MeasuredHeight);

            W = Math.Min(W, ContainerWidth);
            H = Math.Min(H, ContainerHeight);

            CalculatePartitions();

            ApplyPartitions();
        }

        protected override void UpdateContentFrames()
        {
            PriorityRenderOrder.Clear();

            int oy = 0;

            for (int r = 0; r < Rows; r++)
            {
                int ox = 0;

                for (int c = 0; c < Columns; c++)
                {
                    V frame = Items[c, r];

                    if (frame != null)
                    {
                        frame.OX = ox;
                        frame.OY = oy;

                        frame.Update();

                        if (frame.RenderPriority != 0)
                            PriorityRenderOrder.Add(frame, frame);

                    }

                    ox += ColumnWidth(c);
                }

                oy += RowHeight(r);
            }
        }

        protected override void RenderContentFrames()
        {
            foreach ((V key, V item) in PriorityRenderOrder)
                item?.Render();

            if (RenderTopDown)
                RenderContentTopDown();
            else
                RenderContentLeftRight();
        }

        protected virtual void RenderItem(int x, int y)
        {
            V item = Items[x, y];

            if (item != null && !PriorityRenderOrder.ContainsKey(item))
                item.Render();
        }

        protected virtual void RenderContentTopDown()
        {
            for (int y = 0; y < Rows; y ++)
            {
                for (int x = 0; x < Columns; x ++)
                {
                    RenderItem(x, y);
                }
            }
        }

        protected virtual void RenderContentLeftRight()
        {
            for (int x = 0; x < Columns; x ++)
            {
                for (int y = 0; y < Rows; y ++)
                {
                    RenderItem(x, y);
                }
            }
        }

        protected override void RenderDebugOverlays()
        {
            base.RenderDebugOverlays();

            int oy = 0;

            for (int r = 0; r < Rows; r ++)
            {
                Pixel.FrameRect(CX, CY + oy, CW, 1, Color.Red);
                oy += RowHeight(r);
            }

            Pixel.FrameRect(CX, CY + oy, CW, 1, Color.Red);

            int ox = 0;

            for(int c = 0; c < Columns; c ++)
            {
                Pixel.FrameRect(CX + ox, CY, 1, CH, Color.Red);
                ox += ColumnWidth(c);
            }

            Pixel.FrameRect(CX + ox, CY, 1, CH, Color.Red);

        }

        protected virtual void CalculatePartitions()
        {
            int rowSpace, rowExpanders;
            FlexibleRowSpace(out rowSpace, out rowExpanders);
            // Height assigned to expanding rows
            int expanderHeight = rowExpanders == 0 ? 0 : rowSpace / rowExpanders;

            for (int r = 0; r < Rows; r ++)
            {
                if (PartitionRows)
                    RowHeights[r] = PartitionRowHeight(r);
                else
                {
                    if (RowExpanders[r] != 0)
                        RowHeights[r] = expanderHeight;
                    else 
                        RowHeights[r] = MaxRowHeights[r];
                }
            }

            int colSpace, colExpanders;
            FlexibleColumnSpace(out colSpace, out colExpanders);
            // Width assigned to expanding columns
            int expanderWidth = colExpanders == 0 ? 0 : colSpace / colExpanders;

            for (int c = 0; c < Columns; c ++)
            {
                if (PartitionColumns)
                    ColumnWidths[c] = PartitionColumnWidth(c);
                else 
                    ColumnWidths[c] = ColExpanders[c] == 0 ? MaxColWidths[c] : expanderWidth;
            }
        }

        protected virtual void ApplyPartitions()
        {
            ForEachItem((index, item) => 
            {
                if (item == null)
                    return;

                item.W = ColumnWidth(index.X);
                item.H = RowHeight(index.Y);

                if (1 < item.SpanColumns)
                {
                    for (int sx = index.X + 1; sx < index.X + item.SpanColumns; sx ++)
                        item.W += ColumnWidth(sx);
                }

                if (1 < item.SpanRows)
                {
                    for (int sy = index.Y + 1; sy < index.Y + item.SpanRows; sy ++)
                        item.H += RowHeight(sy);
                }

                item.Resize();
            });
        }

        protected virtual void CalculateMeasuredSize()
        {
            _measuredHeight = 0;

            for (int r = 0; r < Rows; r ++)
            {
                int maxHeight = 0;

                for (int c = 0; c < Columns; c ++)
                {
                    V frame = Items[c, r];

                    if (frame == null)
                        continue;

                    maxHeight = Math.Max(maxHeight, frame.MeasuredHeight);
                    maxHeight = Math.Max(maxHeight, frame.RH);
                }

                _measuredHeight += maxHeight;
            }

            _measuredWidth = 0;

            for (int c = 0; c < Columns; c ++)
            {
                int maxWidth = 0;

                for (int r = 0; r < Rows; r ++)
                {
                    V frame = Items[c, r];

                    if (frame == null)
                        continue;

                    maxWidth = Math.Max(maxWidth, frame.MeasuredWidth);
                    maxWidth = Math.Max(maxWidth, frame.RW);
                }

                _measuredWidth += maxWidth;
            }
        }

        protected void ForEachItem(DualReceiver<Vec2i, V> receiver)
        {
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Columns; c++)
                {
                    V frame = Items[c, r];

                    if (frame == null)
                        continue;

                    receiver(new Vec2i(c, r), frame);
                }
            }
        }
    }
}