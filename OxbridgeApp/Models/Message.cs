using System;
using System.Collections.Generic;
using System.Text;

namespace OxbridgeApp.Models
{
    public class Message : IMessage
    {
        public string Header { get; set; }
        public string TeamName { get; set; }
    }
}
