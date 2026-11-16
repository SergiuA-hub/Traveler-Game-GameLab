using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpgradeItemDisplay : MonoBehaviour, IPointerClickHandler
{
    public Image iconImage;
    public Image Selected;
    
    public CityUI cityUi;
    public Cargo upgrade;

    private void OnEnable()
    {
        Selected.gameObject.SetActive(false);
    }
    public void PopulateUpgrade(Cargo cargo,CityUI city )
    {
        //Components
        upgrade = cargo;
        cityUi = city;
        
        //Visual
        iconImage.sprite = cargo.cargoSO.icon;
       
    }
    public void SetActivSelectedUpgrade()
    {
        if(cityUi.settlementsManager.currentManagmentUpgrade != null)
        {
            cityUi.settlementsManager.currentManagmentUpgrade.Selected.gameObject.SetActive(false);
        }

        cityUi.settlementsManager.currentManagmentUpgrade = this;
        Selected.gameObject.SetActive(true);


    }
   
    public void OnPointerClick(PointerEventData eventData)
    {
        
        if(cityUi.currentpage == CityUIPage.UpgradeShop)
        {
            //cityUi.SetCurrentUpgradeItem(this);

        } 

        if(cityUi.currentpage == CityUIPage.CartManagment)
        {
            //cityUi.SetCurrentCartManagemtnUpgrade(this);
        }
    }
}
