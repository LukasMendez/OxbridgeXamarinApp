using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms.GoogleMaps;

namespace OxbridgeApp.Models
{
    public class Coordinate
    {
        public string UserName { get; set; }
        public Position Position { get; set; }

        public Coordinate(string userName, Position position) {
            this.UserName = userName;
            this.Position = position;
        }
    }
}
