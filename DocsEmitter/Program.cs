using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DocsEmitter
{
    class Program
    {
        private const string P3D_GAME_DIR = @"..\..\..\..\p3d\2.5DHero\2.5DHero";
        private const string ASSEMBLY_PATH = @"bin\DesktopGL\Debug\Pokemon3D.exe";

        public const string GIT_REPO_ROOT_P3D = "https://github.com/P3D-Legacy/P3D-Legacy/tree/script-system-next/";
        public const string GIT_REPO_ROOT_KOLBEN = "https://github.com/nilllzz/kolben/tree/master/";

        private static string GetAssemblyPath()
        {
            var assemblyDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var relativePath = Path.Combine(assemblyDir, P3D_GAME_DIR, ASSEMBLY_PATH);
            var absolutePath = Path.GetFullPath((new Uri(relativePath)).LocalPath);
            return absolutePath;
        }

        static void Main(string[] args)
        {
            var assembly = Assembly.LoadFile(GetAssemblyPath());

            var apiClasses = ApiClass.GetApiClasses(assembly);
            var prototypes = ApiPrototype.GetApiPrototypes(assembly).Concat(BuiltInTypes.GetBuiltInPrototypes()).ToArray();
            var emitter = new PageEmitter(apiClasses, prototypes);
            emitter.Emit();
        }
    }
}
