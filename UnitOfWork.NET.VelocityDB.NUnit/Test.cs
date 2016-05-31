using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnitOfWork.NET.LiteDB.NUnit.DTO;
using UnitOfWork.NET.VelocityDB.Classes;
using UnitOfWork.NET.VelocityDB.NUnit.Classes;
using UnitOfWork.NET.VelocityDB.NUnit.Data.Models;
using UnitOfWork.NET.VelocityDB.NUnit.Repositories;

namespace UnitOfWork.NET.VelocityDB.NUnit
{
    [TestFixture]
    public class Test
    {
        [Test]
        public void TestVelocityDatabase()
        {
            using (var db = new TestVelocityDatabase())
            {
                var adminRole = new Role { Description = "Admin" };
                var userRole = new Role { Description = "User" };
                var testUser = new User
                {
                    Login = "test",
                    Password = "test",
                    Roles = new List<Role> { adminRole }
                };

                db.BeginUpdate();
                db.Persist(adminRole);
                db.Persist(userRole);
                db.Persist(testUser);
                db.Commit();

                db.BeginRead();
                foreach (var role in db.Roles.Value)
                {
                    Console.WriteLine($"[{role.Id}] {role.Description}");
                }

                foreach (var user in db.Users.Value)
                {
                    Console.WriteLine($"[{user.Id}] {user.Login}:{user.Password}");
                }
                db.Commit();

                db.BeginUpdate();
                foreach (var user in db.Users.Value)
                    db.DeleteObject(user.Id);

                foreach (var role in db.Roles.Value)
                    db.DeleteObject(role.Id);
                db.Commit();
            }
        }

        [Test]
        public void TestEntityRepository()
        {
            var stopwatch = Stopwatch.StartNew();

            using (var uow = new VelocityUnitOfWork<TestVelocityDatabase>())
            {
                var entity = new User
                {
                    Login = "test",
                    Password = "test"
                };

                uow.Repository<User>().Insert(entity);
                uow.SaveChanges();
                Assert.AreNotEqual(entity.Id, 0);
                Console.WriteLine(entity.Id);

                var users = uow.Repository<User>().All();
                stopwatch.Stop();
                Console.WriteLine(stopwatch.ElapsedMilliseconds);

                foreach (var user in users)
                    Console.WriteLine($"{user.Id} {user.Login}");

                entity.Password = "test2";
                uow.Repository<User>().Update(entity);
                uow.SaveChanges();
                Assert.AreEqual(entity.Password, uow.Repository<User>().Entity(entity.Id).Password);
                uow.Repository<User>().Delete(entity.Id);
                uow.SaveChanges();
                Assert.IsNull(uow.Repository<User>().Entity(entity.Id));
            }
        }

        [Test]
        public void TestDTORepository()
        {
            var stopwatch = Stopwatch.StartNew();

            using (var uow = new VelocityUnitOfWork<TestVelocityDatabase>())
            {
                var users = uow.Repository<User, UserDTO>().List();
                stopwatch.Stop();
                Console.WriteLine(stopwatch.ElapsedMilliseconds);

                var dto = new UserDTO
                {
                    Login = "test",
                    Password = "test"
                };

                uow.Repository<User, UserDTO>().Insert(dto);
                uow.SaveChanges();
                var login = dto.Login;
                dto = uow.Repository<User, UserDTO>().DTO(d => d.Login == login);
                Assert.IsNotNull(dto);
                Assert.AreNotEqual(dto.Id, 0);
                Console.WriteLine(dto.Id);

                foreach (var user in users)
                    Console.WriteLine($"{user.Id} {user.Login}");

                dto.Password = "test2";
                uow.Repository<User, UserDTO>().Update(dto, dto.Id);
                uow.SaveChanges();
                Assert.AreEqual(dto.Password, uow.Repository<User, UserDTO>().DTO(dto.Id).Password);
                uow.Repository<User, UserDTO>().Delete(dto.Id);
                uow.SaveChanges();
                Assert.IsNull(uow.Repository<User, UserDTO>().DTO(dto.Id));
            }
        }

        [Test]
        public void TestCustomRepository()
        {
            var stopwatch = Stopwatch.StartNew();

            using (var uow = new VelocityUnitOfWork<TestVelocityDatabase>())
            {
                var dto = new UserDTO
                {
                    Login = "test",
                    Password = "test"
                };

                uow.CustomRepository<UserRepository>().Insert(dto);
                uow.SaveChanges();
                var login = dto.Login;
                dto = uow.CustomRepository<UserRepository>().DTO(d => d.Login == login);
                Assert.IsNotNull(dto);
                Assert.AreNotEqual(dto.Id, 0);
                Console.WriteLine(dto.Id);

                var users = uow.CustomRepository<UserRepository>().NewList();
                stopwatch.Stop();
                Console.WriteLine(stopwatch.ElapsedMilliseconds);

                foreach (var user in users)
                    Console.WriteLine($"{user.Id} {user.Login}");

                dto.Password = "test2";
                uow.CustomRepository<UserRepository>().Update(dto, dto.Id);
                uow.SaveChanges();

                Assert.AreEqual(dto.Password, uow.CustomRepository<UserRepository>().DTO(dto.Id).Password);
                uow.CustomRepository<UserRepository>().Delete(dto.Id);
                uow.SaveChanges();
                Assert.IsNull(uow.CustomRepository<UserRepository>().DTO(dto.Id));
            }
        }

        [Test]
        public void TestCustomUnitOfWork()
        {
            var stopwatch = Stopwatch.StartNew();

            using (var uow = new TestUnitOfWork())
            {
                var dto = new UserDTO
                {
                    Login = "test",
                    Password = "test"
                };
                uow.Users.Insert(dto);
                uow.SaveChanges();
                var login = dto.Login;
                dto = uow.Users.DTO(d => d.Login == login);
                Assert.IsNotNull(dto);
                Assert.AreNotEqual(dto.Id, 0);
                Console.WriteLine(dto.Id);
                var users = uow.Users.NewList();
                stopwatch.Stop();
                Console.WriteLine(stopwatch.ElapsedMilliseconds);
                foreach (var user in users)
                    Console.WriteLine($"{user.Id} {user.Login}");
                dto.Password = "test2";
                uow.Users.Update(dto, dto.Id);
                uow.SaveChanges();
                Assert.AreEqual(dto.Password, uow.Users.DTO(dto.Id).Password);
                uow.Users.Delete(dto.Id);
                uow.SaveChanges();
                Assert.IsNull(uow.Users.DTO(dto.Id));
            }
        }

        [Test]
        public void TestReflection()
        {
            using (var uow = new TestUnitOfWork())
            {
                Assert.IsNotNull(uow.Repository<User, UserDTO>(), "uow.Users != null by reflection");
                Assert.IsNotNull(uow.Repository<Role, RoleDTO>(), "uow.Roles != null by reflection");

                Assert.IsNotNull(uow.Users, "uow.Users != null");
                Assert.IsNotNull(uow.Roles, "uow.Roles != null");
            }
        }
    }
}

