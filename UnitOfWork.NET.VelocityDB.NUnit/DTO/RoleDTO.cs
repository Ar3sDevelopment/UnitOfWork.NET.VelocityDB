using System.Collections.Generic;

namespace UnitOfWork.NET.LiteDB.NUnit.DTO
{
    public class RoleDTO
    {
        public ulong Id { get; set; }
        public string Description { get; set; }
        public virtual ICollection<UserDTO> Users { get; set; }
    }
}
