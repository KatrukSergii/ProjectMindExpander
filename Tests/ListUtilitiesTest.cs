using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared.Utility;

namespace Tests
{
    [TestClass]
    public class ListUtilitiesTest
    {
        [TestMethod]
        public void EqualTo_2StringLists_True()
        {
            var list1 = new List<string> {"a", "b", "c"};
            var list2 = new List<string> {"a", "b", "c"};
            var isEqual = ListUtilities<string>.EqualTo(list1, list2);
            Assert.IsTrue(isEqual);
        }

        [TestMethod]
        public void EqualTo_2StringListsDifferentLengths_False()
        {
            var list1 = new List<string> { "a", "b", "c" };
            var list2 = new List<string> { "a", "b" };
            var isEqual = ListUtilities<string>.EqualTo(list1, list2);
            Assert.IsFalse(isEqual);
        }

        [TestMethod]
        public void EqualTo_2IntListsSame_True()
        {
            var list1 = new List<int> { 0, 1, 2 };
            var list2 = new List<int> { 0, 1, 2};
            var isEqual = ListUtilities<int>.EqualTo(list1, list2);
            Assert.IsTrue(isEqual);
        }

        [TestMethod]
        public void EqualTo_2IntListsDifferent_False()
        {
            var list1 = new List<int> { 0, 1, 2 };
            var list2 = new List<int> { 0, 1, 100 };
            var isEqual = ListUtilities<int>.EqualTo(list1, list2);
            Assert.IsFalse(isEqual);
        }

        [TestMethod]
        public void EqualTo_2DummyListsSame_True()
        {
            var list1 = new List<DummyObject> { new DummyObject { Field1 = "a", Field2 = 0 }, new DummyObject { Field1 = "b", Field2 = 1 } };
            var list2 = new List<DummyObject> { new DummyObject { Field1 = "a", Field2 = 0 }, new DummyObject { Field1 = "b", Field2 = 1 } };
            var isEqual = ListUtilities<DummyObject>.EqualTo(list1, list2);
            Assert.IsTrue(isEqual);
        }

        [TestMethod]
        public void EqualTo_2DummyListsDifferent_False()
        {
            var list1 = new List<DummyObject> { new DummyObject { Field1 = "a", Field2 = 0 }, new DummyObject { Field1 = "b", Field2 = 1 } };
            var list2 = new List<DummyObject> { new DummyObject { Field1 = "a", Field2 = 0 }, new DummyObject { Field1 = "b", Field2 = 100 } };
            var isEqual = ListUtilities<DummyObject>.EqualTo(list1, list2);
            Assert.IsFalse(isEqual);
        }

    }

    public class DummyObject : IEqualityComparer<DummyObject>
    {
        public string Field1 { get; set; }
        public int Field2 { get; set; }


        public bool Equals(DummyObject x, DummyObject y)
        {
            if (x.Field1 != y.Field1)
            {
                return false;
            }

            if (x.Field2 != y.Field2)
            {
                return false;
            }

            return true;
        }

        public int GetHashCode(DummyObject obj)
        {
            throw new InvalidOperationException("Cannot use Dummy Object in Dictionary");
        }
    }
}
