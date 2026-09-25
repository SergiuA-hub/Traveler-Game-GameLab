using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceListPrefab : MonoBehaviour
{
    public Image resourceImage;
    public TMP_Text resourceName;
    public TMP_Text resourceAmount;
    public TMP_Text info;
    private InventoryItem inv_item;
    private CampDisplayUI callback;
    public void Setup(InventoryItem item, CampDisplayUI cb)
    {
        inv_item = item;
        callback = cb;

        resourceImage.sprite = item.resourceSO.sprite;
        resourceName.text = item.resourceSO.itemName;
        resourceAmount.text = $"{item.amount}";
        if(item.resourceSO.resourceType == ResourceType.Eat)
            info.text = $"+ {item.resourceSO.stat_restore} Hunger";
        else if (item.resourceSO.resourceType == ResourceType.Drink)
            info.text = $"+ {item.resourceSO.stat_restore} Thirst";
        else info.text = "-";
    }

    public void addResource()
    {
        callback.addResourceToConsumption(inv_item);
    }
}
