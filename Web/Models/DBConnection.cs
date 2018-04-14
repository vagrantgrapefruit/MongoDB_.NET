using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Web.Connection
{
    public class DBConnection<T> where T : class
    {
        string conn = null;//"mongodb://127.0.0.1:27017"; 连接字符串
        string database = null;//"test";数据库名称
        string collection = null;//"test";集合名称

        IMongoDatabase mongoDataBase = null;
        IMongoCollection<T> mongoCollection = null;

        public DBConnection()
        {

        }
        /// <summary>
        /// 初始化DBConnection
        /// </summary>
        /// <param name="conn">连接字符串</param>
        /// <param name="database">数据库名称</param>
        /// <param name="collection">集合名称</param>
        public DBConnection(string conn, string database, string collection)
        {
            this.conn = conn;
            this.database = database;
            this.collection = collection;
            mongoDataBase = new MongoClient(conn).GetDatabase(database);
            mongoCollection = mongoDataBase.GetCollection<T>(collection);
        }

        #region 查询单条数据
        /// <summary>
        /// 同步查询一条数据
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <param name="projector">查询字段</param>
        /// <returns></returns>
        public T SelectFirst(Expression<Func<T, bool>> predicate, Expression<Func<T, T>> projector = null)
        {
            try
            {
                if (predicate != null)
                    return SelectFirstAsync(predicate, projector).Result;
                else
                    return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 另一个同步查询一条数据
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <param name="projector">查询字段</param>
        /// <returns></returns>
        public T SelectFirst0(Expression<Func<T, bool>> predicate, Expression<Func<T, T>> projector = null)
        {
            try
            {
                if (predicate != null)
                {
                    var result = mongoCollection.Find(predicate);
                    if (projector != null)
                        result = result.Project(projector);
                    return result.FirstOrDefault();
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 异步查询一条数据
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <param name="projector">查询字段</param>
        /// <returns></returns>
        public async Task<T> SelectFirstAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, T>> projector = null)
        {
            try
            {
                if (predicate != null)
                {
                    var result = mongoCollection.Find(predicate);
                    if (projector != null)
                        result = result.Project(projector);
                    return await result.FirstOrDefaultAsync().ConfigureAwait(false);
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion

        #region 查询List

        /// <summary>
        /// 同步查询前limit条数据，默认查询全部
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="projector"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public List<T> Select(Expression<Func<T, bool>> predicate, Expression<Func<T, T>> projector = null, int? limit = null)
        {
            try
            {
                if (predicate != null)
                    return SelectAsync(predicate, projector, limit).Result;
                else
                    return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 异步查询前limit条数据，默认查询全部
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <param name="projector">查询字段</param>
        /// <param name="limit">查询行数</param>
        /// <returns></returns>
        public async Task<List<T>> SelectAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, T>> projector = null, int? limit = null)
        {
            try
            {
                if (predicate != null)
                {
                    var result = mongoCollection.Find(predicate);

                    if (projector != null)
                        result = result.Project(projector);

                    if (limit != null)
                        result = result.Limit(limit);

                    return await result.ToListAsync().ConfigureAwait(false);
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        #endregion

        #region 插入

        /// <summary>
        /// 同步插入一条数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool Insert(T entity)
        {
            try
            {
                if (entity != null)
                {
                    mongoCollection.InsertOne(entity);
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 异步插入一条数据
        /// </summary>
        /// <param name="enity">数据实体</param>
        public async Task<bool> InsertAsync(T entity)
        {
            try
            {
                if (entity != null)
                {
                    await mongoCollection.InsertOneAsync(entity).ConfigureAwait(false);
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);//为什么要new一个Excption
            }
        }

        #endregion

        #region 删除

        /// <summary>
        /// 删除一条记录
        /// </summary>
        /// <param name="entity">删除的对象实体</param>
        /// <returns></returns>
        public bool DeleteOne(T entity)
        {
            try
            {
                if (entity != null)
                {
                    var deleteResult = mongoCollection.DeleteOne(entity.ToBsonDocument());
                    if (deleteResult.DeletedCount != 0)
                        return true;
                    else return false;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 异步删除一条记录
        /// </summary>
        /// <param name="entity">删除的对象实体</param>
        /// <returns></returns>
        public async Task<bool> DeleteOneAsync(T entity)
        {
            try
            {
                if (entity != null)
                {
                    var deleteResult = await mongoCollection.DeleteOneAsync(entity.ToBsonDocument());
                    if (deleteResult.DeletedCount != 0)
                        return true;
                    else return false;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 根据条件删除记录
        /// </summary>
        /// <param name="predicate">删除条件</param>
        /// <returns></returns>
        public bool DeleteBypredicate(Expression<Func<T, bool>> predicate)
        {
            try
            {
                if (predicate != null)
                {
                    var deleteResult = mongoCollection.DeleteMany(predicate);
                    if (deleteResult.DeletedCount != 0)
                        return true;
                    else return false;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 根据条件异步删除记录
        /// </summary>
        /// <param name="predicate">删除条件</param>
        /// <returns></returns>
        public async Task<bool> DeleteBypredicateAsync(Expression<Func<T, bool>> predicate)
        {
            try
            {
                if (predicate != null)
                {
                    var deleteResult = await mongoCollection.DeleteOneAsync(predicate);
                    if (deleteResult.DeletedCount != 0)
                        return true;
                    else return false;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion

        #region 更新

        /// <summary>
        /// 同步更新一条数据
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <param name="entity">更改实体</param>
        /// <returns></returns>
        public bool UpdataOne(Expression<Func<T, bool>> predicate, T entity)
        {
            try
            {
                if (predicate != null && entity != null)
                {
                    return UpdataOneAsync(predicate, entity).Result;                    
                }
                else return false;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 异步更新一条数据
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <param name="entity">更改实体</param>
        /// <returns></returns>
        public async Task<bool> UpdataOneAsync(Expression<Func<T, bool>> predicate, T entity)
        {
            try
            {
                if (predicate != null && entity != null)
                {
                    UpdateDefinition<T> updateDefinitionList = entity.ToBsonDocument<T>();
                    var updateDefinitionBuilder = new UpdateDefinitionBuilder<T>().Combine(updateDefinitionList);
                    var result = await mongoCollection.UpdateOneAsync<T>(predicate, updateDefinitionBuilder).ConfigureAwait(false);
                    if (result.ModifiedCount != 0)
                        return true;
                    else return false;
                }
                else return false;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 同步更新所有满足条件的数据
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <param name="entity">更改实体</param>
        /// <returns></returns>
        public bool Updata(Expression<Func<T, bool>> predicate, T entity)
        {
            try
            {
                if (predicate != null && entity != null)
                {
                    return UpdataAsync(predicate, entity).Result;
                }
                else return false;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 异步更新所有满足条件的数据
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <param name="entity">更改实体</param>
        /// <returns></returns>
        public async Task<bool> UpdataAsync(Expression<Func<T, bool>> predicate, T entity)
        {
            try
            {
                if (predicate != null && entity != null)
                {
                    UpdateDefinition<T> updateDefinitionList = entity.ToBsonDocument<T>();
                    var updateDefinitionBuilder = new UpdateDefinitionBuilder<T>().Combine(updateDefinitionList);
                    var result = await mongoCollection.UpdateManyAsync<T>(predicate, updateDefinitionBuilder).ConfigureAwait(false);
                    if (result.ModifiedCount != 0)
                        return true;
                    else return false;
                }
                else return false;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion

    }
}