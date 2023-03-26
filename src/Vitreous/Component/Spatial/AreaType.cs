
using Newtonsoft.Json.Linq;
using Xylem.Component;
using Xylem.Registration;
using Xylem.Reference;
using Vitreous.Registration;

namespace Vitreous.Component.Spatial
{
    public class AreaType : RegistryEntry
    {
        public static AreaType Unknown => Registries.AreaTypes[K.Unknown];

        public readonly TextureResource Resource;

        public AreaType(JObject data) : base(data)
        {
            if (data.ContainsKey(K.Resource))
                Resource = R.Textures[J.ReadString(data, K.Resource)];
        }
    }
}