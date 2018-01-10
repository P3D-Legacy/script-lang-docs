using Kolben.Adapters;
using System;

namespace DocsEmitter
{
    class TypeHelper
    {
        public const string API_CLASS_TYPENAME = "net.Pokemon3D.Game.Scripting.V3.ApiClasses.ApiClass";
        public const string API_CLASS_ATTRIBUTE_TYPENAME = "net.Pokemon3D.Game.Scripting.V3.ApiClasses.ApiClassAttribute";
        public const string API_METHOD_SIGNATURE_ATTRIBUTE_TYPENAME = "net.Pokemon3D.Game.Scripting.V3.ApiMethodSignatureAttribute";

        public static string GetTypeName(Type type, int returnTypeCount = 0)
        {
            var name = type.Name;
            if (type == typeof(NetUndefined)) {
                if (returnTypeCount == 1) {
                    return "void";
                } else {
                    return "undefined";
                }
            } else if (name.EndsWith("Prototype") || name.EndsWith("Prototype[]")) {
                return name;
            } else if (type == typeof(int)) {
                return "int";
            } else if (type == typeof(int[])) {
                return "int[]";
            } else if (type == typeof(double)) {
                return "number";
            } else if (type == typeof(double[])) {
                return "number[]";
            } else if (type == typeof(bool)) {
                return "bool";
            } else if (type == typeof(bool[])) {
                return "bool[]";
            } else {
                return name.Substring(0, 1).ToLower() + name.Substring(1);
            }
        }

        public static string LinkType(string typeName)
        {
            var target = typeName;
            if (target.EndsWith("[]")) {
                target = target.Substring(0, target.Length - 2);
            }
            if (target.EndsWith("Prototype")) {
                var link = "proto-" + PageEmitter.GetClassLink(target.Replace("Prototype", ""));
                link = $"<a href=\"{link}\">{typeName.Replace("Prototype", "")}</a>";
                return link;
            }

            return typeName;
        }

    }
}
