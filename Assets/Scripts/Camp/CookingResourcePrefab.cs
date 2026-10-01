using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CookingResourcePrefab : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Image resIcon;
    public TMP_Text resName;
    public TMP_Text resAmount;

    public GameObject resStat1Panel;
    public TMP_Text resStat1;
    public GameObject resStat2Panel;
    public TMP_Text resStat2;
    private InventoryItem item;
    private CookingPotUI callback;
    //mouse over properties
    public Image backgroundImage;

    private Color normalColor = new Color32(0xEB, 0xD1, 0xA9, 200); // #EBD1A9
    private Color hoverColor = new Color32(0xF3, 0xDF, 0xB9, 200); // #F3DFB9
    private void Awake()
    {
        if (backgroundImage == null)
            backgroundImage = GetComponent<Image>();

        if (backgroundImage != null)
            backgroundImage.color = normalColor;
    }

    public void setup(InventoryItem itm, CookingPotUI cb)
    {
        item = itm;
        callback = cb;

        resIcon.sprite = item.resourceSO.sprite;

        resName.text = item.resourceSO.itemName;

        resAmount.text = $"x {item.amount}";

        if (item.resourceSO.stat_restore > 0)
        {
            resStat1Panel.SetActive(true);
            if(item.resourceSO.resourceType == ResourceType.Eat)
                resStat1.text = $"+ {item.resourceSO.stat_restore} Hunger";
            if (item.resourceSO.resourceType == ResourceType.Drink)
                resStat1.text = $"+ {item.resourceSO.stat_restore} Thirst";
        }
        else resStat1Panel.SetActive(false);

        if (item.resourceSO.HP_restore > 0)
        {
            resStat2Panel.SetActive(true);
            
            resStat2.text = $"+ {item.resourceSO.HP_restore} HP";
        }
        else resStat2Panel.SetActive(false);


    }

    public void updateQTY()
    {
        resAmount.text = $"x {item.amount}";
    }

    public bool hasRes(ResourceSO res)
    {
        return item.resourceSO.itemName == res.itemName;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (backgroundImage != null)
            backgroundImage.color = hoverColor;
        callback.startStatsRestore(item);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (backgroundImage != null)
            backgroundImage.color = normalColor;
        callback.stopStatsRestore(item);
    }

    public void OnPointerClick(PointerEventData eventData)
    {

    }

    public void useItem()
    {
        callback.consume(item);
    }
}
