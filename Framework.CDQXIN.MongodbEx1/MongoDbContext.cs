using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Framework.CDQXIN.MongodbEx1
{
    public class MongoDbContext
    {
        private IMongoDatabase _mongoDatabase = null;

        public MongoDbContext(string tickNumber = "")
        {
            _mongoDatabase = Db.GetDb(tickNumber);
        }

        /// <summary>
		/// 插入数据
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="entity"></param>
		public void Insert<T>(T entity)
        {
            _mongoDatabase.GetCollection<T>(typeof(T).Name).InsertOne(entity);
        }

        /// <summary>
        /// 批量插入数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public void InsertBatch<T>(List<T> list)
        {
            _mongoDatabase.GetCollection<T>(typeof(T).Name).InsertMany(list);
        }


        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="t"></param>
        public long Delete<T>(Expression<Func<T, bool>> func)
        {
            var collection = _mongoDatabase.GetCollection<T>(typeof(T).Name);
            return collection.DeleteMany(func).DeletedCount;
        }


        /// <summary>
		/// 查询数据
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="filter">查询条件</param>
		/// <returns></returns>
		public List<T> Find<T>(Expression<Func<T, bool>> filter)
        {
            var collection = _mongoDatabase.GetCollection<T>(typeof(T).Name);
            if (filter == null)
            {
                return collection.Find(new BsonDocument()).ToListAsync().Result;
            }
            else
            {
                return collection.FindSync<T>(filter).ToList();
            }
        }


        /// <summary>
		/// 查询一条数据
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="filter"></param>
		/// <returns></returns>
		public T FindOne<T>(Expression<Func<T, bool>> filter)
        {
            try
            {
                var collection = _mongoDatabase.GetCollection<T>(typeof(T).Name);
                return collection.Find<T>(filter).FirstOrDefault();
            }
            catch
            {
                return default(T);
            }
        }


        /// <summary>
		/// 更新数据
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="entity"></param>
		/// <param name="filter">更新条件</param>
		/// <returns></returns>
		public bool Update<T>(T entity, Expression<Func<T, bool>> filter)
        {
            try
            {
                var collection = _mongoDatabase.GetCollection<T>(typeof(T).Name);
                collection.FindOneAndReplace<T>(filter, entity);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }

    public class DataBase
    {
        public IMongoDatabase db { get; set; }
        public int Index { get; set; }
    }

    public class Db
    {
        private static readonly string dbName = MongoDbConnection.DbName;

        private static List<DataBase> list = new List<DataBase>();

        private static readonly object lockHelper = new object();

        private Db() { }


        public static IMongoDatabase GetDb(string ticknumber)
        {
            if (string.IsNullOrWhiteSpace(ticknumber))
            {
                var client = new MongoClient(MongoDbConnection.Connectstring);
                return client.GetDatabase(dbName);
            }
            else
            {
                int _index = 0, count = 0;
                string conn = DBConfiguration.GetConnection(ticknumber, out _index, out count);
                if (count == 1)//单个mongdb 连接字符串
                {
                    var client = new MongoClient(conn);
                    return client.GetDatabase(dbName);
                }
                else//多个mongdb 连接字符串
                {
                    IMongoDatabase db = null;
                    if (list.Count == 0 || list.FirstOrDefault(c => c.Index == _index) == null)
                    {
                        lock (lockHelper) //双重判断
                        {
                            if (list.Count == 0 || list.FirstOrDefault(c => c.Index == _index) == null)
                            {
                                var client = new MongoClient(conn);
                                list.Add(new DataBase() { db = client.GetDatabase(dbName), Index = _index });
                                db = client.GetDatabase(dbName);
                            }
                        }
                    }
                    return list.FirstOrDefault(c => c.Index == _index).db;
                }
            }
        }
    }


    /// <summary>
	/// MongoDbConnection
	/// </summary>
	public class MongoDbConnection
    {

            /// <summary>
            /// 数据库名
            /// </summary>
            public static readonly string DbName = ConfigurationManager.AppSettings["MongoDbName"] ?? "";

            /// <summary>
            /// 数据库链接字符串
            /// </summary>
            public static readonly string Connectstring = ConfigurationManager.AppSettings["MongoDbConnectstring"] ?? "";
    }
}
