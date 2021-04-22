using System;
using System.Collections.Generic;
using System.Text;

namespace UpdateConsoleApp2
{
    class drinkDocument
    {
        public string _id { get; set; }
        public string product { get; set; }
        public string type { get; set; }
        public Price price { get; set; }
        public int sold { get; set; }
        public Log[] log { get; set; }
    }
    public class Price
    {
        public int M { get; set; }
        public int L { get; set; }

    }
    public class Log
    {
        public DateTime time { get; set; }
        public string size { get; set; }
    }
}
