using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisplayInventoryItem : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    PlayerInventory inventory;
    public TextMeshProUGUI amntText;
    public TextMeshProUGUI nameText;
    public Image img;



    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Setup(InvetoryItem invetoryItem)
    {
        amntText.text = invetoryItem.amount.ToString();
        nameText.text = invetoryItem.resourceSO.itemName;
        img.sprite = invetoryItem.resourceSO.sprite;
    }
}
