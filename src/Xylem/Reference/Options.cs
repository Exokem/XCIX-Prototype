
using Xylem.Data;
using Xylem.Registration;
using Xylem.Component;

namespace Xylem.Reference
{
    public static class XylemOptions
    {
        public static readonly ValueOptionPackage DebugOptions = OptionPackage.Import<ValueOptionPackage>("xylem_debug_options");

        public static string DefaultTypefaceIdentifier = DebugOptions.Get<string>("default_typeface");

        public static Typeface DefaultTypeface => R.Typefaces[DefaultTypefaceIdentifier];
    }
}