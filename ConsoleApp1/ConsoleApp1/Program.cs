using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization; 
using MongoDB.Driver.GeoJsonObjectModel;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var db = client.GetDatabase("ntut") as MongoDatabaseBase;

            choosefuction();
            
            void choosefuction()
            {
                Console.WriteLine("-----------");
                Console.WriteLine("第五章查詢練習 選擇項目\t1~4");
                Console.WriteLine("1.Library查詢\tex:5-1~9");
                Console.WriteLine("2.地理位置\t5-10~12");
                Console.WriteLine("3.聊天搜尋\t5-3、4,5-13~16");
                Console.WriteLine("4.聯絡人查詢\t5-17");                
                try
                {
                    var choose = int.Parse(Console.ReadLine());
                    Console.Clear();

                    switch (choose)
                    {
                        case 1:
                            Librarysearch();
                            break;
                        case 2:
                             Geospatial();
                             break;
                        case 3:
                            FindChat();
                            break;
                        case 4:
                            FindContacts();
                            break;
                        default:
                            Console.WriteLine("請輸入1、2、3、4數字");
                            break;
                    }
                }
                catch
                {
                    Console.Clear();
                    Console.WriteLine("請輸入1、2、3、4數字");
                    
                }
                finally
                {
                    choosefuction();
                }

            }
            void Librarysearch()
            {
                var librarycol = db.GetCollection<LibraryDocument>("library");
                var filter1 = Builders<LibraryDocument>.Filter;

                //5-1查詢大於300的書籍
                var setGTPrice = filter1.Gt(e => e.price, 300); //{price{$gt:300}}
                var result = librarycol.Find(setGTPrice).ToListAsync().Result;//.find({price{$gt:300}})
                foreach (LibraryDocument doc in result)
                {
                    Console.WriteLine("5-1查詢大於300的書籍");
                    Console.WriteLine(doc.book);
                }
                //5-2查詢Jason著作
                var infilter = filter1.AnyIn("authors",new string[] { "Jason" });//{authors:{$in:["Jason"]}}
                var result2 = librarycol.Find(infilter).ToListAsync().Result;//.find({authors:{$in:["Jason"]}})
                Console.WriteLine("\n5-2查詢Jason著作");
                foreach (LibraryDocument doc in result2)
                {
                    
                    Console.WriteLine(doc.book);
                }
                //5-5查詢借書人在某天借閱的書籍
                var setName = filter1.Eq(e => e.borrower.name, "王小明");
                var setGTtime = filter1.Gt(e => e.borrower.timestamp, new DateTime(2015, 7, 24, 00, 00, 00));
                var setLTtime = filter1.Lt(e => e.borrower.timestamp, new DateTime(2015, 7, 24, 23, 59, 59));
                var mix = filter1.And(setName, setGTtime, setLTtime);
                var result3 = librarycol.Find(mix).ToListAsync().Result;
                Console.WriteLine("\n5-5查詢借書人在某天借閱的書籍");
                foreach (LibraryDocument doc in result3)
                {
                    
                    Console.WriteLine(doc.book);
                    Console.WriteLine(doc.borrower.name);
                    Console.WriteLine(doc.borrower.timestamp);
                }
                //5-6查詢未被借閱的書籍
                var noborrower = filter1.Exists(e => e.borrower, false);
                var result4 = librarycol.Find(noborrower).ToListAsync().Result;
                Console.WriteLine("\n5-6尚未被借閱的書籍");
                foreach (LibraryDocument doc in result4)
                {                    
                    Console.WriteLine(doc.book);
                    Console.WriteLine(string.Join("\t", doc.authors));
                }
                //5-7查詢欄位使用Integer的書籍
                var typeint = filter1.Type(e => e.price, BsonType.Int32);
                var result5 = librarycol.Find(typeint).ToListAsync().Result;
                foreach (LibraryDocument doc in result5)
                {
                    Console.WriteLine("\n5-7查詢欄位使用Integer的書籍");
                    Console.WriteLine(doc.book);
                }
                //5-8使用正規表示查詢書籍
                var setRegex = new BsonRegularExpression("nosql", "i");
                var setbookname = filter1.Regex(e => e.book, setRegex);
                var result6 = librarycol.Find(setbookname).ToListAsync().Result;
                foreach (LibraryDocument doc in result6)
                {
                    Console.WriteLine("\n5-8使用正規表示查詢書籍");
                    Console.WriteLine(doc.book);
                }
                //5-9使用Where搜尋大於300的書籍
                var whereprice = filter1.Where(e => e.price >= 300);
                var result7 = librarycol.Find(whereprice).ToListAsync().Result;
                foreach (LibraryDocument doc in result7)
                {
                    Console.WriteLine("\n5-9使用Where搜尋大於300的書籍");
                    Console.WriteLine(doc.book);

                }
            }

            void Geospatial()
             {
                //5-10
                //建立 2dsphere Index
                var Geospatialcol = db.GetCollection<GeospatialDocument>("buildings");
                var filter1 = Builders<GeospatialDocument>.Filter;     
                var Indexgeo = new IndexKeysDefinitionBuilder<GeospatialDocument>().Geo2DSphere(e => e.location);
                var key = new CreateIndexModel<GeospatialDocument>(Indexgeo);
                Geospatialcol.Indexes.CreateOne(key);
                //搜尋
                var temp = new GeoJson2DGeographicCoordinates(121.537858, 25.042894);
                Console.WriteLine("5-10 您所在位置:{0},{1}",temp.Longitude,temp.Latitude);
                var near1key = filter1.Near("location",new GeoJsonPoint<GeoJson2DGeographicCoordinates>(temp), 2000,1000);                   
                var result = Geospatialcol.Find(near1key).ToListAsync().Result;
                Console.WriteLine("附近含有以下地標");
                foreach (GeospatialDocument doc in result)
                {
                    Console.WriteLine("建築物:{0}\t;位置:[{1},{2}]", doc.name,doc.location.Coordinates.Longitude, doc.location.Coordinates.Longitude);
                }
                //5-11
                //建立 2dsphere Index
                var Coordinationcol = db.GetCollection<coordinationDocument>("coordination");
                var filter2 = Builders<coordinationDocument>.Filter;
                var indexgeo2 = new IndexKeysDefinitionBuilder<coordinationDocument>().Geo2DSphere(e => e.location);
                var key2= new CreateIndexModel<coordinationDocument>(indexgeo2);
                Coordinationcol.Indexes.CreateOne(key2);
                //矩形範圍搜尋
                Console.WriteLine("\n5-11 矩形[0,0][1,3]周遭含有的資料");
                var boxsearch = filter2.GeoWithinBox("location",0,0,1,3);
                var result2 =Coordinationcol.Find(boxsearch).ToListAsync().Result;
                foreach(coordinationDocument doc in result2)
                {
                    Console.WriteLine("DATA:{0};\t[{1},{2}]", doc.name, doc.location.Coordinates.Longitude, doc.location.Coordinates.Longitude);
                }
                //5-12
                //圓形範圍搜尋
                Console.WriteLine("\n5-12 中心點為[1,3]，半徑為2，周遭含有的資料");
                var centersearch = filter2.GeoWithinCenter("location",1,3,2);
                var result3 = Coordinationcol.Find(centersearch).ToListAsync().Result;
                foreach (coordinationDocument doc in result3)
                {
                    Console.WriteLine("DATA:{0};\t位置:[{1},{2}]", doc.name,doc.location.Coordinates.Longitude, doc.location.Coordinates.Longitude);
                }
            }

            void FindChat()
            {
                var chatroomcol = db.GetCollection<ChatroomDocument>("chatroom");
                var filter1 = Builders<ChatroomDocument>.Filter;
                var projection = Builders<ChatroomDocument>.Projection;
                var test = Builders<Messages>.Filter;
                //5-3
                Console.WriteLine("5-3");
                var Regaxfind = test.Regex(e => e.content, "義大利麵");
                var elematch = filter1.ElemMatch(e => e.messages, Regaxfind);
                var result4 = chatroomcol.Find(elematch).ToListAsync().Result;
                if (result4.Count()==0)
                {
                    Console.WriteLine("無符合項目");
                }

                else
                {
                    foreach (ChatroomDocument doc in result4)
                    {
                        Console.WriteLine("聊天房號:{0}", doc._id);
                        string arraymember = string.Join("\t", doc.members);
                        Console.WriteLine("Member[{0}]", arraymember);
                        for (int i = 0; i < doc.messages.Count(); i++)
                        {
                            Console.WriteLine("{0}:{1}", doc.messages[i].sender, doc.messages[i].content);
                        }
                         
                    }
                }
                //5-4
                Console.WriteLine("\n5-4");
                var setsize = filter1.Size(e => e.messages, 0);
                var result5 = chatroomcol.Find(setsize).ToListAsync().Result;
                if (result5.Count() == 0)
                {
                    Console.WriteLine("無符合的值");
                }
                else
                {
                    foreach (ChatroomDocument doc in result5)
                    {

                        Console.WriteLine("聊天房號:{0}", doc._id);
                        string arraymember = string.Join("\t", doc.members);
                        Console.WriteLine("Member[{0}]", arraymember);
                        Console.WriteLine("無聊天紀錄\n");

                    }
                    
                }
                //5-13
                Console.WriteLine("\n5-13");
                var result = chatroomcol.Find(new BsonDocument { }).Project(new BsonDocument { { "_id", true }, { "members", true }}).ToListAsync().Result;
                
                foreach (BsonDocument bsondoc1 in result)
                {
                    var doc = BsonSerializer.Deserialize<ChatroomDocument>(bsondoc1);
                    Console.WriteLine(doc._id);
                    Console.WriteLine(string.Join("\t", doc.members));
                     if (doc.messages == null)
                    {
                        break;
                    }
                    else
                    {
                        Console.WriteLine("聊天房號:{0}", doc._id);
                        string arraymember = string.Join("\t", doc.members);
                        Console.WriteLine("Member[{0}]", arraymember);
                        for (int i = 0; i < doc.messages.Count(); i++)
                        {
                            Console.WriteLine("{0}:{1}", doc.messages[i].sender, doc.messages[i].content);
                        }
                    }   
                }
                //5-14
                Console.WriteLine("\n5-14");
                var slice1 = projection.Slice(e => e.messages, -3);
                var result1 = chatroomcol.Find(new BsonDocument { }).Project(slice1).ToListAsync().Result;
                foreach (BsonDocument bsondoc in result1)
                {
                    var doc = BsonSerializer.Deserialize<ChatroomDocument>(bsondoc);
                    if (doc.messages.Count() == 0)
                    {
                        break;
                    }
                    else
                    {
                        Console.WriteLine("聊天房號:{0}", doc._id);
                        string arraymember=string.Join("\t", doc.members);
                        Console.WriteLine("Member[{0}]", arraymember);
                        for (int i = 0; i < doc.messages.Count(); i++)
                        {
                            Console.WriteLine("{0}:{1}", doc.messages[i].sender, doc.messages[i].content);
                        }
                    }
                }
                //5-15
                Console.WriteLine("\n5-15");
                var slice2 = projection.Slice(e => e.messages, 3);
                var result2 = chatroomcol.Find(new BsonDocument { }).Project(slice2).ToListAsync().Result;
                foreach (BsonDocument bsondoc in result2)
                {
                    var doc = BsonSerializer.Deserialize<ChatroomDocument>(bsondoc);
                    if (doc.messages.Count() == 0)
                    {
                        break;
                    }
                    else
                    {
                        Console.WriteLine("聊天房號:{0}", doc._id);
                        string arraymember = string.Join("\t", doc.members);
                        Console.WriteLine("Member[{0}]", arraymember);
                        for (int i = 0; i < doc.messages.Count(); i++)
                        {
                            Console.WriteLine("{0}:{1}", doc.messages[i].sender, doc.messages[i].content);
                        }

                    }
                }
                //5-16
                Console.WriteLine("\n5-16");
                var slice3 = projection.Slice(e => e.messages, 1, 3);
                var result3 = chatroomcol.Find(new BsonDocument { }).Project(slice3).ToListAsync().Result;
                foreach (BsonDocument bsondoc in result3)
                {
                    var doc = BsonSerializer.Deserialize<ChatroomDocument>(bsondoc);
                    if (doc.messages.Count() == 0)
                    {
                        break;
                    }
                    else
                    {
                        Console.WriteLine("聊天房號:{0}", doc._id);
                        string arraymember = string.Join("\t", doc.members);
                        Console.WriteLine("Member[{0}]", arraymember);
                        for (int i = 0; i < doc.messages.Count(); i++)
                        {
                            Console.WriteLine("{0}:{1}",doc.messages[i].sender,doc.messages[i].content);
                        }
                        
                    }
                }
            }

            void FindContacts()
            {
                //5-17
                var contactscol = db.GetCollection<ContactsDocument>("Contacts");
                var filter1 = Builders<ContactsDocument>.Filter;
                var projection = Builders<ContactsDocument>.Projection;
                var builderSort = Builders<ContactsDocument>.Sort;
                var regexname = filter1.Regex(e => e.name, "陳");
                var regexphone = filter1.Regex(e => e.phone, "0955");
                var mixregex = filter1.And(regexname, regexphone);
                var sort = builderSort.Ascending(e => e.age);
                var result = contactscol.Find(mixregex).Project(new BsonDocument { { "_id", false } }).Sort(sort).ToListAsync().Result;

                foreach (BsonDocument bsondoc in result)
                {
                    var doc = BsonSerializer.Deserialize<ContactsDocument>(bsondoc);
                    Console.WriteLine("\n");
                    Console.WriteLine(doc.name);
                    Console.WriteLine(doc.age);
                    Console.WriteLine(doc.phone);
                }
            }
        }
    }
}
