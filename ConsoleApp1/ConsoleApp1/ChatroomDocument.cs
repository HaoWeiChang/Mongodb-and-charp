using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1
{
    class ChatroomDocument
    {
        public string _id { get; set; }
        public string[] members { get; set; }
        public List<Messages> messages { get; set; }
    }
    public class Messages
    {
        public string sender { get; set; }
        public string content { get; set; }

    }

}
