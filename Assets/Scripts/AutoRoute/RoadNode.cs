using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RoadNode : MonoBehaviour
{
    public List<RoadConnection> connections = new();
}