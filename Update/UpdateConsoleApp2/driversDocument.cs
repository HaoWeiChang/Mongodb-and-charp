using System;
using System.Collections.Generic;
using System.Text;

namespace UpdateConsoleApp2
{
    class driversDocument
    {
        public string _id { get; set; }
        public string group { get; set; }
        public string status { get; set; }
        public Seatinfo seatinfo { get; set; }
        public double[] location { get; set; }
        public DateTime lastrModified { get; set; }


        public class Seatinfo
        {
            public int capacity { get; set; }
            public int remaining { get; set; }
            public int riding { get; set; }

        }
    }
}
