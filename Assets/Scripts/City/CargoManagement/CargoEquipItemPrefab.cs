using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CargoEquipItemPrefab : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Image cargoIcon;
    public TMP_Text cargoItemName;
    private CargoSO cargoItem;

    public void setup(CargoSO item)
    {
        cargoItem = item;

        cargoIcon.sprite = cargoItem.icon;
        cargoItemName.text = cargoItem.itemName;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        GameServices.Instance.cityUI.showCargoMouseOver(cargoItem);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GameServices.Instance.cityUI.clearMouseOver();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        GameServices.Instance.player.attachments.RemoveCargo();
        GameServices.Instance.player.attachments.AddCargo(cargoItem);
        GameServices.Instance.cityUI.DisplayCartManagmentPanel();
    }
}
