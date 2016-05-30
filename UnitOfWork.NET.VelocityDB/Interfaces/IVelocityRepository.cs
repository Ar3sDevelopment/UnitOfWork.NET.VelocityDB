using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Caelan.DynamicLinq.Classes;
using UnitOfWork.NET.Interfaces;
using VelocityDb;

namespace UnitOfWork.NET.VelocityDB.Interfaces
{
    public interface IVelocityRepository<TEntity> : IRepository<TEntity> where TEntity : OptimizedPersistable, new()
    {
        new IVelocityUnitOfWork UnitOfWork { get; }

        TEntity Entity(Func<TEntity, bool> expr);

        TEntity Entity(ulong id);

        TEntity Insert(TEntity entity);

        void Update(TEntity entity);

        void Delete(ulong id);

        //void OnSaveChanges(IDictionary<EntityState, IEnumerable<TEntity>> entities);
    }

    public interface IVelocityRepository<TEntity, TDTO> : IRepository<TEntity, TDTO>, IVelocityRepository<TEntity> where TEntity : OptimizedPersistable, new() where TDTO : class, new()
    {
        TDTO DTO(ulong id);

        TDTO DTO(Func<TEntity, bool> expr);

        IEnumerable<TDTO> List();

        IEnumerable<TDTO> List(Func<TEntity, bool> expr);

        TDTO Insert(TDTO dto);

        void Update(TDTO dto, ulong id);
    }
}
