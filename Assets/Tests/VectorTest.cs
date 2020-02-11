using NUnit.Framework;
using System.Collections.Generic;

namespace Tests
{
    public class VectorTest
    {
        [Test]
        public void EmptyImmutableVectorSize()
        {
            Assert.AreEqual(0, new ImmutableVector<int>().Dimensionality);
        }

        [Test]
        public void EmptyMutableVectorSize()
        {
            Assert.AreEqual(0, new MutableVector<int>().Dimensionality);
        }

        [Test]
        public void EnumerateOverEmptyImmutableVector()
        {
            foreach (int none in new ImmutableVector<int>())
                Assert.Fail("No elements expected.");
        }

        [Test]
        public void EnumerateOverEmptyMutableVector()
        {
            foreach (int none in new MutableVector<int>())
                Assert.Fail("No elements expected.");
        }

        [Test]
        public void ImmutableVec3Size()
        {
            Assert.AreEqual(3, new ImmutableVector<int>(2, 4, 6).Dimensionality);
        }

        [Test]
        public void EnumerateOverImmutableVec3()
        {
            var values = new List<int>();
            foreach (int value in new ImmutableVector<int>(2, 4, 6))
                values.Add(value);
            Assert.AreEqual(new int[] { 2, 4, 6 }, values);
        }

        [Test]
        public void MutableVec3Size()
        {
            Assert.AreEqual(3, new MutableVector<int>(2, 4, 6).Dimensionality);
        }

        [Test]
        public void EnumerateOverMutableVec3()
        {
            var values = new List<int>();
            foreach (int value in new MutableVector<int>(2, 4, 6))
                values.Add(value);
            Assert.AreEqual(new int[] { 2, 4, 6 }, values);
        }

        [Test]
        public void ConstructImmutableFromMutable()
        {
            var immutable = new ImmutableVector<int>(new MutableVector<int>(1, 2, 3));
            Assert.AreEqual(new int[] { 1, 2, 3 }, immutable);
        }
    }
}
