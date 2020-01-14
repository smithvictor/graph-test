using System;
using System.Collections.Generic;
using System.Text;

namespace Graph_Test.Constants.Error
{
    /// <summary>
    /// Contains the enum and constants definitions for errors that can occur on the tool
    /// </summary>
    static class ToolError
    {
        /// <summary>
        /// Error enum containing the posible error cases on the tool
        /// </summary>
        public enum Error
        {
            MissingArguments,
            FileNotFound,
            JSONParse,
        }

        /// <summary>
        /// Dictionary containing the relation Error -> String for user information
        /// </summary>
        static readonly Dictionary<Error, string> errorStrings = new Dictionary<Error, string>
        {
            {Error.MissingArguments, "The file path for processing was not included on the startup arguments."},
            {Error.FileNotFound, "The specified file could not be found."},
            {Error.JSONParse, "The contents of the file could not be parsed correctly."}
        };

        /// <summary>
        /// Gets the string asociated to the Error that is being provided
        /// </summary>
        /// <param name="err">Error to be described</param>
        /// <returns></returns>
        public static string GetErrorString(Error err)
        {
            return errorStrings[err];
        }
    }
}
