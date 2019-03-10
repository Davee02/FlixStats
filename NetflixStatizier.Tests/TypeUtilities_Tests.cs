using System;
using NetflixStatizier.Utilities;
using NUnit.Framework;

namespace NetflixStatizier.Tests
{
    [TestFixture]
    public class TypeUtilities_Tests
    {
        [Test]
        public void IsNumericType_Returns_False_For_Non_Numeric()
        {
            Assert.False(TypeUtilities.IsNumericType(null));
            Assert.False(TypeUtilities.IsNumericType(typeof(object)));
            Assert.False(TypeUtilities.IsNumericType(typeof(DBNull)));
            Assert.False(TypeUtilities.IsNumericType(typeof(bool)));
            Assert.False(TypeUtilities.IsNumericType(typeof(char)));
            Assert.False(TypeUtilities.IsNumericType(typeof(DateTime)));
            Assert.False(TypeUtilities.IsNumericType(typeof(string)));
        }

        [Test]
        public void IsNumericType_Returns_False_For_Any_Array()
        {
            Assert.False(TypeUtilities.IsNumericType(typeof(object[])));
            Assert.False(TypeUtilities.IsNumericType(typeof(DBNull[])));
            Assert.False(TypeUtilities.IsNumericType(typeof(bool[])));
            Assert.False(TypeUtilities.IsNumericType(typeof(char[])));
            Assert.False(TypeUtilities.IsNumericType(typeof(DateTime[])));
            Assert.False(TypeUtilities.IsNumericType(typeof(string[])));
            Assert.False(TypeUtilities.IsNumericType(typeof(byte[])));
            Assert.False(TypeUtilities.IsNumericType(typeof(decimal[])));
            Assert.False(TypeUtilities.IsNumericType(typeof(double[])));
            Assert.False(TypeUtilities.IsNumericType(typeof(short[])));
            Assert.False(TypeUtilities.IsNumericType(typeof(int[])));
            Assert.False(TypeUtilities.IsNumericType(typeof(long[])));
            Assert.False(TypeUtilities.IsNumericType(typeof(sbyte[])));
            Assert.False(TypeUtilities.IsNumericType(typeof(float[])));
            Assert.False(TypeUtilities.IsNumericType(typeof(ushort[])));
            Assert.False(TypeUtilities.IsNumericType(typeof(uint[])));
            Assert.False(TypeUtilities.IsNumericType(typeof(ulong[])));
        }

        [Test]
        public void IsNumericType_Returns_False_For_Numeric()
        {
            Assert.True(TypeUtilities.IsNumericType(typeof(byte)));
            Assert.True(TypeUtilities.IsNumericType(typeof(decimal)));
            Assert.True(TypeUtilities.IsNumericType(typeof(double)));
            Assert.True(TypeUtilities.IsNumericType(typeof(short)));
            Assert.True(TypeUtilities.IsNumericType(typeof(int)));
            Assert.True(TypeUtilities.IsNumericType(typeof(long)));
            Assert.True(TypeUtilities.IsNumericType(typeof(sbyte)));
            Assert.True(TypeUtilities.IsNumericType(typeof(float)));
            Assert.True(TypeUtilities.IsNumericType(typeof(ushort)));
            Assert.True(TypeUtilities.IsNumericType(typeof(uint)));
            Assert.True(TypeUtilities.IsNumericType(typeof(ulong)));
        }

        [Test]
        public void IsNumericType_Returns_True_For_Nullable_Numeric()
        {
            Assert.True(TypeUtilities.IsNumericType(typeof(byte?)));
            Assert.True(TypeUtilities.IsNumericType(typeof(decimal?)));
            Assert.True(TypeUtilities.IsNumericType(typeof(double?)));
            Assert.True(TypeUtilities.IsNumericType(typeof(short?)));
            Assert.True(TypeUtilities.IsNumericType(typeof(int?)));
            Assert.True(TypeUtilities.IsNumericType(typeof(long?)));
            Assert.True(TypeUtilities.IsNumericType(typeof(sbyte?)));
            Assert.True(TypeUtilities.IsNumericType(typeof(float?)));
            Assert.True(TypeUtilities.IsNumericType(typeof(ushort?)));
            Assert.True(TypeUtilities.IsNumericType(typeof(uint?)));
            Assert.True(TypeUtilities.IsNumericType(typeof(ulong?)));
        }
    }
}
