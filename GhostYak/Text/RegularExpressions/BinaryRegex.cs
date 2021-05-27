using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Re2.Net;
using System.IO;

namespace GhostYak.Text.RegularExpressions
{
    public class BinaryRegex
    {

        Regex _regex;


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



    }
}
