using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpgradeItemPrefab : MonoBehaviour, IPointerClickHandler
{
    public Image upgradeIcon;
    public Image selected;
    private UpgradesSO upgradeItem;

    public void OnPointerClick(PointerEventData eventData)
    {
        GameServices.Instance.cityUI.SetCurrentUpgradeItem(upgradeItem);
    }

    public void setup(UpgradesSO item)
    {
        upgradeItem = item;
        upgradeIcon.sprite = upgradeItem.icon;
    }
}
