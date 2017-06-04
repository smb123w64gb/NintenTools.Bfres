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
            @"D:\Archive\Wii U\_Roms\MK8"
        };

        // ---- METHODS (PRIVATE) --------------------------------------------------------------------------------------

        private static void Main(string[] args)
        {
            ResFile resFile = new ResFile(@"D:\Pictures\Koopa.bfres");
            LoadResFiles("*.bfres");
            
            Console.WriteLine("Done.");
            Console.ReadLine();
        }

        private static void LoadResFiles(string searchPattern, Action<ResFile> fileAction = null)
        {
            ResFile resFile;
            foreach (string searchPath in _searchPaths)
            {
                foreach (string fileName in Directory.GetFiles(searchPath, searchPattern, SearchOption.AllDirectories))
                {
                    Console.Write($"Loading {fileName}...");
                    
                    _stopwatch.Restart();
                    resFile = new ResFile(fileName);
                    _stopwatch.Stop();
                    Console.WriteLine($" {_stopwatch.ElapsedMilliseconds}ms");

                    fileAction?.Invoke(resFile);
                }
            }
        }
    }
}
