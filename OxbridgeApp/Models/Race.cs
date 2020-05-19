using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms.GoogleMaps;

namespace OxbridgeApp.Models
{
    public class Race
    {
        public int RaceID { get; set; }
        public List<Position> CheckPoints { get; set; }
        public int Laps { get; set; }

        public Race(int raceID, List<Position> checkPoints, int laps) {
            this.RaceID = raceID;
            this.CheckPoints = checkPoints;
            this.Laps = laps;
        }
    }
}
