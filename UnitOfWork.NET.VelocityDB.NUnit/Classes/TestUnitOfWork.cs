using UnitOfWork.NET.VelocityDB.Classes;
using UnitOfWork.NET.VelocityDB.NUnit.Data.Models;
using UnitOfWork.NET.VelocityDB.NUnit.Repositories;

namespace UnitOfWork.NET.VelocityDB.NUnit.Classes
{
	public class TestUnitOfWork : VelocityUnitOfWork<TestVelocityDatabase>
	{
		public UserRepository Users { get; set; }
		public RoleRepository Roles { get; set; }
	}
}
