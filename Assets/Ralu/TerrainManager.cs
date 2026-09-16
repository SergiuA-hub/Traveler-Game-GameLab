using UnityEngine;

public class TerrainManager : MonoBehaviour
{
    public Player_M player;

    public float speedModifier = 1;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            player.gameObject.GetComponent<PlayerStats_M>().terrainSpeedModifier = speedModifier;
        }
    }
}
