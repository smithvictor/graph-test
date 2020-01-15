# Graph Test
## JSON flattening C# .NET Core utility
### Usage

The tool would flatten a JSON structure to a relational database like structure inside another JSON.

Two parameters are required for the tool to work.
1. `Input file path`.
2. `Output file path`. This parameter is optional, in case of not providing it the program would output the JSON to the console output.

### Relationships

* The program would consider a **parent** of an object if the object is contained inside an array that happens to be another object property. This would result in adding another property to the `child` object representing the relationship that these two objects have.

* The program would consider a **parent** of an object if the object is contained inside an object that contains an **entity** property. This would result in adding another property to the `child` object representing the relationship that these two objects have.

### JSON Schema considerations

The program would only work if the entities that want to be flattened have two required properties:
1. Entity name.
2. Unique identifier.

These two properties can be change inside the program to parse another `key` from the objects properties.
