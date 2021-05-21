using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;

namespace GhostYak.Text.RegularExpressions
{
    public class BinaryRegex
    {

        Regex _regex;


        public BinaryRegex(string pattern)
        {
            _regex = new Regex(pattern, RegexOptions.Multiline);
        }

        public Match Match(byte[] input)
        {
            string sInput = Encoding.GetEncoding("latin1").GetString(input);
            return _regex.Match(sInput);
        }

        public Match Match(byte[] input, int startat)
        {
            string sInput = Encoding.GetEncoding("latin1").GetString(input);
            return _regex.Match(sInput, startat);
        }

        public Match Match(byte[] input, int beginning, int length)
        {
            string sInput = Encoding.GetEncoding("latin1").GetString(input);
            return _regex.Match(sInput, beginning, length);
        }

        //------------------------------------------------------------------



        public MatchCollection Matches(byte[] input)
        {
            string sInput = Encoding.GetEncoding("latin1").GetString(input);
            return _regex.Matches(sInput);
        }



        public MatchCollection Matches(byte[] input, int startat)
        {
            string sInput = Encoding.GetEncoding("latin1").GetString(input);
            return _regex.Matches(sInput, startat);
        }



        //------------------------------------------------------------------


        public static Match Match(byte[] input, string pattern)
        {
            string sInput = Encoding.GetEncoding("latin1").GetString(input);
            return Regex.Match(sInput, pattern, RegexOptions.Multiline);
        }



        public static MatchCollection Matches(byte[] input, string pattern)
        {
            string sInput = Encoding.GetEncoding("latin1").GetString(input);
            return Regex.Matches(sInput, pattern, RegexOptions.Multiline);
        }



    }
}
