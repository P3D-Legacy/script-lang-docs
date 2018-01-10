using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DocsEmitter
{
    class PageEmitter
    {
        private const string PROJECT_PATH = @"..\..\..\";

        private static string GetProjectPath(string path)
        {
            var assemblyDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var relativePath = Path.Combine(assemblyDir, PROJECT_PATH, path);
            var absolutePath = Path.GetFullPath((new Uri(relativePath)).LocalPath);
            return absolutePath;
        }

        private ApiClass[] _apiClasses;
        private ApiPrototype[] _prototypes;

        private Dictionary<string, string> _templates = new Dictionary<string, string>();
        private string _pageNav;

        public PageEmitter(ApiClass[] apiClasses, ApiPrototype[] prototypes)
        {
            _apiClasses = apiClasses.OrderBy(a => a.Name).ToArray();
            _prototypes = prototypes.OrderBy(p => p.Name).ToArray();
        }

        public void Emit()
        {
            // generate the nav that gets inserted into {{NAV}}.
            _pageNav = GenerateNav();

            // load all templates
            var templates = Directory.GetFiles(GetProjectPath("docs-templates/"), "*.template.html");
            foreach (var template in templates) {
                var templateContent = File.ReadAllText(template);
                var templateId = Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(template));
                _templates.Add(templateId, templateContent);
            }

            // copy css
            File.Copy(GetProjectPath("docs-templates/style.css"), GetProjectPath("docs/style.css"), true);

            // copy assets
            FileHelper.CopyFilesRecursively(new DirectoryInfo(GetProjectPath("docs-templates/assets")),
                                            new DirectoryInfo(GetProjectPath("docs/assets")));

            // generate all "section" files:
            var sections = Directory.GetFiles(GetProjectPath("docs-templates/"), "*.section.html");
            foreach (var section in sections) {
                EmitSection(section);
            }

            // generate api classes
            foreach (var apiClass in _apiClasses) {
                EmitApiClass(apiClass);
            }

            // generate prototypes
            foreach (var prototype in _prototypes) {
                EmitPrototype(prototype);
            }
        }

        private void EmitPrototype(ApiPrototype prototype)
        {
            var ctor = prototype.Methods.FirstOrDefault(m => m.IsConstructor);
            var shortClassName = prototype.Name[0].ToString().ToLower();
            var ctorText = $"<code><span class=\"arg-type\">var</span> {shortClassName} = <span class=\"arg-type\">new</span> <b>{prototype.Name}</b>();</code>";
            if (ctor.IsConstructor) {
                ctorText = ApiMethodToHtml(prototype.Name, ctor);
            }

            var varsIndex = new List<string>();
            var methodsIndex = new List<string>();
            var getsetIndex = new List<string>();

            var varsText = "-- No Variables --";
            if (prototype.Variables.Length > 0) {
                varsText = "";
                foreach (var variable in prototype.Variables) {
                    varsText += ApiVariableToHtml(prototype.Name, variable) + Environment.NewLine;
                    varsIndex.Add($"<li>{GetImg("variable")} <a href=\"#var-{variable.Name}\">{variable.Name}</a></li>");
                }
            }

            var getsetText = "-- No Getters &amp; Setters --";
            var getsets = prototype.Methods
                    .Where(m => !m.IsConstructor && (m.IsGetter || m.IsSetter))
                    .OrderBy(m => m.Name)
                    .GroupBy(m => m.IsGetter);
            if (getsets.Count() > 0) {
                getsetText = "";
                foreach (var methodSet in getsets) {
                    foreach (var getset in methodSet) {
                        getsetText += ApiMethodToHtml(prototype.Name, getset) + Environment.NewLine;
                        var getsetid = "get";
                        var img = GetImg("getter", "Getter");
                        if (getset.IsStatic) {
                            img += GetImg("static", "Static Getter");
                        }
                        if (getset.IsSetter) {
                            getsetid = "set";
                            img = GetImg("setter", "Setter");
                            if (getset.IsStatic) {
                                img += GetImg("static", "Static Setter");
                            }
                        }
                        getsetIndex.Add($"<li>{img} <a href=\"#{getsetid}-{getset.Name}\">({getsetid}) {getset.Name}</a></li>");
                    }
                }
            }

            var methodsText = "-- No Methods --";
            var methods = prototype.Methods
                    .Where(m => !m.IsConstructor && !m.IsGetter && !m.IsSetter)
                    .OrderBy(m => m.Name).ToArray();
            if (methods.Length > 0) {
                methodsText = "";
                foreach (var method in methods) {
                    methodsText += ApiMethodToHtml(prototype.Name, method) + Environment.NewLine;

                    var img = GetImg("method");
                    if (method.IsStatic) {
                        img += GetImg("static");
                    }
                    methodsIndex.Add($"<li>{img} <a href=\"#method-{method.Name}\">{method.Name}</a></li>");
                }
            }

            var index = $"<ul><li>{GetImg("ctor")} <a href=\"#ctor\">Constructor</a></li>";
            if (varsIndex.Count > 0) {
                index += $"<li>{GetImg("variable")} <a href=\"#vars\">Variables</a>";
                if (varsIndex.Count > 0) {
                    index += "<ul>";
                    index += string.Join(Environment.NewLine, varsIndex);
                    index += "</ul>";
                }
                index += "</li>";
            }
            if (getsetIndex.Count > 0) {
                index += $"<li>{GetImg("accessor", "Getters and Setters")} <a href=\"#get-set\">Getters &amp; Setters</a>";
                if (getsetIndex.Count > 0) {
                    index += "<ul>";
                    index += string.Join(Environment.NewLine, getsetIndex);
                    index += "</ul>";
                }
                index += "</li>";
            }
            if (methodsIndex.Count > 0) {
                index += $"<li>{GetImg("method")} <a href=\"#methods\">Methods</a>";
                if (methodsIndex.Count > 0) {
                    index += "<ul>";
                    index += string.Join(Environment.NewLine, methodsIndex);
                    index += "</ul>";
                }
                index += "</li>";
            }
            index += "</ul>";

            var content = FillTemplate("prototype",
                ("NAME", prototype.Name),
                ("INDEX", index),
                ("CONSTRUCTOR", ctorText),
                ("VARS", varsText),
                ("GETTERSSETTERS", getsetText),
                ("METHODS", methodsText));

            var fileName = "proto-" + GetClassLink(prototype.Name);
            EmitPage(fileName, prototype.Name, content);
        }

        private void EmitApiClass(ApiClass apiClass)
        {
            var methods = apiClass.Methods.OrderBy(m => m.Name).ToArray();
            var methodsText = "";
            foreach (var method in methods) {
                methodsText += ApiMethodToHtml(apiClass.Name, method) + Environment.NewLine;
            }
            var methodList = "<ul>" +
                string.Join(Environment.NewLine, methods
                    .Select(m => $"<li>{GetImg("method") + GetImg("static")} <a href=\"#method-{m.Name}\">{m.Name}</a></li>")) +
                "</ul>";
            var content = FillTemplate("apiclass",
                ("NAME", apiClass.Name),
                ("METHODLIST", methodList),
                ("METHODS", methodsText));

            var fileName = "api-" + GetClassLink(apiClass.Name);
            EmitPage(fileName, apiClass.Name, content);
        }

        private void EmitSection(string sectionFile)
        {
            var sectionContent = File.ReadAllText(sectionFile);
            var sectionTitle = sectionContent.Remove(sectionContent.IndexOf(";"));
            sectionTitle = sectionTitle.Remove(0, sectionTitle.IndexOf("=") + 1);

            sectionContent = sectionContent.Remove(0, sectionContent.IndexOf(";") + 1);

            // remove "section" from filename
            var sectionFileName = Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(sectionFile)) + ".html";

            EmitPage(sectionFileName, sectionTitle, sectionContent);
        }

        private void EmitPage(string fileName, string title, string content)
        {
            var destinationFile = GetProjectPath($"docs/{fileName}");
            var fileContent = FillTemplate("page",
                ("NAV", _pageNav),
                ("TITLE", title),
                ("CONTENT", content)
            );
            File.WriteAllText(destinationFile, fileContent);
        }

        private string FillTemplate(string templateName, params (string key, string value)[] templateStrings)
        {
            var template = _templates[templateName];
            foreach (var templateString in templateStrings) {
                template = template.Replace("{{" + templateString.key + "}}", templateString.value);
            }
            return template;
        }

        private string GenerateNav()
        {
            var nav = "<ul><li><a href=\"index.html\">Home</a></li></ul>";
            nav += "<h4>Api Classes</h4><ul>";
            foreach (var apiClass in _apiClasses) {
                nav += $"<li>{GetImg("apiclass")} <a href=\"api-{GetClassLink(apiClass.Name)}\">{apiClass.Name}</a></li>";
            }
            nav += "</ul>";
            nav += "<h4>Prototypes</h4><ul>";
            foreach (var protoype in _prototypes) {
                nav += $"<li>{GetImg("prototype")} <a href=\"proto-{GetClassLink(protoype.Name)}\">{protoype.Name}</a></li>";
            }
            nav += "</ul>";
            return nav;
        }

        private static string GetClassLink(string name)
        {
            string result = "";

            for (int i = 0; i < name.Length; i++) {
                if (char.IsUpper(name[i])) {
                    if (i > 0) {
                        result += "-" + name[i].ToString().ToLower();
                    } else {
                        result += name[i].ToString().ToLower();
                    }
                } else {
                    result += name[i];
                }
            }

            return result + ".html";
        }

        private static string ApiVariableToHtml(string className, ApiPrototypeVariable variable)
        {
            var shortClassName = className[0].ToString().ToLower();
            return $"<div id=\"var-{variable.Name}\">" +
                $"{GetImg("variable", "Variable")} <b>{variable.Name}</b><br /><br />" +
                $"<code>Type: <span class=\"arg-type\">{variable.Type}</span></code>" +
                $"<code>Usage:<br />" +
                $"- get: <span class=\"arg-type\">var</span> {variable.Name} = {shortClassName}.{variable.Name};<br />" +
                $"- set: {shortClassName}.{variable.Name} = {variable.Name};</code></div><hr />";
        }

        private static string LinkType(string typeName)
        {
            var target = typeName;
            if (target.EndsWith("[]")) {
                target = target.Substring(0, target.Length - 2);
            }
            if (target.EndsWith("Prototype")) {
                var link = "proto-" + GetClassLink(target.Replace("Prototype", ""));
                link = $"<a href=\"{link}\">{typeName.Replace("Prototype", "")}</a>";
                return link;
            }

            return typeName;
        }

        private static string ApiMethodToHtml(string className, ApiMethod method)
        {
            var shortClassName = className[0].ToString().ToLower();
            if (method.IsStatic) {
                shortClassName = className;
            }

            var id = "method";
            var img = GetImg("method");
            if (method.IsGetter) {
                id = "get";
                img = GetImg("getter", "Getter");
            } else if (method.IsSetter) {
                id = "set";
                img = GetImg("setter", "Setter");
            } else if (method.IsConstructor) {
                img = GetImg("ctor", "Constructor");
            }
            if (method.IsStatic) {
                img += GetImg("static", "Static Method");
            }

            var prefix = "";
            if (method.IsGetter || method.IsSetter) {
                prefix = "(" + id + ") ";
            }

            var html = $"<span class=\"method-title\">{img}</span> <b id=\"{id}-{method.Name}\">{prefix}{method.Name}</b><br />";
            if (method.IsStatic) {
                html += $"<i>Static</i><br />";
            }

            if (method.IsGetter) {
                if (method.Signatures.Length == 1) {
                    var usageText = $"<span class=\"arg-type\">var</span> {method.Name} = {shortClassName}.<b>{method.Name}</b>;";
                    html += $"<br /><div><code>Type: <span class=\"arg-type\">" +
                        string.Join("</span> or <span class=\"arg-type\">", method.Signatures[0].ReturnTypes
                            .Select(t => LinkType(t))) +
                        $"</span></code><code>Usage: {usageText}</code></div>";
                }
            } else if (method.IsSetter) {
                if (method.Signatures.Length == 1) {
                    var usageText = $"{shortClassName}.<b>{method.Name}</b> = <i>{method.Name}</i>;";
                    html += $"<br /><div><code>Type: <span class=\"arg-type\">" +
                        string.Join("</span> or <span class=\"arg-type\">", method.Signatures[0].ReturnTypes
                            .Select(t => LinkType(t))) +
                        $"</span></code><code>Usage: {usageText}</code></div>";
                }
            } else {
                if (method.Signatures.Length > 1) {
                    html += "<br />Signatures:";
                }
                foreach (var signature in method.Signatures) {
                    html += $"<br /><div>";
                    if (!method.IsConstructor) {
                        html += $"<code>Return: <span class=\"arg-type\">" +
                            string.Join("</span> or <span class=\"arg-type\">", signature.ReturnTypes
                                .Select(t => LinkType(t))) +
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
                                html += "<span class=\"arg-type\">" + LinkType(requiredParamTypes[i]) + "</span> " + requiredParamNames[i];
                                paramArgs.Add(requiredParamNames[i]);
                            }
                            for (int i = 0; i < signature.OptionalNum; i++) {
                                if (i > 0 || requiredNum > 0) {
                                    html += ", ";
                                }
                                html += "[<span class=\"arg-type\">" + LinkType(optionalParamTypes[i]) + "</span> " + optionalParamNames[i] + "]";
                                paramArgs.Add("[" + optionalParamNames[i] + "]");
                            }
                        } else {
                            for (int i = 0; i < signature.ParamNames.Length; i++) {
                                if (i > 0) {
                                    html += ", ";
                                }
                                html += "<span class=\"arg-type\">" + LinkType(signature.ParamTypes[i]) + "</span> " + signature.ParamNames[i];
                                paramArgs.Add(signature.ParamNames[i]);
                            }
                        }
                        html += "</code>";
                    }

                    if (method.IsConstructor) {
                        html += $"<code>Usage: <span class=\"arg-type\">var</span> {shortClassName} = <span class=\"arg-type\">new</span> <b>{className}</b>({string.Join(", ", paramArgs)});</code>";
                    } else {
                        if (signature.ReturnTypes.Length == 1 && signature.ReturnTypes[0] == "void") {
                            html += $"<code>Usage: {shortClassName}.<b>{method.Name}</b>({string.Join(", ", paramArgs)});</code>";
                        } else {
                            html += $"<code>Usage: <span class=\"arg-type\">var</span> result = {shortClassName}.<b>{method.Name}</b>({string.Join(", ", paramArgs)});</code>";
                        }
                    }

                    html += "</div>";
                }
            }

            html += "<hr />";
            return html;
        }

        private static string GetImg(string img, string tooltip = "")
        {
            return $"<img src=\"assets/img/{img}.png\" alt=\"{tooltip}\" title=\"{tooltip}\" />";
        }
    }
}
