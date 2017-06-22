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
            ResFile resFile = new ResFile(@"D:\Pictures\zBFRES\Koopa.bfres");
            resFile.Save(@"D:\Pictures\Koopa.bfres");

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
