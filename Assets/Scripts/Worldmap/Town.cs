using UnityEngine;

namespace Worldmap
{
    public class Town : MonoBehaviour
    {
        public Location Location;
        public TextMesh Text;
        public StringGameEvent OnTravelRequested;
        public StringGameEvent OnLevelEntryRequested;

        public void Start()
        {
            Text.text = Location.Name;
        }

        public void RequestTravel()
        {
            if (Location.IsCurrent)
            {
                OnLevelEntryRequested.Raise(Location.Name);
            }
            else
            {
                if (Location.CanTravelTo)
                {
                    OnTravelRequested.Raise(Location.Name);
                }
            }
        }
    }
}
