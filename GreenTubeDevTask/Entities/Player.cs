using System;

namespace GreenTubeDevTask.Entities
{
    public class Player : IEntity
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
