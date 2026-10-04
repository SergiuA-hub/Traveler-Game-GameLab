using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShelterResourcePrefab : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Image resIcon;
    public TMP_Text resName;
    public TMP_Text resAmount;

    public TMP_Text resStat;
    private ResourceAmount item;

    private ShelterCampUI callback;
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

    public void setup(ResourceAmount res, ShelterCampUI cb)
    {
        item = res;
        callback = cb;

        resIcon.sprite = item.resourceSO.sprite;

        resName.text = item.resourceSO.itemName;

        resAmount.text = $"x {item.amount}";

        resStat.text = $"+ {item.resourceSO.stat_restore} Reinforcement";       
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
        //callback.startStatsRestore(item);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (backgroundImage != null)
            backgroundImage.color = normalColor;
        //callback.stopStatsRestore(item);
    }

    public void OnPointerClick(PointerEventData eventData)
    {

    }

    public void useItem()
    {
        callback.addReinforcementToCamp(item);
    }
}
