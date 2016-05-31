using System;
using UnitOfWork.NET.Interfaces;
using VelocityDb;
using VelocityDb.Session;

namespace UnitOfWork.NET.VelocityDB.Interfaces
{
    public interface IVelocityUnitOfWork : IUnitOfWork
    {
        void BeforeSaveChanges(SessionBase session);

        /// <summary>
        /// 
        /// </summary>
        void SaveChanges();

        /// <summary>
        /// 
        /// </summary>
        void AfterSaveChanges(SessionBase session);

        TEntity Insert<TEntity>(TEntity entity) where TEntity : OptimizedPersistable, new();

        void Update<TEntity>(TEntity entity) where TEntity : OptimizedPersistable, new();

        void Delete<TEntity>(ulong id) where TEntity : OptimizedPersistable, new();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        //DbEntityEntry Entry<TEntity>(TEntity entity) where TEntity : class, new();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="body"></param>
        void Transaction(Action<IVelocityUnitOfWork> body);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="body"></param>
        bool TransactionSaveChanges(Action<IVelocityUnitOfWork> body);

        /// <summary>
        /// 
        /// </summary>
        new IVelocityRepository<TEntity> Repository<TEntity>() where TEntity : OptimizedPersistable, new();

        /// <summary>
        /// 
        /// </summary>
        new IVelocityRepository<TEntity, TDTO> Repository<TEntity, TDTO>() where TEntity : OptimizedPersistable, new() where TDTO : class, new();
    }
}
