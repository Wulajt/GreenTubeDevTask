using GreenTubeDevTask.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace GreenTubeDevTask.InMemRepositories
{
    public class PlayerRepository : RepositoryBase<Player>, IPlayerRepository, IRepositoryBase<Player>
    {
        public async Task<Player> GetByUsernameAsync(string username)
        {
            return (await GetAllAsync()).Where(player => player.Username == username).FirstOrDefault();
        }
    }
}
