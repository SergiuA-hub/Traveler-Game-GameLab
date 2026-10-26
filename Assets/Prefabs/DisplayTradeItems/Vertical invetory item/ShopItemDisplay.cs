using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopItemDisplay : MonoBehaviour, IPointerClickHandler
{
    public Image IconImage;
    
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI type;
    public TextMeshProUGUI status;
    public TextMeshProUGUI price;

    //Components
    public CityUI cityUI;
    public Cargo cargo;
    

    public void PopulateIcons(Cargo item,CityUI ui)
    {
        cityUI = ui;
        cargo = item;

        IconImage.sprite = item.cargoSO.icon;
       


        Cargo owned = null;
        owned = CargoManager.instance.PlayerOwnedCargo.Find(x => x != null && x.cargoSO == item.cargoSO);

        if (status == null)
            Debug.LogError($"'status' nu e asignat pe prefab-ul {name}", this);
        else
            status.text = owned != null ? owned.itemStatus.ToString() : "Disponibil";
       

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        cityUI.SetCurrentShopItem(this);
        
        
    }
}
