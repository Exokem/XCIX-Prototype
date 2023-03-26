
using System.Collections.Generic;
using Xylem.Framework;
using Xylem.Functional;
using Xylem.Registration;
using Xylem.Framework.Control;
using Xylem.Framework.Layout;
using Vitreous.Component.Spatial;

namespace Vitreous.Framework.Editor
{
    public partial class Editor : Frame
    {
        ContextMenu SectorEntryMenu => new ContextMenu()
            .Add("New Sector", () => PostNewEntryDialog("Sector", AddSector))
            .Add("Rename Sector", (a, b, c) => PostRenameEntryDialog<Sector>("Sector", a, b, c));

        ContextMenu SectorListMenu => new ContextMenu()
            .Add("New Sector", () => PostNewEntryDialog("Sector", AddSector));

        ContextMenu AreaEntryMenu => new ContextMenu()
            .Add("New Area", () => PostNewEntryDialog("Area", AddArea))
            .Add("Rename Area", (a, b, c) => PostRenameEntryDialog<Area>("Area", a, b, c));
            
        ContextMenu AreaListMenu => new ContextMenu()
            .Add("New Area", () => PostNewEntryDialog("Area", AddArea));

        internal void PostNewEntryDialog(string entryType, Receiver<string> addFunction)
        {
            FormDialog.Post($"New {entryType}", true, new[]{"Identifier"}, new string[0], entries => NewEntryReceiver(entries, addFunction));
        }

        internal void PostRenameEntryDialog<V>(string entryType, ContextMenu menu, Frame menuItem, Frame source) where V : RegistryEntry
        {
            if (source is RegistryEntryItem<V> item)
            {
                FormDialog.Post($"Rename {entryType}", true, new[]{"Identifier"}, new[]{$"{item.Item.Identifier}"}, entries => RenameEntryReceiver<V>(entries, item));
            }
        }

        internal void NewEntryReceiver(Dictionary<string, string> entries, Receiver<string> addFunction)
        {
            string identifier = entries.GetOrDefault("Identifier", "").Trim();

            if (identifier.Length != 0)
                addFunction(identifier);
        }

        internal void RenameEntryReceiver<V>(Dictionary<string, string> entries, RegistryEntryItem<V> item) where V : RegistryEntry
        {
            string identifier = entries.GetOrDefault("Identifier", "").Trim();

            if (identifier.Length != 0)
                item.Item.Identifier = identifier;
        }
    }
}