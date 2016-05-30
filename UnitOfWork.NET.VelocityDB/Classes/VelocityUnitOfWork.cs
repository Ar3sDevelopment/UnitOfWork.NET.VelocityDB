using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using UnitOfWork.NET.Interfaces;
using UnitOfWork.NET.VelocityDB.Extenders;
using UnitOfWork.NET.VelocityDB.Interfaces;
using VelocityDb;
using VelocityDb.Session;

namespace UnitOfWork.NET.VelocityDB.Classes
{
    public class VelocityUnitOfWork : NET.Classes.UnitOfWork, IVelocityUnitOfWork
    {
        private readonly bool _autoContext;
        private readonly SessionBase _dbContext;

        public VelocityUnitOfWork(SessionBase context) : this(context, false)
        {
        }

        internal VelocityUnitOfWork(SessionBase context, bool managedContext)
        {
            _dbContext = context;
            _autoContext = managedContext;

            var cb = new ContainerBuilder();

            cb.RegisterGeneric(typeof(VelocityRepository<>)).AsSelf().As(typeof(IVelocityRepository<>)).As(typeof(IRepository<>));
            cb.RegisterGeneric(typeof(VelocityRepository<,>)).AsSelf().As(typeof(IVelocityRepository<,>)).As(typeof(IRepository<,>));

            UpdateContainer(cb);
            UpdateProperties();
        }

        public override void Dispose()
        {
            if (_autoContext)
                _dbContext.Dispose();

            base.Dispose();
        }

        public override IEnumerable<T> Data<T>() => _dbContext.AllObjects<T>().AsEnumerable();

        public virtual void BeforeSaveChanges(SessionBase context)
        {
        }

        public void SaveChanges()
        {
            //_dbContext.ChangeTracker.DetectChanges();

            //var entries = _dbContext.ChangeTracker.Entries();
            //var entriesGroup = entries.Where(t => t.State != EntityState.Unchanged && t.State != EntityState.Detached).ToList().GroupBy(t => ObjectContext.GetObjectType(t.Entity.GetType())).ToList().Select(t => new { t.Key, EntriesByState = t.GroupBy(g => g.State, g => g.Entity).ToList() }).ToList();

            BeforeSaveChanges(_dbContext);

            //var res = _dbContext.SaveChanges();

            _dbContext.Commit();

            //foreach (var item in entriesGroup)
            //{
            //	var entityType = item.Key;
            //	var entitiesByState = item.EntriesByState.ToDictionary(t => t.Key, t => t.AsEnumerable());
            //	var mHelper = typeof(VelocityUnitOfWork).GetMethod("CallOnSaveChanges", BindingFlags.NonPublic | BindingFlags.Instance);
            //	mHelper.MakeGenericMethod(entityType).Invoke(this, new object[] { entitiesByState });
            //}

            AfterSaveChanges(_dbContext);
        }

        public TEntity Insert<TEntity>(TEntity entity) where TEntity : OptimizedPersistable, new()
        {
            _dbContext.BeginUpdate();
            _dbContext.Persist(entity);

            return entity;
        }

        public void Update<TEntity>(TEntity entity) where TEntity : OptimizedPersistable, new()
        {
            _dbContext.BeginUpdate();
            _dbContext.UpdateObject(entity);
        }

        public void Delete<TEntity>(ulong id) where TEntity : OptimizedPersistable, new()
        {
            _dbContext.BeginUpdate();
            _dbContext.DeleteObject(id);
        }

        public virtual void AfterSaveChanges(SessionBase context)
        {
        }

        public void Transaction(Action<IVelocityUnitOfWork> body)
        {
            using (var transaction = _dbContext.BeginTransaction())
            {
                try
                {
                    body.Invoke(this);
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                }
            }
        }

        protected override void RegisterRepository(ContainerBuilder cb, Type repositoryType)
        {
            base.RegisterRepository(cb, repositoryType);

            if (repositoryType.IsGenericTypeDefinition)
                cb.RegisterGeneric(repositoryType).AsSelf().AsEntityRepository().AsImplementedInterfaces();
            else
                cb.RegisterType(repositoryType).AsSelf().AsEntityRepository().AsImplementedInterfaces();
        }

        public bool TransactionSaveChanges(Action<IVelocityUnitOfWork> body)
        {
            using (var transaction = _dbContext.BeginTransaction())
            {
                try
                {
                    body.Invoke(this);
                    SaveChanges();
                    transaction.Commit();
                    return true;
                }
                catch
                {
                    transaction.Rollback();
                    return false;
                }
            }
        }

        //public DbEntityEntry Entry<TEntity>(TEntity entity) where TEntity : class, new() => _dbContext.Entry(entity);

        //public DbSet<TEntity> Set<TEntity>() where TEntity : class, new() => _dbContext.Set<TEntity>();

        public new IVelocityRepository<TEntity> IVelocityUnitOfWork.Repository<TEntity>() where TEntity : OptimizedPersistable, new() => base.Repository<TEntity>() as IVelocityRepository<TEntity>;
        public new IVelocityRepository<TEntity, TDTO> IVelocityUnitOfWork.Repository<TEntity, TDTO>() where TEntity : OptimizedPersistable, new() where TDTO : class, new() => base.Repository<TEntity, TDTO>() as IVelocityRepository<TEntity, TDTO>;

        //private void CallOnSaveChanges<TEntity>(Dictionary<EntityState, IEnumerable<object>> entitiesObj) where TEntity : class, new()
        //{
        //    var entities = entitiesObj.ToDictionary(t => t.Key, t => t.Value.Cast<TEntity>());

        //    Repository<TEntity>().OnSaveChanges(entities);
        //}

        public async Task SaveChangesAsync() => await new TaskFactory().StartNew(SaveChanges);
    }

    public class EntityUnitOfWork<TContext> : VelocityUnitOfWork where TContext : SessionBase, new()
    {
        public EntityUnitOfWork() : base(new TContext(), true)
        {
        }

        public override void BeforeSaveChanges(SessionBase context)
        {
            BeforeSaveChanges((TContext)context);
        }

        public override void AfterSaveChanges(SessionBase context)
        {
            AfterSaveChanges((TContext)context);
        }

        public virtual void BeforeSaveChanges(TContext context)
        {
            base.BeforeSaveChanges(context);
        }

        public virtual void AfterSaveChanges(TContext context)
        {
            base.AfterSaveChanges(context);
        }
    }
}
