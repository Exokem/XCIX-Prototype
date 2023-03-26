
using System.Collections.Generic;
using System;
using Newtonsoft.Json.Linq;
using Microsoft.Xna.Framework;
using Xylem.Component;
using Xylem.Graphics;
using Xylem.Graphics.Patchwork;
using Xylem.Vectors;
using Xylem.Reflection;
using Xylem.Reference;
using Xylem.Registration;
using Vitreous.Procedural;
using Vitreous.Registration;
using Vitreous.Reference;

namespace Vitreous.Component.Composite
{
    public class StructureConnectionOverlay
    {
        public readonly string Reference;
        private readonly StructureResourceDeterminant _determinant;
        private readonly AbstractPatchworkConnector _patchwork;

        public StructureConnectionOverlay(JObject data)
        {
            Reference = J.ReadString(data, K.Reference);

            // if (!Registries.Structures.Has(Reference))
            //     throw new InvalidOperationException($"Reference identifier specified for structure connection overlay does not exist in registry: {Reference}");

            TextureResource resource = R.Textures[J.ReadString(data, K.Overlay)];

            string determinantClass = J.ReadString(data, VK.ResourceDeterminant);

            _determinant = Introspector.Instantiate<StructureResourceDeterminant>(determinantClass, typeof(StructureResourceDeterminant), resource);

            string patchworkClass = J.ReadString(data, K.PatchworkConnector);

            _patchwork = Introspector.Instantiate<AbstractPatchworkConnector>(patchworkClass, typeof(AbstractPatchworkConnector), resource);
        }

        public void RenderWithin(Rectangle area, Direction direction, Structure structure)
        {
            // Resource determinant determines which section of the texture matches the state
            // Patchwork splits that section based on the direction

            // Wow, this worked really well
            // No fancy new classes required, just used the existing ones in a new way

            // Area in the texture that should be partitioned
            Rectangle stateArea = _determinant.DeterminePartition(structure);

            // Rectangle source = _patchwork.GetPatchWithin(stateArea, direction);
            // Rectangle dest = _patchwork.GetPatchWithin(area, direction);

            _patchwork.RenderWithinDynamic(area, stateArea, direction);
        }
    }

    /**
     * A Structure is primarily composed of an Element that describes its Parts. The
     * properties of a Structure may also be represented by its Attributes and States, as
     * well as by specific Qualifiers.
     *
     * TODO: The Element of a Structure can be an existing element or an element defined
     * TODO: within the Structure definition.
     */
    public class StructureEntry : DescribedEntry
    {
        public static StructureEntry Empty => Registries.Structures[K.Empty];

        // The element of a structure describes its parts
        protected readonly ElementEntry Element;
        public readonly TextureResource Texture;
        public readonly TextureResource Connections;
        public readonly TextureResource Partitions;

        private readonly AbstractPatchworkConnector _patchwork;
        private readonly StructureResourceDeterminant _determinant;

        internal readonly Type ContainerType;
        internal readonly JObject ContainerData;

        private readonly Dictionary<string, StructureConnectionOverlay> _connectionOverlays;

        public bool HasTexture => Texture != null;

        public StructureEntry(JObject data) : base(data)
        {
            Element = Registries.Elements[J.ReadString(data, VK.Element, Identifier)];
            Texture = R.Textures[J.ReadString(data, K.Resource, null)];
            Connections = R.Textures[J.ReadString(data, K.Connections, null)];
            Partitions = R.Textures[J.ReadString(data, VK.Partitions, null)];

            if (data.ContainsKey(K.Container) && data[K.Container] is JObject containerData)
            {
                ContainerType = J.ReadType(containerData, K.Type, typeof(NullContainer));
                ContainerData = containerData;
            }

            else
            {
                ContainerType = typeof(NullContainer);
                ContainerData = new JObject();
            }

            if (Connections != null)
            {
                Type patchworkType = J.ReadType(data, K.PatchworkConnector, typeof(OverheadPatchworkConnector));

                _patchwork = AbstractPatchworkConnector.Create(patchworkType, Connections);
            }

            if (Partitions != null)
            {
                Type determinantType = J.ReadType(data, VK.ResourceDeterminant, null);

                if (determinantType != null)
                    _determinant = Introspector.Instantiate<StructureResourceDeterminant>(determinantType, typeof(StructureResourceDeterminant), Partitions);
            }

            // Import Extra Connection Overlays

            _connectionOverlays = new Dictionary<string, StructureConnectionOverlay>();

            J.ReadArray(data, K.ExtraConnections, obj => 
            {
                StructureConnectionOverlay overlay = new StructureConnectionOverlay(obj);
                if (overlay.Reference != Identifier)
                    _connectionOverlays[overlay.Reference] = overlay;
            });
        }

