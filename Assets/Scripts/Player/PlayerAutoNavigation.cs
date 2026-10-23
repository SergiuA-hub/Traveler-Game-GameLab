
using System.Collections.Generic;
using UnityEngine;

public class PlayerAutoNavigation : MonoBehaviour
{
    public RoadNetworkManager roadNetwork;
    public SettlementsManager settlementsManager;

    [SerializeField] private float arrivalDistance = 0.12f;

    private readonly List<Vector2> targets = new();

    private int currentTargetIndex;
    private bool isNavigating;

    public bool IsNavigating => isNavigating;

    public Vector2 CurrentTarget =>
        currentTargetIndex < targets.Count
            ? targets[currentTargetIndex]
            : (Vector2)transform.position;

    public void StartNavigation(Transform destination)
    {
        StopNavigation();

        if (destination == null ||
            roadNetwork == null ||
            settlementsManager == null)
        {
            Debug.LogError("[NAV] Missing references!");
            return;
        }

        RoadNode destinationNode = GetDestinationNode(destination);

        if (destinationNode == null)
        {
            Debug.LogError("[NAV] Destination node not found!");
            return;
        }

        Vector2 playerPosition = transform.position;

        // Find the closest road segment and exact position on it.
        RoadSegment closestSegment = null;
        List<Vector2> closestPath = null;

        int closestEdge = -1;
        Vector2 closestPoint = Vector2.zero;
        float closestDistance = float.PositiveInfinity;
        float distanceFromStart = 0f;

        HashSet<RoadSegment> checkedSegments = new();

        foreach (RoadNode node in roadNetwork.allNodes)
        {
            if (node == null) continue;

            foreach (RoadConnection connection in node.connections)
            {
                if (connection == null || connection.segment == null)
                    continue;

                RoadSegment segment = connection.segment;

                if (!checkedSegments.Add(segment))
                    continue;

                if (segment.startNode == null ||
                    segment.endNode == null)
                    continue;

                List<Vector2> path = GetSegmentPoints(segment);

                float accumulated = 0f;

                for (int i = 0; i < path.Count - 1; i++)
                {
                    Vector2 a = path[i];
                    Vector2 b = path[i + 1];

                    Vector2 ab = b - a;
                    float lengthSquared = ab.sqrMagnitude;

                    if (lengthSquared < 0.000001f)
                        continue;

                    float t = Mathf.Clamp01(
                        Vector2.Dot(playerPosition - a, ab) /
                        lengthSquared
                    );

                    Vector2 projectedPoint = a + ab * t;

                    float distance = (
                        playerPosition - projectedPoint
                    ).sqrMagnitude;

                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestSegment = segment;
                        closestPath = path;
                        closestEdge = i;
                        closestPoint = projectedPoint;

                        distanceFromStart =
                            accumulated + Vector2.Distance(a, projectedPoint);
                    }

                    accumulated += Vector2.Distance(a, b);
                }
            }
        }

        if (closestSegment == null)
        {
            Debug.LogError("[NAV] No road segment found!");
            return;
        }

        float segmentLength = GetPathLength(closestPath);

        // Try both directions from the player's position.
        RoadNode startNode = closestSegment.startNode;
        RoadNode endNode = closestSegment.endNode;

        List<RoadNode> routeFromStart =
            roadNetwork.FindShortestRoute(startNode, destinationNode);

        List<RoadNode> routeFromEnd =
            roadNetwork.FindShortestRoute(endNode, destinationNode);

        float costViaStart = float.PositiveInfinity;
        float costViaEnd = float.PositiveInfinity;

        if (routeFromStart != null)
        {
            costViaStart =
                distanceFromStart + GetRouteLength(routeFromStart);
        }

        if (routeFromEnd != null)
        {
            costViaEnd =
                (segmentLength - distanceFromStart) +
                GetRouteLength(routeFromEnd);
        }

        if (float.IsPositiveInfinity(costViaStart) &&
            float.IsPositiveInfinity(costViaEnd))
        {
            Debug.LogError("[NAV] No route to destination!");
            return;
        }

        bool goTowardsStart = costViaStart <= costViaEnd;

        List<RoadNode> selectedRoute =
            goTowardsStart ? routeFromStart : routeFromEnd;

        // Step 1: enter the road at the closest point.
        AddTarget(closestPoint);

