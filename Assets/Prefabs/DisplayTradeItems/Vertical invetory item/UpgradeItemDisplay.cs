using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class UpgradeItemDisplay : MonoBehaviour, IPointerClickHandler
{
    public Image iconImage;
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemType;
    public TextMeshProUGUI price;
    public TextMeshProUGUI status;
    
    public CityUI cityUi;
    public Cargo upgrade;

    public void PopulateUpgrade(Cargo cargo,CityUI city )
    {
        //Components
        upgrade = cargo;
        cityUi = city;
        
        //Visual
        iconImage.sprite = cargo.cargoSO.icon;
        itemName.text = cargo.cargoSO.name;
        itemType.text = cargo.itemType.ToString();
        price.text = cargo.cargoSO.price.ToString();

        Cargo owned = CargoManager.instance.PlayerOwnedCargo.Find(x => x != null && x.cargoSO == cargo.cargoSO);
        int highest = CargoManager.instance.GetHighestOwnedLevel(cargo.itemType);
        int level = cargo.cargoSO.level;

        if(owned != null)
        {
            status.text = owned.itemStatus.ToString();
            cargo.SetStatus(Status.Owned);
        }else if (level == highest + 1)
        {
            status.text = "AVALABILE";
            cargo.SetStatus(Status.Avalabile);

        }
        else 
        {
            status.text = "LOCKED";
           
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        cityUi.SetCurrentUpgradeItem(this);
    }
}
