using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamSyncDB
{
    public class Game
    {
        public int AppID { get; set; }
        public string Name { get; set; }
        public DateTime ReleaseDate { get; set; }
        public decimal Price { get; set; }
        public bool Windows { get; set; }
        public bool Mac { get; set; }
        public bool Linux { get; set; }
        public int MetacriticScore { get; set; }
        public int Recommendations { get; set; }
        public int Positive { get; set; }
        public int Negative { get; set; }
        public string EstimatedOwners { get; set; }
        public int AveragePlaytime { get; set; }
        public int PeakCcu { get; set; }
        public decimal PctPosTotal { get; set; }
        public int NumReviews { get; set; }
        public List<string> Categories { get; set; }
        public List<string> Genres { get; set; }
        public List<string> Tags { get; set; }

        public Game()
        {
            Categories = new List<string>();
            Genres = new List<string>();
            Tags = new List<string>();
        }
    }
}
