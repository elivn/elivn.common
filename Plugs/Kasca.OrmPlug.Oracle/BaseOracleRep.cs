#region Copyright (C) 2017 Elivn  

/***************************************************************************
*　　	文件功能描述：OSSCore仓储层 —— 仓储基类
*
*　　	创建人： Elivn
*       创建人Email：498353921@qq.com
*    	创建日期：2020-11-25
*       
*****************************************************************************/

#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Dapper;
using Oracle.ManagedDataAccess.Client;
using Kasca.Common.ComModels;
using Kasca.Common.ComModels.Enums;
using Kasca.Common.ComUtils;
using Kasca.Common.Plugs.LogPlug;

namespace Kasca.OrmPlug.Oracle
{
    /// <summary>
    /// 仓储层基类
    /// </summary>
    public class BaseOracleRep<TRep, TType>
        where TRep : class, new()
    //where TType:BaseMo,new()
    {
        protected static string m_TableName;

        protected static string m_writeConnectionString;
        protected static string m_readeConnectionString;

        static BaseOracleRep()
        {
            m_writeConnectionString = ConfigUtil.GetConnectionString("WriteConnection");
            m_readeConnectionString = ConfigUtil.GetConnectionString("ReadeConnection");
            SqlVistorFlag.SetDbProvider(SqlVistorProvider.Oracle);
        }

        #region 底层基础读写分离封装

        /// <summary>
        /// 执行写数据库操作
        /// </summary>
        /// <typeparam name="RType"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        protected internal static async Task<RType> ExcuteWriteAsync<RType>(Func<IDbConnection, Task<RType>> func)
            where RType : ResultMo, new()
            => await Execute(func, m_writeConnectionString);

        /// <summary>
        ///  执行读操作，返回具体类型，自动包装成ResultMo结果实体
        /// </summary>
        /// <typeparam name="RType"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        protected internal static async Task<ResultMo<RType>> ExecuteReadeAsync<RType>(
            Func<IDbConnection, Task<RType>> func) => await Execute(async con =>
        {
            var res = await func(con);
            return new ResultMo<RType>(res);
        }, m_readeConnectionString);

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns></returns>
        public virtual async Task<ResultListMo<TType>> GetAll()
        {
            string sql = $"select * from {m_TableName} ";
            return await GetList(sql, null);
        }

        /// <summary>
        /// 执行读操作，直接返回继承自ResultMo实体
        /// </summary>
        /// <typeparam name="RType"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        protected internal static async Task<RType> ExecuteReadeResAsync<RType>(Func<IDbConnection, Task<RType>> func)
            where RType : ResultMo, new()
            => await Execute(func, m_readeConnectionString);

        private static async Task<RType> Execute<RType>(Func<IDbConnection, Task<RType>> func, string connecStr)
            where RType : ResultMo, new()
        {
            RType t;
            try
            {
                using (var con = new OracleConnection(connecStr))
                {
                    t = await func(con);
                }
            }
            catch (Exception e)
            {
                LogUtil.Error(string.Concat($"数据库操作错误，仓储表：({m_TableName})详情：", e.Message, "\r\n", e.StackTrace), "DataRepConnectionError","DapperRep");
                t = new RType
                {
                    code = (int) ResultTypes.InnerError,
                    msg = "数据更新出错！"
                };
            }

            return t ?? new RType() {code = (int) ResultTypes.ObjectNull, msg = "未发现对应结果"};
        }

        #endregion

        #region   基础CRUD操作方法

        /// <summary>
        ///   插入数据
        /// </summary>
        /// <param name="mo"></param>
        /// <returns></returns>
        public virtual async Task<ResultMo> Add(TType mo)
            => await ExcuteWriteAsync(con => con.Insert(m_TableName, mo));

        /// <summary>
        /// 部分字段的更新
        /// </summary>
        ///  <param name="updateExp">更新字段new{m.Name,....} Or new{ Name="",....}</param>
        /// <param name="whereExp">判断条件，如果为空默认根据Id判断</param>
        /// <param name="mo"></param>
        /// <returns></returns>
        protected virtual async Task<ResultMo> Update(Expression<Func<TType, object>> updateExp,
            Expression<Func<TType, bool>> whereExp, object mo = null)
            => await ExcuteWriteAsync(con => con.UpdatePartail(m_TableName, updateExp, whereExp, mo));

        protected virtual async Task<ResultMo> Update(string updateSql, string whereSql, object para = null)
            => await ExcuteWriteAsync(async con =>
            {
                var sql = string.Concat("UPDATE ", m_TableName, " SET ", updateSql, whereSql);
                var row = await con.ExecuteAsync(sql, para);
                return row > 0 ? new ResultMo() : new ResultMo(ResultTypes.UpdateFail, "更新失败");
            });



        /// <summary>
        ///   列表查询
        /// </summary>
        /// <param name="selectSql">查询语句，包含排序等</param>
        /// <param name="totalSql">查询数量语句，不需要排序</param>
        /// <param name="paras"></param>
        /// <returns></returns>
        protected internal static async Task<PageListMo<T>> GetPageList<T>(string selectSql, string totalSql,
            object paras)
        {
            return await ExecuteReadeResAsync(async con =>
            {
                var total = await con.ExecuteScalarAsync<long>(totalSql, paras);
                if (total <= 0)
                    return new PageListMo<T>();

                var list = await con.QueryAsync<T>(selectSql, paras);
                return new PageListMo<T>(total, list.ToList());
            });
        }

