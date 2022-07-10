using GreenTubeDevTask.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GreenTubeDevTask.InMemRepositories
{
    public abstract class BaseRepository<T> where T : IEntity
    {
        private readonly List<T> _repository = new();
        public IEnumerable<T> Get()
        {
            return _repository;
        }
        public T GetById(Guid id)
        {
            return _repository.Where(item => item.Id == id).FirstOrDefault();
        }
        public void Add(T item)
        {
            _repository.Add(item);
        }
        //public bool DeleteById(Guid id)
        //{
        //    var item = GetById(id);
        //    if (item is null) return false;
        //    return _repository.Remove(item);
        //}
    }
}
