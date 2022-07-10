using GreenTubeDevTask.Entities;
using System;
using System.Collections.Generic;

namespace GreenTubeDevTask.InMemRepositories
{
    public interface IRepositoryBase<T> where T : IEntity
    {
        void Add(T item);
        IEnumerable<T> GetAll();
        T GetById(Guid id);
    }
}