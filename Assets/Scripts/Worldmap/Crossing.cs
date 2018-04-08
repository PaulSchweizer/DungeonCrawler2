using UnityEngine;

namespace Worldmap
{
    public class Crossing : MonoBehaviour
    {
        public Location Location;
        public StringGameEvent OnTravelRequested;

        public void RequestTravel()
        {
            if (Location.CanTravelTo && !Location.IsCurrent)
            {
                OnTravelRequested.Raise(Location.Name);
            }
        }
    }
}
