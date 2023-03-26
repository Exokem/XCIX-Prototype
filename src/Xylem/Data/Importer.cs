
using System.IO;
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Microsoft.Xna.Framework.Graphics;
using Xylem.Registration;
using Xylem.Graphics;
using Xylem.Reference;

namespace Xylem.Data
{
    public delegate void FileReceiver(FileInfo file);
    public delegate void DirectoryReceiver(DirectoryInfo directory, FileReceiver fileReceiver);

    public static class Importer
    {
        public static readonly DirectoryInfo ContentDirectory;

        public static DirectoryInfo ExecutionDirectory { get; private set; }
        public static DirectoryInfo ModuleDirectory { get; private set; }

        public static string CurrentModule { get; private set; }

        private static HashSet<string> OrderedModules;
        private static List<string> ModuleOrder;
        private static Dictionary<string, DirectoryInfo> ModuleDirectories;
        private static Dictionary<string, List<string>> ModuleDependencies;

        public static IEnumerable<string> IndexedModuleOrder => ModuleOrder;

        static Importer()
        {
            ContentDirectory = new DirectoryInfo(Source.Instance.Content.RootDirectory);

            string assemblyLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;

            ExecutionDirectory = new DirectoryInfo(assemblyLocation).Parent;
            ModuleDirectory = ExecutionDirectory.Directory("Modules");

            OrderedModules = new HashSet<string>();
            ModuleOrder = new List<string>();
            ModuleDirectories = new Dictionary<string, DirectoryInfo>();
            ModuleDependencies = new Dictionary<string, List<string>>();
        }

        public static void IndexModules()
        {
            if (!ModuleDirectory.Exists)
                ModuleDirectory.Create();

            IndexImportOrder();
            IndexModuleDescriptors();
            CheckDependencies();
        }

        private static void IndexImportOrder()
        {
            FileInfo modulePriorities = ModuleDirectory.File("module_priorities.jsonc");

            if (!modulePriorities.Exists)
            {
                modulePriorities = ModuleDirectory.File("module_priorities.json");

                if (!modulePriorities.Exists)
                {
                    Output.Suggest($"Module indexing failed: 'module_priorities' (.json/jsonc) not found in module directory");
                    return;
                }
            }

            try 
            {
                JObject data = JObject.Parse(File.ReadAllText(modulePriorities.FullName));
                
                if (!data.ContainsKey(K.Order))
                    throw new KeyNotFoundException();

                J.ReadArrayStrings(data, K.Order, idn => 
                {
                    if (OrderedModules.Contains(idn))
                    {
                        Output.Suggest($"Module '{idn}' duplicated in import order, using earliest inclusion");
                    }

                    else 
                    {
                        OrderedModules.Add(idn);
                        ModuleOrder.Add(idn);
                    }
                });
            }

            catch (Newtonsoft.Json.JsonReaderException e)
            {
                Output.Suggest($"Module indexing failed: 'module_priorities' JSON could not be parsed");
                Output.Suggest(e.StackTrace);
            }

            catch (KeyNotFoundException)
            {
                Output.Suggest($"Module indexing failed: 'module_priorities' JSON missing required key '${K.Order}'");
            }

            catch (InvalidCastException)
            {
                Output.Suggest($"Module indexing failed: 'module_priorities' JSON contains invalid entries in array '{K.Order}'");
            }

            Output.Write($"Indexed module import order with {ModuleOrder.Count} entries");
        }

        private static void IndexModuleDescriptors()
        {
            foreach (DirectoryInfo module in ModuleDirectory.GetDirectories())
            {
                FileInfo moduleInfo = module.File("module_info.jsonc");

                if (!moduleInfo.Exists)
                {
                    moduleInfo = module.File("module_info.json");

                    if (!moduleInfo.Exists)
                    {
                        Output.Suggest($"Invalid Module '{module.Name}' missing required 'module_info' (.json/jsonc)");
                        continue;
                    }
                }

                JObject moduleData = JObject.Parse(File.ReadAllText(moduleInfo.FullName));

                try 
                {
                    if (!moduleData.ContainsKey(K.Identifier))
                        throw new KeyNotFoundException(K.Identifier);

                    string identifier = (string) moduleData[K.Identifier];

                    ModuleDirectories[identifier] = module;

                    List<string> moduleDependencies = new List<string>();

                    J.ReadArrayStrings(moduleData, K.Dependencies, moduleDependencies.Add);

                    ModuleDependencies[identifier] = moduleDependencies;
                }

                catch (KeyNotFoundException e)
                {
                    Output.Suggest($"Module indexing failed for '{module.Name}': JSON missing required key '{e.Message}' ");
                    continue;
                }
            }

            Output.Write($"Indexed {ModuleDirectories.Count} valid modules");
        }

