using System;
using System.Collections.Generic;
using UnitOfWork.NET.Interfaces;
using UnitOfWork.NET.LiteDB.NUnit.DTO;
using UnitOfWork.NET.VelocityDB.Classes;
using UnitOfWork.NET.VelocityDB.NUnit.Data.Models;

namespace UnitOfWork.NET.VelocityDB.NUnit.Repositories
{
    public class UserRepository : VelocityRepository<User, UserDTO>
    {
        public UserRepository(IUnitOfWork manager) : base(manager)
        {
        }

        public IEnumerable<UserDTO> NewList()
        {
            Console.WriteLine("NewList");

            return List();
        }
    }
}
