using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceDropPrefab : MonoBehaviour
{
    public TMP_Text resName;
    public Image resIcon;
    public TMP_Text resQty;
    private ResourceDrop drop;

    public void setup(ResourceDrop res)
    {
        drop = res;
        resName.text = drop.resource.itemName;
        resIcon.sprite = drop.resource.sprite;
        resQty.text = drop.amount.ToString();
    }    
}
