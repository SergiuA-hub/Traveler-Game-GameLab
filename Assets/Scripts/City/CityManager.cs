using Unity.VisualScripting;
using UnityEngine;

public class CityManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject CityUI;

    
    //Open UI -> Get player MOney -> Buy/Sell -> Add/ Remove from Invetory
    //Buying/ Selling
    
    private const string PlayerLayer = "Player";
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer(PlayerLayer))
        {
            Debug.Log("Intra in oras");
        }
    }
}
