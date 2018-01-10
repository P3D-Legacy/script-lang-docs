using Kolben.Adapters;
using System;
using System.Linq;

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
            var link = typeName;
            var isArr = false;

            if (target.EndsWith("[]")) {
                target = target.Substring(0, target.Length - 2);
                isArr = true;
            }
            if (target.EndsWith("Prototype")) {
                var href = "proto-" + PageEmitter.GetClassLink(target.Replace("Prototype", ""));
                link = $"<a href=\"{href}\">{target.Replace("Prototype", "")}</a>";
            } else if (new[] { "undefined", "void", "int", "any" }.Contains(target)) {
                var href = $"doc-{target}.html";
                link = $"<a href=\"{href}\">{target}</a>";
            } else if (new[] { "bool", "string", "number", "object" }.Contains(target)) {
                var href = $"proto-{target}.html";
                if (target == "bool") {
                    href = "proto-boolean.html";
                }
                link = $"<a href=\"{href}\">{target}</a>";
            }

            if (isArr) {
                link += "<a href=\"proto-array.html\">[]</a>";
            }

            return link;
        }

    }
}
