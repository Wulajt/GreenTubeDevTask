using GreenTubeDevTask.Entities;

namespace GreenTubeDevTask.InMemRepositories
{
    public interface IPlayerRepository : IRepositoryBase<Player>
    {
        Player GetByUsername(string username);
    }
}