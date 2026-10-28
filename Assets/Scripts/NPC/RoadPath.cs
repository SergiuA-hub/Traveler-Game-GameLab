using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RoadPath
{
    public string roadName;

    public Transform physicalPath;

    public SettlementSO startSettlement;
    public SettlementSO endSettlement;

    public List<Transform> waypoints = new();

    public float totalLength;

    public void Initialize()
    {
        waypoints.Clear();
        totalLength = 0f;

        if (physicalPath == null)
            return;

        foreach (Transform child in physicalPath)
        {
            waypoints.Add(child);
        }

        for (int i = 0; i < waypoints.Count - 1; i++)
        {
            totalLength += Vector3.Distance(
                waypoints[i].position,
                waypoints[i + 1].position
            );
        }
    }
}