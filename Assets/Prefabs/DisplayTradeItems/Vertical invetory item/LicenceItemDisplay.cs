using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LicenceItemDisplay : MonoBehaviour, IPointerClickHandler
{
    public Image icon;
    public TextMeshProUGUI licencename;
    public TextMeshProUGUI settlemntName;
    public TextMeshProUGUI price;
    

    //Components
    public CityUI CityUI;
    public LicensesSO licenseSO;


    public void PopulateLicense(LicensesSO license,CityUI city)
    {
        CityUI = city;
        licenseSO = license;

        icon.sprite = licenseSO.icon;
        licencename.text = license.name;
        
        price.text = license.cost.ToString();
        
        //resourses ulock
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        CityUI.SetCurrentLicence(this);
    }
}
