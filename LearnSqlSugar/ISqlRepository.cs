using System.Linq.Expressions;

namespace LearnSqlSugar
{
    public interface ISqlRepository<T> where T : class, new()
    {
        #region Query查询
        /// <summary>
        /// 查询所有数据
        /// </summary>
        /// <returns></returns>
        public Task<List<T>> GetAllAsync();


        /// <summary>
        /// lambda 查询一条
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public Task<T> GetSingleByLambdaAsync(Expression<Func<T, bool>> expression);


        /// <summary>
        /// lambda 查询所有
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public Task<List<T>> GetAllByLambdaAsync(Expression<Func<T, bool>> expression);


        /// <summary>
        /// 根据主键Id查询
        /// </summary>
        /// <typeparam name="Tkey"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<T> GetByIdAsync<Tkey>(Tkey id);

        /// <summary>
        /// 查询第一个 注: 未查询到，为null
        /// </summary>
        /// <returns></returns>
        public Task<T> GetFirstOrDefaultAsync();

        /// <summary>
        /// 查询 实体在数据库的数量
        /// </summary>
        /// <returns></returns>
        public Task<int> GetCountAsync();

        public Task<List<T>> GetByNavigate();


        public Task<List<T>> GetByNavigatAndLambda(Expression<Func<T, bool>> expression);


        public Task<List<T>> GetByNavigatFilterAndLambda(Expression<Func<T, bool>> expression, params string[] NavigatFilters);

        #endregion

        #region Insert插入

        public Task<bool> InsertAsync(T t);


        public Task<bool> InsertRangeAsync(List<T> list);


        public Task<int> InsertReturnIdentityAsync(T t);


        public Task<bool> InsertByNavigate(List<T> list);

        public Task<bool> InsertByNavigate(T t);

        public Task<bool> InsertByNavigate(List<T> list, params string[] IgnoreNavigates);

        public Task<bool> InsertByNavigate(T t, params string[] IgnoreNavigates);
        #endregion

        #region Update更新
        /// <summary>
        /// 更新单条
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Task<bool> UpdateAsync(T entity);


        /// <summary>
        /// 更新多条
        /// </summary>
        /// <param name="entitys"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Task<bool> UpdateRangeAsync(List<T> entitys);


        #endregion

        #region Delete删除
        /// <summary>
        /// 按照主键删除
        /// </summary>
        /// <typeparam name="Tkey"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Task<bool> DeleteByIdAsync<Tkey>(Tkey id);


        /// <summary>
        /// 根据主键数组删除
        /// </summary>
        /// <typeparam name="Tkey"></typeparam>
        /// <param name="ids"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Task<bool> DeleteByIdsAsync<Tkey>(List<Tkey> ids);


        /// <summary>
        /// 删除单个 (没有检查表中是否存在)
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public Task<bool> DeleteAsync(T t);


        /// <summary>
        /// 删除多个
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public Task<bool> DeleteRangeAsync(List<T> list);


        /// <summary>
        /// 按照lambda表达式删除
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public Task<bool> DeleteByLambdaAsync(Expression<Func<T, bool>> expression);

        public Task<bool> DeleteByNavigate(List<T> list);

        public Task<bool> DeleteByNavigate(T t);

        public Task<bool> DeleteByNavigate(List<T> list, params string[] IgnoreNavigates);

        public Task<bool> DeleteByNavigate(T t, params string[] IgnoreNavigates);

        /// <summary>
        /// 清空表
        /// </summary>
        /// <returns></returns>
        public bool ClearTable();

        #endregion
    }
}
