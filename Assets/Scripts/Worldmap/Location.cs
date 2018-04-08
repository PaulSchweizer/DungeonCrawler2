using UnityEngine;

namespace Worldmap
{
    [CreateAssetMenu(fileName = "Location", menuName = "DungeonCrawler/Worldmap/Location")]
    public class Location : ScriptableObject
    {
        public enum LocationType { Town, Crossing };
        public LocationType Type;
        public string Name;
        public Vector3 Position;
        public bool CanTravelTo;
        public bool IsCurrent;
    }
}
