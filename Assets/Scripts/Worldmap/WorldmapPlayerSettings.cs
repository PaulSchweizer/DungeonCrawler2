using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Worldmap
{
    [CreateAssetMenu(fileName = "WorldmapPlayerSettings", menuName = "DungeonCrawler/Worldmap/WorldmapPlayerSettings")]
    public class WorldmapPlayerSettings : ScriptableObject
    {
        public string CurrentLocation;
        public bool Traveling;
        public string CurrentConnection;
        public float TravelProgress;
        public string TravelStart;
        public string TravelEnd;
    }
}
