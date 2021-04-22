using System;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace UpdateConsoleApp2
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var db = client.GetDatabase("ntut") as MongoDatabaseBase;
            var driverscol = db.GetCollection<driversDocument>("drivers");
            var filterdata = Builders<driversDocument>.Filter;
            var updatedata = Builders<driversDocument>.Update;

            //新增6-1
            /*var driver = new driversDocument
            {
                _id = "003",
                group = "臺北大車隊",
                status = "rest",
                seatinfo = new driversDocument.Seatinfo
                    {
                        capacity = 5,
                        remaining = 5,
                        riding = 0
                    },  
                location = new double[] { 121.549916,25.050594},       //meaning => location:[121.517224,25.047974]          
                lastrModified = DateTime.Now
            };
            var count=driverscol.Find(filterdata.Eq( e => e._id, driver._id)).ToListAsync().Result.FirstOrDefault();
            if (count == null)
            {
                driverscol.InsertOne(driver);
            }
            else
            {
                Console.WriteLine("已有資料");
            }*/

            //刪除6-2
            var temp = filterdata.Eq(e => e._id, "001");
            //driverscol.DeleteOne(temp);

            //6-3
            var accountscol = db.GetCollection<accountDocument>("accounts");
            var accountupdate = Builders<accountDocument>.Update;
            var accountfilter = Builders<accountDocument>.Filter;
            var inctest = accountupdate.Inc(e => e.balance, 1000);
            //accountscol.UpdateOneAsync(accountfilter.Eq(e => e.name, "小明"), inctest);

            //6-4
            var productcol = db.GetCollection<ProductsDocument>("products");
            var updateproduct = Builders<ProductsDocument>.Update;
            var filterproduct = Builders<ProductsDocument>.Filter;
            var setandmul = updateproduct.Set("type", "USD").Mul(e => e.price, 30.401);
            //productcol.UpdateMany(filterproduct.Eq(e => e.type, "NTD"), setandmul);

            //6-5
            var scorescol = db.GetCollection<scoresDocument>("scores");
            var updatescores = Builders<scoresDocument>.Update;
            var filterscores = Builders<scoresDocument>.Filter;
            var findeq = filterscores.Eq(e => e._id, "001");
            var set = updatescores.Rename(e => e.studentNumber, "studentId");
            //scorescol.UpdateMany(findeq, set);

            //6-6
            var max = updatescores.Max(e => e.score, 60);
            //scorescol.UpdateMany(new BsonDocument { },max);

            //6-7
            var min = updatescores.Min(e => e.score, 100);
            //scorescol.UpdateMany(new BsonDocument { }, min);

            //6-8
            var unset = updatedata.Unset(e => e.group);
            //driverscol.UpdateMany(filterdata.Eq(e => e._id, "001"), unset);

            //6-9
            var setcurrentDate = updatedata.CurrentDate(e => e.lastrModified, UpdateDefinitionCurrentDateType.Date).Set(e => e.status, "rest");
            //driverscol.UpdateOne(filterdata.Eq(e => e._id, "002"), setcurrentDate);


            //6-10
            var arrarycol = db.GetCollection<arrayDocument>("array");
            var filterarray = Builders<arrayDocument>.Filter;
            var updatearray = Builders<arrayDocument>.Update;
            var pushData = updatearray.Push(e => e.list, 80);
            //arrarycol.UpdateMany(filterarray.Eq(e => e._id, "001"), pushData);

            //6-11
            var test =new int[] { 60, 60, 70 };
            var pushdata2 = updatearray.PushEach(e => e.list, test, position: 3);
            //arrarycol.UpdateOne(filterarray.Eq(e => e._id, "001"), pushdata2);

            //6-12
            var poplast = updatearray.PopLast(e => e.list);
            //arrarycol.UpdateOne(filterarray.Eq(e => e._id, "001"), poplast);

            //6-13
            var popfirst = updatearray.PopFirst(e => e.list);
            //arrarycol.UpdateOne(filterarray.Eq(e => e._id, "001"), popfirst);

            //6-14
            var pullnum = updatearray.Pull(e => e.list, 60);
            //arrarycol.UpdateOne(filterarray.Eq(e => e._id, "001"), pullnum);


            //6-15
            var drinkcol = db.GetCollection<drinkDocument>("drink");
            var filterdrink = Builders<drinkDocument>.Filter;
            var updatedrink = Builders<drinkDocument>.Update;           
            //var Updateorder1 = drinkcol.UpdateOne(filterdrink.Eq(e => e._id, "001"), updatedrink.Inc(e => e.sold, 20).Push(e => e.log, new Log { time = DateTime.Now,size ="M"}));
            //var Updateorder2 = drinkcol.UpdateOne(filterdrink.Eq(e => e._id, "002"), updatedrink.Inc(e => e.sold, 40).Push(e => e.log, new Log { time = DateTime.Now, size = "M" }));
            //var Updateorder3 = drinkcol.UpdateOne(filterdrink.Eq(e => e._id, "003"), updatedrink.Inc(e => e.sold, 65).Push(e => e.log, new Log { time = DateTime.Now, size = "L" }));
            //以上為單次上傳資料
            //以下為一次上傳多個資料
            var model = new List<WriteModel<drinkDocument>> 
            { 
                new UpdateOneModel<drinkDocument>(filterdrink.Eq(e => e._id, "001"),updatedrink.Inc(e => e.sold, 20).Push(e => e.log, new Log { time = DateTime.Now,size ="M"})),
                new UpdateOneModel<drinkDocument>(filterdrink.Eq(e => e._id, "002"),updatedrink.Inc(e => e.sold, 40).Push(e => e.log, new Log { time = DateTime.Now, size = "M" })),
                new UpdateOneModel<drinkDocument>(filterdrink.Eq(e => e._id, "003"),updatedrink.Inc(e => e.sold, 65).Push(e => e.log, new Log { time = DateTime.Now, size = "L" }))
            };
            
            drinkcol.BulkWrite(model);
        }
    }
}
