
using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Xylem.Data;
using Xylem;
using Xylem.Reference;
using Xylem.Registration;
using Xylem.Functional;
using Xylem.Reflection;
using Xylem.Framework;
using Vitreous.Component.Spatial;

namespace Vitreous.Framework.Editor
{
    [ModuleSource("Vitreous")]
    public static class EditorData
    {
        const string DefaultContent = "{\n\t\"entries\":[]\n}";

        public static readonly DirectoryInfo EditorDataDirectory;
        public static readonly FileInfo EditorSectorDataFile;
        public static readonly FileInfo EditorAreaDataFile;

        public static readonly List<Sector> EditorSectors;
        public static readonly List<Area> EditorAreas;

        static EditorData()
        {
            EditorDataDirectory = Importer.ExecutionDirectory.Directory("Editor");
            EditorSectorDataFile = EditorDataDirectory.File("editor_sectors.json");
            EditorAreaDataFile = EditorDataDirectory.File("editor_areas.json");

            EditorDataDirectory.Verify();
            EditorSectorDataFile.Verify(DefaultContent);
            EditorAreaDataFile.Verify(DefaultContent);

            EditorSectors = Load<Sector>(EditorSectorDataFile, data => new Sector(data));
            EditorAreas = Load<Area>(EditorAreaDataFile, data => new Area(data));
        }

        private static List<V> Load<V>(FileInfo source, Function<JObject, V> constructor) where V : RegistryEntry
        {
            source.Verify();

            List<V> entries = new List<V>();

            JObject data = JObject.Parse(source.ReadString());

            if (data.ContainsKey(K.Entries) && data[K.Entries] is JArray array)
            {
                foreach (var token in array)
                {
                    if (token is JObject entryData)
                    {
                        try
                        {
                            entries.Add(constructor(entryData));
                        }

                        catch (Exception e)
                        {
                            Output.Suggest($"Entry loading failed in file '{source.Name}': {e.Message}");
                            Output.Suggest(e.StackTrace);
                        }
                    }
                }
            }

            return entries;
        }

        internal static void Save()
        {
            Save<Sector>(EditorSectorDataFile, EditorSectors);
            Save<Area>(EditorAreaDataFile, EditorAreas);
        }

        internal static void Save<V>(FileInfo target, List<V> entries) where V : RegistryEntry
        {
            JObject data = new JObject();

            data[K.Entries] = J.WriteSlimArray(entries);

            target.WriteString(data.ToString());

            NotificationManager.EnqueueNotification($"Saved '{typeof(V)}' Entries");
        }
    }
}