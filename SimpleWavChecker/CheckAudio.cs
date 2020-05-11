using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SimpleWavChecker
{
    class CheckAudio
    {
        public void Run(string inputPath, string outputPath)
        {
            if (Directory.Exists(inputPath))
            {
                var list = CheckAudioFolder(inputPath);
                File.WriteAllLines(outputPath, list);
            }
            else if (File.Exists(inputPath))
            {
                string line = CheckAudioFile(inputPath);
                File.WriteAllText(outputPath, line);
            }
            else
                Console.WriteLine($"Missing input: {inputPath}");
        }
        private IEnumerable<string> CheckAudioFolder(string audioFolderPath)
        {
            return Directory.EnumerateFiles(audioFolderPath, "*.wav", SearchOption.AllDirectories)
                .Select(x => CheckAudioFile(x));
        }
        private string CheckAudioFile(string audioFilePath)
        {
            Wave wave = new Wave();
            string error = "";
            try
            {
                wave.ShallowParse(audioFilePath);
            }
            catch(FormatException e)
            {
                error = e.Message;
            }
            catch(Exception e)
            {
                error = "Unexpected error: " + Cleanup(e.Message);
            }
            return string.Join("\t",
                audioFilePath,
                wave.AudioTypeId,
                wave.NumChannels,
                wave.SampleRate,
                wave.ByteRate,
                wave.BlockAlign,
                wave.BitsPerSample,
                error);
        }

        private string Cleanup(string inputString)
        {
            return Regex.Replace(inputString, "\\s+", " ");
        }
    }
}
