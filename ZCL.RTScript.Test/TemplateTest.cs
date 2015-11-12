using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZCL.RTScript.Logic;

namespace ZCL.RTScript.Test
{
    [TestClass]
    public class TemplateTest
    {
        [TestMethod]
        public void TestLiteralToExpressionList()
        {
            string templateScript = @"Literal content @{ (if true 1) @} end of literal content.";
            RTTemplate template = new RTTemplate(templateScript, new TemplateOptions());
            string result = template.Execute(null);
            Assert.AreEqual(@"Literal content 1 end of literal content.", result);
        }

        [TestMethod]
        public void LiteralReplaceOpShouldWork() {

            string templateScript = @"1 + 1 = @{ (if true @{{sum 1,1} @}) @}";
            RTTemplate template = new RTTemplate(templateScript, new TemplateOptions());
            string result = template.Execute(null);
            Assert.AreEqual(@"1 + 1 = 2 ", result);
        }

        [TestMethod]
        public void TestExpressionListyToLiteral()
        {
            string templateScript = @"Literal content @{ (if true @{back to literal content@}) @} end of literal content.";
            RTTemplate template = new RTTemplate(templateScript, new TemplateOptions());
            string result = template.Execute(null);
            Assert.AreEqual(@"Literal content back to literal content end of literal content.", result);
        }

        [TestMethod]
        public void TestEscapeAtSymbol()
        {
            string templateScript = @"a single @ sign is output as @; two @ signs are output as a single @@ sign so that in literal we can output @@{{ and @@}.";
            RTTemplate template = new RTTemplate(templateScript, new TemplateOptions());
            string result = template.Execute(null);
            Assert.AreEqual("a single @ sign is output as @; two @ signs are output as a single @ sign so that in literal we can output @{ and @}.", result);
        }

        [TestMethod]
        public void TestFunctionList()
        {
            string templateScript = @"@{
    (+ 1 1.4)
    (+ 7 7)
@}";
            RTTemplate template = new RTTemplate(templateScript, new TemplateOptions());
            string result = template.Execute(null);
            Assert.AreEqual("2.414", result);
        }


        [TestMethod]
        [ExpectedException(typeof(RTParsingException))]
        public void TestParsingError()
        {
            string templateScript = @"@{
    abc
@}";
            RTTemplate template = new RTTemplate(templateScript, new TemplateOptions());
            string result = template.Execute(null);
            
            //todo fix this bug, cause is when parsing function, didnt check the current char is start bracket.
        }

        [TestMethod]
        public void TestForFunction()
        {
            string script = @"@{
(:: myfunc (+ 10 x y))
(for i 1 3 @{@{(myfunc i 10)@}
@})
@}";
            RTTemplate template = new RTTemplate(script, new TemplateOptions());
            string result = template.Execute(null);
            Assert.AreEqual(result,
@"21
22
23
");
        }

        [TestMethod]
        public void TestGetFunction()
        {
            string srcText = "1 2 3 4 5";
            string templateScript = @"@{
(for i 1 5 
    (+ ($i) 1)
)
@}";

            RTMatcher matcher = new RTMatcher(@"(\d)\s(\d)\s(\d)\s(\d)\s(\d)", new MatchOptions());
            var match = matcher.Execute(srcText);

            RTTemplate template = new RTTemplate(templateScript, new TemplateOptions());
            string result = template.Execute(match[0]);
            Assert.AreEqual(result, "23456");
        }

        /// <summary>
        /// reverse digits.
        /// </summary>
        [TestMethod]
        public void TestItemCountFunction()
        {
            string srcText = "12345";
            string templateScript = @"@{
(for i (itemCount) 0 
    ($i)
)
@}";

            RTMatcher matcher = new RTMatcher(@"(\d)+", new MatchOptions() { MatchType = MatchType.Capture });
            var match = matcher.Execute(srcText);

            RTTemplate template = new RTTemplate(templateScript, new TemplateOptions());
            string result = template.Execute(match[0]);
            Assert.AreEqual(result, "54321");
        }

        /// <summary>
        /// test if function
        /// </summary>
        [TestMethod]
        public void TestIfFunction()
        {
            string templateScript = @"@{
(if true (* 5 3) 2)
@}";

            RTTemplate template = new RTTemplate(templateScript, new TemplateOptions());
            string result = template.Execute(null);
            Assert.AreEqual(result, "15");
        }

        [TestMethod]
        public void TestDefFunction()
        {
            string templateScript = @"@{
(:: x 2)
(:: y 1)
(if false x y)
@}";

            RTTemplate template = new RTTemplate(templateScript, new TemplateOptions());
            string result = template.Execute(null);
            Assert.AreEqual(result, "1");
        }

        [TestMethod]
        public void TestSetFunction()
        {
            string templateScript = @"@{
(:: x y)
(:: y x)
(::z (if false x y))
(:= y 55)
(+ y 1)
@}";

            RTTemplate template = new RTTemplate(templateScript, new TemplateOptions());
            string result = template.Execute(null);
            Assert.AreEqual(result, "56");
        }

        /// <summary>
        /// Factorial function.
        /// </summary>
        [TestMethod]
        public void TestRecursionBaseCase()
        {
            string templateScript = @"@{
(:: fac (if (= x 1)
            1
            (* (fac (+ x -1)) x)
        )
)

(fac 1)
@}";

            RTTemplate template = new RTTemplate(templateScript, new TemplateOptions());
            string result = template.Execute(null);
            Assert.AreEqual(result, "1");
        }

        [TestMethod]
        public void TestRecursionFac2()
        {
            string templateScript = @"@{
(:: fac (if (= x 1)
            1
            (* (fac (+ x -1)) x)
        )
)

(fac 2)
@}";

            RTTemplate template = new RTTemplate(templateScript, new TemplateOptions());
            string result = template.Execute(null);
            Assert.AreEqual(result, "2");
        }

        [TestMethod]
        public void TestRecursionFac5()
        {
            string templateScript = @"@{
(:: fac (if (= x 1)
            1
            (* (fac (+ x -1)) x)
        )
)

(fac 5)
@}";

            RTTemplate template = new RTTemplate(templateScript, new TemplateOptions());
            string result = template.Execute(null);
            Assert.AreEqual(result, "120");
        }

        [TestMethod]
        public void TestAccumulation()
        {
            string templateScript = @"@{
(:: x 0)
(for i 1 100
    (:= x (+ x i))
)
(+ x)
@}";

            RTTemplate template = new RTTemplate(templateScript, new TemplateOptions());
            string result = template.Execute(null);
            Assert.AreEqual(result, "5050");
        }

        [TestMethod]
        public void TestSumOddNumbers()
        {
            string templateScript = @"@{
(:: x 0)
(for i 1 10
    (:= x   (if (residueIs i 2 1)
                (+ x i)
                x
            )
    )
)
(+ x)
@}";

            RTTemplate template = new RTTemplate(templateScript, new TemplateOptions());
            string result = template.Execute(null);
            Assert.AreEqual(result, "25");
        }


        //todo more unit testing
    }

}
