
using System;
using Microsoft.Xna.Framework.Input;

namespace Xylem.Input
{
    public static class KeyLookup
    {
        public static Keys FromName(string name)
        {
            return (Keys) Enum.Parse(typeof(Keys), name);
        }

        public static string ToName(Keys key)
        {
            return Enum.GetName(typeof(Keys), key);
        }
    }
}