
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Microsoft.Xna.Framework;
using Xylem.Component;
using Xylem.Graphics;
using Xylem.Graphics.Patchwork;
using Xylem.Reference;
using Xylem.Vectors;
using Xylem.Registration;
using Vitreous.Registration;

namespace Vitreous.Component.Composite
{
    public class FloorEntry : DescribedEntry
    {
        public readonly TextureResource Texture;
        public readonly TextureResource Connections;

        private readonly AbstractPatchworkConnector _patchwork;

        public static FloorEntry Empty => Registries.Floors[K.Empty];

        public FloorEntry(JObject data) : base(data)
        {
            Texture = R.Textures[J.ReadString(data, K.Resource, null)];
            Connections = R.Textures[J.ReadString(data, K.Connections, null)];

            if (Connections != null)
            {
                string patchworkConnectorClass = J.ReadString(data, K.PatchworkConnector, null);

                if (patchworkConnectorClass == null)
                    _patchwork = AbstractPatchworkConnector.Create(typeof(OverheadPatchworkConnector), Connections, 1, 2);
                else
                    _patchwork = AbstractPatchworkConnector.Create(patchworkConnectorClass, Connections);
                    // _patchwork = Introspector.Instantiate<AbstractPatchworkConnector>(patchworkConnectorClass, typeof(AbstractPatchworkConnector), Connections);
            }
        }

        public void RenderWithin(Rectangle area, IEnumerable<Direction> connections)
        {
            if (this == Empty)
                return;

            Texture?.IRenderWithin(area);
            _patchwork?.RenderWithin(area, connections);
            _patchwork?.RenderDebugOverlay(area, connections);
        }
    }

    public class Floor : DescribedInstance<FloorEntry>
    {
        public static readonly Floor Empty = new Floor(FloorEntry.Empty);

        public bool IsEmpty => Reference == FloorEntry.Empty;

        public TextureResource Texture => Reference.Texture;

        public Floor(FloorEntry floor) : base(floor)
        {

        }

        public Floor(JObject data) : base(data, Registries.Floors)
        {

        }
    }
}