

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Dapper;
using Npgsql;
using Elivn.ORM.Pgsql.Dapper.OrmExtension;
using Kasca.Common.ComModels;
using Kasca.Common.ComModels.Enums;

namespace Elivn.ORM.Pgsql.Dapper
{
    /// <summary>
    /// 仓储层基类
    /// </summary>
    public abstract class BasePgRep<TRep,TType,IdType>//:SingleInstance<TRep>
        where TRep:class ,new()
      //  where TType:BaseMo<IdType>,new()
    {
        protected static string m_TableName;
        private readonly string _writeConnectionString;
        private readonly string _readConnectionString;

        public BasePgRep(string writeConnectionStr, string readConnectionStr)
        {
            _writeConnectionString = writeConnectionStr;
            _readConnectionString = readConnectionStr;
        }

        #region Add

        /// <summary>
        ///   插入数据
        /// </summary>
        /// <param name="mo"></param>
        /// <returns></returns>
        public virtual async Task<ResultMo<IdType>> Add(TType mo)
        {
            var res = await ExecuteWriteAsync(async con =>
            {
                var row = await con.Insert(m_TableName, mo);
                return row > 0 ? new ResultMo<IdType>() 
                    : new ResultMo<IdType>().WithRes(ResultTypes.AddFail, "添加失败!");
            });
            return res;
        }

        /// <summary>
        ///   插入数据
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public virtual async Task<ResultMo> AddList(IList<TType> list)
        {
            var res = await ExecuteWriteAsync(async con =>
            {
                var row = await con.InsertList(m_TableName, list);
                return row > 0 ? new ResultMo() : new ResultMo().WithRes(ResultTypes.AddFail, "添加失败!");
            });
            return res;
        }


        #endregion

        #region Update


        /// <summary>
        /// 部分字段的更新
        ///     参考用法: Update(u => new {u.status}, u => u.id == 1111 ,new{ status });
        /// </summary>
        ///  <param name="updateExp">
        /// 更新字段,示例：
        ///  u=>new{ u.Name, ....},这样生成的参数是同名参数，会从mo对象同名属性中取值
        ///  或者 u=> new{ Name="",mo.Status,....}，这样生成的是匿名参数，参数值即对象本身的值。
        ///  注解：表达式在解析过程中并无实际入参，所以表达式中（TType）u 下的属性仅做类型推断，无实际值，需要通过mo参数传入，where表达式处理相同。 
        /// </param>
        /// <param name="whereExp">
        /// 判断条件 示例：
        ///     w=>w.id==1  , 如果为空默认根据Id判断
        /// </param>
        /// <param name="mo">update和where表达式中参数值</param>
        /// <returns></returns>
        protected virtual Task<ResultMo> Update(Expression<Func<TType, object>> updateExp,
            Expression<Func<TType, bool>> whereExp, object mo = null)
            => ExecuteWriteAsync(con => con.UpdatePartial(m_TableName, updateExp, whereExp, mo));

        /// <summary>
        ///  直接使用语句更新操作
        /// </summary>
        /// <param name="updateColNamesSql"></param>
        /// <param name="whereSql"></param>
        /// <param name="para"></param>
        /// <returns></returns>
        protected virtual Task<ResultMo> Update(string updateColNamesSql, string whereSql, object para = null)
         => ExecuteWriteAsync(async con =>
         {
             var sql = string.Concat("UPDATE ", m_TableName, " SET ", updateColNamesSql, " ", whereSql);
             var row = await con.ExecuteAsync(sql, para);
             return row > 0 ? new ResultMo() : new ResultMo().WithRes(ResultTypes.UpdateFail, "更新失败");
         });

        #endregion

        #region Delete

        /// <summary>
        /// 软删除，仅仅修改  status = CommonStatus.Delete 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual Task<ResultMo> SoftDeleteById(string id)
        {
            var whereSql = "id=@id";
            var dirPara =  new{  id };

            return SoftDelete(whereSql, dirPara);
        }

        /// <summary>
        /// 软删除，直接修改  status = CommonStatus.Delete 
        /// </summary>
        /// <param name="whereExp">条件表达式</param>
        /// <returns></returns>
        protected virtual Task<ResultMo> SoftDelete(Expression<Func<TType, bool>> whereExp)
        {
            return Update(m => new { status = CommonStatus.Delete }, whereExp);
        }

        /// <summary>
        /// 软删除，直接修改状态
        /// </summary>
        /// <param name="whereSql"></param>
        /// <param name="whereParas"></param>
        /// <returns></returns>
        protected virtual Task<ResultMo> SoftDelete(string whereSql, object whereParas=null)
        {
            if (string.IsNullOrEmpty(whereSql))
            {
                return Task.FromResult(new ResultMo(ResultTypes.ParaError, "where语句不能为空！"));
            }
            return ExecuteWriteAsync(async con =>
            {
                var sql = $"UPDATE {m_TableName} SET status={(int)CommonStatus.Delete} WHERE {whereSql}";

                var rows = await con.ExecuteAsync(sql, whereParas);
                return rows > 0 ? new ResultMo() : new ResultMo().WithRes(ResultTypes.UpdateFail, "soft delete Failed!");
            });
        }

        #endregion

        #region Get
        /// <summary>
        /// 通过id获取实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<ResultMo<TType>> GetById(string id)
        {
            return GetById<TType>(id);
        }
        /// <summary>
        /// 通过id获取实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual Task<ResultMo<RType>> GetById<RType>(string id)
        {
            var sql = string.Concat("select * from ", m_TableName, " WHERE id=@id");
            var dirPara = new Dictionary<string, object> { { "@id", id } };

            return Get<RType>(sql, dirPara);
        }

        /// <summary>
        ///  获取单个实体对象
        /// </summary>
        /// <param name="whereExp">判断条件，如果为空默认根据Id判断</param>
        /// <returns></returns>
        protected virtual Task<ResultMo<TType>> Get(Expression<Func<TType, bool>> whereExp)
            => ExecuteReadAsync(con => con.Get(m_TableName, whereExp));

        /// <summary>
        /// 通过sql语句获取实体
        /// </summary>
        /// <param name="getSql"> 查询sql语句</param>
        /// <param name="para"></param>
        /// <returns></returns>
        protected virtual Task<ResultMo<RType>> Get<RType>(string getSql, object para)
        {
            return ExecuteReadAsync(con => con.QuerySingleOrDefaultAsync<RType>(getSql, para));
        }
        #endregion

        #region Get(Page)List

        /// <summary>
        ///   列表查询
        /// </summary>
        /// <param name="whereExp"></param>
        /// <returns></returns>
        protected virtual Task<ResultListMo<TType>> GetList(Expression<Func<TType, bool>> whereExp)
            => ExecuteReadSubAsync(con => con.GetList(m_TableName, whereExp));


        /// <summary>
        ///   列表查询
        /// </summary>
        /// <param name="getSql">查询语句</param>
        /// <param name="paras">参数内容</param>
        /// <returns></returns>
        protected Task<ResultListMo<TType>> GetList(string getSql, object paras)
        {
            return GetList<TType>(getSql, paras);
        }

        /// <summary>
        ///   列表查询
        /// </summary>
        /// <param name="getSql">查询语句</param>
        /// <param name="paras">参数内容</param>
        /// <returns></returns>
        protected virtual async Task<ResultListMo<RType>> GetList<RType>(string getSql,
            object paras)
        {
            return await ExecuteReadSubAsync(async con =>
            {
                var list = (await con.QueryAsync<RType>(getSql, paras))?.ToList();

                return list?.Count > 0
                    ? new ResultListMo<RType>(list)
                    : new ResultListMo<RType>().WithRes(ResultTypes.ObjectNull, "没有查到相关信息！");
            });
        }


        /// <summary>
        ///   列表查询
        /// </summary>
        /// <param name="selectSql">查询语句，包含排序等</param>
        /// <param name="totalSql">查询数量语句，不需要排序,如果为空，则不计算和返回总数信息</param>
        /// <param name="paras">参数内容</param>
        /// <returns></returns>
        protected Task<PageListMo<TType>> GetPageList(string selectSql, object paras,
            string totalSql = null)
        {
            return GetPageList<TType>(selectSql, paras, totalSql);
        }
        /// <summary>
        ///   列表查询
        /// </summary>
        /// <param name="selectSql">查询语句，包含排序等</param>
        /// <param name="totalSql">查询数量语句，不需要排序,如果为空，则不计算和返回总数信息</param>
        /// <param name="paras">参数内容</param>
        /// <returns></returns>
        protected virtual async Task<PageListMo<RType>> GetPageList<RType>(string selectSql, object paras,
            string totalSql = null)
        {
            return await ExecuteReadSubAsync(async con =>
            {
                long total = 0;

                if (!string.IsNullOrEmpty(totalSql))
                {
                    total = await con.ExecuteScalarAsync<long>(totalSql, paras);
                    if (total <= 0) return new PageListMo<RType>().WithRes(ResultTypes.ObjectNull, "没有查到相关信息！");
                }

                var list = await con.QueryAsync<RType>(selectSql, paras);
                return new PageListMo<RType>(total, list.ToList());
            });
        }

        #endregion

        #region 底层基础读写分离操作封装

        /// <summary>
        /// 执行写数据库操作
        /// </summary>
        /// <typeparam name="RespType"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        protected Task<RespType> ExecuteWriteAsync<RespType>(Func<IDbConnection, Task<RespType>> func) where RespType : ResultMo, new()
            => ExecuteAsync(func, true);

        /// <summary>
        ///  执行读操作，返回具体类型，自动包装成Resp结果实体
        /// </summary>
        /// <typeparam name="RespParaType"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        protected Task<ResultMo<RespParaType>> ExecuteReadAsync<RespParaType>(Func<IDbConnection, Task<RespParaType>> func) => ExecuteAsync(async con =>
        {
            var res =await func(con);
            return res != null ? new ResultMo<RespParaType>(res) : new ResultMo<RespParaType>().WithRes(ResultTypes.ObjectNull, "未发现相关数据！");
        }, false);

