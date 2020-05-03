using Nez;
using System.IO;
using System.Linq;

namespace IceCreamJam.Source.Utils {
    static class MonogameShaderCompiler {

        public const string compilerPath = @"C:\Program Files (x86)\MSBuild\MonoGame\v3.0\Tools\2MGFX.exe";

        public static void Compile(string shaderContentFolder) {
            string fullContentPath = Directory.GetCurrentDirectory() + "../../";

            var files = Directory
                .EnumerateFiles(shaderContentFolder + "/Raw", "*.*", SearchOption.AllDirectories)
                .Where(s => ".fx" == Path.GetExtension(s).ToLower())
                .Select(s => Path.GetFileName(s));

            string command = "/C ";
            foreach(string fileName in files) {
                command += "\"" + compilerPath + "\" " 
                    + shaderContentFolder + "/Raw" + fileName + " " 
                    + shaderContentFolder + "/Compiled" + Path.ChangeExtension(fileName, ".mgfxo") 
                    + "&";
            }

            Debug.Log(command);

            var process = System.Diagnostics.Process.Start("CMD.exe", command);
        }

    }
}
