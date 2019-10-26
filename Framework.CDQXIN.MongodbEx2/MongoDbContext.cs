using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Framework.CDQXIN.MongodbEx2
{
    /// <summary>
    /// MongoDb帮助类
    /// </summary>
    /// <summary>
    /// MongoDb帮助类
    /// </summary>
    public class DB
    {
        private static readonly string connStr = "mongodb://127.0.0.1:27017";//GlobalConfig.Settings["mongoConnStr"];

        private static readonly string dbName = "student";//GlobalConfig.Settings["mongoDbName"];

        private static IMongoDatabase db = null;

        private static readonly object lockHelper = new object();

        private DB() { }

        public static IMongoDatabase GetDb()
        {
            if (db == null)
            {
                lock (lockHelper)
                {
                    if (db == null)
                    {
                        var client = new MongoClient(connStr);
                        db = client.GetDatabase(dbName);
                    }
                }
            }
            return db;
        }
    }

    public class MongoDbHelper<T>
    {
        private IMongoDatabase db = null;

        private IMongoCollection<T> collection = null;

        public MongoDbHelper()
        {
            this.db = DB.GetDb();
            collection = db.GetCollection<T>(typeof(T).Name);
        }
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public T Insert(T entity)
        {
            var flag = ObjectId.GenerateNewId();
            entity.GetType().GetProperty("_id").SetValue(entity, flag);
            collection.InsertOneAsync(entity);
            return entity;
        }


        /// <summary>
        /// 修改一个值
        /// </summary>
        /// <param name="express"></param>
        /// <param name="field"></param>
        /// <param name="value"></param>
        public bool Modify(Expression<Func<T, bool>> express, object updateField)
        {
            if (updateField == null) return false;

            var props = updateField.GetType().GetProperties();
            var field = props[0].Name;
            var value = props[0].GetValue(updateField);
            var updated = Builders<T>.Update.Set(field, value);

            UpdateResult result = collection.UpdateOneAsync(express, updated).Result;
            return result.ModifiedCount > 0 ? true : false;
        }



        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity"></param>
        public bool Update(Expression<Func<T, bool>> express, T entity)
        {
            try
            {
                var old = collection.Find(express).ToList().FirstOrDefault();

                foreach (var prop in entity.GetType().GetProperties())
                {
                    if (!prop.Name.Equals("_id"))
                    {
                        var newValue = prop.GetValue(entity);
                        var oldValue = old.GetType().GetProperty(prop.Name).GetValue(old);

                        if (newValue != null)
                        {
                            if (oldValue == null)
                                oldValue = "";
                            if (!newValue.ToString().Equals(oldValue.ToString()))
                            {
                                old.GetType().GetProperty(prop.Name).SetValue(old, newValue.ToString());
                            }
                        }
                    }
                }

                // var filter = Builders<T>.Filter.Eq("Id", entity.Id);
                ReplaceOneResult result = collection.ReplaceOneAsync(express, old).Result;
                if (result.ModifiedCount > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                var aaa = ex.Message + ex.StackTrace;
                throw;
            }
        }


        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity"></param>
        public bool Delete(Expression<Func<T, bool>> express)
        {
            DeleteResult result = collection.DeleteOneAsync(express).Result;
            return result.DeletedCount > 0 ? true : false;
        }



        /// <summary>
        /// 查询条件查询数据
        /// </summary>
        /// <returns></returns>
        public List<T> QueryAll(Expression<Func<T, bool>> express)
        {
            return collection.Find(express).ToList();
        }


        /// <summary>
        /// 根据条件查询一条数据
        /// </summary>
        /// <param name="express"></param>
        /// <returns></returns>
        public T QueryByFirst(Expression<Func<T, bool>> express)
        {
            return collection.Find(express).ToList().FirstOrDefault();
        }




        /// <summary>
        /// 批量添加
        /// </summary>
        /// <param name="list"></param>
        public void InsertBatch(List<T> list)
        {
            collection.InsertManyAsync(list);
        }


        /// <summary>
        /// 根据Id批量删除
        /// </summary>
        public bool DeleteBatch(List<ObjectId> list)
        {
            var filter = Builders<T>.Filter.In("_id", list);
            DeleteResult result = collection.DeleteManyAsync(filter).Result;
            return result.DeletedCount > 0 ? true : false;
        }

    }
}
