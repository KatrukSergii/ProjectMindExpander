using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using Shared.Utility;

namespace Tests
{
    [TestClass]
    public class TypeUtilityTests
    {
        [TestMethod]
        public void GetGenericType_NonGenericType_Emptystring()
        {
            var type = "Foo";
            var returnType = TypeUtility.GetGenericType(type);
            Assert.AreEqual(string.Empty,returnType);
        }

        [TestMethod]
        public void GetGenericType_GenericType_GenericType()
        {
            var type = "Foo<Bar>";
            var returnType = TypeUtility.GetGenericType(type);
            Assert.AreEqual("Foo", returnType);
        }

        // TODO  FIX!
        [TestMethod]
        public void GetGenericType_NestedGenericType_GenericType()
        {
            string type = "Foo<Bar<X>>";
            var returnType = TypeUtility.GetGenericType(type);

            // Returns <Foo<Bar>
            Assert.AreEqual("Foo", returnType);
        }

        [TestMethod]
        public void GetGenericTypeParameter_NonGenericType_EmptyString()
        {
            var type = "Foo";
            var returnType = TypeUtility.GetGenericTypeParameter(type);
            Assert.AreEqual(string.Empty,returnType);
        }

        [TestMethod]
        public void GetGenericTypeParameter_GenericType_GenericTypeParameter()
        {
            var type = "Foo<Bar>";
            var returnType = TypeUtility.GetGenericTypeParameter(type);
            Assert.AreEqual("Bar", returnType);
        }

        [TestMethod]
        public void GetGenericTypeParameter_NestedGenericType_GenericTypeParameter()
        {
            var type = "Foo<Bar<X>>";
            var returnType = TypeUtility.GetGenericTypeParameter(type);
            Assert.AreEqual("Bar<X>", returnType);
        }

        private const string _prefix = "PREFIX_";
        [TestMethod]
        public void ConvertTypeToObservableTypeName_NonGenericType_SameTypeName()
        {
            var type = "Foo";
            var types = new List<string> {"a", "b", "c"};
            var returnType = TypeUtility.ConvertTypeNameToObservableTypeName(type, types, _prefix);
            Assert.AreEqual("Foo", returnType);
        }

        [TestMethod]
        public void ConvertTypeToObservableTypeName_ObservableType_ModifiedType()
        {
            var type = "Foo";
            var types = new List<string> {"a", "b", "Foo"};
            var returnType = TypeUtility.ConvertTypeNameToObservableTypeName(type, types, _prefix);
            Assert.AreEqual("PREFIX_Foo", returnType);
        }

        [TestMethod]
        public void ConvertTypeToObservableTypeName_ListWithNonObservableParameterType_ModifiedListType()
        {
            var type = "List<string>";
            var types = new List<string> {"a", "b", "c"};
            var returnType = TypeUtility.ConvertTypeNameToObservableTypeName(type, types, _prefix);
            Assert.AreEqual("PREFIX_List<string>",returnType);
        }

        [TestMethod]
        public void ConvertTypeToObservableTypeName_ListWithObservableParameterType_ModifiedListTypeAndObservableParameterType()
        {
            var type = "List<Foo>";
            var types = new List<string> {"a", "b", "Foo"};
            var returnType = TypeUtility.ConvertTypeNameToObservableTypeName(type, types, _prefix);
            Assert.AreEqual("PREFIX_List<PREFIX_Foo>", returnType);
        }
        
        [TestMethod]
        public void ConvertTypeToObservableTypeName_NestedTypes_CorrectPrefixesOnObservableTypes()
        {
            var type = "Foo<Bar<Badger<Monkey>>>";
            var types = new List<string> {"Foo", "Bar", "Badger", "Monkey"};
            var returnType = TypeUtility.ConvertTypeNameToObservableTypeName(type, types, _prefix);
            Assert.AreEqual("PREFIX_Foo<PREFIX_Bar<PREFIX_Badger<PREFIX_Monkey>>>", returnType);
        }

        [TestMethod]
        public void ConvertTypeToObservableTypeName_NestedGenericList_CorrectPrefixesOnAllTypes()
        {
            var type = "List<List<string>>";
            var types = new List<string> {"Foo", "Bar"};
            var returnType = TypeUtility.ConvertTypeNameToObservableTypeName(type, types, _prefix);
            Assert.AreEqual("PREFIX_List<PREFIX_List<string>>", returnType);
        }

        [TestMethod]
        public void ConvertTypeToObservableTypeName_MixedNestedTypes_CorrectPrefixesOnAllTypes()
        {
            var type = "List<Foo<Bar<Collection<string>>>";
            var types = new List<string> {"Foo", "Bar"};
            var returnType = TypeUtility.ConvertTypeNameToObservableTypeName(type, types, _prefix);
            Assert.AreEqual("PREFIX_List<PREFIX_Foo<PREFIX_Bar<Collection<string>>>", returnType);
        }
    }
}