        // Step 2: follow the current segment towards
        // whichever endpoint produces the shortest route.
        if (goTowardsStart)
        {
            for (int i = closestEdge; i >= 0; i--)
            {
                AddTarget(closestPath[i]);
            }
        }
        else
        {
            for (int i = closestEdge + 1;
                 i < closestPath.Count; i++)
            {
                AddTarget(closestPath[i]);
            }
        }

        // Step 3: follow the remaining road network.
        for (int i = 0; i < selectedRoute.Count - 1; i++)
        {
            RoadNode from = selectedRoute[i];
            RoadNode to = selectedRoute[i + 1];

            RoadConnection connection =
                from.connections.Find(
                    c => c != null &&
                         c.neighbor == to &&
                         c.segment != null &&
                         c.segment.GetOtherNode(from) == to
                );

            if (connection == null)
            {
                Debug.LogError(
                    $"[NAV] Missing connection: {from.name} -> {to.name}"
                );

                StopNavigation();
                return;
            }

            RoadSegment segment = connection.segment;

            List<Transform> waypoints =
                segment.GetWaypoints(from);

            foreach (Transform waypoint in waypoints)
            {
                if (waypoint != null)
                    AddTarget(waypoint.position);
            }

            AddTarget(to.transform.position);
        }

        currentTargetIndex = 0;
        isNavigating = targets.Count > 0;

        Debug.Log(
            $"[NAV] Started: {closestSegment.name} | " +
            $"Direction: {(goTowardsStart ? "START" : "END")} | " +
            $"Destination: {destinationNode.name} | " +
            $"Targets: {targets.Count}"
        );
    }

    private List<Vector2> GetSegmentPoints(RoadSegment segment)
    {
        List<Vector2> points = new();

        points.Add(segment.startNode.transform.position);

        foreach (Transform waypoint in segment.waypoints)
        {
            if (waypoint != null)
                points.Add(waypoint.position);
        }

        points.Add(segment.endNode.transform.position);

        return points;
    }

    private float GetPathLength(List<Vector2> points)
    {
        float length = 0f;

        for (int i = 1; i < points.Count; i++)
        {
            length += Vector2.Distance(
                points[i - 1],
                points[i]
            );
        }

        return length;
    }

    private float GetRouteLength(List<RoadNode> route)
    {
        float length = 0f;

        for (int i = 0; i < route.Count - 1; i++)
        {
            RoadNode from = route[i];
            RoadNode to = route[i + 1];

            RoadConnection connection =
                from.connections.Find(
                    c => c != null &&
                         c.neighbor == to &&
                         c.segment != null &&
                         c.segment.GetOtherNode(from) == to
                );

            if (connection == null)
                return float.PositiveInfinity;

            length += connection.segment.totalLength;
        }

        return length;
    }

    private void AddTarget(Vector2 position)
    {
        // Avoid adding consecutive duplicate targets.
        if (targets.Count > 0)
        {
            if (Vector2.Distance(
                targets[targets.Count - 1],
                position
            ) < 0.01f)
            {
                return;
            }
        }

        targets.Add(position);
    }

    private RoadNode GetDestinationNode(Transform destination)
    {
        foreach (var settlement in settlementsManager.settlementsList)
        {
            if (settlement.settlmentGo != null &&
                settlement.settlmentGo.transform == destination)
            {
                return settlement.nodeLocation;
            }
        }

        return null;
    }

    public Vector2 GetMoveDirection(Vector2 playerPosition)
    {
        if (!isNavigating)
            return Vector2.zero;

        while (currentTargetIndex < targets.Count)
        {
            Vector2 delta =
                targets[currentTargetIndex] - playerPosition;

            if (delta.magnitude > arrivalDistance)
                return delta.normalized;

            currentTargetIndex++;
        }

        Debug.Log("[NAV] Destination reached!");
        StopNavigation();

        return Vector2.zero;
    }

    public void StopNavigation()
    {
        isNavigating = false;
        targets.Clear();
        currentTargetIndex = 0;
    }

    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying || !isNavigating)
            return;

        Gizmos.color = Color.cyan;

        Vector3 previous = transform.position;

        for (int i = currentTargetIndex; i < targets.Count; i++)
        {
            Vector3 point = targets[i];

            Gizmos.DrawLine(previous, point);
            Gizmos.DrawSphere(point, 0.15f);

            previous = point;
        }
    }
}
