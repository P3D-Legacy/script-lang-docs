using Kolben.Adapters;
using System;
using System.Linq;

namespace DocsEmitter.Sections
{
    class GlobalFunctions : SectionEmitter
    {
        public override string Id => "GLOBALFUNCTIONS";

        public override string EmitHtml(string sectionContent, PageEmitter emitter)
        {
            var methods = GetGlobalFunctions().OrderBy(m => m.Name).ToArray();
            var methodsText = "";
            foreach (var method in methods) {
                methodsText += method.EmitHtml("") + Environment.NewLine;
            }
            var methodList = "<ul>" +
                string.Join(Environment.NewLine, methods
                    .Select(m => $"<li>{PageEmitter.GetImg("method") + PageEmitter.GetImg("static")} <a href=\"#method-{m.Name}\">{m.Name}</a></li>")) +
                "</ul>";
            var content = emitter.FillTemplate(sectionContent,
                ("METHODLIST", methodList),
                ("METHODS", methodsText));

            return content;
        }

        private static ApiMethod[] GetGlobalFunctions()
        {
            return new[]
            {
                new ApiMethod
                {
                    Name = "eval",
                    Description = "Executes the parameter as Kolben script code and returns the result.",
                    FunctionType = ScriptFunctionType.Standard,
                    IsStatic = true,
                    Signatures = new[]
                    {
                        new ApiMethodSignature
                        {
                            ReturnTypes = new[] { "any" },
                            ParamNames = new[] { "code" },
                            ParamTypes = new[] { TypeHelper.GetTypeName(typeof(string)) }
                        }
                    }
                },
                new ApiMethod
                {
                    Name = "sizeof",
                    Description = "Gets the length/size of a variable's content.",
                    FunctionType = ScriptFunctionType.Standard,
                    IsStatic = true,
                    Signatures = new[]
                    {
                        new ApiMethodSignature
                        {
                            ReturnTypes = new[] { TypeHelper.GetTypeName(typeof(int), 1) },
                            ParamNames = new[] { "variable" },
                            ParamTypes = new[] { "any" }
                        }
                    }
                },
                new ApiMethod
                {
                    Name = "typeof",
                    Description = "Gets the type name of a variable's content.",
                    FunctionType = ScriptFunctionType.Standard,
                    IsStatic = true,
                    Signatures = new[]
                    {
                        new ApiMethodSignature
                        {
                            ReturnTypes = new[] { TypeHelper.GetTypeName(typeof(string), 1) },
                            ParamNames = new[] { "variable" },
                            ParamTypes = new[] { "any" }
                        }
                    }
                },
                new ApiMethod
                {
                    Name = "nameof",
                    Description = "Gets the name of an object.",
                    FunctionType = ScriptFunctionType.Standard,
                    IsStatic = true,
                    Signatures = new[]
                    {
                        new ApiMethodSignature
                        {
                            ReturnTypes = new[] { TypeHelper.GetTypeName(typeof(string), 1) },
                            ParamNames = new[] { "variable" },
                            ParamTypes = new[] { "any" }
                        }
                    }
                },
                new ApiMethod
                {
                    Name = "toComplex",
                    Description = "Converts a primitive value (string, bool, number) into a prototype instance of that type.",
                    FunctionType = ScriptFunctionType.Standard,
                    IsStatic = true,
                    Signatures = new[]
                    {
                        new ApiMethodSignature
                        {
                            ReturnTypes = new[] { "any" },
                            ParamNames = new[] { "primitive" },
                            ParamTypes = new[] { "any" }
                        }
                    }
                },
                new ApiMethod
                {
                    Name = "toPrimitive",
                    Description = "Converts a prototype instance (string, bool, number) into its primitive value.",
                    FunctionType = ScriptFunctionType.Standard,
                    IsStatic = true,
                    Signatures = new[]
                    {
                        new ApiMethodSignature
                        {
                            ReturnTypes = new[] { TypeHelper.GetTypeName(typeof(string), 3), TypeHelper.GetTypeName(typeof(bool), 3), TypeHelper.GetTypeName(typeof(int), 3) },
                            ParamNames = new[] { "primitive" },
                            ParamTypes = new[] { "any" }
                        }
                    }
                },
                new ApiMethod
                {
                    Name = "isNaN",
                    Description = "Returns whether the input is NaN (not a number).",
                    FunctionType = ScriptFunctionType.Standard,
                    IsStatic = true,
                    Signatures = new[]
                    {
                        new ApiMethodSignature
                        {
                            ReturnTypes = new[] { TypeHelper.GetTypeName(typeof(bool), 1) },
                            ParamNames = new[] { "value" },
                            ParamTypes = new[] { "any" }
                        }
                    }
                },
                new ApiMethod
                {
                    Name = "isFinite",
                    Description = "Returns whether the input is a finite number.",
                    FunctionType = ScriptFunctionType.Standard,
                    IsStatic = true,
                    Signatures = new[]
                    {
                        new ApiMethodSignature
                        {
                            ReturnTypes = new[] { TypeHelper.GetTypeName(typeof(bool), 1) },
                            ParamNames = new[] { "value" },
                            ParamTypes = new[] { "any" }
                        }
                    }
                },
                new ApiMethod
                {
                    Name = "sync",
                    Description = "Waits for async tasks to complete.",
                    FunctionType = ScriptFunctionType.Standard,
                    IsStatic = true,
                    Signatures = new[]
                    {
                        new ApiMethodSignature
                        {
                            ReturnTypes = new[] { TypeHelper.GetTypeName(typeof(NetUndefined), 1) },
                            ParamNames = new string[0],
                            ParamTypes = new string[0]
                        },
                        new ApiMethodSignature
                        {
                            ReturnTypes = new[] { TypeHelper.GetTypeName(typeof(NetUndefined), 1) },
                            ParamNames = new[] { "taskIds" },
                            ParamTypes = new[] { TypeHelper.GetTypeName(typeof(string[])) }
                        }
                    }
                }
            };
        }
    }
}
