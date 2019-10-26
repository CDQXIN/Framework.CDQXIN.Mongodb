using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Framework.CDQXIN.MongodbEx1
{
    public class DBConfiguration
    {
        /// <summary>
        /// 加载XML读写字符串
        /// </summary>
        /// <returns></returns>
        public static DBConnections GetConnection()
        {
            DBConnections conn = new DBConnections();
            var obj = MemoryCacheHelper.getCacheValue("_DBConnection");
            if (obj != null)
            {
                return obj as DBConnections;
            }
            string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "connection.xml");
            XElement root = XElement.Load(path);

            foreach (var elm in root.Element("SqlConn").Elements("DB"))
            {
                conn.SqlConns.Add(elm.Value);
            }

            foreach (var elm in root.Element("MongoConn").Elements("DB"))
            {
                conn.MongoDbConns.Add(elm.Value);
            }

            MemoryCacheHelper.InsertFileDependency("_DBConnection", conn, path);
            return conn;
        }

        /// <summary>
        /// 获取链接字符串
        /// </summary>
        /// <param name="TicketNumber">准考证号</param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static string GetConnection(string TicketNumber, out int index, out int count, ConnTypeEnum conn = ConnTypeEnum.MongoDB)
        {
            var connect = GetConnection();
            long num = 0; index = 0;
            if (conn == ConnTypeEnum.SQL)
            {
                count = connect.SqlConns.Count;
                if (count == 1)
                {
                    return connect.SqlConns.First();
                }
                else
                {
                    if (long.TryParse(TicketNumber, out num))
                    {
                        index = Convert.ToInt32(num % count);
                        return connect.SqlConns[Convert.ToInt32(num % count)];
                    }
                    else
                    {
                        return connect.SqlConns.First();
                    }
                }
            }
            else
            {
                count = connect.MongoDbConns.Count;
                if (count == 1)
                {
                    return connect.MongoDbConns.First();
                }
                else
                {
                    if (long.TryParse(TicketNumber, out num))
                    {
                        index = Convert.ToInt32(num % count);
                        return connect.MongoDbConns[Convert.ToInt32(num % count)];
                    }
                    else
                    {
                        return connect.MongoDbConns.First();
                    }
                }
            }
        }

    }

    public class DBConnections
    {
        /// <summary>
        /// SQL链接字符串
        /// </summary>
        public List<string> SqlConns { get; set; } = new List<string>();

        /// <summary>
        /// MongoDB 字符串
        /// </summary>
        public List<string> MongoDbConns { get; set; } = new List<string>();
    }

    public enum ConnTypeEnum
    {
        SQL = 1,
        MongoDB = 2
    }
}
