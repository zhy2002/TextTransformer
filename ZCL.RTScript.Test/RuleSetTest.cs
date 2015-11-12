using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZCL.RTScript;

namespace ZCL.RTScript.Test
{
    [TestClass]
    public class RuleSetTest
    {
        /// <summary>
        /// test parsing and matching
        /// </summary>
        [TestMethod]
        public void RuleSetTest1()
        {
            string srcText = @"1, 2 3, 4 5, 6 7, 8 9, 10";
            var rule = new RTRuleData(@"(\d+)\s*,\s*(\d+)", "", "", new RuleOptions());
            RTRuleSet rules = new RTRuleSet(srcText, new[] { rule });
            string result = rules.ExecuteAll();
            Assert.AreEqual(result, "");
        }

        /// <summary>
        /// test merge and calling function with constants
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public void RuleSetTest2()
        {
            string srcText = @"1, 2 3, 4 5, 6 7, 8 9, 10";
            var rule = new RTRuleData(@"(\d+)\s*,\s*(\d+)", "[@{(+ 1 2 3)@}]", "@{(merge)@}", new RuleOptions());
            RTRuleSet rules = new RTRuleSet(srcText, new[] { rule });
            string result = rules.ExecuteAll();
            Assert.AreEqual(result, "[6] [6] [6] [6] [6]");
        }

        [TestMethod]
        public void TestGetFunction() {
            string srcText = @"goose says this is very good.";
            var rule = new RTRuleData(@"g(o)+[ds]", "[@{($1)@}]", "@{(merge)@}", new RuleOptions());
            RTRuleSet rules = new RTRuleSet(srcText, new[] { rule });
            string result = rules.ExecuteAll();
            Assert.AreEqual("[o]e says this is very [o].", result);
        }

        /// <summary>
        /// test defining var with constant
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public void RuleSetTest3()
        {
            string srcText = @"1, 2 3, 4 5, 6 7, 8 9, 10";
            var rule = new RTRuleData(@"(\d+)\s*,\s*(\d+)", "[@{(:: test 3) (+ test 11)@}]", "@{(merge)@}", new RuleOptions());
            RTRuleSet rules = new RTRuleSet(srcText, new[] { rule });
            string result = rules.ExecuteAll();
            Assert.AreEqual(result, "[14] [14] [14] [14] [14]");
        }

        /// <summary>
        /// test defining var with var
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public void RuleSetTest4()
        {
            string srcText = @"1, 2 3, 4 5, 6 7, 8 9, 10";
            var rule = new RTRuleData(@"(\d+)\s*,\s*(\d+)", "[@{(:: var1 3) (::var2 var1) (+  var1 var2 11)@}]", "@{(merge)@}", new RuleOptions());
            RTRuleSet rules = new RTRuleSet(srcText, new[] { rule });
            string result = rules.ExecuteAll();
            Assert.AreEqual(result, "[17] [17] [17] [17] [17]");
        }

        /// <summary>
        /// test defining var with func
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public void RuleSetTest5()
        {
            string srcText = @"1, 2 3, 4 5, 6 7, 8 9, 10";
            var rule = new RTRuleData(@"(\d+)\s*,\s*(\d+)", "[@{(:: var1 (+ 1 1)) (::var2 (+ var1 1)) (+ var1 var2 11)@}]", "@{(merge)@}", new RuleOptions());
            RTRuleSet rules = new RTRuleSet(srcText, new[] { rule });
            string result = rules.ExecuteAll();
            Assert.AreEqual(result, "[16] [16] [16] [16] [16]");
        }

        /// <summary>
        /// test defining and calling function
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public void RuleSetTest6()
        {
            string srcText = @"1, 2 3, 4 5, 6 7, 8 9, 10";
            var rule = new RTRuleData(@"(\d+)\s*,\s*(\d+)", "[@{(:: add1 (+ x 1)) (::x 2) (+  add1 4)@}]", "@{(merge)@}", new RuleOptions());
            RTRuleSet rules = new RTRuleSet(srcText, new[] { rule });
            string result = rules.ExecuteAll();
            Assert.AreEqual(result, "[7] [7] [7] [7] [7]");
        }

        /// <summary>
        /// test if apply function works
        /// </summary>
        [TestMethod]
        public void RuleSetTest7()
        {
            string srcText = @"1, 2 3, 4 5, 6 7, 8 9, 10";
            var rule = new RTRuleData(@"(\d+)\s*,\s*(\d+)", "[@{ (:: add1 (+ x 1)) (add1 6) @}]", "@{(merge)@}", new RuleOptions());
            RTRuleSet rules = new RTRuleSet(srcText, new[] { rule });
            string result = rules.ExecuteAll();
            Assert.AreEqual(result, "[7] [7] [7] [7] [7]");
        }

        /// <summary>
        /// test that loop works
        /// </summary>
        [TestMethod]
        public void RuleSetTest8()
        {
            string srcText = @"1, 2 3, 4 5, 6 7, 8 9, 10";
            var rule = new RTRuleData(@"(\d+)\s*,\s*(\d+)", "[@{ (for x 1 5 (+ x)) @}]", "@{(merge)@}", new RuleOptions());
            RTRuleSet rules = new RTRuleSet(srcText, new[] { rule });
            string result = rules.ExecuteAll();
            Assert.AreEqual(result, "[12345] [12345] [12345] [12345] [12345]");
        }

    }
}