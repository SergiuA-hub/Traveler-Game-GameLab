using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    public List<TeleportLocation> locations;
    public void movePlayer(string location)
    {
        Debug.Log($"Ask to Teleported to {location}.");
        foreach (var loc in locations)
        {
            if(loc.locationName.Equals(location))
            {
                Debug.Log($"Teleported to {location}.");
                GameServices.Instance.player.transform.position = loc.locationPosition.position;
                return;
            }
        }

        Debug.LogWarning($"Teleport location '{location}' not found.");
    }
}

[Serializable]
public class TeleportLocation
{
    public string locationName;
    public Transform locationPosition;
}