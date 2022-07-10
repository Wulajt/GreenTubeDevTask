using GreenTubeDevTask.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace GreenTubeDevTask.InMemRepositories
{
    public class PlayerRepository : BaseRepository<Player>
    {
        public Player GetByUsername(string username)
        {
            return Get().Where(player => player.Username == username).FirstOrDefault();
        }
        public bool UsernameExists(string username)
        {
            return Get().ToList().Exists(player => player.Username == username);
        }
    }
}
