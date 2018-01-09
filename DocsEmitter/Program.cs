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
            var prototypes = ApiPrototype.GetApiPrototypes(assembly);
            var emitter = new PageEmitter(apiClasses, prototypes);
            emitter.Emit();
        }
    }
}