        // Does this structure have an extra connection to the structure of the specified 
        // identifier.
        public bool HasExtraConnection(string identifier) => _connectionOverlays.ContainsKey(identifier);

        public bool HasAnyExtraConnections() => _connectionOverlays.Count != 0;

        public void RenderWithin(Rectangle area, Structure instance, IEnumerable<Direction> connections, IEnumerable<KeyValuePair<Direction, Structure>> extraConnections) 
        {
            if (this == Empty)
                return;

            if (_determinant == null)
                Texture?.IRenderWithin(area);

            else if (instance != null)
            {
                Rectangle partition = _determinant.DeterminePartition(instance.Attributes, instance.States, instance.Qualifiers);
                Partitions?.IRenderWithinPartial(area, partition);
            }

            else 
                Texture?.IRenderWithin(area);

            _patchwork?.RenderWithin(area, connections);
            RenderExtraConnections(area, extraConnections);
            _patchwork?.RenderDebugOverlay(area, connections);
        }

        protected void RenderExtraConnections(Rectangle area, IEnumerable<KeyValuePair<Direction, Structure>> extraConnections)
        {
            foreach (var entry in extraConnections)
            {
                Structure other = entry.Value;

                if (_connectionOverlays.ContainsKey(other.Identifier))
                    _connectionOverlays[other.Identifier].RenderWithin(area, entry.Key, other);
            }
        }

        public bool ConnectionAllowed(Direction direction, Structure instance) => _determinant.ConnectionAllowed(direction, instance);
    }

    public class Structure : DescribedInstance<StructureEntry>
    {
        public static readonly Structure Empty = new Structure(StructureEntry.Empty);

        public TextureResource Texture => Reference.Texture;
        public bool IsEmpty => Reference == StructureEntry.Empty;
        public ElementContainer Elements;

        public Structure(StructureEntry structure) : base(structure)
        {
            Elements = Introspector.Instantiate<ElementContainer>(Reference.ContainerType, typeof(ElementContainer), Reference.ContainerData);
        }

        public Structure(JObject data) : base(data, Registries.Structures)
        {
            if (data.ContainsKey(K.Container))
                Elements = ElementContainer.Create<ElementContainer>(Reference.ContainerType, Reference.ContainerData);
        }

        public bool ConnectionAllowed(Direction direction) => Reference.ConnectionAllowed(direction, this);

        public override void ImportExtraData(JObject data)
        {
            J.ReadArray(data, VK.Attributes, attributeData => 
            {
                string reference = J.ReadString(attributeData, K.Reference, null);

                if (reference != null && Attributes.Contains(reference))
                    Attributes[reference].ImportExtraData(attributeData);
            });

            J.ReadArray(data, VK.States, stateData =>
            {
                string reference = J.ReadString(stateData, K.Reference, null);

                if (reference != null && States.Contains(reference))
                    States[reference].ImportExtraData(stateData);
            });

            J.ReadArray(data, VK.Qualifiers, qualifierData =>
            {
                string reference = J.ReadString(qualifierData, K.Reference, null);

                if (reference != null && Qualifiers.Contains(reference))
                    Qualifiers[reference].ImportExtraData(qualifierData);
            });
        }

        // public override bool Equals(object obj)
        // {
        //     if (obj is Structure other)
        //     {
        //         return other.Reference == Reference && base.Equals(other);
        //     }

        //     return false;
        // }
    }
}