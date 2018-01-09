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
        private ApiProtoype[] _prototypes;

        private Dictionary<string, string> _templates = new Dictionary<string, string>();
        private string _pageNav;

        public PageEmitter(ApiClass[] apiClasses, ApiProtoype[] prototypes)
        {
            _apiClasses = apiClasses;
            _prototypes = prototypes;

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

            // generate all "section" files:
            var sections = Directory.GetFiles(GetProjectPath("docs-templates/"), "*.section.html");
            foreach (var section in sections) {
                EmitSection(section);
            }

            // generate api classes
            foreach (var apiClass in _apiClasses) {
                EmitApiClass(apiClass);
            }
        }

        private void EmitApiClass(ApiClass apiClass)
        {
            var methods = apiClass.Methods.OrderBy(m => m.Name).ToArray();
            var methodsText = "";
            foreach (var method in methods) {
                methodsText += ApiMethodToHtml(method);
            }
            var methodList = "<ul>" +
                string.Join(Environment.NewLine, methods.Select(m => $"<li><a href=\"#method-{m.Name}\">{m.Name}</a></li>")) +
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
            var nav = "";
            nav += "<h4>Api Classes</h4><ul>";
            foreach (var apiClass in _apiClasses) {
                nav += $"<li><a href=\"api-{GetClassLink(apiClass.Name)}\">{apiClass.Name}</a></li>";
            }
            nav += "</ul>";
            nav += "<h4>Prototypes</h4><ul>";
            foreach (var protoype in _prototypes) {
                nav += $"<li><a href=\"proto-{GetClassLink(protoype.Name)}\">{protoype.Name}</a></li>";
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

        private static string ApiMethodToHtml(ApiMethod method)
        {
            var html = $"<h4 id=\"method-{method.Name}\">{method.Name}</h4>";
            var props = new List<string>();
            if (method.IsConstructor) {
                props.Add("Constructor");
            }
            if (method.IsGetter) {
                props.Add("Getter");
            }
            if (method.IsSetter) {
                props.Add("Setter");
            }
            if (method.IsStatic) {
                props.Add("Static");
            }
            var propStr = string.Join(", ", props);
            if (!string.IsNullOrWhiteSpace(propStr)) {
                html += $"<i>{propStr}</i>";
            }
            html += "<br />Signatures:<br />";
            foreach (var signature in method.Signatures) {
                html += $"<br /><div><code>Return: <span class=\"arg-type\">{string.Join("</span> or <span class=\"arg-type\">", signature.ReturnTypes)}</span></code>";

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
                            html += "<span class=\"arg-type\">" + requiredParamTypes[i] + "</span> " + requiredParamNames[i];
                        }
                        for (int i = 0; i < signature.OptionalNum; i++) {
                            if (i > 0 || requiredNum > 0) {
                                html += ", ";
                            }
                            html += "[<span class=\"arg-type\">" + optionalParamTypes[i] + "</span> " + optionalParamNames[i] + "]";
                        }
                    } else {
                        for (int i = 0; i < signature.ParamNames.Length; i++) {
                            if (i > 0) {
                                html += ", ";
                            }
                            html += "<span class=\"arg-type\">" + signature.ParamTypes[i] + "</span> " + signature.ParamNames[i];
                        }
                    }
                    html += "</code>";
                }
                html += "</div>";
            }
            html += "<hr />";
            return html;
        }
    }
}
