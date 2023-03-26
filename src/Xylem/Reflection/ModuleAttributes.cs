
namespace Xylem.Reflection
{
    public abstract class ModuleAttribute : System.Attribute
    {
        public readonly string Identifier;

        protected ModuleAttribute(string identifier)
        {
            Identifier = identifier;
        }
    }

    public class ModuleSource : ModuleAttribute
    {
        public ModuleSource(string identifier) : base(identifier) {}
    }

    public class ModuleRegistryInitializer : ModuleAttribute
    {
        public ModuleRegistryInitializer(string identifier) : base(identifier) {}
    }

    public class ModuleOptions : ModuleAttribute
    {
        public ModuleOptions(string identifier) : base(identifier) {}
    }
}