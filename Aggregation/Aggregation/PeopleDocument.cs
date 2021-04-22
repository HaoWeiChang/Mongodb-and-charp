using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson;
namespace Aggregation
{
    class PeopleDocument
    {
        public ObjectId _id { get; set; }
        public int statistic_yyymm { get; set; }
        public long district_code { get; set; }
        public string site_id { get; set; }
        public string village { get; set; }
        public int birth_total { get; set; }
        public int birth_total_m { get; set; }
        public int birth_total_f { get; set; }
        public int death_total { get; set; }
        public int death_m { get; set; }
        public int death_f { get; set; }
        public int marry_pair { get; set; }
        public int divorce_pair { get; set; }



        //csv資料內容
            /*"_id" : ObjectId("6080415ce6c77a8d2e596156"),
            "statistic_yyymm" : 10901,
            "district_code" : NumberLong(65000010003),
            "site_id" : "新北市板橋區",
            "village" : "赤松里",
            "birth_total" : 1,
            "birth_total_m" : 0,
            "birth_total_f" : 1,
            "death_total" : 2,
            "death_m" : 2,
            "death_f" : 0,
            "marry_pair" : 1,
            "divorce_pair" : 0*/
    }
}
