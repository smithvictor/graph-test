using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using Err = Graph_Test.Constants.Error;
using Rules = Graph_Test.Constants.Parsing;

namespace Graph_Test
{
    /// <summary>
    /// JSON flat conversion tool
    /// </summary>
    class Program
    {
        private static JObject resultJSON = new JObject();
        /// <summary>
        /// Process the file specified inside the arguments of execution
        /// </summary>
        /// <param name="args">String array containing the path of the path to be read on the first element and the path of the output file to be written. If the output file is ignored the output would be displayed on the console output</param>
        static void Main(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                Console.WriteLine(Err.ToolError.GetErrorString(Err.ToolError.Error.MissingArguments));
            }
            string fileContent;
            try
            {
                fileContent = File.ReadAllText(args[0]);
            }
            catch
            {
                Console.WriteLine(Err.ToolError.GetErrorString(Err.ToolError.Error.FileNotFound));
                return;
            }
            JArray json;
            try
            {
                json = JArray.Parse(fileContent);
            }
            catch
            {
                Console.WriteLine(Err.ToolError.GetErrorString(Err.ToolError.Error.JSONParse));
                return;
            }
            ProcessArray(json);
            Console.WriteLine(JsonConvert.SerializeObject(resultJSON, Formatting.Indented));
            Console.ReadLine();
        }

        static void ProcessArray(JArray array, JObject parent = null)
        {
            foreach (JObject o in array)
            {
                ProcessObject(o, parent);
            }
        }

        static void ProcessObject(JObject obj, JObject parent = null)
        {
            if (obj.Type == JTokenType.Object)
            {
                JObject newValue = new JObject();
                JObject son = null;
                JArray relationships = null;
                foreach (JProperty p in obj.Properties())
                {
                    var value = p.Value;
                    if (value.Type == JTokenType.Array)
                    {
                        relationships = (JArray)value;
                    }
                    else if (value.Type == JTokenType.Object)
                    {
                        son = (JObject)value;
                    }
                    else
                    {
                        newValue.Add(p.Name, p.Value);
                    }
                }
                if (relationships != null)
                {
                    ProcessArray(relationships, newValue);
                }
                if (son != null && son.Property(Rules.Rules.ENTITY_PROPERTY) != null)
                {
                    ProcessObject(son, null);
                    newValue.Add($"{son[Rules.Rules.ENTITY_PROPERTY].Value<string>().ToLower()}{ToTitleCase(Rules.Rules.UNIQUE_PROPERTY)}", son.Property(Rules.Rules.UNIQUE_PROPERTY).Value);
                }
                if (parent != null)
                {
                    string relationshipId = $"{parent[Rules.Rules.ENTITY_PROPERTY].Value<string>().ToLower()}{ToTitleCase(Rules.Rules.UNIQUE_PROPERTY)}";
                    if (newValue.Property(relationshipId) == null)
                    {
                        newValue.Add(relationshipId, parent.Property(Rules.Rules.UNIQUE_PROPERTY).Value);
                    }
                }
                string entity = newValue[Rules.Rules.ENTITY_PROPERTY].Value<string>();
                JProperty res = resultJSON.Property(entity);
                JArray entityGroup = res == null ? new JArray() : (JArray)res.Value;
                if (res == null)
                {
                    resultJSON.Add(entity, entityGroup);
                }
                VerifyAndAddValue(newValue, entityGroup, entity);
            }
        }

        static void VerifyAndAddValue(JObject obj, JArray group, string entity)
        {
            if (group.Count == 0)
            {
                group.Add(obj);
                return;
            }
            var id = obj[Rules.Rules.UNIQUE_PROPERTY].Value<int>();
            var search = group.Children().Where(c => c[Rules.Rules.UNIQUE_PROPERTY].Value<int>() == id).Count();
            if (search == 0)
            {
                group.Add(obj);
            }
        }

        static string ToTitleCase(string title)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(title.ToLower());
        }
    }
}
