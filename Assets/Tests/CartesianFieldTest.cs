using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class CartesianFieldTest
    {
        [Test]
        public void EmptyFieldVolume()
        {
            Assert.AreEqual(0, new CartesianField<bool>(new ImmutableVector<int>(), _ => false).Volume);
        }

        [Test]
        public void EnumerateOverEmptyField()
        {
            foreach (var none in new CartesianField<bool>(new ImmutableVector<int>(), _ => false))
                Assert.Fail("No elements expected.");
        }

        [Test]
        public void EnumerateOverEmptyFieldWithNonzeroDimension()
        {
            foreach (var none in new CartesianField<bool>(new ImmutableVector<int>(0, 1, 0), _ => false))
                Assert.Fail("No elements expected.");
        }
    }
}
