using System;
using System.Collections.Generic;
using System.Text;

namespace OxbridgeApp.Models
{
    public class SortedLeaderboard
    {
        public string Header { get; set; }
        public string TeamName { get; set; }
        public List<LeaderboardItem> Leaderboard { get; set; } 

    }
}
