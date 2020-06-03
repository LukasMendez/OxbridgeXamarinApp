using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms.GoogleMaps;

namespace OxbridgeApp.Models
{
    public class Coordinate : IMessage
    {
        public string Header { get; set; }
        public string UserName { get; set; }
        public Position Position { get; set; }

        public Coordinate(string header, string userName, Position position) {
            this.Header = header;
            this.UserName = userName;
            this.Position = position;
        }
    }
}
