
using Xylem.Component;
using Xylem.Interaction;
using Xylem.Reference;
using Xylem.Data;
using Xylem.Reflection;

namespace Xylem.Registration
{
    [ModuleRegistryInitializer("Xylem")]
    public static class R
    {
        public static readonly OptionDataPackageRegistry Options;

        public static readonly InteractionRegistry<Effect> Effects;
        public static readonly InteractionRegistry<Cause> Causes;

        public static readonly Registry<InputBindingEntry> InputBindings;

        public static readonly Registry<TextureResource> Textures;
        public static readonly Registry<Typeface> Typefaces;

        static R()
        {
            Options = new OptionDataPackageRegistry("Options", K.OptionsPackage);

            Effects = new InteractionRegistry<Effect>("Interactions\\Effects", K.Effect);
            Causes = new InteractionRegistry<Cause>("Interactions\\Causes", K.Cause);

            InputBindings = new Registry<InputBindingEntry>("Interactions\\InputBindings", "input_binding");

            Textures = new Registry<TextureResource>("Resources\\Textures", "texture");
            Typefaces = new Registry<Typeface>("Resources\\Typefaces", "typeface");
        }

        public static void Initialize() {}
    }
}