        /// <summary>
        ///   列表查询
        /// </summary>
        /// <param name="selectSql">查询语句，包含排序等</param>
        /// <param name="totalSql">查询数量语句，不需要排序</param>
        /// <param name="paras"></param>
        /// <returns></returns>
        protected internal static async Task<PageListMo<TType>> GetPageList(string selectSql, string totalSql,
            object paras)
        {
            return await ExecuteReadeResAsync(async con =>
            {
                var total = await con.ExecuteScalarAsync<long>(totalSql, paras);
                if (total <= 0) return new PageListMo<TType>();

                var list = await con.QueryAsync<TType>(selectSql, paras);
                return new PageListMo<TType>(total, list.ToList());
            });
        }

        /// <summary>
        ///   列表查询
        /// </summary>
        /// <param name="whereExp"></param>
        /// <returns></returns>
        protected internal static async Task<ResultListMo<TType>> GetList(Expression<Func<TType, bool>> whereExp)
            => await ExecuteReadeResAsync(con => con.GetList(m_TableName, whereExp));

        /// <summary>
        ///  通过id获取信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<ResultMo<TType>> GetById(string id)
        {
            var dirPara = new Dictionary<string, object> {{":id", id}};
            var sql = string.Concat("select * from ", m_TableName, " WHERE \"ID\"=:id");
            return await Get(sql, dirPara);
        }

        /// <summary>
        ///  获取单个实体对象
        /// </summary>
        /// <param name="whereExp">判断条件，如果为空默认根据Id判断</param>
        /// <returns></returns>
        protected virtual async Task<ResultMo<TType>> Get(Expression<Func<TType, bool>> whereExp)
            => await ExecuteReadeAsync(con => con.Get(m_TableName, whereExp));

        /// <summary>
        /// 通过sql语句获取实体
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <param name="sql"></param>
        /// <param name="para"></param>
        /// <returns></returns>
        protected internal static async Task<ResultMo<TType>> Get(string sql, object para)
        {
            return await ExecuteReadeAsync(con => con.QueryFirstOrDefaultAsync<TType>(sql, para));
        }

        /// <summary>
        /// 通过sql语句获取实体
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <param name="sql"></param>
        /// <param name="para"></param>
        /// <returns></returns>
        public  async Task<ResultMo<T>> Get<T>(string sql, object para)
        {
            return await ExecuteReadeAsync(con => con.QueryFirstOrDefaultAsync<T>(sql, para));
        }

        /// <summary>
        /// 通过sql语句获取多页数据
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="para"></param>
        /// <returns></returns>
        protected internal static async Task<ResultListMo<TType>> GetList(string sql, object para)
        {
            return await ExecuteReadeResAsync(async con =>
            {
                var list =( await con.QueryAsync<TType>(sql, para));
                return new ResultListMo<TType>(list.ToList());
            });
        }

        /// <summary>
        /// 通过sql语句获取多页数据
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="para"></param>
        /// <returns></returns>
        protected internal static async Task<ResultListMo<T>> GetList<T>(string sql, object para)
        {
            return await ExecuteReadeResAsync(async con =>
            {
                var list = await con.QueryAsync<T>(sql, para);
                return new ResultListMo<T>(list.ToList());
            });
        }

        /// <summary>
        /// 软删除，仅仅修改State状态
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<ResultMo> SoftDeleteById(string id)
        {
            var sql = string.Concat("UPDATE ", m_TableName, " SET STATUS=:status WHERE ID=:id");
            var dirPara = new Dictionary<string, object> {{":id", id}, {":status", (int) CommonStatus.Delete}};

            var deRes = await ExcuteWriteAsync(async con =>
            {
                var rows = await con.ExecuteAsync(sql, dirPara);
                return rows > 0 ? new ResultMo() : new ResultMo(ResultTypes.UpdateFail, "soft delete Failed!");
            });
            return deRes;
        }

        /// <summary>
        /// 强制删除，删除物理数据【慎用】
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<ResultMo> ForceDeleteById(string id)
        {
            var sql = string.Concat("delete from ", m_TableName, " WHERE ID=:id");
            var dirPara = new Dictionary<string, object> {{":id", id}};

            var deRes = await ExcuteWriteAsync(async con =>
            {
                var rows = await con.ExecuteAsync(sql, dirPara);
                return rows > 0 ? new ResultMo() : new ResultMo(ResultTypes.UpdateFail, "force delete Failed");
            });
            return deRes;
        }

        #endregion

        #region 单例模块

        private static object _lockObj = new object();

        private static TRep _instance;

        /// <summary>
        ///   接口请求实例  
        ///  当 DefaultConfig 设值之后，可以直接通过当前对象调用
        /// </summary>
        public static TRep Instance
        {
            get
            {
                if (_instance != null)
                    return _instance;

                lock (_lockObj)
                {
                    if (_instance == null)
                        _instance = new TRep();
                }

                return _instance;
            }

        }

        #endregion
    }
}






