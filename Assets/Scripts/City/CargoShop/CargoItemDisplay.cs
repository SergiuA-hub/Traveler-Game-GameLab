using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CargoItemDisplay : MonoBehaviour, IPointerClickHandler
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

    public void OnPointerClick(PointerEventData eventData)
    {
        GameServices.Instance.cityUI.SetCurrentShopItem(cargoItem);
    }

}
