using System;
using UnityEngine;

[Serializable]
public class StaticNPCSpawnPoint
{
    [SerializeField] public Transform spawnPoint;
    [SerializeField] public GameObject npcPrefab;
    public Season season;
}
