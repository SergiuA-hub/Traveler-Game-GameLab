using UnityEngine;
public enum NpcType
{
    Commoner,
    Trader,
    Knight
}

public class Npc : MonoBehaviour
{

    public Vector3 pathOffset = new Vector3(0f, 0.5f, 0f);
    public float speed = 1f;
    public bool forward = true;


    [HideInInspector] public RoadPath currentRoad;
    [HideInInspector] public int currentWaypointIndex;
    public GameObject visuals;

    private NpcManager npcManager;

    public void setNpcManager(NpcManager npcM)
    {
        npcManager = npcM;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (npcManager == null)
            return;

        if (other.name.Contains("Player"))
        {
            Debug.Log($"{other.name} is close");
            npcManager.playerInRange(this);
        }
        
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (npcManager == null)
            return;
        if (other.name.Contains("Player"))
        {
            Debug.Log($"{other.name} is close");
            npcManager.playerOutOfRange(this);
        }

    }
}