using System.Collections.Generic;
using Xylem.Functional;
using Xylem.Vectors;

namespace Xylem.Graphics.Patchwork
{
    public abstract class AbstractConnectionProcessor
    {
        // Summary:
        //      Clears any cached connections.
        public abstract void ClearCache();

        // Summary:
        //      Resolves any overlaps among the cached connections.
        protected abstract void ResolveOverlaps();
    }

    // Summary:
    //      An extension of the base AbstractConnectionProcessor that only needs to track
    //      the direction of each connection.
    public abstract class FunctionalConnectionProcessor : AbstractConnectionProcessor
    {
        // Summary: 
        //      Scans for connections around the specified vector position using the
        //      provided comparator to determine when a connection exists. This function
        //      is not responsible for cache clearing or overlap resolution.
        public abstract void ScanConnections(Vec2i v, DualFunction<Vec2i, Vec2i, bool> comparator);

        // Summary:
        //      Allows external iteration over all cached connections. 
        public abstract IEnumerable<Direction> Connections();

        // Summary:
        //      Wraps the main processor functionality into a specific standard sequence.
        public void Process(Vec2i v, DualFunction<Vec2i, Vec2i, bool> comparator)
        {
            ClearCache();
            ScanConnections(v, comparator);
            ResolveOverlaps();
        }
    }

    // Summary:
    //      An extension of the base AbstractConnectionProcessor that intends to attach
    //      extra data to each cached connection direction.
    public abstract class OrnateConnectionProcessor<V> : AbstractConnectionProcessor
    {
        public abstract V this[Direction direction] { get; }

        public abstract void ScanConnections(Vec2i v, DualFunction<Vec2i, Vec2i, bool> comparator, Function<Vec2i, V> accessor);

        // Summary:
        //      Allows external iteration over all cached connections. 
        public abstract IEnumerable<KeyValuePair<Direction, V>> Connections();

        public void Process(Vec2i v, DualFunction<Vec2i, Vec2i, bool> comparator, Function<Vec2i, V> accessor)
        {
            ClearCache();
            ScanConnections(v, comparator, accessor);
            ResolveOverlaps();
        }
    }

    public class ExtraConnectionProcessor<V> : OrnateConnectionProcessor<V>
    {
        private readonly Dictionary<Direction, V> _connectionData;

        public ExtraConnectionProcessor()
        {
            _connectionData = new Dictionary<Direction, V>();
        }

        public override void ClearCache() => _connectionData.Clear();

        public override IEnumerable<KeyValuePair<Direction, V>> Connections() => _connectionData;

        public override void ScanConnections(Vec2i v, DualFunction<Vec2i, Vec2i, bool> comparator, Function<Vec2i, V> accessor)
        {
            foreach (Direction dir in Direction.StandardValues())
            {
                Vec2i query = dir.Offset(v);

                if (comparator(v, query))
                    _connectionData[dir] = accessor(query);
            }
        }

        protected override void ResolveOverlaps() {}

        public override V this[Direction direction] => _connectionData[direction];
    }

    public class ConnectionProcessor : FunctionalConnectionProcessor
    {
        private readonly HashSet<Direction> _cachedCardinals;
        private readonly HashSet<Direction> _cachedUnions;
        private readonly HashSet<Direction> _cachedIntersections;

        public ConnectionProcessor()
        {
            _cachedCardinals = new HashSet<Direction>();
            _cachedUnions = new HashSet<Direction>();
            _cachedIntersections = new HashSet<Direction>();
        }

        public override IEnumerable<Direction> Connections()
        {
            foreach (var card in _cachedCardinals)
                yield return card;
            foreach (var union in _cachedUnions)
                yield return union;
            foreach (var inter in _cachedIntersections)
                yield return inter;
        }

        public override void ClearCache()
        {
            _cachedCardinals.Clear();
            _cachedUnions.Clear();
            _cachedIntersections.Clear();
        }

        public override void ScanConnections(Vec2i v, DualFunction<Vec2i, Vec2i, bool> comparator)
        {
            HashSet<Direction> cardinals = new HashSet<Direction>();

            // ABC: anticlockwise, between, clockwise; e.g. LEFT, UP_LEFT, UP
            foreach (Direction a in Direction.Cardinals())
            {
                // Get next clockwise direction as well as intersection
                // e.g. UP -> RIGHT & UP_RIGHT
                Direction c = a.NextCardinal();
                Direction b = a.Intersection(c);

                // Calculate offset and compare only if not done already
                // Determine if structure at each offset matches this one
                // The separate cardinals set is meant to prevent recalculations
                bool linkA = cardinals.Contains(a) || comparator(a.Offset(v), v);
                bool linkB = cardinals.Contains(b) || comparator(b.Offset(v), v);
                bool linkC = cardinals.Contains(c) || comparator(c.Offset(v), v);

                cardinals.AddIf(linkA, a);
                cardinals.AddIf(linkB, b);
                cardinals.AddIf(linkC, c);

                if (linkA)
                    _cachedCardinals.Add(a);
                if (linkC)
                    _cachedCardinals.Add(c);

                if (linkA && linkB && linkC)
                    _cachedIntersections.Add(b);
                else if (linkA && linkC)
                    _cachedUnions.Add(a.Union(c));
            }
        }

        // This is very badly written
        protected override void ResolveOverlaps()
        {
            HashSet<Direction> culledCards = new HashSet<Direction>();

            foreach (Direction card in _cachedCardinals)
            {
                if (card.IsVertical)
                {
                    if (_cachedUnions.Contains(Direction.Of(card.Name + "_and_left")) && _cachedUnions.Contains(Direction.Of(card.Name + "_and_right")))
                    {
                        culledCards.Add(card);
                    }

                    else if (_cachedIntersections.Contains(Direction.Of(card.Name + "_left")) && _cachedIntersections.Contains(Direction.Of(card.Name + "_right")))
                    {
                        culledCards.Add(card);
                    }
                }

                else 
                {
                    if (card == Direction.LEFT)
                    {
                        if (_cachedUnions.Contains(Direction.UP_AND_LEFT) && _cachedUnions.Contains(Direction.DOWN_AND_LEFT))
                        {
                            culledCards.Add(card);
                        }

                        else if (_cachedIntersections.Contains(Direction.UP_LEFT) && _cachedIntersections.Contains(Direction.DOWN_LEFT))
                        {
                            culledCards.Add(card);
                        }
                    }

                    else if (card == Direction.RIGHT)
                    {
                        if (_cachedUnions.Contains(Direction.UP_AND_RIGHT) && _cachedUnions.Contains(Direction.DOWN_AND_RIGHT))
                        {
                            culledCards.Add(card);
                        }

                        else if (_cachedIntersections.Contains(Direction.UP_RIGHT) && _cachedIntersections.Contains(Direction.DOWN_RIGHT))
                        {
                            culledCards.Add(card);
                        }
                    }
                }
            }

            foreach (var card in culledCards)
                _cachedCardinals.Remove(card);
        }
    }
}