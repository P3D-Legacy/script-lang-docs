using Kolben.Adapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DocsEmitter
{
    struct ApiPrototype
    {
        public string Name;
        public string Description;
        public bool IsBuiltIn;
        public ApiMethod[] Methods;
        public ApiPrototypeVariable[] Variables;

        public string GetSourceLink()
        {
            if (IsBuiltIn) {
                return Program.GIT_REPO_ROOT_KOLBEN + $"Kolben/Types/Prototypes/{Name}Prototype.cs";
            } else {
                return Program.GIT_REPO_ROOT_P3D + $"2.5DHero/2.5DHero/World/ActionScript/V3/Prototypes/{Name}Prototype.vb";
            }
        }

        public static ApiPrototype[] GetApiPrototypes(Assembly assembly)
        {
            // load type
            var apiMethodSignatureAttributeType = assembly.GetType(TypeHelper.API_METHOD_SIGNATURE_ATTRIBUTE_TYPENAME);

            // gather
            var results = new List<ApiPrototype>();

            var prototypes = assembly.GetTypes()
                .Where(t => t.GetCustomAttributes(typeof(ScriptPrototypeAttribute), true).Length > 0);

            foreach (var prototypeType in prototypes) {
                var typeDef = prototypeType.GetCustomAttribute<ScriptPrototypeAttribute>();
                // prototype name
                var name = typeDef.VariableName;

                // construct prototype:
                var prototype = new ApiPrototype() { Name = name };

                // get variables
                var vars = prototypeType.GetFields(BindingFlags.Public | BindingFlags.Instance)
                    .Where(f => f.GetCustomAttribute<ScriptVariableAttribute>() != null).ToArray();

                prototype.Variables = vars.Select(v => new ApiPrototypeVariable { Name = v.Name, Type = TypeHelper.GetTypeName(v.FieldType) }).ToArray();

                // get methods
                var methods = prototypeType.GetMethods(BindingFlags.Public | BindingFlags.Static)
                    .Where(m => m.GetCustomAttributes(typeof(ScriptFunctionAttribute), true).Length > 0).ToArray();

                prototype.Methods = methods.Select(m =>
                {
                    var methodDef = m.GetCustomAttribute<ScriptFunctionAttribute>();
                    return new ApiMethod
                    {
                        Name = methodDef.VariableName,
                        FunctionType = methodDef.FunctionType,
                        IsStatic = methodDef.IsStatic,
                        Signatures = m.GetCustomAttributes(apiMethodSignatureAttributeType, true).Select(s =>
                        {
                            var returnTypes = (Type[])apiMethodSignatureAttributeType.GetProperty("ReturnType").GetValue(s);
                            return new ApiMethodSignature
                            {
                                OptionalNum = (int)apiMethodSignatureAttributeType.GetProperty("OptionalNum").GetValue(s),
                                ParamNames = (string[])apiMethodSignatureAttributeType.GetProperty("ParamNames").GetValue(s),
                                ParamTypes = ((Type[])apiMethodSignatureAttributeType.GetProperty("ParamTypes").GetValue(s))
                                    .Select(pt => TypeHelper.GetTypeName(pt)).ToArray(),
                                ReturnTypes = returnTypes.Select(pt => TypeHelper.GetTypeName(pt, returnTypes.Length)).ToArray()
                            };
                        }).ToArray()
                    };
                }).ToArray();

                results.Add(prototype);
            }

            return results.ToArray();
        }

        public string EmitHtml(PageEmitter emitter)
        {
            var ctor = Methods.FirstOrDefault(m => m.FunctionType == ScriptFunctionType.Constructor);
            var shortClassName = Name[0].ToString().ToLower();
            var ctorText = $"<code><span class=\"arg-type\">var</span> {shortClassName} = <span class=\"arg-type\">new</span> <b>{Name}</b>();</code>";
            if (ctor.FunctionType == ScriptFunctionType.Constructor) {
                ctorText = ctor.EmitHtml(Name);
            }

            var varsIndex = new List<string>();
            var methodsIndex = new List<string>();
            var getsetIndex = new List<string>();

            var varsText = "-- No Variables --";
            if (Variables.Length > 0) {
                varsText = "";
                foreach (var variable in Variables) {
                    varsText += variable.EmitHtml(Name) + Environment.NewLine;
                    varsIndex.Add($"<li>{PageEmitter.GetImg("variable")} <a href=\"#var-{variable.Name}\">{variable.Name}</a></li>");
                }
            }

            var getsetText = "-- No Getters &amp; Setters --";
            var getsets = Methods
                    .Where(m => m.IsAccessor)
                    .OrderBy(m => m.Name)
                    .GroupBy(m => m.FunctionType);
            if (getsets.Count() > 0) {
                getsetText = "";
                foreach (var methodSet in getsets) {
                    foreach (var getset in methodSet) {
                        getsetText += getset.EmitHtml(Name) + Environment.NewLine;
                        var getsetid = "get";
                        var img = PageEmitter.GetImg("getter", "Getter");
                        if (getset.IsStatic) {
                            img += PageEmitter.GetImg("static", "Static Getter");
                        }
                        if (getset.FunctionType == ScriptFunctionType.Setter) {
                            getsetid = "set";
                            img = PageEmitter.GetImg("setter", "Setter");
                            if (getset.IsStatic) {
                                img += PageEmitter.GetImg("static", "Static Setter");
                            }
                        }
                        getsetIndex.Add($"<li>{img} <a href=\"#{getsetid}-{getset.Name}\">({getsetid}) {getset.Name}</a></li>");
                    }
                }
            }

            var methodsText = "-- No Methods --";
            var methods = Methods
                    .Where(m => m.FunctionType == ScriptFunctionType.Standard)
                    .OrderBy(m => m.Name).ToArray();
            if (methods.Length > 0) {
                methodsText = "";
                foreach (var method in methods) {
                    methodsText += method.EmitHtml(Name) + Environment.NewLine;

                    var img = PageEmitter.GetImg("method");
                    if (method.IsStatic) {
                        img += PageEmitter.GetImg("static");
                    }
                    methodsIndex.Add($"<li>{img} <a href=\"#method-{method.Name}\">{method.Name}</a></li>");
                }
            }

            var indexersText = "-- No Indexers --";
            var hasIndexer = false;
            var indexerGetIndex = "";
            var indexerSetIndex = "";
            var indexers = Methods
                .Where(m => m.IsIndexer)
                .GroupBy(m => m.FunctionType);
            if (indexers.Count() > 0) {
                indexersText = "";
                hasIndexer = true;
                foreach (var group in indexers) {
                    foreach (var indexer in group) {
                        if (indexer.FunctionType == ScriptFunctionType.IndexerGet) {
                            indexerGetIndex = $"<li>{PageEmitter.GetImg("indexer-get")} <a href=\"#index-get-{indexer.Name}\">(get) indexer</a></li>";
                        }
                        if (indexer.FunctionType == ScriptFunctionType.IndexerSet) {
                            indexerSetIndex = $"<li>{PageEmitter.GetImg("indexer-set")} <a href=\"#index-set-{indexer.Name}\">(set) indexer</a></li>";
                        }
                        indexersText += indexer.EmitHtml(Name) + Environment.NewLine;
                    }
                }
            }

            var index = $"<ul><li>{PageEmitter.GetImg("ctor")} <a href=\"#ctor\">Constructor</a></li>";
            if (varsIndex.Count > 0) {
                index += $"<li>{PageEmitter.GetImg("variable")} <a href=\"#vars\">Variables</a>";
                if (varsIndex.Count > 0) {
                    index += "<ul>";
                    index += string.Join(Environment.NewLine, varsIndex);
                    index += "</ul>";
                }
                index += "</li>";
            }
            if (getsetIndex.Count > 0) {
                index += $"<li>{PageEmitter.GetImg("accessor", "Getters and Setters")} <a href=\"#get-set\">Getters &amp; Setters</a>";
                if (getsetIndex.Count > 0) {
                    index += "<ul>";
                    index += string.Join(Environment.NewLine, getsetIndex);
                    index += "</ul>";
                }
                index += "</li>";
            }
            if (methodsIndex.Count > 0) {
                index += $"<li>{PageEmitter.GetImg("method")} <a href=\"#methods\">Methods</a>";
                if (methodsIndex.Count > 0) {
                    index += "<ul>";
                    index += string.Join(Environment.NewLine, methodsIndex);
                    index += "</ul>";
                }
                index += "</li>";
            }
            if (hasIndexer) {
                index += $"<li>{PageEmitter.GetImg("indexer")} <a href=\"#indexers\">Indexers</a></li><ul>";
                index += indexerGetIndex;
                index += indexerSetIndex;
                index += "</ul>";
            }
            index += "</ul>";

            var description = "";
            if (!string.IsNullOrEmpty(Description)) {
                description = $"<i>{Description}</i><br />";
            }

            var codeIcon = IsBuiltIn ? "cs" : "vb";

            var content = emitter.FillTemplate("prototype",
                ("NAME", Name),
                ("INDEX", index),
                ("CONSTRUCTOR", ctorText),
                ("VARS", varsText),
                ("GETTERSSETTERS", getsetText),
                ("METHODS", methodsText),
                ("SOURCELINK", GetSourceLink()),
                ("INDEXERS", indexersText),
                ("DESCRIPTION", description),
                ("CODEICON", codeIcon));

            return content;
        }
    }
}
