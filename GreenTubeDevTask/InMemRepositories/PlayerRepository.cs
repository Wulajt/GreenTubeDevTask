using GreenTubeDevTask.Entities;
using System.Linq;

namespace GreenTubeDevTask.InMemRepositories
{
    public class PlayerRepository : RepositoryBase<Player>, IPlayerRepository, IRepositoryBase<Player>
    {
        public Player GetByUsername(string username)
        {
            return GetAll().Where(player => player.Username == username).FirstOrDefault();
        }
    }
}
