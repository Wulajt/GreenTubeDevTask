using GreenTubeDevTask.Entities;
using System.Threading.Tasks;

namespace GreenTubeDevTask.InMemRepositories
{
    public interface IPlayerRepository : IRepositoryBase<Player>
    {
        Task<Player> GetByUsernameAsync(string username);
    }
}