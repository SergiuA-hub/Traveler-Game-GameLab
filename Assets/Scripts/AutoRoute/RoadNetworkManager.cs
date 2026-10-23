
using System.Collections.Generic;
using UnityEngine;

public class RoadNetworkManager : MonoBehaviour
{
    public List<RoadNode> allNodes = new();

    public RoadNode GetClosestNode(Vector3 position)
    {
        RoadNode closest = null;
        float minDistance = float.MaxValue;

        foreach (var node in allNodes)
        {
            if (node == null) continue;

            float distance = (
                node.transform.position - position
            ).sqrMagnitude;

            if (distance < minDistance)
            {
                minDistance = distance;
                closest = node;
            }
        }

        return closest;
    }

    public List<RoadNode> FindShortestRoute(
        RoadNode start,
        RoadNode destination)
    {
        if (start == null || destination == null)
            return null;

        var distances = new Dictionary<RoadNode, float>();
        var previous = new Dictionary<RoadNode, RoadNode>();
        var unvisited = new HashSet<RoadNode>();

        foreach (var node in allNodes)
        {
            if (node == null) continue;

            distances[node] = float.MaxValue;
            unvisited.Add(node);
        }

        if (!unvisited.Contains(start) ||
            !unvisited.Contains(destination))
            return null;

        distances[start] = 0f;

        while (unvisited.Count > 0)
        {
            RoadNode current = null;
            float smallest = float.MaxValue;

            foreach (var node in unvisited)
            {
                if (distances[node] < smallest)
                {
                    smallest = distances[node];
                    current = node;
                }
            }

            if (current == null)
                break;

            if (current == destination)
                break;

            unvisited.Remove(current);

            foreach (var connection in current.connections)
            {
                if (connection == null ||
                    connection.neighbor == null ||
                    connection.segment == null)
                    continue;

                RoadNode neighbor = connection.neighbor;
                RoadSegment segment = connection.segment;

                if (!unvisited.Contains(neighbor))
                    continue;

                if (segment.GetOtherNode(current) != neighbor)
                    continue;

                float newDistance =
                    distances[current] + segment.totalLength;

                if (newDistance < distances[neighbor])
                {
                    distances[neighbor] = newDistance;
                    previous[neighbor] = current;
                }
            }
        }

        if (start != destination &&
            !previous.ContainsKey(destination))
            return null;

        var route = new List<RoadNode>();
        RoadNode step = destination;

        while (step != start)
        {
            route.Add(step);
            step = previous[step];
        }

        route.Add(start);
        route.Reverse();

        return route;
    }
}
