using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWavChecker
{
    public static class IO
    {
        public static byte[] ReadBytes(string filePath, int n)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                int count = Math.Min(n, (int)fs.Length);
                using (BinaryReader br = new BinaryReader(fs))
                {
                    return br.ReadBytes(count);
                }
            }
        }
    }
}
