using System;
using System.Collections.Generic;
using System.Linq;
using NetflixStatizier.Utilities;
using NUnit.Framework;

namespace NetflixStatizier.Tests
{
    [TestFixture]
    public class EnumerableUtilities_Tests
    {
        [Test]
        public void GetNth_On_Empty_List_Returns_Empty_List()
        {
            var emptyList = Enumerable.Empty<string>().ToList();

            var returnedList = emptyList.GetNth(5).ToList();

            Assert.That(returnedList.Count, Is.EqualTo(0));
            Assert.That(returnedList, Is.EqualTo(emptyList));
        }

        [Test]
        public void GetNth_With_N_Zero_Throws_ArgumentOutOfRangeException()
        {
            var list = GetListWithNumbersBetween1And20().ToList();

            var ex = Assert.Throws<ArgumentOutOfRangeException>(delegate { var returnedList = list.GetNth(0).ToList(); });
            Assert.That(ex.ParamName, Is.EqualTo("n"));
        }

        [Test]
        public void GetNth_With_N_One_Returns_Original_List()
        {
            var list = GetListWithNumbersBetween1And20().ToList();

            var returnedList = list.GetNth(1).ToList();

            Assert.That(returnedList.Count, Is.EqualTo(list.Count));
            Assert.That(returnedList, Is.EqualTo(list));
        }


        private static IEnumerable<int> GetListWithNumbersBetween1And20() => Enumerable.Range(1, 20);
    }
}
