using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Wivuu.DataSeed.Tests.ExprTests
{
    using System.Diagnostics;
    using System.Linq.Expressions;
    using static Expr;

    [TestClass]
    public class TestExprs
    {
        private T Map<K, T>(K source, T dest)
        {
            using (var scope = Scope())
            {
                var expr = Lambda<Func<int, string, string>>(
                    param: new[] { scope.Param<int>("x"), scope.Param<string>("message") },
                    body: scope.Block(
                        scope.Expr(() => Trace.WriteLine("Hello!" + scope.Ref("message"))),
                        scope.Expr(() => scope.Ref("x").ToString())
                    )
                );

                var result = expr.Compile().Invoke(5, "Test");
                return dest;
            }
        }

        [TestMethod]
        public void TestExprs1()
        {
            var a = new
            {
                Item1 = "A",
                Item2 = 5
            };

            // Map record
            var b = Map(a, new Record());
            Assert.AreEqual(a.Item1, b.Item1);
            Assert.AreEqual(a.Item2, b.Item2);
        }

    }

    class Record
    {
        public string Item1 { get; set; }
        public int Item2 { get; set; }
    }
}
