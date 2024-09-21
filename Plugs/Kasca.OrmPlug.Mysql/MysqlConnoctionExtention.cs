#region Copyright (C) 2017 Elivn 
/***************************************************************************
*　　	文件功能描述：OSSCore仓储层 ——  Connection 方法扩展类
*
*　　	创建人： Elivn
*       创建人Email：498353921@qq.com
*    	创建日期：2017-5-7
*       
*****************************************************************************/

#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Kasca.Common.ComModels;
using Kasca.Common.ComModels.Enums;
using Kasca.Common.Extention.DTO;

namespace Kasca.OrmPlug.Mysql
{
    internal static class MysqlConnoctionExtention
    {
        #region    插入扩展

        public static async Task<ResultMo> Insert<TType>(this IDbConnection con, string tableName, TType mo)
            //where TType : BaseMo
        {
            if (string.IsNullOrEmpty(tableName))
                tableName = mo.GetType().Name;

            var sql = GetInsertSql<TType>(tableName, false);

            long row = await con.ExecuteAsync(sql, mo);
       

            return row > 0 ? new ResultMo() : new ResultIdMo(ResultTypes.AddFail, "添加操作失败！");
        }

        public static Task<int> InsertList<TType>(this IDbConnection con, string tableName, IList<TType> list)
        {
            if (string.IsNullOrEmpty(tableName))
                tableName = typeof(TType).Name;

            var sql = GetInsertSql<TType>(tableName, false);

            return con.ExecuteAsync(sql, list);
        }



        private static string GetInsertSql<TType>(string tableName, bool haveAuto)
        {
            //  todo 未来针对类型，添加语句缓存
            var properties = typeof(TType).GetProperties();

            var sqlCols = new StringBuilder("INSERT INTO ");
            sqlCols.Append(tableName).Append(" (");

            var sqlValues = new StringBuilder(" VALUES (");
            var isStart = false;

            foreach (var propertyInfo in properties)
            {
                if (haveAuto)
                {
                    var isAuto = propertyInfo.GetCustomAttribute<AutoColumnAttribute>() != null;
                    if (isAuto)
                    {
                        continue;
                    }
                }
                if (isStart)
                {
                    sqlCols.Append(",");
                    sqlValues.Append(",");
                }
                else
                    isStart = true;
                sqlCols.Append("`").Append(propertyInfo.Name).Append("`");
                sqlValues.Append("@").Append(propertyInfo.Name);
            }
            sqlCols.Append(")");
            sqlValues.Append(")");
            sqlCols.Append(sqlValues);

            if (haveAuto)
                sqlCols.Append(";SELECT LAST_INSERT_ID();");
            return sqlCols.ToString();
        }
        #endregion

        internal static async Task<ResultMo> UpdatePartail<TType>(this IDbConnection con, string tableName,
            Expression<Func<TType, object>> update, Expression<Func<TType, bool>> where, object mo)
            //where TType : BaseMo
        {
            if (string.IsNullOrEmpty(tableName))
                tableName = typeof(TType).Name;

            var visitor = new SqlExpressionVisitor();

            var updateSql = GetVisitExpressSql(visitor, update, SqlVistorType.Update);
            var whereSql = GetVisitExpressSql(visitor, where, SqlVistorType.Where);
            var sql = string.Concat("UPDATE ", tableName, " SET ", updateSql, whereSql);

            var paras = GetExcuteParas(mo, visitor);
            var row = await con.ExecuteAsync(sql, paras);
            return row > 0 ? new ResultMo() : new ResultMo(ResultTypes.UpdateFail, "更新失败");
        }

        /// <summary>
        ///  获取单项扩展
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <param name="con"></param>
        /// <param name="whereExp"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static async Task<TType> Get<TType>(this IDbConnection con, string tableName, Expression<Func<TType, bool>> whereExp)
        {
            if (string.IsNullOrEmpty(tableName))
                tableName = typeof(TType).Name;

            var sqlVisitor = new SqlExpressionVisitor();
            var whereSql = GetVisitExpressSql(sqlVisitor, whereExp, SqlVistorType.Where);

            var sqlStr = string.Concat("SELECT * FROM ", tableName, whereSql);
            var paras = GetExcuteParas(null, sqlVisitor);

            return await con.QuerySingleOrDefaultAsync<TType>(sqlStr, paras);
        }
        public static async Task<IList<TType>> GetList<TType>(this IDbConnection con, string tableName, Expression<Func<TType, bool>> whereExp)
        {
            if (string.IsNullOrEmpty(tableName))
                tableName = typeof(TType).Name;

            var sqlVisitor = new SqlExpressionVisitor();
            var whereSql = GetVisitExpressSql(sqlVisitor, whereExp, SqlVistorType.Where);

            var sqlStr = string.Concat("SELECT * FROM ", tableName, whereSql);
            var paras = GetExcuteParas(null, sqlVisitor);

            var listRes = (await con.QueryAsync<TType>(sqlStr, paras)).ToList();
            return listRes.Count == 0 ? null : listRes.ToList();
        }

        /// <summary>
        ///   处理where条件表达式，如果表达式为空，默认使用Id
        /// </summary>
        /// <param name="visitor"></param>
        /// <param name="exp"></param>
        /// <param name="visType"></param>
        private static string GetVisitExpressSql(SqlExpressionVisitor visitor, Expression exp, SqlVistorType visType)
        {
            if (visType == SqlVistorType.Update)
            {
                var updateFlag = new SqlVistorFlag(SqlVistorType.Update);
                visitor.Visit(exp, updateFlag);
                return updateFlag.Sql;
            }

            var whereFlag = new SqlVistorFlag(SqlVistorType.Where);
            visitor.Visit(exp, whereFlag);
            var sql = string.Concat(" WHERE ", whereFlag.Sql);


            return sql;
        }

        private static object GetExcuteParas(object mo, SqlExpressionVisitor visitor)
        {
            if (!visitor.Parameters.Any())
                return mo;

            var paras = new DynamicParameters(visitor.Parameters);
            if (mo == null || !visitor.Properties.Any())
                return paras;

            paras.AddDynamicParams(mo);
            return paras;
        }

        /// <summary>
        ///  查询数据条数
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <param name="con"></param>
        /// <param name="whereExp"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static async Task<int> Count<TType>(this IDbConnection con, string tableName, Expression<Func<TType, bool>> whereExp)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentNullException("未发现表名，请查看基类中m_tableName是否已经赋值！");
            }

            var sqlVisitor = new SqlExpressionVisitor();
            var whereSql = GetVisitExpressSql(sqlVisitor, whereExp, SqlVistorType.Where);

            var sqlStr = string.Concat("SELECT COUNT(1) FROM ", tableName, whereSql);
            var paras = GetExcuteParas(null, sqlVisitor);

            return await con.QuerySingleAsync<int>(sqlStr, paras);
        }
    }
}

