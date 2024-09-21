﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Kasca.Common.ComModels;
using Kasca.Common.ComModels.Enums;

namespace Elivn.ORM.Pgsql.Dapper.OrmExtension
{
    internal static class ConnectionExtension 
    {
        #region    插入扩展

        public static Task<int> Insert<TType>(this IDbConnection con, string tableName, TType mo)

        {
            if (string.IsNullOrEmpty(tableName))
                tableName = mo.GetType().Name;

            var sql = GetInsertSql<TType>(tableName);

            return con.ExecuteAsync(sql, mo);
        }


        public static Task<int> InsertList<TType>(this IDbConnection con, string tableName, IList<TType> list)
        {
            if (string.IsNullOrEmpty(tableName))
                tableName = typeof(TType).Name;

            var sql = GetInsertSql<TType>(tableName);

            return con.ExecuteAsync(sql, list);
        }


        private static string GetInsertSql<TType>(string tableName)
        {
            //  todo 未来针对类型，添加语句缓存
            var properties = typeof(TType).GetProperties();

            var sqlCols = new StringBuilder("INSERT INTO ");
            sqlCols.Append(tableName).Append(" (");

            var sqlValues = new StringBuilder(" VALUES (");
            var isStart = false; 

            foreach (var propertyInfo in properties)
            {
                //if (haveAuto)
                //{
                //    var isAuto = propertyInfo.GetCustomAttribute<AutoColumnAttribute>() != null;
                //    if (isAuto)
                //    {
                //        continue;
                //    }
                //}

                if (isStart)
                {
                    sqlCols.Append(",");
                    sqlValues.Append(",");
                }
                else
                    isStart = true;

                sqlCols.Append(propertyInfo.Name);
                sqlValues.Append("@").Append(propertyInfo.Name);
            }
            sqlCols.Append(")");
            sqlValues.Append(")");
            sqlCols.Append(sqlValues);
            
            return sqlCols.ToString();
        }
        #endregion

        internal static async Task<ResultMo> UpdatePartial<TType>(this IDbConnection con, string tableName,
            Expression<Func<TType, object>> update, Expression<Func<TType, bool>> where, object mo)
            //where TType : BaseMo
        {
            if (string.IsNullOrEmpty(tableName))
                tableName = typeof(TType).Name;

            var visitor = new SqlExpressionVisitor();

            var updateSql = GetVisitExpressSql(visitor, update, SqlVistorType.Update);
            var whereSql = GetVisitExpressSql(visitor, where, SqlVistorType.Where);
            var sql = string.Concat("UPDATE ", tableName, " SET ", updateSql, whereSql);

            var paras = GetExecuteParas(mo, visitor);
            var row = await con.ExecuteAsync(sql, paras);
            return row > 0 ? new ResultMo() : new ResultMo().WithRes(ResultTypes.UpdateFail, "操作失败！");
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
            var paras = GetExecuteParas(null, sqlVisitor);

            return await con.QuerySingleOrDefaultAsync<TType>(sqlStr, paras);
        }
        public static async Task<ResultListMo<TType>> GetList<TType>(this IDbConnection con, string tableName, Expression<Func<TType, bool>> whereExp)
        {
            if (string.IsNullOrEmpty(tableName))
                tableName = typeof(TType).Name;

            var sqlVisitor = new SqlExpressionVisitor();
            var whereSql = GetVisitExpressSql(sqlVisitor, whereExp, SqlVistorType.Where);

            var sqlStr = string.Concat("SELECT * FROM ", tableName, whereSql);
            var paras = GetExecuteParas(null, sqlVisitor);

            var listRes = (await con.QueryAsync<TType>(sqlStr, paras)).ToList();

            return listRes.Count == 0
                ? new ResultListMo<TType>().WithRes(ResultTypes.ObjectNull, "没有查到相关信息！")
                : new ResultListMo<TType>(listRes.ToList());
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
                return updateFlag.sql;
            }

            string sql;
            if (exp == null)
               throw new ArgumentNullException("whereExp","where表达式不能为空！");
            else
            {
                var whereFlag = new SqlVistorFlag(SqlVistorType.Where);
                visitor.Visit(exp, whereFlag);
                sql = string.Concat(" WHERE ", whereFlag.sql);
            }

            return sql;
        }

        private static object GetExecuteParas(object mo, SqlExpressionVisitor visitor)
        {
            if (!visitor.parameters.Any())
                return mo;

            var paras = new DynamicParameters(visitor.parameters);
            if (mo == null || !visitor.properties.Any())
                return paras;

            paras.AddDynamicParams(mo);
            return paras;
        }
    }
}

