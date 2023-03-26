
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Xylem.Framework
{
    public class RenderPriorityComparer : IComparer<Frame>
    {
        public int Compare([AllowNull] Frame x, [AllowNull] Frame y)
        {
            if (x == null && y == null)
                return 0;
            
            if (x == null)
                return -1;

            if (y == null)
                return 1;

            return y.RenderPriority - x.RenderPriority;
        }
    }
}