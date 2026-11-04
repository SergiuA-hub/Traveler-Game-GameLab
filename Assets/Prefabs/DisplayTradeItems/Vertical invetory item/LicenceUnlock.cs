using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LicenceUnlock : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI item_name;

    public void Create(ResourceSO resource)
    {
        icon.sprite = resource.sprite;
        item_name.text = resource.itemName;
    }
}
