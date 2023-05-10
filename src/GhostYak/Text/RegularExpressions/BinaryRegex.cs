using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Re2.Net;
using System.IO;
using System.Diagnostics;

namespace GhostYak.Text.RegularExpressions
{
    public class BinaryRegex
    {

        private Regex _regex;
        private static int _TEST_BUFFER_SIZE = 10240;


        public BinaryRegex(string pattern)
        {
            _regex = new Regex(pattern, RegexOptions.Multiline | RegexOptions.Latin1);
        }

        public Match Match(byte[] input)
        {
            return _regex.Match(input);
        }

        public Match Match(byte[] input, int startat)
        {
            return _regex.Match(input, startat);
        }

        public Match Match(byte[] input, int beginning, int length)
        {
            return _regex.Match(input, beginning, length);
        }

        //------------------------------------------------------------------



        public MatchCollection Matches(byte[] input)
        {
            return _regex.Matches(input);
        }



        public MatchCollection Matches(byte[] input, int startat)
        {
            return _regex.Matches(input, startat);
        }



        //------------------------------------------------------------------


        public static Match Match(byte[] input, string pattern)
        {
            return Regex.Match(input, pattern, RegexOptions.Multiline | RegexOptions.Latin1);
        }



        public static MatchCollection Matches(byte[] input, string pattern)
        {
            return Regex.Matches(input, pattern, RegexOptions.Multiline | RegexOptions.Latin1);
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
            Debug.Assert(_TEST_BUFFER_SIZE / 256 == matchs.Count);
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
