using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Ghostyak.Text.RegularExpressions;

namespace BinaryRegexTest
{
    [TestClass]
    public class BinaryRegexTest
    {
        byte[] _source = null;

        byte[] GetArray()
        {
            if(_source == null)
            {
                _source = new byte[512];
                for (int i = 0; i < _source.Length; i++)
                {
                    _source[i] = (byte)i;
                }
            }

            return _source;
        }

        [TestMethod]
        public void TestRegexMatchesByInstance()
        {
            byte[] _source = GetArray();
            BinaryRegex br = new BinaryRegex("\x00[\x00-\xFF]{254}\xFF");
            var matchs = br.Matches(_source);
            Assert.AreEqual(2, matchs.Count);
        }

        [TestMethod]
        public void TestRegexMatchesByStatic()
        {
            byte[] _source = GetArray();
            var matchs = BinaryRegex.Matches(_source, "\x00[\x00-\xFF]{254}\xFF");
            Assert.AreEqual(2, matchs.Count);
        }

        [TestMethod]
        public void TestRegexMatchByInstance()
        {
            byte[] _source = GetArray();
            BinaryRegex br = new BinaryRegex("\x10[\x00-\xFF]{16}\x21");
            var match = br.Match(_source);
            Assert.AreEqual(16, match.Index);
        }

        [TestMethod]
        public void TestRegexMatchByStatic()
        {
            byte[] _source = GetArray();
            var match = BinaryRegex.Match(_source, "\x10[\x00-\xFF]{16}\x21");
            Assert.AreEqual(16, match.Index);
        }
    }
}
