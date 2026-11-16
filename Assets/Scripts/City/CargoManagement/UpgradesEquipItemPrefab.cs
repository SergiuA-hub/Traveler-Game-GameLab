using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpgradesEquipItemPrefab : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Image upgradeIcon;
    private UpgradesSO upgradeItem;

    public void OnPointerEnter(PointerEventData eventData)
    {
        GameServices.Instance.cityUI.showUpgradeMouseOver(upgradeItem);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GameServices.Instance.cityUI.clearMouseOver();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //GameServices.Instance.cityUI.SetCurrentUpgradeItem(upgradeItem);
        GameServices.Instance.player.attachments.AddUpgrade(upgradeItem);
        GameServices.Instance.cityUI.DisplayCartManagmentPanel();
    }

    public void setup(UpgradesSO item)
    {
        upgradeItem = item;
        upgradeIcon.sprite = upgradeItem.icon;
    }
}
