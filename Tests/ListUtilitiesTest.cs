using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared.Utility;

namespace Tests
{
    [TestClass]
    public class ListUtilityTest
    {
        [TestMethod]
        public void EqualTo_2StringLists_True()
        {
            var list1 = new List<string> {"a", "b", "c"};
            var list2 = new List<string> {"a", "b", "c"};
            var isEqual = ListUtility.EqualTo(list1, list2);
            Assert.IsTrue(isEqual);
        }

        [TestMethod]
        public void EqualTo_2StringListsDifferentLengths_False()
        {
            var list1 = new List<string> { "a", "b", "c" };
            var list2 = new List<string> { "a", "b" };
            var isEqual = ListUtility.EqualTo(list1, list2);
            Assert.IsFalse(isEqual);
        }

        [TestMethod]
        public void EqualTo_2IntListsSame_True()
        {
            var list1 = new List<int> { 0, 1, 2 };
            var list2 = new List<int> { 0, 1, 2};
            var isEqual = ListUtility.EqualTo(list1, list2);
            Assert.IsTrue(isEqual);
        }

        [TestMethod]
        public void EqualTo_2IntListsDifferent_False()
        {
            var list1 = new List<int> { 0, 1, 2 };
            var list2 = new List<int> { 0, 1, 100 };
            var isEqual = ListUtility.EqualTo(list1, list2);
            Assert.IsFalse(isEqual);
        }

        [TestMethod]
        public void EqualTo_2DummyListsSame_True()
        {
            var list1 = new List<DummyObject> { new DummyObject { Field1 = "a", Field2 = 0 }, new DummyObject { Field1 = "b", Field2 = 1 } };
            var list2 = new List<DummyObject> { new DummyObject { Field1 = "a", Field2 = 0 }, new DummyObject { Field1 = "b", Field2 = 1 } };
            var isEqual = ListUtility.EqualTo(list1, list2);
            Assert.IsTrue(isEqual);
        }

        [TestMethod]
        public void EqualTo_2DummyListsDifferent_False()
        {
            var list1 = new List<DummyObject> { new DummyObject { Field1 = "a", Field2 = 0 }, new DummyObject { Field1 = "b", Field2 = 1 } };
            var list2 = new List<DummyObject> { new DummyObject { Field1 = "a", Field2 = 0 }, new DummyObject { Field1 = "b", Field2 = 100 } };
            var isEqual = ListUtility.EqualTo(list1, list2);
            Assert.IsFalse(isEqual);
        }

    }

    public class DummyObject : IEquatable<DummyObject>
    {
        public string Field1 { get; set; }
        public int Field2 { get; set; }

        public bool Equals(DummyObject other)
        {
            if (Field1 != other.Field1)
            {
                return false;
            }

            if (Field2 != other.Field2)
            {
                return false;
            }

            return true;
        }
    }
}
