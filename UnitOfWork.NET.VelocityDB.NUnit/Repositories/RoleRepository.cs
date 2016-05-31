using UnitOfWork.NET.Interfaces;
using UnitOfWork.NET.LiteDB.NUnit.DTO;
using UnitOfWork.NET.VelocityDB.Classes;
using UnitOfWork.NET.VelocityDB.NUnit.Data.Models;

namespace UnitOfWork.NET.VelocityDB.NUnit.Repositories
{
    public class RoleRepository : VelocityRepository<Role, RoleDTO>
    {
        public RoleRepository(IUnitOfWork manager) : base(manager)
        {
        }
    }
}
