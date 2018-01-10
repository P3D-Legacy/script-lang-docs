using DocsEmitter.Sections;
using Kolben.Adapters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

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
        private SectionEmitter[] _sectionEmitters;

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

            // load section emitters:
            _sectionEmitters = Assembly.GetAssembly(typeof(SectionEmitter)).GetTypes()
                .Where(t => t.IsSubclassOf(typeof(SectionEmitter)))
                .Select(t => (SectionEmitter)Activator.CreateInstance(t)).ToArray();
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
            var content = prototype.EmitHtml(this);

            var fileName = "proto-" + GetClassLink(prototype.Name);
            EmitPage(fileName, prototype.Name, content);
        }

        private void EmitApiClass(ApiClass apiClass)
        {
            var content = apiClass.EmitHtml(this);

            var fileName = "api-" + GetClassLink(apiClass.Name);
            EmitPage(fileName, apiClass.Name, content);
        }

        private void EmitSection(string sectionFile)
        {
            var sectionLines = File.ReadAllLines(sectionFile).ToList();
            var id = "";
            var title = "";
            while (sectionLines.Count > 0 && Regex.IsMatch(sectionLines[0], "^.*?=.*?;$")) {
                var line = sectionLines[0];
                var key = line.Remove(line.IndexOf("="));
                var arg = line.Remove(0, line.IndexOf("=") + 1);
                arg = arg.Substring(0, arg.Length - 1);

                switch (key) {
                    case "ID":
                        id = arg;
                        break;
                    case "TITLE":
                        title = arg;
                        break;
                }
                sectionLines.RemoveAt(0);
            }

            var sectionContent = string.Join(Environment.NewLine, sectionLines);
            if (id != "") {
                // try getting a section emitter
                var emitter = _sectionEmitters.FirstOrDefault(e => e.Id == id);
                if (emitter != null) {
                    sectionContent = emitter.EmitHtml(sectionContent, this);
                }
            }

            // remove "section" from filename
            var sectionFileName = Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(sectionFile)) + ".html";

            EmitPage(sectionFileName, title, sectionContent);
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

        public string FillTemplate(string templateName, params (string key, string value)[] templateStrings)
        {
            if (!_templates.TryGetValue(templateName, out var template)) {
                template = templateName; // template "name" is actually the template content.
            }
            foreach (var templateString in templateStrings) {
                template = template.Replace("{{" + templateString.key + "}}", templateString.value);
            }
            return template;
        }

        private string GenerateNav()
        {
            // home
            var nav = $"<ul><li>{GetImg("home")} <a href=\"index.html\">Home</a></li></ul>";

            // articles
            nav += $"<details><summary>{GetImg("folder")} <b>Articles</b></summary><ul>";
            nav += $"<li>{GetImg("document")} <a href=\"doc-proto-and-apiclass.html\">Prototypes and Api Classes</a></li>";
            nav += $"<li>{GetImg("document")} <a href=\"doc-int.html\">Int</a></li>";
            nav += $"<li>{GetImg("document")} <a href=\"doc-void.html\">Void</a></li>";
            nav += $"<li>{GetImg("document")} <a href=\"doc-any.html\">Any</a></li>";
            nav += $"<li>{GetImg("document")} <a href=\"doc-undefined.html\">Undefined</a></li>";
            nav += "</ul></details>";

            // built in types
            nav += $"<details open><summary>{GetImg("folder")} <b>Built-In types</b></summary><ul>";
            nav += $"<li>{GetImg("static")} <a href=\"builtin-global-functions.html\">Global Functions</a></li>";
            foreach (var protoype in _prototypes.Where(p => p.IsBuiltIn)) {
                nav += $"<li>{GetImg("prototype")} <a href=\"proto-{GetClassLink(protoype.Name)}\">{protoype.Name}</a></li>";
            }
            nav += "</ul></details>";

            // api classes
            nav += $"<details open><summary>{GetImg("folder")} <b>Api Classes</b></summary><ul>";
            foreach (var apiClass in _apiClasses) {
                nav += $"<li>{GetImg("apiclass")} <a href=\"api-{GetClassLink(apiClass.Name)}\">{apiClass.Name}</a></li>";
            }
            nav += "</ul></details>";

            // prototypes
            nav += $"<details open><summary><b>{GetImg("folder")} Prototypes</b></summary><ul>";
            foreach (var protoype in _prototypes.Where(p => !p.IsBuiltIn)) {
                nav += $"<li>{GetImg("prototype")} <a href=\"proto-{GetClassLink(protoype.Name)}\">{protoype.Name}</a></li>";
            }
            nav += "</ul></details>";

            return nav;
        }

        public static string GetClassLink(string name)
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

        public static string GetImg(string img, string tooltip = "")
        {
            return $"<img src=\"assets/img/{img}.png\" alt=\"{tooltip}\" title=\"{tooltip}\" />";
        }
    }
}
