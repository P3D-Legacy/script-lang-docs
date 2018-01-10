namespace DocsEmitter.Sections
{
    abstract class SectionEmitter
    {
        public abstract string Id { get; }
        public abstract string EmitHtml(string sectionContent, PageEmitter emitter);
    }
}
