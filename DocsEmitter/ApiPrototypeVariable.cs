namespace DocsEmitter
{
    struct ApiPrototypeVariable
    {
        public string Name;
        public string Type;

        public string EmitHtml(string className)
        {
            var shortClassName = className[0].ToString().ToLower();
            return $"<div id=\"var-{Name}\">" +
                $"{PageEmitter.GetImg("variable", "Variable")} <b>{Name}</b><br /><br />" +
                $"<code>Type: <span class=\"arg-type\">{TypeHelper.LinkType(Type)}</span></code>" +
                $"<code>Usage:<br />" +
                $"- get: <span class=\"arg-type\">var</span> {Name} = {shortClassName}.{Name};<br />" +
                $"- set: {shortClassName}.{Name} = {Name};</code></div><hr />";
        }
    }
}
