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
            string result = JsonConvert.SerializeObject(resultJSON, Formatting.Indented);
            if (args.Length == 2)
            {
                File.WriteAllText(args[1],result);
                Console.WriteLine($"File successfully saved at {args[1]}");
            }
            else
            {
                Console.WriteLine(result);
            }
            Console.WriteLine("\n\n\nPress a key to close the program...");
            Console.ReadLine();
        }
        /// <summary>
        /// Iterates over a JObject array as a JArray and sends them to another process with the provided JObject parameter
        /// </summary>
        /// <param name="array">JArray used for iteration</param>
        /// <param name="parent">JObject to be passed as a parameter for each element</param>
        static void ProcessArray(JArray array, JObject parent = null)
        {
            foreach (JObject o in array)
            {
                ProcessObject(o, parent);
            }
        }
        /// <summary>
        /// Iterates over the properties of a JObject to be flatten later 
        /// </summary>
        /// <param name="obj">JObject to be flattened</param>
        /// <param name="parent">JObject that has a parent child relationship to the current element</param>
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
                        //In case of an array of nested objects the current element or Object would be considered the parent of the array elements
                        relationships = (JArray)value;
                    }
                    else if (value.Type == JTokenType.Object)
                    {
                        //If the property contains an Object as a value it would be considered as its father if it is identified as an entity, otherwise added as a attribute or property
                        son = (JObject)value;
                        if (son.Property(Rules.Rules.ENTITY_PROPERTY) == null)
                        {
                            newValue.Add(p.Name, value);
                        }
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
                    //The notation for id relationships would be {parentEntityName}{capitalizedId}
                    newValue.Add($"{son[Rules.Rules.ENTITY_PROPERTY].Value<string>().ToLower()}{ToTitleCase(Rules.Rules.UNIQUE_PROPERTY)}", son.Property(Rules.Rules.UNIQUE_PROPERTY).Value);
                }
                if (parent != null)
                {
                    //The notation for id relationships would be {parentEntityName}{capitalizedId}
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
                VerifyAndAddValue(newValue, entityGroup);
            }
        }
        /// <summary>
        /// Searches the global json object for a match using the JObject provided, if the object is not on the entity group it is added, otherwise ignored
        /// </summary>
        /// <param name="obj">Object used as search parameter</param>
        /// <param name="group">Group of entities to search and add</param>
        static void VerifyAndAddValue(JObject obj, JArray group)
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
        /// <summary>
        /// Converts the string to a capitalized form based on the OS culture information and returns it on a new string object
        /// </summary>
        /// <param name="title">String to be capitalized</param>
        /// <returns></returns>
        static string ToTitleCase(string title)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(title.ToLower());
        }
    }
}