        private static void CheckDependencies()
        {
            HashSet<string> checkedModules = new HashSet<string>();

            foreach (string moduleIdentifier in ModuleOrder)
            {
                foreach (string dependencyIdentifier in ModuleDependencies[moduleIdentifier])
                {
                    if (!checkedModules.Contains(dependencyIdentifier))
                        Output.Suggest($"Module '{moduleIdentifier}' indexed before dependency '{dependencyIdentifier}'");
                }

                checkedModules.Add(moduleIdentifier);
            }
        }

        public static Texture2D ImportTextureResource(string resourceDirectory, string resource)
        {
            FileInfo resourceFile = ModuleDirectory.File($"{CurrentModule}\\{resourceDirectory}\\{resource}");

            return Texture2D.FromFile(GraphicsContext.Device, resourceFile.FullName);
        }

        public static void ImportModules()
        {
            foreach (string moduleIdentifier in ModuleOrder)
            {
                if (!ModuleDirectories.ContainsKey(moduleIdentifier))
                {
                    Output.Suggest($"Import failed for module '{moduleIdentifier}': module appears in import order but its descriptor was not indexed");
                    continue;
                }

                DirectoryInfo moduleDirectory = ModuleDirectories[moduleIdentifier];

                CurrentModule = moduleDirectory.Name;

                foreach (Registry registry in Registry.Registries())
                {
                    int initial = registry.Size;

                    DirectoryInfo registryDir = moduleDirectory.Directory(registry.Folder);

                    if (!registryDir.Exists)
                        continue;

                    if (registry.ImportDirectory != null)
                    {
                        DirectoryInfo[] subdirs = registryDir.GetDirectories();
                        foreach (DirectoryInfo dir in subdirs)
                            registry.ImportDirectory(dir, registry.ImportFile);
                    }
                    
                    if (registry.ImportFile != null)
                    {
                        FileInfo[] files = registryDir.GetFiles();
                        foreach (FileInfo file in files)
                            registry.ImportFile(file);
                    }

                    Output.Write($"Imported {registry.Size - initial} '{registry.Key}'", CurrentModule);
                }

                CurrentModule = null;
            }
        }

        // public static void ImportModules()
        // {
        //     if (!ModuleDirectory.Exists)
        //         ModuleDirectory.Create();

        //     foreach (DirectoryInfo moduleDir in ModuleDirectory.GetDirectories())
        //     {
        //         CurrentModule = moduleDir.Name;

        //         foreach (Registry registry in Registry.Registries())
        //         {
        //             int initial = registry.Size;

        //             DirectoryInfo registryDir = moduleDir.Directory(registry.Folder);

        //             if (!registryDir.Exists)
        //                 continue;

        //             if (registry.ImportDirectory != null)
        //             {
        //                 DirectoryInfo[] subdirs = registryDir.GetDirectories();
        //                 foreach (DirectoryInfo dir in subdirs)
        //                     registry.ImportDirectory(dir, registry.ImportFile);
        //             }
                    
        //             if (registry.ImportFile != null)
        //             {
        //                 FileInfo[] files = registryDir.GetFiles();
        //                 foreach (FileInfo file in files)
        //                     registry.ImportFile(file);
        //             }

        //             Output.Write($"Imported {registry.Size - initial} '{registry.Key}'", CurrentModule);
        //         }

        //         CurrentModule = null;
        //     }
        // }

        public static void ImportRecursive(DirectoryInfo directory, FileReceiver action)
        {
            if (!directory.Exists)
                return;

            foreach (FileInfo file in directory.GetFiles())
            {
                if (file.Exists)
                    action(file);
            }

            foreach (DirectoryInfo directoryInfo in directory.GetDirectories())
            {
                if (directoryInfo.Exists)
                {
                    ImportRecursive(directoryInfo, action);
                }
            }
        }
    }
}
