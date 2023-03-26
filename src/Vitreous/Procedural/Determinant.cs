
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Xylem.Component;
using Xylem.Functional;
using Xylem.Vectors;
using Xylem.Reflection;
using Vitreous.Component.Core;
using Vitreous.Component.Composite;

namespace Vitreous.Procedural
{
    public abstract class AbstractResourceDeterminant
    {
        public static V Create<V>(System.Type type, params object[] args) where V : AbstractResourceDeterminant 
        {
            return Introspector.Instantiate<V>(type, typeof(AbstractResourceDeterminant), args);
        }

        private readonly Dictionary<string, Rectangle> _resourcePartitions;
        
        private bool _finalized;

        public readonly TextureResource Resource;

        protected AbstractResourceDeterminant(TextureResource resource)
        {
            // Order matters, leave as-is
            _resourcePartitions = new Dictionary<string, Rectangle>();
            Resource = resource;

            PartitionResource();
            _finalized = true;
        }

        // Summary: 
        //      Determines how the texture resource should be partitioned, and how each
        //      partition should be designated.
        protected abstract void PartitionResource();

        // Summary:
        //      Determines which partition of the texture resource should be used based on
        //      the input attributes, states, and qualifiers.
        public abstract Rectangle DeterminePartition
        (
            OrderedSet<Attribute> attributes,
            OrderedSet<State> states,
            OrderedSet<Qualifier> qualifiers
        );

        public Rectangle GetResourcePartition(string key) => _resourcePartitions[key];

        protected void SetResourcePartition(string key, Rectangle partition)
        {
            if (!_finalized)
                _resourcePartitions[key] = partition;
        }
    }

    public abstract class StructureResourceDeterminant : AbstractResourceDeterminant
    {
        protected StructureResourceDeterminant(TextureResource resource) : base(resource) {}

        // Determines whether the current aggregate state of a structure allows it to 
        // receive an incoming connection from the specified direction. 
        public abstract bool ConnectionAllowed
        (
            Direction direction, 
            OrderedSet<Attribute> attributes,
            OrderedSet<State> states,
            OrderedSet<Qualifier> qualifiers
        );

        public bool ConnectionAllowed(Direction direction, Structure structure) => ConnectionAllowed(direction, structure.Attributes, structure.States, structure.Qualifiers);

        public Rectangle DeterminePartition(Structure structure) => DeterminePartition(structure.Attributes, structure.States, structure.Qualifiers);
    }

    // Summary:
    //      Determines the partitioning for the resources of door structures.
    //
    //      The format for texture resources accepted by this resource determinant is as
    //      follows.
    //
    //      1. The texture is divided into four, horizontally adjacent, sections of equal
    //         width.
    //      2. In left-to-right order, the sections correspond to: horizontal closed, 
    //         vertical closed, horizontal open, vertical open.
    //
    public class DoorResourceDeterminant : StructureResourceDeterminant
    {
        protected static readonly string HZC = "hzc";
        protected static readonly string VTC = "vtc";
        protected static readonly string HZO = "hzo";
        protected static readonly string VTO = "vto";

        public DoorResourceDeterminant(TextureResource resource) : base(resource) {}

        protected override void PartitionResource()
        {
            int w = Resource.Texture.Width / 4;
            int h = Resource.Texture.Height;

            SetResourcePartition(HZC, new Rectangle(w * 0, 0, w, h));
            SetResourcePartition(VTC, new Rectangle(w * 1, 0, w, h));
            SetResourcePartition(HZO, new Rectangle(w * 2, 0, w, h));
            SetResourcePartition(VTO, new Rectangle(w * 3, 0, w, h));
        }

        public override Rectangle DeterminePartition
        (
            OrderedSet<Attribute> attributes,
            OrderedSet<State> states,
            OrderedSet<Qualifier> qualifiers
        )
        {
            bool open = qualifiers["open"].Value;

            StateValue axis = states["axis"].Value;

            if (axis.Identifier == "horizontal")
                return GetResourcePartition(open ? HZO : HZC);
            else if (axis.Identifier == "vertical")
                return GetResourcePartition(open ? VTO : VTC);

            // Default to horizontal closed
            return GetResourcePartition(HZC);
        }

        public override bool ConnectionAllowed
        (
            Direction direction, 
            OrderedSet<Attribute> attributes,
            OrderedSet<State> states,
            OrderedSet<Qualifier> qualifiers
        )
        {
            StateValue axis = states["axis"].Value;

            if (axis.Identifier == "horizontal")
                return direction.IsHorizontal;
            else if (axis.Identifier == "vertical")
                return direction.IsVertical;
            else 
                return false;
        }
    }
}