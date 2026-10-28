using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class UpgradeItemDisplay : MonoBehaviour, IPointerClickHandler
{
    public Image iconImage;
    
    
    public CityUI cityUi;
    public Cargo upgrade;

    public void PopulateUpgrade(Cargo cargo,CityUI city )
    {
        //Components
        upgrade = cargo;
        cityUi = city;
        
        //Visual
        iconImage.sprite = cargo.cargoSO.icon;
       
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        cityUi.SetCurrentUpgradeItem(this);
    }
}
