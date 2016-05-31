using System.Collections.Generic;
using VelocityDb;

namespace UnitOfWork.NET.VelocityDB.NUnit.Data.Models
{
    public class Role : OptimizedPersistable
    {
        public string Description { get; set; }
        public List<User> Users { get; set; }
    }
}
