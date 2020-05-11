using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWavChecker
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine(">SimpleWavChecker.exe [INPUT_PATH] [OUTPUT_PATH]");
            }
            else
            {
                string inputPath = args[0];
                string outputPath = args[1];
                CheckAudio ca = new CheckAudio();
                ca.Run(inputPath, outputPath);
            }
        }
    }
}
