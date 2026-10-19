using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Npc : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
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
    private INpcInteraction interaction;

    public GameObject tourch;
    public bool rooted = false;
    public bool interested = true;
    public List<string> interestedReplicas;
    public List<string> notInterestedReplicas;
    public NpcChatBubble chatBubble;

    private Coroutine restoreSpeedCoroutine;
    private const float RESTORE_DELAY = 3f;
    private void Awake()
    {
        interaction = GetComponent<INpcInteraction>();
        interested = true;
    }
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

    public void Interact()
    {
        interaction?.Interact();
    }

    private void OnDestroy()
    {
        if (timeManager == null)
            return;

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

    public void animateIdle()
    {
        animator.SetTrigger("IsIdle");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (npcManager == null)
            return;

        if (other.name.Contains("Player"))
        {
            npcManager.playerInRange(this);
        }
        
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (npcManager == null)
            return;
        if (other.name.Contains("Player"))
        {
            npcManager.playerOutOfRange(this);
            UnRoot();
        }

    }

    public void Root()
    {
        rooted = true;
        animateIdle();
    }

    public void UnRoot()
    {
        rooted = false;
        animateMove();
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (interested)
        {
            // Daca exista deja timerul de revenire, il anulam
            if (restoreSpeedCoroutine != null)
            {
                StopCoroutine(restoreSpeedCoroutine);
                restoreSpeedCoroutine = null;
            }

            chatBubble.Show(interestedReplicas[UnityEngine.Random.Range(0, interestedReplicas.Count)]);

            speed = 0.5f;
        }
        else
        {
            chatBubble.Show(notInterestedReplicas[UnityEngine.Random.Range(0, notInterestedReplicas.Count)]);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (interested)
        {
            //chatBubble.Hide();

            // Pornim timerul de 3 secunde
            restoreSpeedCoroutine = StartCoroutine(RestoreSpeedAfterDelay());
        }
        else
        {
            // Pentru NPC-urile neinteresate ramane comportamentul vechi
            speed = 1f;
            chatBubble.Hide();
        }
    }

    private IEnumerator RestoreSpeedAfterDelay()
    {
        yield return new WaitForSeconds(RESTORE_DELAY);

        chatBubble.Hide();
        speed = 1f;
        restoreSpeedCoroutine = null;
        
    }

    public void showNotCurious()
    {
        chatBubble.Show("Not interested!", 1.5f);
    }
}