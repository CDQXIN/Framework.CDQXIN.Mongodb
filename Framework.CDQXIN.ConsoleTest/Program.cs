

using Framework.CDQXIN.MongodbEx2;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.CDQXIN.ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //var mongoDbHelper = new MongoDbHelper("mongodb://127.0.0.1:27017", "student");

            ////mongoDbHelper.CreateCollection<Student>("SysLog1", new[] { "LogDT" });
            //int num = 0;
            //List<student> lis = mongoDbHelper.FindByPage<student,student>("student", s => s.Id>0, s => s, 1, 10, out num);
            //Console.WriteLine(JsonConvert.SerializeObject(lis));
            //Console.ReadLine();
            //int rsCount = 0;
            //mongoDbHelper.FindByPage<SysLogInfo, SysLogInfo>("SysLog1", t => t.Level == "Info", t => t, 1, 20, out rsCount);

            //mongoDbHelper.Insert<SysLogInfo>("SysLog1", new SysLogInfo { LogDT = DateTime.Now, Level = "Info", Msg = "测试消息" });

            //mongoDbHelper.Update<SysLogInfo>("SysLog1", new SysLogInfo { LogDT = DateTime.Now, Level = "Error", Msg = "测试消息2" }, t => t.LogDT == new DateTime(1900, 1, 1));

            //mongoDbHelper.Delete<SysLogInfo>(t => t.Level == "Info");

            //mongoDbHelper.ClearCollection<SysLogInfo>("SysLog1");
            MongoDbHelper<student> mop = new MongoDbHelper<student>();

            List<student> lis = mop.QueryAll(p=>p.studentid>0).ToList();
            Console.WriteLine(JsonConvert.SerializeObject(lis));
            Console.ReadLine();
        }

    }

    public class student 
    {
        [BsonElement(elementName: "_id")]
        public ObjectId Id { get; set; }
        public int studentid { get; set; }
        public string name { get; set; }
        public int age { get; set; }
    }

    
}
