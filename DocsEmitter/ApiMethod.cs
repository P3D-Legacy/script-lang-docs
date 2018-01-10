using Kolben.Adapters;

namespace DocsEmitter
{
    struct ApiMethod
    {
        public bool IsStatic;
        public ScriptFunctionType FunctionType;
        public string Name;
        public ApiMethodSignature[] Signatures;

        public bool IsAccessor
            => FunctionType == ScriptFunctionType.Getter || FunctionType == ScriptFunctionType.Setter;
        public bool IsIndexer
            => FunctionType == ScriptFunctionType.IndexerGet || FunctionType == ScriptFunctionType.IndexerSet;
    }
}
