using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWavChecker
{
    public static class Sanity
    {
        public static void Requires(bool b, string message)
        {
            if (!b)
                throw new FormatException(message);
        }
        public static void Requires(bool b)
        {
            if (!b)
                throw new FormatException();
        }
    }

    public class SizeException : Exception
    {
        public SizeException() : base() { }
        public SizeException(string message) : base(message) { }
    }
    public class FormatExcetpion : Exception
    {
        public FormatExcetpion() : base() { }
        public FormatExcetpion(string message) : base(message) { }
    }
}
