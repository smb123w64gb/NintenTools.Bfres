using System;
using System.Diagnostics;
using System.IO;

namespace Syroot.NintenTools.Bfres.Test
{
    internal class Program
    {
        // ---- FIELDS -------------------------------------------------------------------------------------------------

        private static Stopwatch _stopwatch = new Stopwatch();
        private static string[] _searchPaths = new string[]
        {
            @"D:\Pictures\zBFRES",
            @"D:\Archive\Wii U\_Roms\MK8"
        };

        // ---- METHODS (PRIVATE) --------------------------------------------------------------------------------------

        private static void Main(string[] args)
        {
            ResFile resFile = new ResFile(@"D:\Pictures\zBFRES\Roy.bfres");
            Shape firstShape = resFile.Models[0].Shapes[0];
            resFile.Models[0].Shapes.Clear();
            resFile.Models[0].Shapes.Add("ACoolMesh", firstShape);
            firstShape.Name = "ACoolMesh";
            resFile.Save(@"D:\Pictures\Roy.bfres");

            //LoadResFiles();
            //Console.WriteLine("Done.");
            //Console.ReadLine();
        }

        private static void LoadResFiles(Action<ResFile> fileAction = null)
        {
            foreach (string searchPath in _searchPaths)
            {
                foreach (string fileName in Directory.GetFiles(searchPath, "*.bfres", SearchOption.AllDirectories))
                {
                    Console.Write($"Loading {fileName}...");
                    
                    _stopwatch.Restart();
                    ResFile resFile = new ResFile(fileName);
                    _stopwatch.Stop();
                    Console.WriteLine($" {_stopwatch.ElapsedMilliseconds}ms");

                    fileAction?.Invoke(resFile);
                }
            }
        }
    }
}
