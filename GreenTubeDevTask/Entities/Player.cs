using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GreenTubeDevTask.Entities
{
    public class Player : IEntity
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
