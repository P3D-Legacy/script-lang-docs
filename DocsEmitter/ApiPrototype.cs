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
        public ApiMethod[] Methods;
        public ApiPrototypeVariable[] Variables;

        public string GetSourceLink()
        {
            return Program.GIT_REPO_ROOT + $"2.5DHero/2.5DHero/World/ActionScript/V3/Prototypes/{Name}Prototype.vb";
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
                        IsConstructor = methodDef.FunctionType == ScriptFunctionType.Constructor,
                        IsGetter = methodDef.FunctionType == ScriptFunctionType.Getter,
                        IsSetter = methodDef.FunctionType == ScriptFunctionType.Setter,
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
    }
}
