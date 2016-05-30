using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Caelan.DynamicLinq.Classes;
using Caelan.DynamicLinq.Extensions;
using ClassBuilder.Classes;
using ClassBuilder.Interfaces;
using UnitOfWork.NET.Classes;
using UnitOfWork.NET.Interfaces;
using UnitOfWork.NET.VelocityDB.Interfaces;
using VelocityDb;

namespace UnitOfWork.NET.VelocityDB.Classes
{
    public class VelocityRepository<TEntity> : Repository<TEntity>, IVelocityRepository<TEntity> where TEntity : OptimizedPersistable, new()
    {
        public new IVelocityUnitOfWork UnitOfWork => base.UnitOfWork as IVelocityUnitOfWork;

        public VelocityRepository(IUnitOfWork manager) : base(manager)
        {
        }

        public TEntity Entity(Func<TEntity, bool> expr) => Element(expr);
        public TEntity Entity(ulong id) => Data.FirstOrDefault(t => t.Id == id);

        public virtual TEntity Insert(TEntity entity) => UnitOfWork.Insert(entity);

        public virtual void Update(TEntity entity) => UnitOfWork.Update(entity);

        public virtual void Delete(ulong id) => UnitOfWork.Delete<TEntity>(id);

        //public virtual void OnSaveChanges(IDictionary<EntityState, IEnumerable<TEntity>> entities)
        //{
        //}

        public async Task<TEntity> InsertAsync(TEntity entity) => await new TaskFactory().StartNew(() => Insert(entity));
        public async Task UpdateAsync(TEntity entity) => await new TaskFactory().StartNew(() => Update(entity));
        public async Task DeleteAsync(ulong id) => await new TaskFactory().StartNew(() => Delete(id));
        public async Task<TEntity> EntityAsync(ulong id) => await new TaskFactory().StartNew(() => Entity(id));
        public async Task<TEntity> EntityAsync(Func<TEntity, bool> expr) => await new TaskFactory().StartNew(() => Entity(expr));
    }

    public class VelocityRepository<TEntity, TDTO> : VelocityRepository<TEntity>, IVelocityRepository<TEntity, TDTO> where TEntity : OptimizedPersistable, new() where TDTO : class, new()
    {
        public IMapper<TEntity, TDTO> DTOMapper { get; set; }
        public IMapper<TDTO, TEntity> EntityMapper { get; set; }

        public VelocityRepository(IUnitOfWork manager) : base(manager)
        {
        }

        public TDTO ElementBuilt(Func<TEntity, bool> expr) => DTO(expr);

        public IEnumerable<TDTO> AllBuilt() => List();

        public IEnumerable<TDTO> AllBuilt(Func<TEntity, bool> expr) => List(expr);

        public DataSourceResult<TDTO> DataSource(int take, int skip, ICollection<Sort> sort, Filter filter, Func<TEntity, bool> expr) => DataSource(take, skip, sort, filter, expr, t => Builder.BuildList(t).ToList<TDTO>());

        public TDTO DTO(ulong id)
        {
            var entity = Entity(id);

            return entity != null ? Builder.Build(entity).To<TDTO>() : null;
        }

        public TDTO DTO(Func<TEntity, bool> expr)
        {
            var entity = Entity(expr);

            return entity != null ? Builder.Build(entity).To<TDTO>() : null;
        }

        public IEnumerable<TDTO> List() => Builder.BuildList(All()).ToList<TDTO>();

        public IEnumerable<TDTO> List(Func<TEntity, bool> expr) => Builder.BuildList(All(expr)).ToList<TDTO>();

        private DataSourceResult<TDTO> DataSource(int take, int skip, ICollection<Sort> sort, Filter filter, Func<TEntity, bool> expr, Func<IEnumerable<TEntity>, IEnumerable<TDTO>> buildFunc)
        {
            var orderBy = typeof(TEntity).GetProperties(BindingFlags.Instance | BindingFlags.Public).Select(t => t.Name).FirstOrDefault();

            var res = All(expr);

            if (orderBy != null)
                res = res.OrderBy(orderBy);

            var ds = res.AsQueryable().ToDataSourceResult(take, skip, sort, filter);


            return new DataSourceResult<TDTO>
            {
                Data = buildFunc(ds.Data).ToList(),
                Total = ds.Total
            };
        }

        public TDTO Insert(TDTO dto) => Builder.Build(Insert(Builder.Build(dto).To<TEntity>())).To<TDTO>();

        public void Update(TDTO dto, ulong id)
        {
            var entity = Entity(id);
            Builder.Build(dto).To(entity);
            Update(entity);
        }

        public async Task<TDTO> InsertAsync(TDTO dto) => await new TaskFactory().StartNew(() => Insert(dto));
        public async Task UpdateAsync(TDTO dto, params object[] ids) => await new TaskFactory().StartNew(() => Update(dto, ids));
        public async Task<IEnumerable<TDTO>> ListAsync(Func<TEntity, bool> expr) => await new TaskFactory().StartNew(() => List(expr));
        public async Task<IEnumerable<TDTO>> ListAsync() => await new TaskFactory().StartNew(List);
        public async Task<DataSourceResult<TDTO>> DataSourceAsync(int take, int skip, ICollection<Sort> sort, Filter filter, Func<TEntity, bool> expr) => await new TaskFactory().StartNew(() => DataSource(take, skip, sort, filter, expr));
        public async Task<TDTO> DTOAsync(Func<TEntity, bool> expr) => await new TaskFactory().StartNew(() => DTO(expr));
        public async Task<TDTO> DTOAsync(params object[] ids) => await new TaskFactory().StartNew(() => DTO(ids));
    }
}
