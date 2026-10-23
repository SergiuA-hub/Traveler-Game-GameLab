
using System.Collections.Generic;
using UnityEngine;

public class RoadSegment : MonoBehaviour
{
    public RoadNode startNode;
    public RoadNode endNode;

    public List<Transform> waypoints = new();
    public float totalLength;

    [ContextMenu("Initialize Segment")]
    public void Initialize()
    {
        waypoints.Clear();

        foreach (Transform child in transform)
        {
            waypoints.Add(child);
        }

        CalculateLength();
    }

    public void CalculateLength()
    {
        totalLength = 0f;

        if (startNode == null || endNode == null)
            return;

        Vector3 previous = startNode.transform.position;

        foreach (Transform waypoint in waypoints)
        {
            totalLength += Vector3.Distance(
                previous,
                waypoint.position
            );

            previous = waypoint.position;
        }

        totalLength += Vector3.Distance(
            previous,
            endNode.transform.position
        );
    }

    public List<Transform> GetWaypoints(RoadNode fromNode)
    {
        if (fromNode == startNode)
        {
            return new List<Transform>(waypoints);
        }

        if (fromNode == endNode)
        {
            List<Transform> reversed = new List<Transform>(waypoints);
            reversed.Reverse();
            return reversed;
        }

        Debug.LogError(
            $"Node {fromNode?.name} is not part of segment {name}"
        );

        return new List<Transform>();
    }

    public RoadNode GetOtherNode(RoadNode node)
    {
        if (node == startNode) return endNode;
        if (node == endNode) return startNode;

        return null;
    }
    public int GetClosestWaypointIndex(
        Vector2 position,
        RoadNode fromNode)
    {
        List<Transform> path = GetWaypoints(fromNode);

        int closestIndex = -1;
        float closestDistance = float.MaxValue;

        for (int i = 0; i < path.Count; i++)
        {
            if (path[i] == null) continue;

            float distance = Vector2.SqrMagnitude(
                position - (Vector2)path[i].position
            );

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestIndex = i;
            }
        }

        return closestIndex;
    }
    private void OnDrawGizmosSelected()
    {
        if (startNode == null || endNode == null)
            return;

        Gizmos.color = Color.yellow;

        Vector3 previous = startNode.transform.position;

        foreach (Transform waypoint in waypoints)
        {
            if (waypoint == null) continue;

            Gizmos.DrawLine(previous, waypoint.position);
            Gizmos.DrawSphere(waypoint.position, 0.12f);

            previous = waypoint.position;
        }

        Gizmos.DrawLine(previous, endNode.transform.position);
    }
}
