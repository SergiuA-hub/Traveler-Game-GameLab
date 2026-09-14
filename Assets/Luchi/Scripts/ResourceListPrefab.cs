using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceListPrefab : MonoBehaviour
{
    public Image resourceImage;
    public TMP_Text resourceName;
    public TMP_Text resourceAmount;
    public TMP_Text info;
    private InvetoryItem inv_item;
    public void Setup(InvetoryItem item)
    {
        inv_item = item;
        resourceImage.sprite = item.resourceSO.sprite;
        resourceName.text = item.resourceSO.itemName;
        resourceAmount.text = $"{item.amount}";
        if(item.resourceSO.resourceType == ResourceType.Eat)
            info.text = $"+ {item.resourceSO.stat_restore} Hunger";
        else if (item.resourceSO.resourceType == ResourceType.Drink)
            info.text = $"+ {item.resourceSO.stat_restore} Thirst";
        else info.text = "-";
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
