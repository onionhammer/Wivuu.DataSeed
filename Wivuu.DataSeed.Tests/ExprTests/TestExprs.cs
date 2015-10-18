using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Wivuu.DataSeed.Tests.ExprTests
{
    using static Expr;

    [TestClass]
    public class TestExprs
    {
        private T Map<K, T>(K source, T dest)
        {
            using (var scope = Scope())
            {
                var expr = Lambda<Action<K, T>>(
                    param: new[] { scope.Param<K>("src"), scope.Param<T>("dest") },
                    body: scope.Block(
                        from destProperty in typeof(T).GetProperties()
                        join srcProperty in typeof(K).GetProperties() 
                            on destProperty.Name equals srcProperty.Name
                        select Expr.Call(
                            scope.Ref("dest"),
                            destProperty.SetMethod,
                            Expr.Call(scope.Ref("src"), srcProperty.GetMethod)
                        )
                    )
                );

                expr.Compile().Invoke(source, dest);
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
