
namespace Xylem.Vectors
{
    public struct Vec2i
    {
        public int X, Y;

        public Vec2i(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static Vec2i operator +(Vec2i a, Vec2i b)
        {
            return new Vec2i(a.X + b.X, a.Y + b.Y);
        }

        public static Vec2i operator -(Vec2i a, Vec2i b)
        {
            return new Vec2i(a.X - b.X, a.Y - b.Y);
        }

        public static bool operator <(Vec2i a, Vec2i b)
        {
            return a.X < b.X || a.Y < b.Y;
        }

        public static bool operator >(Vec2i a, Vec2i b)
        {
            return b.X < a.X || b.Y < a.Y;
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }
    }

    public struct Vec3i
    {
        public int X, Y, Z;

        public Vec3i(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }
}