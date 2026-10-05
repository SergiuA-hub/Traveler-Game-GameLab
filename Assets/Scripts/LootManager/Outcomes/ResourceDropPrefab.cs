using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceDropPrefab : MonoBehaviour
{
    public TMP_Text resName;
    public Image resIcon;
    public TMP_Text resQty;
    private ResourceAmount drop;

    public void setup(ResourceAmount resA)
    {
        drop = resA;
        resName.text = drop.resourceSO.itemName;
        resIcon.sprite = drop.resourceSO.sprite;
        resQty.text = drop.amount.ToString();
    }    
}
