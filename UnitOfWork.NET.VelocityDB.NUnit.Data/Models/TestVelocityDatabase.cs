using System;
using VelocityDb;
using VelocityDb.Session;

namespace UnitOfWork.NET.VelocityDB.NUnit.Data.Models
{
    public class TestVelocityDatabase : SessionNoServerShared
    {
        private static int Random
        {
            get
            {
                var rand = new Random((int)DateTime.Now.Ticks);
                return rand.Next();
            }
        }

        public TestVelocityDatabase() : base("dbtest_" + Random)
        {
            Roles = new Lazy<AllObjects<Role>>(() => AllObjects<Role>());
            Users = new Lazy<AllObjects<User>>(() => AllObjects<User>());
        }

        public Lazy<AllObjects<Role>> Roles { get; set; }
        public Lazy<AllObjects<User>> Users { get; set; }
    }
}
