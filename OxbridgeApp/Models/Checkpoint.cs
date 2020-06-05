using System;
using System.Collections.Generic;
using System.Text;

namespace OxbridgeApp.Models
{
    public class Checkpoint : IMessage
    {
        public string Header { get; set; }
        public string TeamName { get; set; }
        public object Tag { get; set; }
        public DateTime CompleteTime { get; set; } //set by server

        public Checkpoint(string header, string teamName, object tag ) {
            this.Header = header;
            this.TeamName = teamName;
            this.Tag = tag;
            this.CompleteTime = new DateTime(); //because not nullable
        }
    }
}
