using Gradient.ObjectHistory;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Gradient.ObjectHistoryTest
{
    [TestClass]
    public class TestHistory
    {
        public History<int> History { get; set; }

        [TestMethod]
        public void TestInt1()
        {
            var history = History<int>.Create(0);
            Assert.AreEqual(0, history.Value);

            history.AddValue(10);
            Assert.AreEqual(10, history.Value);

            history.AddValue(20);
            Assert.AreEqual(20, history.Value);

            history.Undo();
            Assert.AreEqual(10, history.Value);

            history.Redo();
            Assert.AreEqual(20, history.Value);

            history.Undo();
            Assert.AreEqual(10, history.Value);

            history.AddValue(30);
            Assert.AreEqual(30, history.Value);

            history.Undo();
            Assert.AreEqual(10, history.Value);

            history.Redo();
            Assert.AreEqual(30, history.Value);
        }

        [DataTestMethod]
        [DataRow(NullCondition.AllowAll, OnNullError.Ignore, false, 1)]
        [DataRow(NullCondition.AllowAll, OnNullError.Throw, false, 1)]
        [DataRow(NullCondition.AllowInitial, OnNullError.Ignore, false, 1)]
        [DataRow(NullCondition.AllowInitial, OnNullError.Throw, false, 1)]
        [DataRow(NullCondition.Disallow, OnNullError.Ignore, true, 0)] // regardless of OnNullError.Ignore, cannot allow as it is in impossible state
        [DataRow(NullCondition.Disallow, OnNullError.Throw, true, 0)]
        public void TestCreateNull(NullCondition nullCondition, OnNullError onNullError, bool expectedThrow, int expectedCount)
        {
            if (expectedThrow)
                Assert.ThrowsException<NotSupportedException>(() => History<MyClass>.Create(null, nullCondition, onNullError));
            else
            {
                var history = History<MyClass>.Create(null, nullCondition, onNullError);
                Assert.AreEqual(expectedCount, history.Data.Count);
            }
        }

        [DataTestMethod]
        [DataRow(NullCondition.AllowAll, OnNullError.Ignore, false, 2)]
        [DataRow(NullCondition.AllowAll, OnNullError.Throw, false, 2)]
        [DataRow(NullCondition.AllowInitial, OnNullError.Throw, true, 1)]
        [DataRow(NullCondition.AllowInitial, OnNullError.Ignore, false, 1)]
        [DataRow(NullCondition.Disallow, OnNullError.Ignore, false, 1)]
        [DataRow(NullCondition.Disallow, OnNullError.Throw, true, 1)]
        public void TestAddNull(NullCondition nullCondition, OnNullError onNullError, bool expectedThrow, int expectedCount)
        {
            var myClass = new MyClass();
            var history = History<MyClass>.Create(myClass, nullCondition, onNullError);
            if (expectedThrow)
                Assert.ThrowsException<NotSupportedException>(() => history.AddValue(null));
            else
                history.AddValue(null);
            Assert.AreEqual(expectedCount, history.Data.Count);
        }

        [DataTestMethod]
        [DataRow(NullCondition.AllowInitial, OnNullError.Ignore, false, 1)]
        [DataRow(NullCondition.AllowInitial, OnNullError.Throw, true, 1)]
        public void TestAddSame(NullCondition nullCondition, OnNullError onNullError, bool expectedThrow, int expectedCount)
        {
            var myClass = new MyClass();
            var history = History<MyClass>.Create(myClass, nullCondition, onNullError);
            if (expectedThrow)
                Assert.ThrowsException<NotSupportedException>(() => history.AddValue(myClass));
            else
                history.AddValue(myClass);
            Assert.AreEqual(expectedCount, history.Data.Count);
        }

        [DataTestMethod]
        [DataRow(NullCondition.AllowAll, OnNullError.Ignore, false, 1)]
        [DataRow(NullCondition.AllowAll, OnNullError.Throw, true, 1)]
        [DataRow(NullCondition.AllowInitial, OnNullError.Ignore, false, 1)]
        [DataRow(NullCondition.AllowInitial, OnNullError.Throw, true, 1)]
        public void TestAddSameNull(NullCondition nullCondition, OnNullError onNullError, bool expectedThrow, int expectedCount)
        {
            var history = History<MyClass>.Create(null, nullCondition, onNullError);
            if (expectedThrow)
                Assert.ThrowsException<NotSupportedException>(() => history.AddValue(null));
            else
                history.AddValue(null);
            Assert.AreEqual(expectedCount, history.Data.Count);
        }


        [DataTestMethod]
        [DataRow(NullCondition.AllowAll, OnNullError.Ignore, false, 2)]
        [DataRow(NullCondition.AllowAll, OnNullError.Throw, true, 2)]
        [DataRow(NullCondition.AllowInitial, OnNullError.Ignore, false, 2)]
        [DataRow(NullCondition.AllowInitial, OnNullError.Throw, true, 2)]
        public void TestAddSameNullAfterUndo(NullCondition nullCondition, OnNullError onNullError, bool expectedThrow, int expectedCount)
        {
            var history = History<MyClass>.Create(null, nullCondition, onNullError);
            history.AddValue(new MyClass());
            history.Undo();
            if (expectedThrow)
                Assert.ThrowsException<NotSupportedException>(() => history.AddValue(null));
            else
                history.AddValue(null);
            Assert.AreEqual(expectedCount, history.Data.Count);
        }

        [DataTestMethod]
        public void TestInsertBefore()
        {
            var history = History<int>.Create(0);
            history.AddValue(1);
            history.AddValue(2);
            history.AddValue(3);
            Assert.AreEqual(4, history.Data.Count);

            history.Undo(); // 3 -> 2
            history.Undo(); // 2 -> 1
            Assert.AreEqual(4, history.Data.Count);

            history.InsertBefore(11);
            Assert.AreEqual(3, history.Data.Count);

            Assert.AreEqual(0, history.Data[0]);
            Assert.AreEqual(11, history.Data[1]);
            Assert.AreEqual(1, history.Data[2]);
        }
    }
}