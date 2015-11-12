using System;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZCL.RTScript;
using ZCL.RTScript.AbstractionLayer;
using ZCL.RTScript.Logic;

namespace ZCL.RTScript.Test
{
    [TestClass]
    public class MatcherTest
    {
        [TestMethod]
        public void DefaultMatchTypeIsGroup()
        {
            var matchOptions = new MatchOptions();
            Assert.AreEqual(MatchType.Group, matchOptions.MatchType);
        }

        [TestMethod]
        public void DefaultRegexOptionsIsNone()
        {
            var matchOptions = new MatchOptions();
            Assert.AreEqual(RegexOptions.None, matchOptions.RegexOptions);
        }

        [ExpectedException(typeof(ArgumentNullException))]
        [TestMethod]
        public void ArgumentNullExceptionWhenRegexIsNull()
        {
            new RTMatcher(null, new MatchOptions());
        }

        private IRTMatchList GetMatchList(string srcText, string regex, RegexOptions options = RegexOptions.None, MatchType matchType = MatchType.Group)
        {
            return new RTMatcher(regex, new MatchOptions() { MatchType = matchType, RegexOptions = options }).Execute(srcText);
        }

        [ExpectedException(typeof(ArgumentNullException))]
        [TestMethod]
        public void ArgumentNullExceptionWhenSrcTextIsNull()
        {
            new RTMatcher(".+", new MatchOptions()).Execute(null);
        }

        [TestMethod]
        public void MatchWithEmptyRegex()
        {
            var result = GetMatchList("abc", string.Empty);
            Assert.AreEqual(4, result.Count);
            for (var i = 0; i < result.Count; i++)
            {
                var match = result[i];
                Assert.AreEqual(1, match.Count);
                Assert.AreEqual(string.Empty, match.GetItem(0));
            }
        }

        [TestMethod]
        public void MatchEmptyString()
        {
            var result = GetMatchList(string.Empty, @"\w+");
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void MatchWords()
        {
            var result = GetMatchList("this is a test", @"\w+");
            Assert.AreEqual(4, result.Count);
            Assert.AreEqual(1, result[0].Count);
            Assert.AreEqual("this", result[0].GetItem(0));
            Assert.AreEqual(1, result[1].Count);
            Assert.AreEqual("is", result[1].GetItem(0));
            Assert.AreEqual(1, result[2].Count);
            Assert.AreEqual("a", result[2].GetItem(0));
            Assert.AreEqual(1, result[3].Count);
            Assert.AreEqual("test", result[3].GetItem(0));
        }

        [TestMethod]
        public void MatchFirstNameLastName()
        {
            string srcText = @"john rambo
michael jackson
bertrand russell";

            var result = GetMatchList(srcText, @"(\w+)\s+(\w+)");
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("john", result[0].GetItem(1));
            Assert.AreEqual("rambo", result[0].GetItem(2));
            Assert.AreEqual("michael", result[1].GetItem(1));
            Assert.AreEqual("jackson", result[1].GetItem(2));
            Assert.AreEqual("bertrand", result[2].GetItem(1));
            Assert.AreEqual("russell", result[2].GetItem(2));
        }

        [TestMethod]
        public void MatchLines()
        {
            string srcText = @"Line1
Line2 

 Line4 ";
            var result = GetMatchList(srcText, @".+$", RegexOptions.Multiline);
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("Line1", result[0].GetItem(0));
            Assert.AreEqual("Line2 ", result[1].GetItem(0));
            Assert.AreEqual(" Line4 ", result[2].GetItem(0));
        }

        [TestMethod]
        public void MatchNumbers()
        {
            string srcText = @"12 33 45
32 45 1
43 46 98
222 91 17";
            var result = GetMatchList(srcText, @"(\d+)\s+(\d+)\s+(\d+)\s*$", RegexOptions.Multiline);
            Assert.AreEqual(4, result.Count);
            for (int i = 0; i < result.Count; i++)
            {
                var match = result[i];
                Assert.AreEqual(4, match.Count);
            }
        }

        [TestMethod]
        public void MatchWordTable() {
            string srcText = @"apple orange pear
red yellow green blue
card paper
";
            var result = GetMatchList(srcText, @"(\S+[^\S\r\n]*)+", RegexOptions.None, MatchType.Capture);
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual(3, result[0].Count);
            Assert.AreEqual(4, result[1].Count);
            Assert.AreEqual(2, result[2].Count);
        }

        [TestMethod]
        public void MatchNameValues() {
            string srcText = @"john 15 -23 55 61
jane 32 49 15";

            var result = GetMatchList(srcText, @"(\w+)\s+(-?\d+[ \t]*)+", RegexOptions.None, MatchType.Capture);
            Assert.AreEqual(4, result.Count);
            Assert.AreEqual(1, result[0].Count);
            Assert.AreEqual(4, result[1].Count);
            Assert.AreEqual(1, result[2].Count);
            Assert.AreEqual(3, result[3].Count);

        }
    }
}
