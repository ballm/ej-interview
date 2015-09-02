using System.Diagnostics;
using System.Linq;
using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;

namespace Interview
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void AllShouldReturnEmptyIEnumerableWhenRepositoryIsFirstInitialised()
        {
            var result = CreateRepository().All();           

            Assert.AreEqual(Enumerable.Empty<IStoreable>(), result);
        }

        [Test]
        public void SaveWithANewItemShouldAddIt()
        {
            IList<IStoreable> items;
            var repository = CreateAndPopulateRepository(new IComparable[] {1}, out items);

            var result = repository.All();

            Assert.AreSame(items[0], result.ElementAt(0));
        }

        [Test]
        public void SaveTwiceWithTwoNewItemsShouldAddThemBoth()
        {
            IList<IStoreable> items;
            var repository = CreateAndPopulateRepository(new IComparable[] {1, 2}, out items);
            
            var result = repository.All().ToList();

            Assert.AreSame(items[0], result[0]);
            Assert.AreSame(items[1], result[1]);
        }

        [Test]
        public void SaveShouldUpdateAnExistingItemWithMatchingId()
        {
            IList<IStoreable> items;
            var repository = CreateAndPopulateRepository(new IComparable[] { "ABC", "ABC" }, out items);
            
            var result = repository.All().ToList();

            Assert.AreEqual(1, result.Count);
            Assert.AreSame(items[1], result[0]);
        }

        [Test]
        public void SaveShouldBeSuccesfulIfTheTypeOfIdIsDifferentToExistingItemIds()
        {
            IList<IStoreable> items;
            var repository = CreateAndPopulateRepository(new IComparable[] { 1, "ABC" }, out items);
            
            var result = repository.All().ToList();

            Assert.AreEqual(items[0], result[0]);
            Assert.AreEqual(items[1], result[1]);
        }

        [Test]
        public void FindByIdWithAnExistingIdShouldReturnTheItem()
        {
            IList<IStoreable> items;
            var repository = CreateAndPopulateRepository(new IComparable[] { 1, 2 }, out items);

            var result = repository.FindById(1);

            Assert.AreSame(items[0], result);
        }

        [Test]
        public void FindByIdWithANotExistingIdShouldReturnDefault()
        {
            IList<IStoreable> items;
            var repository = CreateAndPopulateRepository(new IComparable[] { 1 }, out items);

            var result = repository.FindById(2);

            Assert.AreEqual(default(IStoreable), result);
        }

        [Test]
        public void FindByIdWhenTheRepositoryIsStoringItemsWithDifferentIdTypesShouldReturnItemWithMatchingId()
        {
            IList<IStoreable> items;
            var repository = CreateAndPopulateRepository(new IComparable[] { 1, "A" }, out items);            

            var result = repository.FindById("A");

            Assert.AreEqual(items[1], result);
        }
        
        [Test]
        public void DeleteShouldRemoveOnlyTheItemWithMatchingId()
        {
            IList<IStoreable> items;
            var repository = CreateAndPopulateRepository(new IComparable[] { 1, 2 }, out items);

            repository.Delete(1);

            var result1 = repository.FindById(1);
            var result2 = repository.FindById(2);

            Assert.AreEqual(default(IStoreable), result1);
            Assert.AreSame(items[1], result2);
        }

        [Test]
        public void DeleteShouldNotThrowAnExceptionIfTheItemDoesNotExist()
        {
            var repository = CreateRepository();

            Assert.DoesNotThrow(() => repository.Delete("ABC"));
        }

        private static IStoreable CreateTestItem(IComparable id)
        {
            var item = new Mock<IStoreable>();
            item.Setup(m => m.Id).Returns(id);

            return item.Object;
        }

        private static IRepository<IStoreable> CreateRepository()
        {
            return new MemoryRepository<IStoreable>();
        } 

        private static IRepository<IStoreable> CreateAndPopulateRepository(IEnumerable<IComparable> ids, out IList<IStoreable> items)
        {
            var repository = CreateRepository();
            
            items = new List<IStoreable>();
            foreach (var id in ids)
            {
                var item = CreateTestItem(id);
                repository.Save(item);

                items.Add(item);
            }

            return repository;
        }

    }
}