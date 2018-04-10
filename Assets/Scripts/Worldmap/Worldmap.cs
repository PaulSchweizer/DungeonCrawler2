using System;
using System.Collections;
using UnityEngine;

namespace Worldmap
{
    [Serializable]
    public struct Connection
    {
        public Vector3[] Points;
        public string Start;
        public string End;
        public float Speed;
    }

    public class Worldmap : MonoBehaviour
    {
        [Header("Scene Items")]
        public GameObject PlayerPawn;
        public Location CurrentLocation;

        [Header("Data")]
        public Location[] Locations;
        public Connection[] Connections;
        public WorldmapPlayerSettings PlayerSettings;

        [Header("Prefabs")]
        public GameObject TownPrefab;
        public GameObject CrossingPrefab;
        public Road RoadPrefab;

        [Header("Events")]
        public StringGameEvent OnLevelSwitch;
        public StringGameEvent OnSceneReady;

        private void Start()
        {
            foreach(Location location in Locations)
            {
                if (location.Type == Location.LocationType.Crossing)
                {
                    CreateCrossing(location);
                }
                if (location.Type == Location.LocationType.Town)
                {
                    CreateTown(location);
                }
            }
            foreach (Connection connection in Connections)
            {
                CreateRoad(connection);
            }

            Initialize(PlayerSettings.CurrentLocation);
        }

        #region Creation

        private void CreateTown(Location location)
        {
            Town town = Instantiate(TownPrefab, location.Position, Quaternion.identity, transform).GetComponent<Town>();
            town.Location = location;
        }

        private void CreateCrossing(Location location)
        {
            Crossing crossing = Instantiate(CrossingPrefab, location.Position, Quaternion.identity, transform).GetComponent<Crossing>();
            crossing.Location = location;
        }

        private void CreateRoad(Connection connection)
        {
            Road road = Instantiate(RoadPrefab, Vector3.zero, Quaternion.identity, transform);
            road.Connection = connection;
        }

        #endregion

        #region Initialize

        public void Enter(string currentLocationName)
        {
            print(currentLocationName);
            //if (currentLocationName == "") currentLocationName = PlayerSettings.CurrentLocation;
            Initialize(currentLocationName);
            OnSceneReady.Raise("");
        }

        public void Initialize(string currentLocationName)
        {
            CurrentLocation = GetLocationByName(currentLocationName);
            PlayerPawn.transform.position = GetLocationByName(currentLocationName).Position;
            for( int i = 0; i < Locations.Length; i++ )
            {
                if (HasConnection(CurrentLocation, Locations[i]) || Locations[i] == CurrentLocation)
                {
                    Locations[i].CanTravelTo = true;
                }
                else
                {
                    Locations[i].CanTravelTo = false;
                }

                if (Locations[i] == CurrentLocation)
                {
                    Locations[i].IsCurrent = true;
                }
                else
                {
                    Locations[i].IsCurrent = false;
                }
            }
            UpdatePlayerSettings();
        }

        public void UpdatePlayerSettings()
        {
            PlayerSettings.CurrentLocation = CurrentLocation.Name;
        }

        #endregion

        #region Travel

        public void StartTravel(string location)
        {
            Location start = CurrentLocation;
            Location end = GetLocationByName(location);
            //if (start == end & CurrentLocation.Type == Location.LocationType.Town)
            //{
            //    OnLevelSwitch.Raise(CurrentLocation.Name);
            //}
            //else
            //{
                Connection connection = GetConnection(start, end);
                StartCoroutine(Travel(start, end, connection));
            //}
        }

        private Location GetLocationByName(string name)
        {
            foreach (Location location in Locations)
            {
                if (location.Name == name)
                {
                    return location;
                }
            }
            return default(Location);
        }

        private bool HasConnection(Location start, Location end)
        {
            foreach (Connection connection in Connections)
            {
                if ((connection.Start == start.Name && connection.End == end.Name) ||
                    (connection.Start == end.Name && connection.End == start.Name))
                {
                    return true;
                }
            }
            return false;
        }

        private Connection GetConnection(Location start, Location end)
        {
            foreach(Connection connection in Connections)
            {
                if ((connection.Start == start.Name && connection.End == end.Name) ||
                    (connection.Start == end.Name && connection.End == start.Name) ||
                    connection.Start == CurrentLocation.Name || 
                    connection.End == CurrentLocation.Name)
                {
                    return connection;
                }
            }
            return default(Connection);
        }

        private IEnumerator Travel(Location start, Location end, Connection connection)
        {
            float totalDistance = Vector3.Distance(end.Position, PlayerPawn.transform.position);
            float total = 0;
            while (Vector3.Distance(end.Position, PlayerPawn.transform.position) > 0)
            {
                total += Time.deltaTime * connection.Speed;
                Vector3 pos = Vector3.Lerp(start.Position, end.Position, total);
                PlayerPawn.transform.position = pos;
                yield return null;
            }
            Initialize(end.Name);
        }

        #endregion
    }
}
