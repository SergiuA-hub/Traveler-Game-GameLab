using UnityEngine;

public class TerrainManager : MonoBehaviour
{
    public Player_M player;

    public float RoadSpeedModifier = 1;
    public float GrassSpeedModifier = 0.5f; 


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
            player.stats.terrainSpeedModifier = RoadSpeedModifier;
            //player.gameObject.GetComponent<PlayerStats_M>().terrainSpeedModifier = RoadSpeedModifier;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            player.stats.terrainSpeedModifier = GrassSpeedModifier;
            //player.gameObject.GetComponent<PlayerStats_M>().terrainSpeedModifier = GrassSpeedModifier;
        }
    }
}
