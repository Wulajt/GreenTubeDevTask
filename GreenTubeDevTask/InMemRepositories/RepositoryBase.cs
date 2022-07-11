using GreenTubeDevTask.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GreenTubeDevTask.InMemRepositories
{
    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : IEntity
    {
        protected readonly List<T> _repository = new();
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await Task.FromResult(
                _repository);
        }
        public async Task<T> GetByIdAsync(Guid id)
        {
            return await Task.FromResult(
                _repository.Where(item => item.Id == id).FirstOrDefault());
        }
        public async Task AddAsync(T item)
        {
            _repository.Add(item);
            await Task.CompletedTask;
        }
        //public bool DeleteById(Guid id)
        //{
        //    var item = GetById(id);
        //    if (item is null) return false;
        //    return _repository.Remove(item);
        //}
    }
}
