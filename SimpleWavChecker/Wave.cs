using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Net.NetworkInformation;

namespace SimpleWavChecker
{
    class Wave
    {
        public short AudioTypeId { get; private set; } = 0;
        public short NumChannels { get; private set; } = 1;
        public int SampleRate { get; private set; } = 1;
        public int ByteRate { get; private set; } = 1;
        public short BlockAlign { get; private set; } = 1;
        public short BitsPerSample { get; private set; } = 1;
        const int SHALLOW_THRESHOLD = 200;
        public Wave() { }
        public void ShallowParse(string audioPath)
        {
            var bytes = IO.ReadBytes(audioPath,SHALLOW_THRESHOLD);
            Bytes = bytes;
            ParseRiff(bytes);
        }
        List<Chunk> ChunkList = new List<Chunk>();
        Chunk FormatChunk = new Chunk { Name = "" };
        Chunk DataChunk = new Chunk { Name = "" };
        byte[] Bytes = new byte[0];
        private void ParseRiff(byte[] bytes)
        {
            Sanity.Requires(bytes.Length >= 44, "File size less than 44 bytes.");
            Sanity.Requires(Encoding.ASCII.GetString(bytes, 0, 4) == "RIFF", "File is not RIFF.");
            Sanity.Requires(Encoding.ASCII.GetString(bytes, 8, 4) == "WAVE", "File is not WAVE.");
            ParseChunk(bytes, 12);
            PostProcess();
        }

        private void ParseChunk(byte[] bytes, int offset)
        {
            // A valid audio.
            if (offset == bytes.Length)
                return;
            // Ignore the uncomplete chunk. Since we only shallow parse here.
            if (offset + 8 > bytes.Length)            
                return;
            string chunkName = Encoding.ASCII.GetString(bytes, offset, 4);
            int chunkSize = BitConverter.ToInt32(bytes, offset + 4);
            bool complete = offset + 8 + chunkSize <= bytes.Length;
            Chunk chunk = new Chunk { Name = chunkName, Offset = offset, Size = chunkSize, Complete = complete };
            if(chunk.Name=="fmt ")
            {
                Sanity.Requires(complete, "Format chunk is imcomplete.");
                Sanity.Requires(FormatChunk.Name == "", "More than one format chunk.");
                FormatChunk = chunk;
            }
            else if (chunk.Name == "data")
            {
                Sanity.Requires(DataChunk.Name == "", "More than one data chunk.");
                DataChunk = chunk;
            }
            ChunkList.Add(chunk);
            if (complete)
                ParseChunk(bytes, offset + 8 + chunkSize);
        }

        public void PostProcess()
        {
            Sanity.Requires(FormatChunk.Name != "", "Missing format chunk.");
            Sanity.Requires(DataChunk.Name != "", "Missing data chunk.");
            ParseFormat();
        }

        private void ParseFormat()
        {
            Sanity.Requires(FormatChunk.Size >= 16, "Format chunk is less than 16.");
            AudioTypeId = BitConverter.ToInt16(Bytes, FormatChunk.Offset + 8);
            NumChannels = BitConverter.ToInt16(Bytes, FormatChunk.Offset + 10);
            SampleRate = BitConverter.ToInt32(Bytes, FormatChunk.Offset + 12);
            ByteRate = BitConverter.ToInt32(Bytes, FormatChunk.Offset + 16);
            BlockAlign = BitConverter.ToInt16(Bytes, FormatChunk.Offset + 20);
            BitsPerSample = BitConverter.ToInt16(Bytes, FormatChunk.Offset + 22);
            Sanity.Requires((BitsPerSample & 7) == 0, "Bits per sample is not a multiple of 8.");
            Sanity.Requires(ByteRate == (NumChannels * SampleRate * BitsPerSample) >> 3, "Byte rate, channel, sample rate, bits per sampel error.");
            Sanity.Requires(BlockAlign == NumChannels * BitsPerSample >> 3, "Block align, channel, bits per sample error.");
        }
    }
    struct Chunk
    {
        public string Name;
        public int Offset;
        public int Size;
        public bool Complete;
    }
}
