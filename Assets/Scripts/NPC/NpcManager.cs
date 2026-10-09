using System;
using System.Collections.Generic;
using UnityEngine;

public class NpcManager : MonoBehaviour
{
    public Transform player;
    public TimeManager timeManager;

    public float npcVisibleDistance = 30f;
    public bool optimizeNpcVizibility;
    public int maxAliveNPC;
    private float visibilityTimer = 0f;

    public List<RoadPath> roads = new();

    public GameObject NPC;
    public Transform npcSpawner;

    public List<Npc> aliveNPCs = new();

    public List<Npc> npcPlayerCanInteract;
    private string lastSettlement;
    private void Start()
    {
        foreach (RoadPath road in roads)
        {
            road.Initialize();
        }

        for (int i = 0; i < 5; i++)
        {
            spawnRandomNPC();
        }

        timeManager.onHourChanged.AddListener(hourlNpcSpawnerCheck);
    }    

    public void hourlNpcSpawnerCheck(DateTime time)
    {
        if(aliveNPCs.Count < maxAliveNPC)
        {
            spawnRandomNPC();
        }
    }
    
    public void spawnRandomNPC()
    {
        bool direction = UnityEngine.Random.Range(0, 2) == 1;
        RoadPath randomRoad = roads[UnityEngine.Random.Range(0, roads.Count)];
        string startLocation = direction ? randomRoad.startSettlement.settlementName : randomRoad.endSettlement.settlementName;

        if (lastSettlement == startLocation)
            return;
        
        SpawnNPC(randomRoad, direction);
        Debug.Log("NPC SPAWN");
    }

    private void Update()
    {
        for (int i = aliveNPCs.Count - 1; i >= 0; i--)
        {
            MoveNPC(aliveNPCs[i]);
        }

        if (!optimizeNpcVizibility)
            return;

        visibilityTimer += Time.deltaTime;

        if (visibilityTimer >= 0.5f)
        {
            visibilityTimer = 0f;

            foreach (Npc npc in aliveNPCs)
            {
                UpdateNPCVisibility(npc);
            }
        }
    }
    private void UpdateNPCVisibility(Npc npc)
    {
        float sqrDistance =
            (npc.transform.position - player.position).sqrMagnitude;

        float maxDistanceSqr =
            npcVisibleDistance * npcVisibleDistance;

        bool shouldBeVisible =
            sqrDistance <= maxDistanceSqr;

        if (npc.visuals.activeSelf != shouldBeVisible)
        {
            npc.visuals.SetActive(shouldBeVisible);
        }
    }

    public void SpawnNPC(RoadPath road, bool forward)
    {
        if (road == null || road.waypoints.Count == 0)
            return;

        int startIndex = forward ? 0 : road.waypoints.Count - 1;

        GameObject npcObject = Instantiate(
            NPC,
            road.waypoints[startIndex].position,
            Quaternion.identity,
            npcSpawner
        );

        Npc npc = npcObject.GetComponent<Npc>();

        if (npc == null)
        {
            Destroy(npcObject);
            return;
        }

        npc.currentRoad = road;
        npc.forward = forward;

        npc.currentWaypointIndex = forward
            ? 1
            : road.waypoints.Count - 2;

        npc.transform.position =
            road.waypoints[startIndex].position + npc.pathOffset;

        lastSettlement = forward ? road.startSettlement.settlementName : road.endSettlement.settlementName;
        aliveNPCs.Add(npc);
    }

    private void MoveNPC(Npc npc)
    {
        if (npc == null)
            return;

        if (npc.currentRoad == null)
            return;

        if (npc.currentWaypointIndex < 0 ||
            npc.currentWaypointIndex >= npc.currentRoad.waypoints.Count)
        {
            aliveNPCs.Remove(npc);
            Destroy(npc.gameObject);
            return;
        }

        Vector3 target =
            npc.currentRoad.waypoints[npc.currentWaypointIndex].position
            + npc.pathOffset;

        npc.transform.position = Vector3.MoveTowards(
            npc.transform.position,
            target,
            npc.speed * Time.deltaTime
        );

        if (Vector3.Distance(npc.transform.position, target) < 0.01f)
        {
            if (npc.forward)
                npc.currentWaypointIndex++;
            else
                npc.currentWaypointIndex--;
        }
    }

    public void playerInRange(Npc npc)
    {
        if (!npcPlayerCanInteract.Contains(npc))
        {
            npcPlayerCanInteract.Add(npc);
        }
    }

    public void playerOutOfRange(Npc npc)
    {
        if (npcPlayerCanInteract.Contains(npc))
        {
            npcPlayerCanInteract.Remove(npc);
        }
    }
}