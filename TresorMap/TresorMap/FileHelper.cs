using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace TreasureMap
{
    public class FileHelper
    {
        public static char Separator = '-';
        public static readonly string GameFolder = @"C:\Users\CamlLight\source\repos\TressorMap\TresorMap\TresorMap";

        public static IEnumerable<string> ReadFiles()
        {
            string fileName = "map.txt";
            string path = $@"{GameFolder}\{fileName}";

            if (File.Exists(path))
            {
                return File.ReadAllLines(path);
            }
            else
                throw new ArgumentNullException($"File not found @ {path}");
        }

        public static string ReadLine()
        {
            return string.Empty;
        }

        public static void SaveFile(IList<string> state)
        {
            string fileName = "result.txt";
            string path = $@"{GameFolder}\{fileName}";

            if (File.Exists(path))
            {
                File.Delete(path);
            }


            using (StreamWriter file = new(path))
            {
                foreach (string line in state)
                {
                    file.WriteLineAsync(line);
                }
            }
        }

        public static void Log(string message)
        {
            Console.WriteLine(message);
        }
    }
}
