
using System.Collections.Generic;
using Xylem.Functional;

namespace Xylem.Vectors
{
    public class Direction
    {
        private static readonly Dictionary<string, Direction> _index = new Dictionary<string, Direction>();

        private static readonly Dictionary<Vec2i, Direction> _offsetIndex = new Dictionary<Vec2i, Direction>();

        private static readonly List<Direction> _values = new List<Direction>();
        private static readonly List<Direction> _cardinals = new List<Direction>();
        private static readonly List<Direction> _unions = new List<Direction>();
        private static readonly List<Direction> _intersections = new List<Direction>();
        
        public static readonly Direction UP, RIGHT, DOWN, LEFT;
        public static readonly Direction UP_AND_LEFT, UP_AND_RIGHT, DOWN_AND_LEFT, DOWN_AND_RIGHT;
        public static readonly Direction UP_LEFT, UP_RIGHT, DOWN_LEFT, DOWN_RIGHT;

        static Direction()
        {
            UP    = new Direction( 0, -1, _isCardinal: true, name: "up");
            RIGHT = new Direction( 1,  0, _isCardinal: true, name: "right");
            DOWN  = new Direction( 0,  1, _isCardinal: true, name: "down");
            LEFT  = new Direction(-1,  0, _isCardinal: true, name: "left");

            // Represent two adjacent perpendicular directions, but not the resultant
            // direction between them
            UP_AND_LEFT    = new Direction(-1, -1, _isUnion: true, name: "up_and_left");
            UP_AND_RIGHT   = new Direction( 1, -1, _isUnion: true, name: "up_and_right");
            DOWN_AND_RIGHT = new Direction( 1,  1, _isUnion: true, name: "down_and_right");
            DOWN_AND_LEFT  = new Direction(-1,  1, _isUnion: true, name: "down_and_left");

            // Represent the resultant direction between two perpendicular directions
            UP_LEFT    = new Direction(-1, -1, _isIntersection: true, name: "up_left");
            UP_RIGHT   = new Direction( 1, -1, _isIntersection: true, name: "up_right");
            DOWN_RIGHT = new Direction( 1,  1, _isIntersection: true, name: "down_right");
            DOWN_LEFT  = new Direction(-1,  1, _isIntersection: true, name: "down_left");
        }

        public readonly string Name;
        public readonly int OX, OY;
        public readonly int Ordinal, CardinalOrdinal = -1, UnionOrdinal = -1, IntersectionOrdinal = -1;
        public readonly bool IsCardinal, IsUnion, IsIntersection;
        public readonly bool IsHorizontal, IsVertical;

        public readonly bool IsUp, IsDown;

        private Direction(int ox, int oy, bool _isCardinal = false, bool _isUnion = false, bool _isIntersection = false, string name = null)
        {
            Name = name;
            _index[name] = this;

            OX = ox;
            OY = oy;

            _offsetIndex[new Vec2i(ox, oy)] = this;

            IsHorizontal = OX != 0 && OY == 0;
            IsVertical = OY != 0 && OX == 0;

            IsUp = name.Contains("up");
            IsDown = name.Contains("down");

            Ordinal = _values.Count;
            _values.Add(this);

            IsCardinal = _isCardinal;
            IsUnion = _isUnion;
            IsIntersection = _isIntersection;

            if (IsCardinal)
            {
                CardinalOrdinal = _cardinals.Count;
                _cardinals.Add(this);
            }
            
            if (IsUnion)
            {
                UnionOrdinal = _unions.Count;
                _unions.Add(this);
            }

            if (IsIntersection)
            {
                IntersectionOrdinal = _intersections.Count;
                _intersections.Add(this);
            }
        }

        public void Offset(int x, int y, out int ox, out int oy)
        {
            ox = x + OX;
            oy = y + OY;
        }

        public void Offset(ref int x, ref int y)
        {
            x += OX;
            y += OY;
        }

        public Vec2i Offset(int x, int y) => new Vec2i(x + OX, y + OY);
        public Vec2i Offset(Vec2i v) => new Vec2i(v.X + OX, v.Y + OY);

        public Vec2i OffsetX(Vec2i v) => new Vec2i(v.X + OX, v.Y);
        public Vec2i OffsetY(Vec2i v) => new Vec2i(v.X, v.Y + OY);

        public Direction NextCardinal()
        {
            if (CardinalOrdinal < 0)
                return this;

            int ordinal = CardinalOrdinal;
            
            if (++ ordinal == _cardinals.Count)
                ordinal = 0;

            return _cardinals[ordinal];
        }

        public Direction PreviousCardinal()
        {
            if (CardinalOrdinal < 0)
                return this;

            int ordinal = CardinalOrdinal;
            
            if (-- ordinal < 0)
                ordinal = _cardinals.Count - 1;

            return _cardinals[ordinal];
        }

        public Direction Union(Direction other)
        {
            // Union is only allowed for two distinct perpendicular cardinal directions
            if (other == this || !(IsCardinal && other.IsCardinal) || (IsHorizontal && other.IsHorizontal))
                return this;

            Direction vertical = IsVertical ? this : other;
            Direction horizontal = IsHorizontal ? this : other;

            return Of($"{vertical.Name}_and_{horizontal.Name}");
        }

        public Direction Intersection(Direction other)
        {
            // Intersection is only allowed for two distinct perpendicular cardinal directions
            if (other == this || !(IsCardinal && other.IsCardinal) || (IsHorizontal && other.IsHorizontal))
                return this;

            Direction vertical = IsVertical ? this : other;
            Direction horizontal = IsHorizontal ? this : other;

            return Of($"{vertical.Name}_{horizontal.Name}");
        }

        public override bool Equals(object obj)
        {
            if (obj is Direction dir)
            {
                return dir.OX == OX && dir.OY == OY && dir.Ordinal == Ordinal;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Ordinal;
        }

        // Summary:
        //      Iterates over the standard 8 adjacency directions
        public static IEnumerable<Direction> StandardValues()
        {
            for (int i = 0; i < _cardinals.Count; i++)
                yield return _cardinals[i];

            for (int i = 0; i < _intersections.Count; i++)
                yield return _intersections[i];
        }

        public static IEnumerable<Direction> Values()
        {
            for (int i = 0; i < _values.Count; i++)
                yield return _values[i];
        }

        public static IEnumerable<Direction> Cardinals()
        {
            for (int i = 0; i < _cardinals.Count; i++)
                yield return _cardinals[i];
        }

        public static IEnumerable<Direction> Unions()
        {
            for (int i = 0; i < _unions.Count; i++)
                yield return _unions[i];
        }

        public static IEnumerable<Direction> Intersections()
        {
            for (int i = 0; i < _intersections.Count; i++)
                yield return _intersections[i];
        }

        public static void ForUnion(Looper<Direction> looper)
        {
            for (int i = 0; i < _unions.Count; i ++)
                looper(i, _unions[i]);
        }

        public static Direction Intersection(int i) => _intersections[i];

        public static int Count => _values.Count;

        public static Direction Of(string name)
        {
            return _index.GetOrDefault(name, null);
        }

        public static Direction Of(Vec2i offsetVec)
        {
            return _offsetIndex.GetOrDefault(offsetVec, null);
        }
    }
}