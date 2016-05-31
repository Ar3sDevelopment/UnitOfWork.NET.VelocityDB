using System.Collections.Generic;
using VelocityDb;

namespace UnitOfWork.NET.VelocityDB.NUnit.Data.Models
{
    public class User : OptimizedPersistable
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public List<Role> Roles { get; set; }
    }
}
