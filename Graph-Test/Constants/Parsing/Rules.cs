using System;
using System.Collections.Generic;
using System.Text;

namespace Graph_Test.Constants.Parsing
{
    /// <summary>
    /// Rules required for correct parsing and entity differentiation
    /// </summary>
    static class Rules
    {
        /// <summary>
        /// String containing the name of the property that contains the entity value of the object
        /// </summary>
        public static readonly string ENTITY_PROPERTY = "_entity";
        /// <summary>
        /// String containing the name of the property that contains the unique value for object uniqueness
        /// </summary>
        public static readonly string UNIQUE_PROPERTY = "id";
    }
}
