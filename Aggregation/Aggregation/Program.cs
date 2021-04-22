using System;
using MongoDB.Driver;
using MongoDB.Bson;

namespace Aggregation
{
    class Program
    {
        static void Main(string[] args)
        {
            //8-1
            var client = new MongoClient("mongodb://localhost:27017");
            var db = client.GetDatabase("ntut") as MongoDatabaseBase;
            var custmorcol = db.GetCollection<CustomersDocument>("customers");
            var filtercustomer = Builders<CustomersDocument>.Filter;
            var fnmap = @"function()
                        {   
                            emit(this.district,{count:1,age:this.age})
                        }";
            var fnreduced = @"function(key,values)
                            {
                                var reduced ={count:0,age:0};
                                for(var idx=0;idx<values.length;idx++)
                                {
                                    var val = values[idx];
                                    reduced.age += val.age;
                                    reduced.count +=val.count;
                                }
                                return reduced;
                            }";
            var fnfinalized = @"function(key,reduced)
                                {
                                    reduced.avgAge=reduced.age / reduced.count;
                                    return reduced;
                                }";
            var option = new MapReduceOptions<CustomersDocument, BsonDocument> { Filter = filtercustomer.Eq(e => e.city, "台北市"), Finalize = fnfinalized, OutputOptions = MapReduceOutputOptions.Inline };
            var result = custmorcol.MapReduce(fnmap, fnreduced, option).ToListAsync().Result;
            foreach (BsonDocument doc in result)
            {
                Console.WriteLine(doc.ToJson());
            }

            //8-2        
            var taiwandb = client.GetDatabase("taiwan") as MongoDatabaseBase;
            var peoplecol = taiwandb.GetCollection<PeopleDocument>("people");
            var filterpeople = Builders<PeopleDocument>.Filter;


            var result1 = peoplecol.Aggregate()
                .Group(
                    new BsonDocument { 
                        { "_id", "result-8-2" }, 
                        { "birth", new BsonDocument { { "$sum", "$birth_total" } } },
                        { "death", new BsonDocument { { "$sum", "$death_total" } } },
                        { "marry", new BsonDocument { { "$sum", "$marry_pair" } } },
                        { "divorce_pair", new BsonDocument { { "$sum", "$divorce_pair" } } },
                        })
                .Project(
                    new BsonDocument { 
                        { "_id", 1 }, 
                        { "diff", new BsonDocument { { "$sum", "$death" } } } 
                        } )
                .ToListAsync().Result;
            foreach (BsonDocument doc in result1)
            {
                Console.WriteLine(doc.ToJson());
            }


            //額外練習
              var result2 = peoplecol.Aggregate()
                .Group
                (
                    new BsonDocument
                    {
                        { "_id",new BsonDocument{ { "$substrCP",new BsonArray { "$site_id",0,3} } } }  ,
                        { "birth", new BsonDocument { { "$sum", "$birth_total" } } },
                        { "death", new BsonDocument { { "$sum", "$death_total" } } }
                    }
                )
                .Project
                (
                    new BsonDocument
                    {
                        {"id",1 },
                        {"diff",new BsonDocument{ { "$subtract",new BsonArray { "$birth$death", "$death" } } } }
                    }
                )
                .Sort(new BsonDocument { { "diff",1} })
                .Limit(3)
                .ToListAsync().Result;
           

            foreach (BsonDocument doc in result2)
            {
                Console.WriteLine(doc.ToJson());
            }
        }
    }
}
