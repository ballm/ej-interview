using System;
using System.Collections.Generic;
using System.Linq;

namespace Interview
{
    class MemoryRepository<T> : IRepository<T> where T : IStoreable
    {
        private readonly List<T> _store;

        public MemoryRepository()
        {
            _store = new List<T>();
        }

        public IEnumerable<T> All()
        {
            return _store;
        }

        public void Delete(IComparable id)
        {
            _store.Remove(FindById(id));
        }

        public void Save(T item)
        {
            Delete(item.Id);

            _store.Add(item);
        }

        public T FindById(IComparable id)
        {
            var idType = id.GetType();

            return _store.SingleOrDefault(item => item.Id.GetType() == idType && item.Id.CompareTo(id) == 0);            
        }
    }
}
