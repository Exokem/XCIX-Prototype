
using System;
using System.IO;

namespace Xylem.Data
{
    public static class DirectoryInfoExtensions
    {
        public static void WriteData(FileInfo file, string data)
        {
            try 
            {
                System.IO.File.WriteAllText(file.FullName, data);
            }

            catch (Exception e)
            {
                Output.Suggest($"Write operation failed: {e.Message}");
                Output.Suggest(e.StackTrace);
            }
        }

        public static string ReadData(FileInfo file)
        {
            try 
            {
                return System.IO.File.ReadAllText(file.FullName);
            }

            catch (Exception e)
            {
                Output.Suggest($"Read operation failed: {e.Message}");
                Output.Suggest(e.StackTrace);

                return "";
            }
        }

        public static FileInfo File(this DirectoryInfo container, string file)
        {
            return new FileInfo($"{container.FullName}\\{file}");
        }

        public static DirectoryInfo Directory(this DirectoryInfo container, string directory)
        {
            return new DirectoryInfo($"{container.FullName}\\{directory}");
        }

        public static void Verify(this FileInfo file, string defaultContent = null)
        {
            if (!file.Exists)
            {
                file.Create();
                if (defaultContent != null)
                    file.WriteString(defaultContent);
            }
        }

        public static void Verify(this DirectoryInfo directory)
        {
            if (!directory.Exists)
                directory.Create();
        }

        public static void WriteString(this FileInfo file, string data)
        {
            WriteData(file, data);
        }

        public static string ReadString(this FileInfo file)
        {
            return ReadData(file);
        }
    }
}