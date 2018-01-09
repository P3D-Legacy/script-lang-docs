namespace DocsEmitter
{
    struct ApiMethod
    {
        public bool IsConstructor;
        public bool IsStatic;
        public bool IsGetter;
        public bool IsSetter;
        public string Name;
        public ApiMethodSignature[] Signatures;
    }
}
