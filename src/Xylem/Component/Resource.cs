
using Newtonsoft.Json.Linq;
using Microsoft.Xna.Framework.Graphics;
using Xylem.Data;
using Xylem.Graphics;
using Xylem.Reference;
using Xylem.Registration;
using Vitreous.Registration;

namespace Xylem.Component
{
    public abstract class ResourceEntry : RegistryEntry
    {
        protected readonly string _resourceKey;

        protected ResourceEntry(JObject data) : base(data)
        {
            _resourceKey = J.ReadString(data, K.Resource);
        }
    }

    public class TextureResource : ResourceEntry, IRenderedComponent
    {
        public Texture2D Texture { get; }

        public TextureResource(JObject data) : base(data)
        {
            Texture = Importer.ImportTextureResource(R.Textures.Folder, _resourceKey);
        }
    }
}