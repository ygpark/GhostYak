using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace GhostYak.Text.RegularExpressions
{
    public class BinaryRegexNet
    {

        private Regex _regex;
        private static int _TEST_BUFFER_SIZE = 10240;


        public BinaryRegexNet(string pattern)
        {
            _regex = new Regex(pattern, RegexOptions.Multiline);
        }

        public Match Match(byte[] input)
        {
            return this.Match(input, 0, input.Length);
        }

        public Match Match(byte[] input, int startat)
        {
            return this.Match(input, startat, input.Length);
        }

        public Match Match(byte[] input, int beginning, int length)
        {
            string sInput = new string('\0', input.Length * 2);
            unsafe
            {
                fixed (char* charArray = sInput)
                {
                    byte* buffer = (byte*)(charArray);
                    for (int i = 0; i < input.Length; i++)
                    {
                        buffer[i * 2] = input[i];
                    }
                }
            }
            return _regex.Match(sInput, beginning, length);
        }

        //------------------------------------------------------------------



        public MatchCollection Matches(byte[] input)
        {
            return this.Matches(input, 0);
        }



        public MatchCollection Matches(byte[] input, int startat)
        {
            string sInput = new string('\0', input.Length * 2);
            unsafe
            {
                fixed (char* charArray = sInput)
                {
                    byte* buffer = (byte*)(charArray);
                    for (int i = 0; i < input.Length; i++)
                    {
                        buffer[i * 2] = input[i];
                    }
                }
            }
            return _regex.Matches(sInput, startat);
        }



        //------------------------------------------------------------------


        public static Match Match(byte[] input, string pattern)
        {
            string sInput = new string('\0', input.Length * 2);
            unsafe
            {
                fixed (char* charArray = sInput)
                {
                    byte* buffer = (byte*)(charArray);
                    for (int i = 0; i < input.Length; i++)
                    {
                        buffer[i * 2] = input[i];
                    }
                }
            }

            return Regex.Match(sInput, pattern, RegexOptions.Multiline);
        }



        public static MatchCollection Matches(byte[] input, string pattern)
        {
            string sInput = new string('\0', input.Length * 2);
            unsafe
            {
                fixed (char* charArray = sInput)
                {
                    byte* buffer = (byte*)(charArray);
                    for (int i = 0; i < input.Length; i++)
                    {
                        buffer[i * 2] = input[i];
                    }
                }
            }

            return Regex.Matches(sInput, pattern, RegexOptions.Multiline);
        }

        public static void Test()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            TestRegexMatchesByInstance();
            TestRegexMatchesByStatic();
            TestRegexMatchByInstance();
            TestRegexMatchByStatic();
            sw.Stop();
            Console.WriteLine("BinaryRegex.Test(): Elapsed {0}", sw.Elapsed);
        }

        private static void TestRegexMatchesByInstance()
        {
            byte[] _source = _source = new byte[_TEST_BUFFER_SIZE];
            for (int i = 0; i < _source.Length; i++)
                _source[i] = (byte)i;
            BinaryRegex br = new BinaryRegex("\x00[\x00-\xFF]{254}\xFF");
            var matchs = br.Matches(_source);
            Debug.Assert(_TEST_BUFFER_SIZE/256 == matchs.Count);
        }

        private static void TestRegexMatchesByStatic()
        {
            byte[] _source = _source = new byte[_TEST_BUFFER_SIZE];
            for (int i = 0; i < _source.Length; i++)
                _source[i] = (byte)i;
            var matchs = BinaryRegex.Matches(_source, "\x00[\x00-\xFF]{254}\xFF");
            Debug.Assert(_TEST_BUFFER_SIZE / 256 == matchs.Count);
        }

        private static void TestRegexMatchByInstance()
        {
            byte[] _source = _source = new byte[_TEST_BUFFER_SIZE];
            for (int i = 0; i < _source.Length; i++)
                _source[i] = (byte)i;
            BinaryRegex br = new BinaryRegex("\x10[\x00-\xFF]{16}\x21");
            var match = br.Match(_source);
            Debug.Assert(16 == match.Index);
        }

        private static void TestRegexMatchByStatic()
        {
            byte[] _source = _source = new byte[_TEST_BUFFER_SIZE];
            for (int i = 0; i < _source.Length; i++)
                _source[i] = (byte)i;
            var match = BinaryRegex.Match(_source, "\x10[\x00-\xFF]{16}\x21");
            Debug.Assert(16 == match.Index);
        }

    }
}
