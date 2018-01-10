using Kolben.Adapters;
using System.Collections.Generic;
using System.Linq;

namespace DocsEmitter
{
    struct ApiMethod
    {
        public bool IsStatic;
        public ScriptFunctionType FunctionType;
        public string Name;
        public string Description;
        public ApiMethodSignature[] Signatures;

        public bool IsAccessor
            => FunctionType == ScriptFunctionType.Getter || FunctionType == ScriptFunctionType.Setter;
        public bool IsIndexer
            => FunctionType == ScriptFunctionType.IndexerGet || FunctionType == ScriptFunctionType.IndexerSet;

        public string EmitHtml(string className)
        {
            var shortClassName = "";
            if (IsStatic) {
                shortClassName = className;
            } else if (!string.IsNullOrEmpty(className)) {
                shortClassName = className[0].ToString().ToLower();
            }

            var id = "method";
            var img = PageEmitter.GetImg("method");
            switch (FunctionType) {
                case ScriptFunctionType.Getter:
                    id = "get";
                    img = PageEmitter.GetImg("getter", "Getter");
                    break;
                case ScriptFunctionType.Setter:
                    id = "set";
                    img = PageEmitter.GetImg("setter", "Setter");
                    break;
                case ScriptFunctionType.IndexerGet:
                    id = "index-get";
                    img = PageEmitter.GetImg("indexer-get", "Indexer");
                    break;
                case ScriptFunctionType.IndexerSet:
                    id = "index-set";
                    img = PageEmitter.GetImg("indexer-set", "Indexer");
                    break;
                case ScriptFunctionType.Constructor:
                    img = PageEmitter.GetImg("ctor", "Constructor");
                    break;
            }

            if (IsStatic) {
                img += PageEmitter.GetImg("static", "Static Method");
            }

            var prefix = "";
            if (IsAccessor) {
                prefix = "(" + id + ") ";
            }

            var methodName = Name;
            if (FunctionType == ScriptFunctionType.IndexerGet) {
                methodName = "(get) indexer";
            } else if (FunctionType == ScriptFunctionType.IndexerSet) {
                methodName = "(set) indexer";
            }

            var html = $"<span class=\"method-title\">{img}</span> <b id=\"{id}-{Name}\">{prefix}{methodName}</b><br />";
            if (IsStatic) {
                html += "<i>Static</i><br />";
            }
            if (!string.IsNullOrEmpty(Description)) {
                html += $"<i>{Description}</i><br />";
            }

            if (FunctionType == ScriptFunctionType.Getter) {
                if (Signatures.Length == 1) {
                    var usageText = $"<span class=\"arg-type\">var</span> {Name} = {shortClassName}.<b>{Name}</b>;";
                    html += $"<br /><div><code>Type: <span class=\"arg-type\">" +
                        string.Join("</span> or <span class=\"arg-type\">", Signatures[0].ReturnTypes
                            .Select(t => TypeHelper.LinkType(t))) +
                        $"</span></code><code>Usage: {usageText}</code></div>";
                }
            } else if (FunctionType == ScriptFunctionType.Setter) {
                if (Signatures.Length == 1) {
                    var usageText = $"{shortClassName}.<b>{Name}</b> = {Name};";
                    html += $"<br /><div><code>Type: <span class=\"arg-type\">" +
                        string.Join("</span> or <span class=\"arg-type\">", Signatures[0].ReturnTypes
                            .Select(t => TypeHelper.LinkType(t))) +
                        $"</span></code><code>Usage: {usageText}</code></div>";
                }
            } else {
                if (Signatures.Length > 1) {
                    html += "<br />Signatures:";
                }
                foreach (var signature in Signatures) {
                    html += $"<br /><div>";
                    if (FunctionType != ScriptFunctionType.Constructor && FunctionType != ScriptFunctionType.IndexerSet) {
                        html += $"<code>Return: <span class=\"arg-type\">" +
                            string.Join("</span> or <span class=\"arg-type\">", signature.ReturnTypes
                                .Select(t => TypeHelper.LinkType(t))) +
                            $"</span></code>";
                    }

                    var paramArgs = new List<string>();
                    if (signature.ParamNames.Length > 0) {
                        html += "<code>Arguments: ";
                        if (signature.OptionalNum > 0) {
                            var requiredNum = signature.ParamNames.Length - signature.OptionalNum;
                            var requiredParamNames = signature.ParamNames.Take(requiredNum).ToArray();
                            var requiredParamTypes = signature.ParamTypes.Take(requiredNum).ToArray();
                            var optionalParamNames = signature.ParamNames.Skip(requiredNum).ToArray();
                            var optionalParamTypes = signature.ParamTypes.Skip(requiredNum).ToArray();
                            for (int i = 0; i < requiredNum; i++) {
                                if (i > 0) {
                                    html += ", ";
                                }
                                html += "<span class=\"arg-type\">" + TypeHelper.LinkType(requiredParamTypes[i]) + "</span> " + requiredParamNames[i];
                                paramArgs.Add(requiredParamNames[i]);
                            }
                            for (int i = 0; i < signature.OptionalNum; i++) {
                                if (i > 0 || requiredNum > 0) {
                                    html += ", ";
                                }
                                html += "[<span class=\"arg-type\">" + TypeHelper.LinkType(optionalParamTypes[i]) + "</span> " + optionalParamNames[i] + "]";
                                paramArgs.Add("[" + optionalParamNames[i] + "]");
                            }
                        } else {
                            for (int i = 0; i < signature.ParamNames.Length; i++) {
                                if (i > 0) {
                                    html += ", ";
                                }
                                html += "<span class=\"arg-type\">" + TypeHelper.LinkType(signature.ParamTypes[i]) + "</span> " + signature.ParamNames[i];
                                paramArgs.Add(signature.ParamNames[i]);
                            }
                        }
                        html += "</code>";
                    }

                    if (FunctionType == ScriptFunctionType.Constructor) {
                        html += $"<code>Usage: <span class=\"arg-type\">var</span> {shortClassName} = <span class=\"arg-type\">new</span> <b>{className}</b>({string.Join(", ", paramArgs)});</code>";
                    } else if (FunctionType == ScriptFunctionType.IndexerGet) {
                        html += $"<code>Usage: <span class=\"arg-type\">var</span> result = {shortClassName}<b>[{string.Join(", ", paramArgs)}]</b>;</code>";
                    } else if (FunctionType == ScriptFunctionType.IndexerSet) {
                        html += $"<code>Usage: {shortClassName}<b>[{string.Join(", ", paramArgs)}]</b> = value;</code>";
                    } else {
                        var accessStr = $"{shortClassName}.<b>{Name}</b>";
                        if (string.IsNullOrEmpty(className)) {
                            accessStr = $"<b>{Name}</b>";
                        }
                        if (signature.ReturnTypes.Length == 1 && signature.ReturnTypes[0] == "void") {
                            html += $"<code>Usage: {accessStr}({string.Join(", ", paramArgs)});</code>";
                        } else {
                            html += $"<code>Usage: <span class=\"arg-type\">var</span> result = {accessStr}({string.Join(", ", paramArgs)});</code>";
                        }
                    }

                    html += "</div>";
                }
            }

            html += "<hr />";
            return html;
        }
    }
}
