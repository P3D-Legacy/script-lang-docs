using Kolben.Adapters;
using System;

namespace DocsEmitter
{
    static class BuiltInTypes
    {
        public static ApiPrototype[] GetBuiltInPrototypes()
        {
            return new[]
            {
                // array
                new ApiPrototype
                {
                    Name = "Array",
                    Description = "Prototype for the primitve array[] type.",
                    IsBuiltIn = true,
                    Variables = new ApiPrototypeVariable[0],
                    Methods = new[]
                    {
                        new ApiMethod
                        {
                            Name = "constructor",
                            Description = "Takes any number of arguments, types can be mixed.",
                            FunctionType = ScriptFunctionType.Constructor,
                            Signatures = new[]
                            {
                                new ApiMethodSignature
                                {
                                    ParamNames = new[] { "...args" },
                                    ParamTypes = new[] { "any[]" },
                                    ReturnTypes = new[] { TypeHelper.GetTypeName(typeof(NetUndefined), 1) },
                                    OptionalNum = 1
                                }
                            }
                        },
                        new ApiMethod
                        {
                            Name = "IndexerGet",
                            Description = "Returns the item in the array at index position.",
                            FunctionType = ScriptFunctionType.IndexerGet,
                            Signatures = new[]
                            {
                                new ApiMethodSignature
                                {
                                    ParamNames = new[] { "index" },
                                    ParamTypes = new[] { TypeHelper.GetTypeName(typeof(int)) },
                                    ReturnTypes = new[] { "any" }
                                }
                            }
                        },
                        new ApiMethod
                        {
                            Name = "IndexerSet",
                            Description = "Overwrites the item in the array at index position.",
                            FunctionType = ScriptFunctionType.IndexerSet,
                            Signatures = new[]
                            {
                                new ApiMethodSignature
                                {
                                    ParamNames = new[] { "index" },
                                    ParamTypes = new[] { TypeHelper.GetTypeName(typeof(int)) },
                                    ReturnTypes = new[] { TypeHelper.GetTypeName(typeof(NetUndefined), 1) }
                                }
                            }
                        },
                        new ApiMethod
                        {
                            Name = "length",
                            Description = "Gets the amount of items in the array.",
                            FunctionType = ScriptFunctionType.Getter,
                            Signatures = new[]
                            {
                                new ApiMethodSignature
                                {
                                    ReturnTypes = new[] { TypeHelper.GetTypeName(typeof(int), 1) }
                                }
                            }
                        },
                        new ApiMethod
                        {
                            Name = "includes",
                            Description = "Determines whether the array includes an item.<br />" +
                                "Comparer example: </i><code>a.includes(item, (a, b) => { a.id == b.id; });</code><i>",
                            FunctionType = ScriptFunctionType.Standard,
                            Signatures = new[]
                            {
                                new ApiMethodSignature
                                {
                                    ParamNames = new[] { "item" },
                                    ParamTypes = new[] { "any" },
                                    ReturnTypes = new[] { TypeHelper.GetTypeName(typeof(bool), 1) }
                                },
                                new ApiMethodSignature
                                {
                                    ParamNames = new[] { "item", "comparer" },
                                    ParamTypes = new[] { "any", "function" },
                                    ReturnTypes = new[] { TypeHelper.GetTypeName(typeof(bool), 1) }
                                }
                            }
                        },
                        new ApiMethod
                        {
                            Name = "any",
                            Description = "If the array contains any items matching the search.",
                            FunctionType = ScriptFunctionType.Standard,
                            Signatures = new[]
                            {
                                new ApiMethodSignature
                                {
                                    ParamNames = new string[0],
                                    ParamTypes = new string[0],
                                    ReturnTypes = new[] { TypeHelper.GetTypeName(typeof(bool), 1) }
                                },
                                new ApiMethodSignature
                                {
                                    ParamNames = new[] { "comparer" },
                                    ParamTypes = new[] { "function" },
                                    ReturnTypes = new[] { TypeHelper.GetTypeName(typeof(bool), 1) }
                                }
                            }
                        },
                        new ApiMethod
                        {
                            Name = "where",
                            Description = "Filters the array with the given search.",
                            FunctionType = ScriptFunctionType.Standard,
                            Signatures = new[]
                            {
                                new ApiMethodSignature
                                {
                                    ParamNames = new[] { "filter" },
                                    ParamTypes = new[] { "function" },
                                    ReturnTypes = new[] { "any[]" }
                                }
                            }
                        },
                        new ApiMethod
                        {
                            Name = "first",
                            Description = $"Returns the first item in the array that matches the search or <span class=\"arg-type\">{TypeHelper.LinkType("undefined")}</span> for no match.",
                            FunctionType = ScriptFunctionType.Standard,
                            Signatures = new[]
                            {
                                new ApiMethodSignature
                                {
                                    ParamNames = new string[0],
                                    ParamTypes = new string[0],
                                    ReturnTypes = new[] { "any" }
                                },
                                new ApiMethodSignature
                                {
                                    ParamNames = new[] { "comparer" },
                                    ParamTypes = new[] { "function" },
                                    ReturnTypes = new[] { "any" }
                                }
                            }
                        },
                        new ApiMethod
                        {
                            Name = "last",
                            Description = $"Returns the last item in the array that matches the search or <span class=\"arg-type\">{TypeHelper.LinkType("undefined")}</span> for no match.",
                            FunctionType = ScriptFunctionType.Standard,
                            Signatures = new[]
                            {
                                new ApiMethodSignature
                                {
                                    ParamNames = new string[0],
                                    ParamTypes = new string[0],
                                    ReturnTypes = new[] { "any" }
                                },
                                new ApiMethodSignature
                                {
                                    ParamNames = new[] { "comparer" },
                                    ParamTypes = new[] { "function" },
                                    ReturnTypes = new[] { "any" }
                                }
                            }
                        },
                        new ApiMethod
                        {
                            Name = "select",
                            Description = "Transforms all elements of the array with a transformation function.",
                            FunctionType = ScriptFunctionType.Standard,
                            Signatures = new[]
                            {
                                new ApiMethodSignature
                                {
                                    ParamNames = new[] { "transformer" },
                                    ParamTypes = new[] { "function" },
                                    ReturnTypes = new[] { "any[]" }
                                }
                            }
                        },
                        new ApiMethod
                        {
                            Name = "single",
                            Description = "Finds a single item within the array and returns it.",
                            FunctionType = ScriptFunctionType.Standard,
                            Signatures = new[]
                            {
                                new ApiMethodSignature
                                {
                                    ParamNames = new string[0],
                                    ParamTypes = new string[0],
                                    ReturnTypes = new[] { "any" }
                                },
                                new ApiMethodSignature
                                {
                                    ParamNames = new[] { "comparer" },
                                    ParamTypes = new[] { "function" },
                                    ReturnTypes = new[] { "any" }
                                }
                            }
                        },
                        new ApiMethod
                        {
                            Name = "count",
                            Description = "Returns the amount of items in the array that match the search.",
                            FunctionType = ScriptFunctionType.Standard,
                            Signatures = new[]
                            {
                                new ApiMethodSignature
                                {
                                    ParamNames = new string[0],
                                    ParamTypes = new string[0],
                                    ReturnTypes = new[] { TypeHelper.GetTypeName(typeof(int), 1) }
                                },
                                new ApiMethodSignature
                                {
                                    ParamNames = new[] { "comparer" },
                                    ParamTypes = new[] { "function" },
                                    ReturnTypes = new[] { TypeHelper.GetTypeName(typeof(int), 1) }
                                }
                            }
                        },
                        new ApiMethod
                        {
                            Name = "all",
                            Description = "Returns whether all items in the array conform to a constraint.",
                            FunctionType = ScriptFunctionType.Standard,
                            Signatures = new[]
                            {
                                new ApiMethodSignature
                                {
                                    ParamNames = new[] { "constraint" },
                                    ParamTypes = new[] { "function" },
                                    ReturnTypes = new[] { TypeHelper.GetTypeName(typeof(bool), 1) }
                                }
                            }
                        },
                        new ApiMethod
                        {
                            Name = "push",
                            Description = "Adds items to the end of the array.",
                            FunctionType = ScriptFunctionType.Standard,
                            Signatures = new[]
                            {
                                new ApiMethodSignature
                                {
                                    ParamNames = new[] { "...items" },
                                    ParamTypes = new[] { "any[]" },
                                    ReturnTypes = new[] { TypeHelper.GetTypeName(typeof(NetUndefined), 1) }
                                }
                            }
                        },
                        new ApiMethod
                        {
                            Name = "pop",
                            Description = "Removes the last item from the array and returns it.",
                            FunctionType = ScriptFunctionType.Standard,
                            Signatures = new[]
                            {
                                new ApiMethodSignature
                                {
                                    ParamNames = new string[0],
                                    ParamTypes = new string[0],
                                    ReturnTypes = new[] { "any" }
                                }
                            }
                        },
                    }
                },
                // boolean
                new ApiPrototype
                {
                    Name = "Boolean",
                    Description = "Prototype for the primitive bool type.",
                    IsBuiltIn = true,
                    Variables = new ApiPrototypeVariable[0],
                    Methods = new []
                    {
                        new ApiMethod
                        {
                            Name = "constructor",
                            FunctionType = ScriptFunctionType.Constructor,
                            Signatures = new[]
                            {
                                new ApiMethodSignature
                                {
                                    ParamNames = new[] { "value" },
                                    ParamTypes = new[] { TypeHelper.GetTypeName(typeof(bool)) },
                                    ReturnTypes = new[] { TypeHelper.GetTypeName(typeof(NetUndefined), 1) }
                                }
                            }
                        },
                    }
                },
                // number
                new ApiPrototype
                {
                    Name = "Number",
                    Description = "Prototype for the primitive number type.",
                    IsBuiltIn = true,
                    Variables = new ApiPrototypeVariable[0],
                    Methods = new []
                    {
                        new ApiMethod
                        {
                            Name = "constructor",
                            FunctionType = ScriptFunctionType.Constructor,
                            Signatures = new[]
                            {
                                new ApiMethodSignature
                                {
                                    ParamNames = new[] { "value" },
                                    ParamTypes = new[] { TypeHelper.GetTypeName(typeof(double)) },
                                    ReturnTypes = new[] { TypeHelper.GetTypeName(typeof(NetUndefined), 1) }
                                }
                            }
                        },
                    }
                },
                // object
                new ApiPrototype
                {
                    Name = "Object",
                    Description = "The base prototype for all prototypes. Methods from this prototype are available to all prototypes.",
                    IsBuiltIn = true,
                    Variables = new ApiPrototypeVariable[0],
                    Methods = new []
                    {
                        new ApiMethod
                        {
                            Name = "create",
                            Description = "Creates an instance of a prototype (either by name or reference to the prototype), \"args\" are the constructor arguments for the prototype.",
                            FunctionType = ScriptFunctionType.Standard,
                            IsStatic = true,
                            Signatures = new[]
                            {
                                new ApiMethodSignature
                                {
                                    ParamNames = new[] { "prototypeName", "...args" },
                                    ParamTypes = new[] { TypeHelper.GetTypeName(typeof(string)), "any[]" },
                                    ReturnTypes = new[] { "any" }
                                },
                                new ApiMethodSignature
                                {
                                    ParamNames = new[] { "prototype", "...args" },
                                    ParamTypes = new[] { TypeHelper.GetTypeName(typeof(Object)), "any[]" },
                                    ReturnTypes = new[] { "any" }
                                },
                            }
                        },
                        new ApiMethod
                        {
                            Name = "addMember",
                            Description = "Adds a member to a prototype and all new objects created from that prototype. Values for \"signatureConfig\" are \"readOnly\", \"static\", \"indexerGet\" and \"indexerSet\".",
                            FunctionType = ScriptFunctionType.Standard,
                            IsStatic = true,
                            Signatures = new[]
                            {
                                new ApiMethodSignature
                                {
                                    ParamNames = new[] { "memberName", "defaultValue", "signatureConfig" },
                                    ParamTypes = new[] { TypeHelper.GetTypeName(typeof(string)), "any", TypeHelper.GetTypeName(typeof(string[])) },
                                    ReturnTypes = new[] { TypeHelper.GetTypeName(typeof(NetUndefined), 1) },
                                    OptionalNum = 2
                                },
                            }
                        },
                    }
                },
                // string
                new ApiPrototype
                {
                    Name = "String",
                    Description = "Prototype for the primitive string type.",
                    IsBuiltIn = true,
                    Variables = new ApiPrototypeVariable[0],
                    Methods = new []
                    {
                        new ApiMethod
                        {
                            Name = "constructor",
                            FunctionType = ScriptFunctionType.Constructor,
                            Signatures = new[]
                            {
                                new ApiMethodSignature
                                {
                                    ParamNames = new[] { "value" },
                                    ParamTypes = new[] { TypeHelper.GetTypeName(typeof(string)) },
                                    ReturnTypes = new[] { TypeHelper.GetTypeName(typeof(NetUndefined), 1) }
                                }
                            }
                        },
                        new ApiMethod
                        {
                            Name = "length",
                            Description = "Gets the amount of characters in the string.",
                            FunctionType = ScriptFunctionType.Getter,
                            Signatures = new[]
                            {
                                new ApiMethodSignature
                                {
                                    ReturnTypes = new[] { TypeHelper.GetTypeName(typeof(int), 1) }
                                }
                            }
                        },
                        new ApiMethod
                        {
                            Name = "empty",
                            Description = "Returns an empty string.",
                            IsStatic = true,
                            FunctionType = ScriptFunctionType.Getter,
                            Signatures = new[]
                            {
                                new ApiMethodSignature
                                {
                                    ReturnTypes = new[] { TypeHelper.GetTypeName(typeof(string), 1) }
                                }
                            }
                        },
                        new ApiMethod
                        {
                            Name = "charAt",
                            Description = "Returns a character within the string.",
                            FunctionType = ScriptFunctionType.Standard,
                            Signatures = new[]
                            {
                                new ApiMethodSignature
                                {
                                    ParamNames = new[] { "index" },
                                    ParamTypes = new[] { TypeHelper.GetTypeName(typeof(int)) },
                                    ReturnTypes = new[] { TypeHelper.GetTypeName(typeof(string), 1) }
                                }
                            }
                        },
                        new ApiMethod
                        {
                            Name = "concat",
                            Description = "Concatenates multiple strings to this string.",
                            FunctionType = ScriptFunctionType.Standard,
                            Signatures = new[]
                            {
                                new ApiMethodSignature
                                {
                                    ParamNames = new[] { "...strings" },
                                    ParamTypes = new[] { TypeHelper.GetTypeName(typeof(string[])) },
                                    ReturnTypes = new[] { TypeHelper.GetTypeName(typeof(string), 1) }
                                }
                            }
                        },
                        new ApiMethod
                        {
                            Name = "includes",
                            Description = "Returns whether the string contains another string.",
                            FunctionType = ScriptFunctionType.Standard,
                            Signatures = new[]
                            {
                                new ApiMethodSignature
                                {
                                    ParamNames = new[] { "needle" },
                                    ParamTypes = new[] { TypeHelper.GetTypeName(typeof(string)) },
                                    ReturnTypes = new[] { TypeHelper.GetTypeName(typeof(bool), 1) }
                                }
                            }
                        },
                        new ApiMethod
                        {
                            Name = "endsWith",
                            Description = "Returns whether the string ends with another string.",
                            FunctionType = ScriptFunctionType.Standard,
                            Signatures = new[]
                            {
                                new ApiMethodSignature
                                {
                                    ParamNames = new[] { "needle" },
                                    ParamTypes = new[] { TypeHelper.GetTypeName(typeof(string)) },
                                    ReturnTypes = new[] { TypeHelper.GetTypeName(typeof(bool), 1) }
                                }
                            }
                        },
                        new ApiMethod
                        {
                            Name = "startsWith",
                            Description = "Returns whether the string starts with another string.",
                            FunctionType = ScriptFunctionType.Standard,
                            Signatures = new[]
                            {
                                new ApiMethodSignature
                                {
                                    ParamNames = new[] { "needle" },
                                    ParamTypes = new[] { TypeHelper.GetTypeName(typeof(string)) },
                                    ReturnTypes = new[] { TypeHelper.GetTypeName(typeof(bool), 1) }
                                }
                            }
                        },
                        new ApiMethod
                        {
                            Name = "indexOf",
                            Description = "Returns the index of the first occurrence of a string within this string.",
                            FunctionType = ScriptFunctionType.Standard,
                            Signatures = new[]
                            {
                                new ApiMethodSignature
                                {
                                    ParamNames = new[] { "needle" },
                                    ParamTypes = new[] { TypeHelper.GetTypeName(typeof(string)) },
                                    ReturnTypes = new[] { TypeHelper.GetTypeName(typeof(int), 1) }
                                }
                            }
                        },
                        new ApiMethod
                        {
                            Name = "lastIndexOf",
                            Description = "Returns the index of the last occurrence of a string within this string.",
                            FunctionType = ScriptFunctionType.Standard,
                            Signatures = new[]
                            {
                                new ApiMethodSignature
                                {
                                    ParamNames = new[] { "needle" },
                                    ParamTypes = new[] { TypeHelper.GetTypeName(typeof(string)) },
                                    ReturnTypes = new[] { TypeHelper.GetTypeName(typeof(int), 1) }
                                }
                            }
                        },
                        new ApiMethod
                        {
                            Name = "padEnd",
                            Description = "Pads the end of the string until the string reaches a certain length.",
                            FunctionType = ScriptFunctionType.Standard,
                            Signatures = new[]
                            {
                                new ApiMethodSignature
                                {
                                    ParamNames = new[] { "targetLength" },
                                    ParamTypes = new[] { TypeHelper.GetTypeName(typeof(int)) },
                                    ReturnTypes = new[] { TypeHelper.GetTypeName(typeof(string), 1) }
                                },
                                new ApiMethodSignature
                                {
                                    ParamNames = new[] { "targetLength", "padStr" },
                                    ParamTypes = new[] { TypeHelper.GetTypeName(typeof(int)), TypeHelper.GetTypeName(typeof(string)) },
                                    ReturnTypes = new[] { TypeHelper.GetTypeName(typeof(string), 1) }
                                },
                            }
                        },
                        new ApiMethod
                        {
                            Name = "padStart",
                            Description = "Pads the start of the string until the string reaches a certain length.",
                            FunctionType = ScriptFunctionType.Standard,
                            Signatures = new[]
                            {
                                new ApiMethodSignature
                                {
                                    ParamNames = new[] { "targetLength" },
                                    ParamTypes = new[] { TypeHelper.GetTypeName(typeof(int)) },
                                    ReturnTypes = new[] { TypeHelper.GetTypeName(typeof(string), 1) }
                                },
                                new ApiMethodSignature
                                {
                                    ParamNames = new[] { "targetLength", "padStr" },
                                    ParamTypes = new[] { TypeHelper.GetTypeName(typeof(int)), TypeHelper.GetTypeName(typeof(string)) },
                                    ReturnTypes = new[] { TypeHelper.GetTypeName(typeof(string), 1) }
                                },
                            }
                        },
                        new ApiMethod
                        {
                            Name = "repeat",
                            Description = "Repeats the string a number of times.",
                            FunctionType = ScriptFunctionType.Standard,
                            Signatures = new[]
                            {
                                new ApiMethodSignature
                                {
                                    ParamNames = new[] { "amount" },
                                    ParamTypes = new[] { TypeHelper.GetTypeName(typeof(int)) },
                                    ReturnTypes = new[] { TypeHelper.GetTypeName(typeof(string), 1) }
                                },
                            }
                        },
                        new ApiMethod
                        {
                            Name = "replace",
                            Description = "Replaces parts within the string with another string.",
                            FunctionType = ScriptFunctionType.Standard,
                            Signatures = new[]
                            {
                                new ApiMethodSignature
                                {
                                    ParamNames = new[] { "replace", "with" },
                                    ParamTypes = new[] { TypeHelper.GetTypeName(typeof(string)), TypeHelper.GetTypeName(typeof(string)) },
                                    ReturnTypes = new[] { TypeHelper.GetTypeName(typeof(string), 1) }
                                },
                            }
                        },
                        new ApiMethod
                        {
                            Name = "slice",
                            Description = "Returns a slice of the string.",
                            FunctionType = ScriptFunctionType.Standard,
                            Signatures = new[]
                            {
                                new ApiMethodSignature
                                {
                                    ParamNames = new[] { "startIndex" },
                                    ParamTypes = new[] { TypeHelper.GetTypeName(typeof(int)) },
                                    ReturnTypes = new[] { TypeHelper.GetTypeName(typeof(string), 1) }
                                },
                                new ApiMethodSignature
                                {
                                    ParamNames = new[] { "startIndex", "length" },
                                    ParamTypes = new[] { TypeHelper.GetTypeName(typeof(int)), TypeHelper.GetTypeName(typeof(int)) },
                                    ReturnTypes = new[] { TypeHelper.GetTypeName(typeof(string), 1) }
                                },
                            }
                        },
                        new ApiMethod
                        {
                            Name = "split",
                            Description = "Splits the string at a delimiter.",
                            FunctionType = ScriptFunctionType.Standard,
                            Signatures = new[]
                            {
                                new ApiMethodSignature
                                {
                                    ParamNames = new[] { "delimiters", "limit" },
                                    ParamTypes = new[] { TypeHelper.GetTypeName(typeof(string[])), TypeHelper.GetTypeName(typeof(int)) },
                                    ReturnTypes = new[] { TypeHelper.GetTypeName(typeof(string[]), 1) },
                                    OptionalNum = 1
                                },
                                new ApiMethodSignature
                                {
                                    ParamNames = new[] { "delimiter", "limit" },
                                    ParamTypes = new[] { TypeHelper.GetTypeName(typeof(string)), TypeHelper.GetTypeName(typeof(int)) },
                                    ReturnTypes = new[] { TypeHelper.GetTypeName(typeof(string[]), 1) },
                                    OptionalNum = 1
                                },
                            }
                        },
                        new ApiMethod
                        {
                            Name = "toLower",
                            Description = "Converts all alphabetic characters in the string to their lower case counterparts.",
                            FunctionType = ScriptFunctionType.Standard,
                            Signatures = new[]
                            {
                                new ApiMethodSignature
                                {
                                    ParamNames = new string[0],
                                    ParamTypes = new string[0],
                                    ReturnTypes = new[] { TypeHelper.GetTypeName(typeof(string), 1) }
                                },
                            }
                        },
                        new ApiMethod
                        {
                            Name = "toUpper",
                            Description = "Converts all alphabetic characters in the string to their upper case counterparts.",
                            FunctionType = ScriptFunctionType.Standard,
                            Signatures = new[]
                            {
                                new ApiMethodSignature
                                {
                                    ParamNames = new string[0],
                                    ParamTypes = new string[0],
                                    ReturnTypes = new[] { TypeHelper.GetTypeName(typeof(string), 1) }
                                },
                            }
                        },
                        new ApiMethod
                        {
                            Name = "trim",
                            Description = "Trims characters from the start and end of the string.",
                            FunctionType = ScriptFunctionType.Standard,
                            Signatures = new[]
                            {
                                new ApiMethodSignature
                                {
                                    ParamNames = new string[0],
                                    ParamTypes = new string[0],
                                    ReturnTypes = new[] { TypeHelper.GetTypeName(typeof(string), 1) }
                                },
                                new ApiMethodSignature
                                {
                                    ParamNames = new[] { "trimChar" },
                                    ParamTypes = new[] { TypeHelper.GetTypeName(typeof(string)) },
                                    ReturnTypes = new[] { TypeHelper.GetTypeName(typeof(string), 1) },
                                },
                                new ApiMethodSignature
                                {
                                    ParamNames = new[] { "trimChars" },
                                    ParamTypes = new[] { TypeHelper.GetTypeName(typeof(string[])) },
                                    ReturnTypes = new[] { TypeHelper.GetTypeName(typeof(string), 1) },
                                },
                            }
                        },
                        new ApiMethod
                        {
                            Name = "trimStart",
                            Description = "Trims characters from the start of the string.",
                            FunctionType = ScriptFunctionType.Standard,
                            Signatures = new[]
                            {
                                new ApiMethodSignature
                                {
                                    ParamNames = new string[0],
                                    ParamTypes = new string[0],
                                    ReturnTypes = new[] { TypeHelper.GetTypeName(typeof(string), 1) }
                                },
                                new ApiMethodSignature
                                {
                                    ParamNames = new[] { "trimChar" },
                                    ParamTypes = new[] { TypeHelper.GetTypeName(typeof(string)) },
                                    ReturnTypes = new[] { TypeHelper.GetTypeName(typeof(string), 1) },
                                },
                                new ApiMethodSignature
                                {
                                    ParamNames = new[] { "trimChars" },
                                    ParamTypes = new[] { TypeHelper.GetTypeName(typeof(string[])) },
                                    ReturnTypes = new[] { TypeHelper.GetTypeName(typeof(string), 1) },
                                },
                            }
                        },
                        new ApiMethod
                        {
                            Name = "trimEnd",
                            Description = "Trims characters from the end of the string.",
                            FunctionType = ScriptFunctionType.Standard,
                            Signatures = new[]
                            {
                                new ApiMethodSignature
                                {
                                    ParamNames = new string[0],
                                    ParamTypes = new string[0],
                                    ReturnTypes = new[] { TypeHelper.GetTypeName(typeof(string), 1) }
                                },
                                new ApiMethodSignature
                                {
                                    ParamNames = new[] { "trimChar" },
                                    ParamTypes = new[] { TypeHelper.GetTypeName(typeof(string)) },
                                    ReturnTypes = new[] { TypeHelper.GetTypeName(typeof(string), 1) },
                                },
                                new ApiMethodSignature
                                {
                                    ParamNames = new[] { "trimChars" },
                                    ParamTypes = new[] { TypeHelper.GetTypeName(typeof(string[])) },
                                    ReturnTypes = new[] { TypeHelper.GetTypeName(typeof(string), 1) },
                                },
                            }
                        },
                        new ApiMethod
                        {
                            Name = "remove",
                            Description = "Removes a set of characters from the string.",
                            FunctionType = ScriptFunctionType.Standard,
                            Signatures = new[]
                            {
                                new ApiMethodSignature
                                {
                                    ParamNames = new[] { "startIndex", "length" },
                                    ParamTypes = new[] { TypeHelper.GetTypeName(typeof(int)), TypeHelper.GetTypeName(typeof(int)) },
                                    ReturnTypes = new[] { TypeHelper.GetTypeName(typeof(string), 1) },
                                    OptionalNum = 1
                                },
                            }
                        },
                    }
                }
            };
        }
    }
}
