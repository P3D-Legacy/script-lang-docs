using Kolben.Adapters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DocsEmitter
{
    struct ApiClass
    {
        public string Name;
        public ApiMethod[] Methods;

        private const string API_CLASS_TYPENAME = "net.Pokemon3D.Game.Scripting.V3.ApiClasses.ApiClass";
        private const string API_CLASS_ATTRIBUTE_TYPENAME = "net.Pokemon3D.Game.Scripting.V3.ApiClasses.ApiClassAttribute";
        private const string API_METHOD_SIGNATURE_ATTRIBUTE_TYPENAME = "net.Pokemon3D.Game.Scripting.V3.ApiMethodSignatureAttribute";

        private static string GetTypeName(Type type)
        {
            var name = type.Name;
            if (type == typeof(NetUndefined)) {
                name = "undefined";
            }

            return name.Substring(0, 1).ToLower() + name.Substring(1);
        }

        public static ApiClass[] GetApiClasses(Assembly assembly)
        {
            // load types
            var apiClassType = assembly.GetType(API_CLASS_TYPENAME);
            var apiClassAttributeType = assembly.GetType(API_CLASS_ATTRIBUTE_TYPENAME);
            var apiMethodSignatureAttributeType = assembly.GetType(API_METHOD_SIGNATURE_ATTRIBUTE_TYPENAME);

            // gather api classes:
            var results = new List<ApiClass>();

            var apiClasses = assembly.GetTypes()
                .Where(t => t.IsSubclassOf(apiClassType) && t.GetCustomAttributes(apiClassAttributeType, true).Length > 0);
            foreach (var apiClass in apiClasses) {
                var attr = apiClass.GetCustomAttribute(apiClassAttributeType);
                // class name
                var name = (string)apiClassAttributeType.GetProperty("ClassName").GetValue(attr);

                // get methods
                var methods = apiClass.GetMethods(BindingFlags.Public | BindingFlags.Static)
                    .Where(m => m.GetCustomAttributes(apiMethodSignatureAttributeType, true).Length > 0).ToArray();

                // construct class
                var cl = new ApiClass { Name = name };
                var clMethods = new List<ApiMethod>();

                foreach (var method in methods) {
                    var signatures = method.GetCustomAttributes(apiMethodSignatureAttributeType, true);

                    var clMethod = new ApiMethod() { IsStatic = true, Name = method.Name };
                    var clMethodSignatures = new List<ApiMethodSignature>();
                    foreach (var signature in signatures) {
                        var returnTypes = (Type[])apiMethodSignatureAttributeType.GetProperty("ReturnType").GetValue(signature);
                        var paramNames = (string[])apiMethodSignatureAttributeType.GetProperty("ParamNames").GetValue(signature);
                        var paramTypes = (Type[])apiMethodSignatureAttributeType.GetProperty("ParamTypes").GetValue(signature);
                        var optionalNum = (int)apiMethodSignatureAttributeType.GetProperty("OptionalNum").GetValue(signature);

                        var methodSignature = new ApiMethodSignature()
                        {
                            OptionalNum = optionalNum,
                            ParamNames = paramNames,
                            ParamTypes = paramTypes.Select(pt => GetTypeName(pt)).ToArray(),
                            ReturnTypes = returnTypes.Select(pt => GetTypeName(pt)).ToArray()
                        };

                        clMethodSignatures.Add(methodSignature);
                    }
                    clMethod.Signatures = clMethodSignatures.ToArray();

                    clMethods.Add(clMethod);
                }
                cl.Methods = clMethods.ToArray();

                results.Add(cl);
            }

            return results.ToArray();
        }
    }
}
