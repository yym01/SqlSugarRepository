using SqlSugar;
using DryIoc;
using System.Linq.Expressions;
using LearnSqlSugar.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearnSqlSugar
{
    public class SqlRepository<T> : ISqlRepository<T> where T : class, new()
    {
        private ILogger logger = null;

        private SqlSugarClient _dbContext;

        public SqlRepository(IContainer container)
        {
            if(container is null || !container.IsRegistered<ILogger>())
            {
                throw new ArgumentException("IContainer - container is null or container.IsRegistered<ILogger>() is false!");
            }
            logger = container.Resolve<ILogger>();
            var conn = Appsettings.app(new string[] { "LearnSqlSugar", "DbContexts", "ConnectionString" });
            var dbtype = Appsettings.app(new string[] { "LearnSqlSugar", "DbContexts", "DataBaseType" });
            var IsEnableLoger = 
               "True".Equals(Appsettings.app(new string[] { "LearnSqlSugar", "DbContexts", "IsEnableLoger" })
               ,StringComparison.CurrentCultureIgnoreCase) ? true : false;
            if(IsEnableLoger)
            {
                _dbContext = SqlSugarDbFactory.GetSqlSugarClient(conn,dbtype,logger);
            }
            else
            {
                _dbContext = SqlSugarDbFactory.GetSqlSugarClient(conn, dbtype);
            }
        }

        public SqlRepository()
        {
            var conn = Appsettings.app(new string[] { "LearnSqlSugar", "DbContexts", "ConnectionString" });
            var dbtype = Appsettings.app(new string[] { "LearnSqlSugar", "DbContexts", "DataBaseType" });
            _dbContext = SqlSugarDbFactory.GetSqlSugarClient(conn, dbtype);
        }

        /// <summary>
        /// 根据 assemblyName 查询程序集中所有存在SugarTable特性的实体类，并生成表
        /// </summary>
        /// <param name="assemblyName"></param>
        public void CreateTableByAssembly(string assemblyName)
        {
            try
            {
                var types = LearnSqlSugar.Extensions.ReflectionExtensions.GetTypesWithAttribute<SugarTable>(assemblyName);
                foreach (var type in types)
                {
                    _dbContext.CodeFirst.InitTables(type);
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        #region Query查询
        /// <summary>
        /// 查询所有数据
        /// </summary>
        /// <returns></returns>
        public async Task<List<T>> GetAllAsync()
        {
            return await _dbContext.Queryable<T>().ToListAsync();
        }

        /// <summary>
        /// lambda 查询一条
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public async Task<T> GetSingleByLambdaAsync(Expression<Func<T, bool>> expression)
        {
            return await _dbContext.Queryable<T>().Where(expression).SingleAsync();
        }

        /// <summary>
        /// lambda 查询所有
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public async Task<List<T>> GetAllByLambdaAsync(Expression<Func<T, bool>> expression)
        {
            return await _dbContext.Queryable<T>().Where(expression).ToListAsync();
        }

        /// <summary>
        /// 根据主键Id查询
        /// </summary>
        /// <typeparam name="Tkey"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<T> GetByIdAsync<Tkey>(Tkey id)
        {
            return await _dbContext.Queryable<T>().InSingleAsync(id);
        }

        /// <summary>
        /// 查询第一个 注: 未查询到，为null
        /// </summary>
        /// <returns></returns>
        public async Task<T> GetFirstOrDefaultAsync()
        {
            return await _dbContext.Queryable<T>().FirstAsync();
        }

        /// <summary>
        /// 查询 实体在数据库的数量
        /// </summary>
        /// <returns></returns>
        public async Task<int> GetCountAsync()
        {
            return await _dbContext.Queryable<T>().CountAsync();
        }

        public async Task<List<T>> GetByNavigate()
        {
            return await _dbContext.Queryable<T>().IncludesAllFirstLayer().ToListAsync();
        }

        public async Task<List<T>> GetByNavigatAndLambda(Expression<Func<T, bool>> expression)
        {
            return await _dbContext.Queryable<T>().Where(expression).IncludesAllFirstLayer().ToListAsync();
        }

        public async Task<List<T>> GetByNavigatFilterAndLambda(Expression<Func<T, bool>> expression, params string[] NavigatFilters)
        {
            return await _dbContext.Queryable<T>().IncludesAllFirstLayer(NavigatFilters).ToListAsync();
        }
        #endregion

        #region Insert插入

        public async Task<bool> InsertAsync(T t)
        {
            if(t == null) throw new ArgumentNullException(nameof(t));
            return await _dbContext.Insertable(t).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> InsertRangeAsync(List<T> list)
        {
            foreach(T item in list)
            {
                if (item == null) throw new ArgumentNullException(nameof (item));
            }
            return await _dbContext.Insertable(list).ExecuteCommandAsync() > 0;
        }

        public async Task<int> InsertReturnIdentityAsync(T t)
        {
            if(t == null) throw new ArgumentNullException (nameof (t));
            return await _dbContext.Insertable(t).ExecuteReturnIdentityAsync();
        }

        /// <summary>
        /// 导航更新，包含所有的二级导航
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public async Task<bool> InsertByNavigate(List<T> list)
        {
            return await _dbContext.InsertNav(list).IncludesAllFirstLayer().ExecuteCommandAsync();
        }

        public async Task<bool> InsertByNavigate(T t)
        {
            return await _dbContext.InsertNav(t).IncludesAllFirstLayer().ExecuteCommandAsync();
        }


        /// <summary>
        /// 导航更新，并排除更新名为IgnoreNavigate的导航
        /// </summary>
        /// <param name="list"></param>
        /// <param name="IgnoreNavigates"></param>
        /// <returns></returns>
        public async Task<bool> InsertByNavigate(List<T> list,params string[] IgnoreNavigates)
        {
            return await _dbContext.InsertNav(list).IncludesAllFirstLayer(IgnoreNavigates).ExecuteCommandAsync();
        }

        public async Task<bool> InsertByNavigate(T t, params string[] IgnoreNavigates)
        {
            return await _dbContext.InsertNav(t).IncludesAllFirstLayer(IgnoreNavigates).ExecuteCommandAsync();
        }


        #endregion

        #region Update更新
        /// <summary>
        /// 更新单条
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<bool> UpdateAsync(T entity)
        {
            if(entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            return await _dbContext.Updateable<T>(entity).ExecuteCommandAsync() > 0;
        }

        /// <summary>
        /// 更新多条
        /// </summary>
        /// <param name="entitys"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<bool> UpdateRangeAsync(List<T> entitys)
        {
            foreach (var entity in entitys)
            {
                if (entity == null)
                {
                    throw new ArgumentNullException(nameof(entity));
                }
            }
            return await _dbContext.Updateable<T>(entitys).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> UpdateByNavigate(T t)
        {
            return await _dbContext.UpdateNav(t).IncludesAllFirstLayer().ExecuteCommandAsync();
        }

        public async Task<bool> UpdateByNavigate(List<T> list)
        {
            return await _dbContext.UpdateNav(list).IncludesAllFirstLayer().ExecuteCommandAsync();
        }

        public async Task<bool> UpdateByNavigate(T t,params string[] IgnoreNavigates)
        {
            return await _dbContext.UpdateNav(t).IncludesAllFirstLayer(IgnoreNavigates).ExecuteCommandAsync();
        }

        public async Task<bool> UpdateByNavigate(List<T> list, params string[] IgnoreNavigates)
        {
            return await _dbContext.UpdateNav(list).IncludesAllFirstLayer(IgnoreNavigates).ExecuteCommandAsync();
        }

        #endregion

        #region Delete删除
        /// <summary>
        /// 按照主键删除
        /// </summary>
        /// <typeparam name="Tkey"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<bool> DeleteByIdAsync<Tkey>(Tkey id)
        {
            if(id == null)
                throw new ArgumentNullException(nameof(id));
            //var queryRes = await GetByIdAsync(id);
            //if(queryRes == null)
            //    return true;
            var res = await _dbContext.Deleteable<T>().In(id).ExecuteCommandAsync() > 0;
            return res;
        }

        /// <summary>
        /// 根据主键数组删除
        /// </summary>
        /// <typeparam name="Tkey"></typeparam>
        /// <param name="ids"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<bool> DeleteByIdsAsync<Tkey>(List<Tkey> ids)
        {
            foreach(var id in ids)
            {
                if(id == null)
                    throw new ArgumentNullException(nameof(id));
            }
            var res = await _dbContext.Deleteable<T>().In(ids).ExecuteCommandAsync() > 0;
            return res;
        }

        /// <summary>
        /// 删除单个 (没有检查表中是否存在)
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public async Task<bool> DeleteAsync(T t)
        {
            return await _dbContext.Deleteable<T>(t).ExecuteCommandAsync() > 0;
        }

        /// <summary>
        /// 删除多个
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public async Task<bool> DeleteRangeAsync(List<T> list)
        {
            return await _dbContext.Deleteable<T>(list).ExecuteCommandAsync() > 0;
        }

        /// <summary>
        /// 按照lambda表达式删除
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public async Task<bool> DeleteByLambdaAsync(Expression<Func<T, bool>> expression)
        {
            return await _dbContext.Deleteable<T>().Where(expression).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> DeleteByNavigate(List<T> list)
        {
            return await _dbContext.DeleteNav(list).IncludesAllFirstLayer().ExecuteCommandAsync();
        }

        public async Task<bool> DeleteByNavigate(T t)
        {
            return await _dbContext.DeleteNav(t).IncludesAllFirstLayer().ExecuteCommandAsync();
        }

        public async Task<bool> DeleteByNavigate(List<T> list, params string[] IgnoreNavigates)
        {
            return await _dbContext.DeleteNav(list).IncludesAllFirstLayer(IgnoreNavigates).ExecuteCommandAsync();
        }

        public async Task<bool> DeleteByNavigate(T t, params string[] IgnoreNavigates)
        {
            return await _dbContext.DeleteNav(t).IncludesAllFirstLayer(IgnoreNavigates).ExecuteCommandAsync();
        }


        /// <summary>
        /// 清空表
        /// </summary>
        /// <returns></returns>
        public bool ClearTable()
        {
            return _dbContext.DbMaintenance.TruncateTable<T>();
        }

        #endregion

    }
}
