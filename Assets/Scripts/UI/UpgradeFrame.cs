using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpgradeFrame : MonoBehaviour, IPointerClickHandler
{
    public Image upgradeIcon;
    public UpgradesSO upgradeSlot;

    public void OnPointerClick(PointerEventData eventData)
    {
        GameServices.Instance.player.attachments.RemoveUpgrade(upgradeSlot);
        GameServices.Instance.cityUI.DisplayCartManagmentPanel();
    }

    public void showUpgrade(UpgradesSO upgrade)
    {
        if(upgrade == null)
        {
            upgradeSlot = null;
            upgradeIcon.gameObject.SetActive(false);
            return;
        }

        upgradeSlot = upgrade;

        upgradeIcon.sprite = upgradeSlot.icon;
        upgradeIcon.gameObject.SetActive(true);
    }
}
