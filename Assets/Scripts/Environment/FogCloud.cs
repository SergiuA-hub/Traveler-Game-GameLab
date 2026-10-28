using System.Collections.Generic;
using UnityEngine;

public class FogCloud : MonoBehaviour
{
    private bool playerInside = false;
    public float dmgFrequency = 1;
    public int dmgAmount = 1;
    public float dmgChance = 0.5f;
    public int priority;

    private float timer = 0f;
    private float timeSinceEntry = 0f;

    public Player_M player;
    private static List<FogCloud> activeClouds = new List<FogCloud>();
    
    

    void Start()
    {

    }

    void Update()
    {
        if (playerInside && IsHighestPriorityCloud())
        {
            timer += Time.deltaTime;
            timeSinceEntry += Time.deltaTime;

            if (timer >= dmgFrequency)
            {
                timer = 0f;
                if (Random.value < dmgChance)
                {
                    player.TakeDamage(dmgAmount);
                    Debug.Log("Fog damage: " + dmgAmount.ToString());
                }
                Debug.Log(timeSinceEntry.ToString() + " seconds in fog cloud");
            }
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            activeClouds.Add(this);
            playerInside = true;
        }

    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            timer = 0f;
            timeSinceEntry = 0f;
            activeClouds.Remove(this);
        }

    }

    private bool IsHighestPriorityCloud()
    {
        foreach (FogCloud cloud in activeClouds)
        {
            if (cloud.priority > priority)
            {
                return false;
            }
        }

        return true;
    }
}