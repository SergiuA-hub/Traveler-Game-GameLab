using System;
using UnityEngine;
public enum NpcType
{
    Commoner,
    Trader,
    Knight
}

public class Npc : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Vector3 pathOffset = new Vector3(0f, 0.5f, 0f);
    public float speed = 1f;
    public bool forward = true;


    [HideInInspector] public RoadPath currentRoad;
    [HideInInspector] public int currentWaypointIndex;
    public GameObject visuals;
    public Animator animator;
    private NpcManager npcManager;
    private TimeManager timeManager;

    public GameObject tourch;
    public void setNpcManager(NpcManager npcM, TimeManager tm)
    {
        npcManager = npcM;
        timeManager = tm;

        if(timeManager.currentTime.Hour > 7 && timeManager.currentTime.Hour < 20)
        {
            turnOffLight(timeManager.currentTime);
        }

        timeManager.onEvening.AddListener(turnOnLight);
        timeManager.onMorning.AddListener(turnOffLight);
    }

    private void OnEnable()
    {
        animateMove();
    }

    private void OnDestroy()
    {
        timeManager.onEvening.RemoveListener(turnOnLight);
        timeManager.onMonthChanged.RemoveListener(turnOffLight);
    }
    public void turnOnLight(DateTime t)
    {
        tourch.SetActive(true);
    }

    public void turnOffLight(DateTime t)
    {
        tourch.SetActive(false);
    }

    public void animateMove()
    {
        animator.SetTrigger("IsWalking");
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