        /// <summary>
        /// 执行读操作，直接返回继承自Resp实体
        /// </summary>
        /// <typeparam name="SubRespType"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        protected async Task<SubRespType> ExecuteReadSubAsync<SubRespType>(Func<IDbConnection, Task<SubRespType>> func) where SubRespType : ResultMo, new()
            => await ExecuteAsync(func, false);

        /// <summary>
        ///  最终执行操作
        /// </summary>
        /// <typeparam name="RType"></typeparam>
        /// <param name="func"></param>
        /// <param name="isWrite"></param>
        /// <returns></returns>
        protected virtual async Task<RType> ExecuteAsync<RType>(Func<IDbConnection, Task<RType>> func, bool isWrite)
            where RType : ResultMo, new()
        {
            RType t;
            //try
            //{
                using (var con = new NpgsqlConnection(isWrite ? _writeConnectionString : _readConnectionString))
                {
                    t = await func(con);
                }
            //}
            //catch (Exception e)
            //{
            //    LogHelper.Error(string.Concat("数据库操作错误,仓储表名：", m_TableName, "，详情：", e.Message, "\r\n", e.StackTrace), "DataRepConnectionError",
            //        "DapperRep_PG");
            //    t = new RType
            //    {
            //        sys_ret = (int)SysResultTypes.ApplicationError,
            //        ret = (int) ResultTypes.InnerError,
            //        msg = isWrite ? "数据写入出错！" : "数据读取出错！"
            //    };
            //}
            return t ?? new RType() { code = (int) ResultTypes.ObjectNull, msg = "未发现对应结果" };
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






