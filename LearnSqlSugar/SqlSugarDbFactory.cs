using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;

namespace LearnSqlSugar
{
    public class SqlSugarDbFactory
    {
        public static SqlSugarClient GetSqlSugarClient(string conn, string type) =>
            GetSqlSugarClient(conn, type, null);

        public static SqlSugarClient GetSqlSugarClient(string conn, string type, ILogger logger)
        {
            if ("MySql".Equals(type, StringComparison.CurrentCultureIgnoreCase))
            {
                return GetDb(conn, DbType.MySql, logger);
            }
            if ("Sqlite".Equals(type, StringComparison.CurrentCultureIgnoreCase))
            {
                return GetDb(conn, DbType.Sqlite, logger);
            }
            if ("SqlServer".Equals(type, StringComparison.CurrentCultureIgnoreCase))
            {
                return GetDb(conn, DbType.Sqlite, logger);
            }
            throw new ArgumentException(
                string.Format(
                    "SqlSugarDbFactory - GetSqlSugarClient 无效的参数: {0} 参数名称: {1}",
                    type,
                    nameof(type)
                )
            );
        }

        private static SqlSugarClient GetDb(string conn, DbType dbType, ILogger logger)
        {
            SqlSugarClient db = new SqlSugarClient(
                new ConnectionConfig()
                {
                    ConnectionString = conn, //连接符字串
                    DbType = dbType, // DbType.SqlServer,
                    IsAutoCloseConnection = true, //自动关闭连接
                    InitKeyType = InitKeyType.Attribute, //从特性读取主键自增信息
                }
            );
            if (logger != null)
            {
                //db.Aop.OnLogExecuting = (sql, pars) =>
                //{
                //    //logger.LogInformation($"{ DateTime.Now.ToString()}  SQL执行前: \r\n{sql}");
                //};
                //sql执行完成事件
                db.Aop.OnLogExecuted = (sql, pars) =>
                {
                    logger.Info($"{DateTime.Now.ToString()}  SQL执行成功: \r\n{sql}");
                };
                //sql报错事件
                db.Aop.OnError = (exp) => //SQL报错
                {
                    logger.Error($"{DateTime.Now.ToString()}  SQL报错: \r\n{exp.Message}");
                };
            }
            return db;
        }
    }
}
