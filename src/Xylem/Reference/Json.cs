using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Xylem.Functional;

namespace Xylem.Reference
{
    public static class J
    {
        public static bool ContainsArray(JObject data, string key, out JArray array)
        {
            bool result = data.ContainsKey(key) && data[key] is JArray;
            array = result ? (JArray) data[key] : null;
            return result;
        }
        
        public static bool ContainsString(JObject data, string key, out string value)
        {
            bool result = data.ContainsKey(key) && data[key].Type == JTokenType.String;
            value = result ? (string) data[key] : null;
            return result;
        }

        public static string ReadString(JObject data, string key)
        {
            if (data.ContainsKey(key))
                return (string) data[key];
            else throw new NullReferenceException($"Failed to extract string token for key '{key}':\n {data}");
        }

        public static string ReadString(JObject data, string key, string defaultValue)
        {
            if (data.ContainsKey(key))
                return (string) data[key];
            else return defaultValue;
        }

        public static Type ReadType(JObject data, string key, Type defaultType)
        {
            if (data.ContainsKey(key))
                return Type.GetType((string) data[key]);
            else return defaultType;
        }

        public static int ReadInt(JObject data, string key, int defaultValue)
        {
            if (data.ContainsKey(key))
                return (int) data[key];
            else return defaultValue;
        }

        public static int ReadInt(JObject data, string key)
        {
            if (data.ContainsKey(key))
                return (int) data[key];
            else throw new NullReferenceException($"Failed to extract int token for key '{key}':\n {data}");
        }

        public static float ReadFloat(JObject data, string key, float defaultValue)
        {
            if (data.ContainsKey(key))
                return (float) data[key];
            else 
                return defaultValue;
        }

        public static bool ReadBool(JObject data, string key, bool defaultValue)
        {
            if (data.ContainsKey(key))
                return (bool) data[key];
            else return defaultValue;
        }

        public static bool ReadBool(JObject data, string key)
        {
            if (data.ContainsKey(key))
                return (bool) data[key];
            else throw new NullReferenceException($"Failed to extract bool token for key '{key}':\n {data}");
        }

        public delegate void JObjectParser(JObject data);

        public static void ReadArray(JObject data, string key, JObjectParser parser) 
        {
            if (data.ContainsKey(key))
            {
                if (data[key] is JArray array)
                {
                    foreach (var token in array)
                    {
                        if (token is JObject entry)
                            parser(entry);
                    }
                }
            }
        }

        public static void ReadArrayStrings(JObject data, string key, Receiver<string> parser)
        {
            if (data.ContainsKey(key))
            {
                if (data[key] is JArray array)
                {
                    foreach (var token in array)
                    {
                        if (token.Type == JTokenType.String)
                            parser((string) token);
                    }
                }
            }
        }

        public delegate void JTokenParser(JToken data);

        public static void ReadArrayTokens(JObject data, string key, JTokenParser parser) 
        {
            if (data.ContainsKey(key))
            {
                if (data[key] is JArray array)
                {
                    foreach (var token in array)
                        parser(token);
                }
            }
        }

        public delegate void JGridEntryParser(int x, int y, JToken data);

        public static void ReadGrid(JObject data, string key, JGridEntryParser parser)
        {
            if (data.ContainsKey(key))
            {
                int y = 0;
                if (data[key] is JArray rows)
                {
                    foreach (JToken rowToken in rows)
                    {
                        int x = 0;
                        if (rowToken is JArray row)
                        {
                            foreach (JToken entry in row)
                                parser(x ++, y, entry);
                        }

                        y ++;
                    }
                }
            }
        }

        public static void WriteGrid<V>(JObject data, string key, V[,] items, Function<V, JToken> exporter, int rows, int columns)
        {
            JArray rowData = new JArray();
            for (int y = 0; y < rows; y ++)
            {
                JArray row = new JArray();
                for (int x = 0; x < columns; x ++)
                {
                    V item = items[x, y];
                    row.Add(exporter(item));
                }

                rowData.Add(row);
            }

            data[key] = rowData;
        }

        public static JArray WriteArray<V>(IEnumerable<V> contents) where V : JsonComposite
        {
            JArray array = new JArray();

            foreach (V v in contents)
                array.Add(v.Export());

            return array;
        }

        public static JArray WriteSlimArray<V>(IEnumerable<V> contents) where V : JsonComposite
        {
            JArray array = new JArray();

            foreach (V v in contents)
                array.Add(v.ExportSlim());

            return array;
        }
    }

    public abstract class JsonComposite
    {
        public abstract void Export(JObject data);

        public JObject Export()
        {
            JObject data = new JObject();
            Export(data);
            return data;
        }

        public virtual void ExportSlim(JObject data) => Export(data);

        public JObject ExportSlim()
        {
            JObject data = new JObject();
            ExportSlim(data);
            return data;
        }
    }

    // public interface JsonComposite<V> : JsonComposite where V : JsonComposite
    // {
    //     public virtual void Import(ref V v, JObject data) {}
    // }
}
