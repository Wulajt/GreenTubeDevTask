using GreenTubeDevTask.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GreenTubeDevTask.InMemRepositories
{
    public interface IRepositoryBase<T> where T : IEntity
    {
        Task AddAsync(T item);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(Guid id);
    }
}