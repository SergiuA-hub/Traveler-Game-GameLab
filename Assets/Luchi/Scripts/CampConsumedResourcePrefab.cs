using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CampConsumedResourcePrefab : MonoBehaviour
{
    public Image resourceImage;
    public TMP_Text resourceName;
    public TMP_Text resourceAmount;
    private ConsumedResource consumed_resource;
    public void Setup(ConsumedResource resource)
    {
        consumed_resource = resource;
        resourceImage.sprite = resource.resourceSO.sprite;
        resourceName.text = resource.resourceSO.itemName;
        resourceAmount.text = $"{resource.amount}";
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
